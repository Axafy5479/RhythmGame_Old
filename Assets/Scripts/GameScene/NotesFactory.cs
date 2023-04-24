//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using MakeSheetScene;

//namespace GameScene
//{
//    /// <summary>
//    /// Notesを作るだけ。 プレイヤーの工場もライバルの工場も、作ったNotesはTimeKeeper(Singleton)に渡す
//    /// </summary>
//    public class NotesFactory
//    {

//        public NotesFactory(bool isPlayer,GameObject normalNotePrefab,GameObject longNotePrefab)
//        {
//            NormalNotePrefab = normalNotePrefab;
//            LongNotePrefab = longNotePrefab;
//            IsPlayer = isPlayer;
//        }

//        private GameObject NormalNotePrefab { get; }
//        private GameObject LongNotePrefab { get; }


//        /// <summary>
//        /// 譜面データ
//        /// </summary>
//        private SheetData sheetData { get; set; }

//        /// <summary>
//        /// すべてのDivに関して、いつ叩かれるべきかを保持
//        /// </summary>
//        private List<float> divTiming = new List<float>();



//        private bool IsPlayer { get; }
//        public bool AUTO => IsPlayer;

//        public List<List<Time_NoteCtrl_Pair>> MakeAllNotes(SheetData sheetData, Transform[] makeNotePoints, Transform[] beatPoints)
//        {
//            //notesMap = new Dictionary<int, NoteController>();
//            int totalDiv = 0;

//            //TimeKeeperに渡したい情報(このメソッドを通して作成する)
//            List<List<Time_NoteCtrl_Pair>> noteQueue = new List<List<Time_NoteCtrl_Pair>>();

//            //譜面データ
//            this.sheetData = sheetData;

//            //初めに、すべてのdivの時刻を計算しておく
//            DecidingTiming();

//            //すべての小節
//            for (int barIndex = 0; barIndex < sheetData.AllBarNum; barIndex++)
//            {
//                BarData barData = sheetData.barDatas[barIndex];
//                List<Time_NoteCtrl_Pair> pairs_bar = new List<Time_NoteCtrl_Pair>();

//                //すべてのdiv
//                for (int div = 0; div < barData.DivDatas.Length; div++)
//                {
//                    float beatingTime = divTiming[totalDiv] - 60 * (4 / barData.BPM);
//                    Time_NoteCtrl_Pair pair = new Time_NoteCtrl_Pair(beatingTime);
//                    int[][] notesData = barData.DivDatas[div].GetNotes(IsPlayer);

//                    //すべてのポジション
//                    for (int pos = 0; pos < 4; pos++)
//                    {
//                        int[] note = notesData[pos];
                       
//                        //NoteTypeが normalかlongの時に、Noteを生成
//                        NoteTypeForSheet type = (NoteTypeForSheet)note[0];
//                        int from = note[1];
//                        int id = note[2];
//                        int parent = note[3];
//                        int child = note[4];

//                        NoteController noteCtrl = null;
//                        switch (type)
//                        {
//                            case NoteTypeForSheet.Normal:

//                                //NoteController
//                                noteCtrl = GameObject.Instantiate(NormalNotePrefab, makeNotePoints[from]).GetComponent<NormalNoteController>();
                 
//                                //NoteControllerの初期化
//                                (noteCtrl as NormalNoteController).Initialize(id,from, beatPoints[pos],pos, divTiming[totalDiv] , IsPlayer, parent,child);


                        

//                                if (!IsPlayer)
//                                {
//                                    (noteCtrl as NormalNoteController).BeatTimeRival = divTiming[totalDiv];
//                                }

//                                pair.SetController(pos, noteCtrl);
//                                break;

//                            case NoteTypeForSheet.LongBegin:
//                                //NoteController
//                                noteCtrl = GameObject.Instantiate(LongNotePrefab, makeNotePoints[from]).GetComponent<LongNoteController>();
//                                //notesMap.Add(id, noteCtrl);

//                                //プレス時間の計算
//                                float endTime = CalculatePressTime(totalDiv, barIndex, div, pos);
//                                //NoteControllerの初期化
//                                (noteCtrl as LongNoteController).Initialize(id,from,barData.BPM, beatPoints[pos], pos, divTiming[totalDiv],endTime, IsPlayer, 1,parent);

                          

//                                if (!IsPlayer)
//                                {
//                                    (noteCtrl as LongNoteController).BeatTimeRival = divTiming[totalDiv];
//                                }
//                                pair.SetController(pos, (noteCtrl as LongNoteController));
//                                break;

