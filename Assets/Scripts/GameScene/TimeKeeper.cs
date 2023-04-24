using MakeSheetScene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class TimeKeeper : MonoPair<TimeKeeper>
    {
        [SerializeField] private Transform autoTrn;
        [SerializeField] private Transform manualTrn;

        private LaneInputManager[] lanes;
        [SerializeField] private Transform[] spawnTrn;

        private Transform[] lanesTrn;

        [SerializeField] private Binder binderPrefab;

        private Queue<OneDivData> queue;

        /// <summary>
        /// すべてのDivに関して、いつ叩かれるべきかを保持
        /// </summary>
        private float[][] divTiming;

        private AudioSource source;

        private Coroutine coroutine;

        public LaneInputManager[] Lanes { get => lanes; }

        private Dictionary<int, OneNoteData> childNotesMap;

        /// <summary>
        /// 演奏開始に向けた準備
        /// </summary>
        /// <param name="sheetData"></param>
        public void Initialize(SheetData sheetData,bool auto)
        {
            lanes = new LaneInputManager[4];
            autoTrn.gameObject.SetActive(false);
            manualTrn.gameObject.SetActive(false);

            Transform lanesParentTrn = auto ? autoTrn : manualTrn;
            lanesParentTrn.gameObject.SetActive(true);

            for (int i = 0; i < 4; i++)
            {
                lanes[i] = lanesParentTrn.GetChild(i).GetComponent<LaneInputManager>();
            }
            lanesTrn = Array.ConvertAll(lanes,l=>l.transform);

            //発射時間を計算
            DecidingTiming(sheetData);

            childNotesMap = new Dictionary<int, OneNoteData>();

            //キューを作る
            queue = MakeNoteQueue(sheetData);
        }

        /// <summary>
        /// 演奏開始時に呼ぶ
        /// </summary>
        /// <param name="source"></param>
        public void StartKeeping(AudioSource source)
        {
            lanesTrn = Array.ConvertAll(lanes, l => l.transform);

            lanes.For((l,i)=> l.Initialize(IsPlayer,source,i) );

            this.source = source;
            coroutine = StartCoroutine(KeepTime());
        }
        private IEnumerator KeepTime()
        {
            while (true)
            {
                if (queue.Count > 0)
                {
                    OneDivData oneDivData = queue.Peek();
                    if (oneDivData.LaunchTime <= source.time)
                    {
                        List<(int pos,NoteController noteCtrl,OneNoteData noteData)> launching = new List<(int pos, NoteController noteCtrl, OneNoteData noteData)>();

                        for (int i = 0; i < 4; i++)
                        {
                            OneNoteData item = oneDivData.Notes[i];
                            if (item != null && item.ParentId == -1)
                            {
                                //発射ノーツとしてリストに登録
                                NoteController ctrl = NotesPool.NotesPoolManager.Instance.Rent(IsPlayer, item.NoteType);
                                launching.Add((i,ctrl,item));
                            }
                        }





                        foreach (var item in launching)
                        {
                            item.noteCtrl.Launch(spawnTrn[item.noteData.From],I(true).lanesTrn,I(false).lanesTrn, item.noteData, item.noteData.ChildId != -1);
                            Lanes[item.pos].SetAndLaunchNote(item.noteCtrl);
                        }

                        //binderで結ぶ
                        if (launching.Count > 1)
                        {
                            for (int i = 0; i < launching.Count - 1; i++)
                            {
                                Binder b = NotesPool.NotesPoolManager.Instance.RentBinder();
                                b.NotesSetting(launching[i].noteCtrl, launching[i + 1].noteCtrl);
                                launching[i].noteCtrl.Binding = b;
                                launching[i + 1].noteCtrl.Binding = b;

                            }
                        }


                        queue.Dequeue();
                    }
                }


                if(queue.Count==0 || source.time<0.05f) //すべてのノーツを出し切った or 曲が終了のとき、ループを抜ける
                {
                    break;
                }
                
               
                yield return new WaitForSeconds(1f/60);

                
            }


            //すべてのノーツを発射したら　レーンのノーツが消えるまで待つ
            foreach (var item in Lanes)
            {
                yield return new WaitWhile(() => item.NotesOnLane.Count > 0);
            }

            //レーンのノーツがなくなった後に1秒待って、終了を通知
            yield return new WaitForSeconds(1);

            if (!IsPlayer)
            {
                coroutine = null;
                yield break;
            }

            Debug.Log("終わったよ");
            GameManager.I.Finish();
        }

        public void LaunchChildNote(Transform startPosTrn,int parentNoteId)
        {
            OneNoteData noteData = null;
            try
            {
                noteData = childNotesMap[parentNoteId];
            }
            catch
            {
                Debug.LogError($"{parentNoteId}を親に持つ子Noteが見つかりませんでした");
                return;
            }
            NoteController ctrl = NotesPool.NotesPoolManager.Instance.Rent(IsPlayer, noteData.NoteType);
            ctrl.Launch(startPosTrn, I(true).lanesTrn, I(false).lanesTrn, noteData,noteData.ChildId != -1);
            Lanes[noteData.Pos].SetAndLaunchNote(ctrl);
        }

        #region Making Queue
        private Queue<OneDivData> MakeNoteQueue(SheetData sheetData)
        {
            Queue<OneDivData> ans = new Queue<OneDivData>();

            for (int bar = 0; bar < sheetData.AllBarNum; bar++)
            {
                BarData barData = sheetData.barDatas[bar];
                for (int div = 0; div < barData.DivDatas.Length; div++)
                {
                    DivData divData = barData.DivDatas[div];
                    int[][] notes = divData.GetNotes(IsPlayer);
                    float launchTime = divTiming[bar][div] - 60 * (4 / barData.BPM);
                    ans.Enqueue(MakeDivData(sheetData, notes, divTiming[bar][div],launchTime, bar, div));

                }
            }

            return ans;
        }
        private OneDivData MakeDivData(SheetData sheetData,int[][] notesData,float beatTime,float launchTime,int bar,int div)
        {
            OneNoteData[] notes = new OneNoteData[4];
            for (int pos = 0; pos < 4; pos++)
            {


                NoteTypeForSheet type = (NoteTypeForSheet)notesData[pos][0];

                notes[pos] = type switch
                {
                    NoteTypeForSheet.Normal => new NormalNoteData(notesData[pos][2], notesData[pos][1], pos, notesData[pos][3], notesData[pos][4], beatTime),
                    NoteTypeForSheet.LongBegin => new LongNoteData(notesData[pos][2],notesData[pos][1], pos, notesData[pos][3], notesData[pos][4], GetLongEndNoteInfo(sheetData, bar, div, pos), beatTime),
                    _ => null //(無視でOK)
                };

                if (notes[pos]!=null&&notes[pos].ParentId != -1)
                {
                    childNotesMap.Add(notes[pos].ParentId, notes[pos]);
                    notes[pos] = null;
                }
            }
            return new OneDivData(notes, launchTime);
        }
        /// <summary>
        /// LongNote(存在位置により指定)のプレス時間を計算 & endNoteのidも返す
        /// </summary>
        /// <param name="beginTime">LongNoteのタップ時間</param>
        /// <param name="barIndex">LongBeginの小節数</param>
        /// <param name="divIndex">LongBeginのDiv</param>
        /// <param name="posIndex">LongBeginのPos</param>
        /// <returns></returns>
        private (float endTime,int id) GetLongEndNoteInfo(SheetData sheetData, int barIndex, int divIndex, int posIndex)
        {
            //指定された位置(小節、div、Pos)にLongが配置されていることを確認　　(一応)
            var type_start = (MakeSheetScene.NoteTypeForSheet)sheetData.barDatas[barIndex].DivDatas[divIndex].GetNotes(IsPlayer)[posIndex][0];
            if (type_start != MakeSheetScene.NoteTypeForSheet.LongBegin)
            {
                Debug.LogError($"この位置のNoteは{type_start}であり、LongBeginではありません");
            }

            //LongBeginの上をLongEndが見つかるまでたどっていく
            //二つのfor分を用いて、div、小節を進んでいく
            for (int bar = barIndex; bar < sheetData.AllBarNum; bar++)
            {

                int initialIndex = bar == barIndex ? divIndex + 1 : 0;

                for (int div = initialIndex; div < sheetData.barDatas[bar].DivDatas.Length; div++)
                {
                    //ここでは、位置を一意に決定できている。
                    //配置されているNoteのTypeを取得
                    var type = (MakeSheetScene.NoteTypeForSheet)sheetData.barDatas[bar].DivDatas[div].GetNotes(IsPlayer)[posIndex][0];
                    int id = sheetData.barDatas[bar].DivDatas[div].GetNotes(IsPlayer)[posIndex][2];

                    //Middleなら、探索を継続
                    if (type == MakeSheetScene.NoteTypeForSheet.LongMiddle)
                    {
                        continue;
                    }

                    //LongEndなら探索終了。プレス時間を決定できる
                    else if (type == MakeSheetScene.NoteTypeForSheet.LongEnd)
                    {
                        return (divTiming[bar][div],id);
                    }

                    //それ以外なら、BeginをEndの間にMiddle以外が存在することとなり、譜面がおかしい
                    else
                    {
                        Debug.LogError($"Longが終了する前に{type}が存在します");
                    }
                }
            }

            //LongBegin以降にEndが見つからない
            Debug.LogError($"Longが終了する前に曲が終了しています");
            return (-1,-1);
        }

        /// <summary>
        /// 楽譜を見て、すべてのdivに対して、クリック時間を記録
        /// </summary>
        /// <param name="sheetData"></param>
        private void DecidingTiming(SheetData sheetData)
        {
            divTiming = new float[sheetData.AllBarNum][];

            //譜面に指定されている値 (for タイミング合わせ)
            float time = sheetData.offset;

            for (int bar = 0; bar < sheetData.AllBarNum; bar++)
            {
                BarData barData = sheetData.barDatas[bar];
                divTiming[bar] = new float[barData.DivDatas.Length];
                for (int div = 0; div < barData.DivDatas.Length; div++)
                {
                    divTiming[bar][div] = time;
                    time += 60 * (4 / barData.BPM) / barData.DivDatas.Length;
                }
            }

        }
        #endregion

        protected override void OnDestroy()
        {

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            NotesPool.NotesPoolManager.Instance.ReturnAll();
            base.OnDestroy();
        }
    }



}