//                            default:
//                                break;
//                        }

//                        //角度の調節
//                        if (noteCtrl != null)
//                        {
//                            Vector3 dv = beatPoints[pos].position - noteCtrl.transform.position;
//                            float atan = Mathf.Atan(dv.x / dv.y);
//                            float angle = atan * Mathf.Rad2Deg;
//                            noteCtrl.transform.Rotate(new Vector3(0, 0, -1), angle);
//                        }
//                    }
//                    totalDiv++;
//                    pairs_bar.Add(pair);
//                }

//                noteQueue.Add(pairs_bar);
//            }

//            //すべてのNoteCtrlと発車時間のデータをTimeKeeperに送る
//            //TimeKeeper.I.SetNoteCtrlData(IsPlayer,noteQueue);
//            return noteQueue;
//        }


//        /// <summary>
//        /// 楽譜を見て、すべてのdivに対して、クリック時間を記録
//        /// </summary>
//        /// <param name="sheetData"></param>
//        private void DecidingTiming()
//        {
//            //譜面に指定されている値 (for タイミング合わせ)
//            float time = sheetData.offset;

//            foreach (var bar in sheetData.barDatas)
//            {
//                foreach (var div in bar.DivDatas)
//                {
//                    divTiming.Add(time);
//                    time += 60 * (4 / bar.BPM) / bar.DivDatas.Length;
//                }
//            }
//        }



//        /// <summary>
//        /// LongNote(存在位置により指定)のプレス時間を計算する
//        /// </summary>
//        /// <param name="beginTime">LongNoteのタップ時間</param>
//        /// <param name="barIndex">LongBeginの小節数</param>
//        /// <param name="divIndex">LongBeginのDiv</param>
//        /// <param name="posIndex">LongBeginのPos</param>
//        /// <returns></returns>
//        private float CalculatePressTime(int beginTotalDiv,int barIndex,int divIndex,int posIndex)
//        {
//            //指定された位置(小節、div、Pos)にLongが配置されていることを確認　　(一応)
//            var type_start = (MakeSheetScene.NoteTypeForSheet)sheetData.barDatas[barIndex].DivDatas[divIndex].GetNotes(IsPlayer)[posIndex][0];
//            if (type_start!= MakeSheetScene.NoteTypeForSheet.LongBegin)
//            {
//                Debug.LogError($"この位置のNoteは{type_start}であり、LongBeginではありません");
//            }

//            //LongBeginの上をLongEndが見つかるまでたどっていく
//            //二つのfor分を用いて、div、小節を進んでいく
//            int endTotalDiv = beginTotalDiv;
//            for (int bar = barIndex; bar < sheetData.AllBarNum; bar++)
//            {

//                int initialIndex = bar == barIndex ? divIndex + 1 : 0;

//                for (int div = initialIndex; div < sheetData.barDatas[bar].DivDatas.Length; div++)
//                {
//                    endTotalDiv++;
//                    //ここでは、位置を一意に決定できている。
//                    //配置されているNoteのTypeを取得
//                    var type = (MakeSheetScene.NoteTypeForSheet)sheetData.barDatas[bar].DivDatas[div].GetNotes(IsPlayer)[posIndex][0];

//                    //Middleなら、探索を継続
//                    if (type == MakeSheetScene.NoteTypeForSheet.LongMiddle)
//                    {
//                        continue;
//                    }

//                    //LongEndなら探索終了。プレス時間を決定できる
//                    else if(type == MakeSheetScene.NoteTypeForSheet.LongEnd)
//                    {
//                        return divTiming[endTotalDiv];
//                    }

//                    //それ以外なら、BeginをEndの間にMiddle以外が存在することとなり、譜面がおかしい
//                    else
//                    {
//                        Debug.LogError($"Longが終了する前に{type}が存在します");
//                    }
//                }
//            }

//            //LongBegin以降にEndが見つからない
//            Debug.LogError($"Longが終了する前に曲が終了しています");
//            return -1;
//        }

//    }

//    public class Time_NoteCtrl_Pair
//    {
//        public Time_NoteCtrl_Pair( float startTime)
//        {
//            StartTime = startTime;
//            Notes = new NoteController[4];
           
//        }

//        public void SetController(int pos,NoteController note)
//        {
//            Notes[pos] = note;
//        }

//        public float StartTime { get; }
//        public NoteController[] Notes { get; }
//    }
//}
