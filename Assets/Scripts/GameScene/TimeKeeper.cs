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
        /// ���ׂĂ�Div�Ɋւ��āA���@�����ׂ�����ێ�
        /// </summary>
        private float[][] divTiming;

        private AudioSource source;

        private Coroutine coroutine;

        public LaneInputManager[] Lanes { get => lanes; }

        private Dictionary<int, OneNoteData> childNotesMap;

        /// <summary>
        /// ���t�J�n�Ɍ���������
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

            //���ˎ��Ԃ��v�Z
            DecidingTiming(sheetData);

            childNotesMap = new Dictionary<int, OneNoteData>();

            //�L���[�����
            queue = MakeNoteQueue(sheetData);
        }

        /// <summary>
        /// ���t�J�n���ɌĂ�
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
                                //���˃m�[�c�Ƃ��ă��X�g�ɓo�^
                                NoteController ctrl = NotesPool.NotesPoolManager.Instance.Rent(IsPlayer, item.NoteType);
                                launching.Add((i,ctrl,item));
                            }
                        }





                        foreach (var item in launching)
                        {
                            item.noteCtrl.Launch(spawnTrn[item.noteData.From],I(true).lanesTrn,I(false).lanesTrn, item.noteData, item.noteData.ChildId != -1);
                            Lanes[item.pos].SetAndLaunchNote(item.noteCtrl);
                        }

                        //binder�Ō���
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


                if(queue.Count==0 || source.time<0.05f) //���ׂẴm�[�c���o���؂��� or �Ȃ��I���̂Ƃ��A���[�v�𔲂���
                {
                    break;
                }
                
               
                yield return new WaitForSeconds(1f/60);

                
            }


            //���ׂẴm�[�c�𔭎˂�����@���[���̃m�[�c��������܂ő҂�
            foreach (var item in Lanes)
            {
                yield return new WaitWhile(() => item.NotesOnLane.Count > 0);
            }

            //���[���̃m�[�c���Ȃ��Ȃ������1�b�҂��āA�I����ʒm
            yield return new WaitForSeconds(1);

            if (!IsPlayer)
            {
                coroutine = null;
                yield break;
            }

            Debug.Log("�I�������");
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
                Debug.LogError($"{parentNoteId}��e�Ɏ��qNote��������܂���ł���");
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
                    _ => null //(������OK)
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
        /// LongNote(���݈ʒu�ɂ��w��)�̃v���X���Ԃ��v�Z & endNote��id���Ԃ�
        /// </summary>
        /// <param name="beginTime">LongNote�̃^�b�v����</param>
        /// <param name="barIndex">LongBegin�̏��ߐ�</param>
        /// <param name="divIndex">LongBegin��Div</param>
        /// <param name="posIndex">LongBegin��Pos</param>
        /// <returns></returns>
        private (float endTime,int id) GetLongEndNoteInfo(SheetData sheetData, int barIndex, int divIndex, int posIndex)
        {
            //�w�肳�ꂽ�ʒu(���߁Adiv�APos)��Long���z�u����Ă��邱�Ƃ��m�F�@�@(�ꉞ)
            var type_start = (MakeSheetScene.NoteTypeForSheet)sheetData.barDatas[barIndex].DivDatas[divIndex].GetNotes(IsPlayer)[posIndex][0];
            if (type_start != MakeSheetScene.NoteTypeForSheet.LongBegin)
            {
                Debug.LogError($"���̈ʒu��Note��{type_start}�ł���ALongBegin�ł͂���܂���");
            }

            //LongBegin�̏��LongEnd��������܂ł��ǂ��Ă���
            //���for����p���āAdiv�A���߂�i��ł���
            for (int bar = barIndex; bar < sheetData.AllBarNum; bar++)
            {

                int initialIndex = bar == barIndex ? divIndex + 1 : 0;

                for (int div = initialIndex; div < sheetData.barDatas[bar].DivDatas.Length; div++)
                {
                    //�����ł́A�ʒu����ӂɌ���ł��Ă���B
                    //�z�u����Ă���Note��Type���擾
                    var type = (MakeSheetScene.NoteTypeForSheet)sheetData.barDatas[bar].DivDatas[div].GetNotes(IsPlayer)[posIndex][0];
                    int id = sheetData.barDatas[bar].DivDatas[div].GetNotes(IsPlayer)[posIndex][2];

                    //Middle�Ȃ�A�T�����p��
                    if (type == MakeSheetScene.NoteTypeForSheet.LongMiddle)
                    {
                        continue;
                    }

                    //LongEnd�Ȃ�T���I���B�v���X���Ԃ�����ł���
                    else if (type == MakeSheetScene.NoteTypeForSheet.LongEnd)
                    {
                        return (divTiming[bar][div],id);
                    }

                    //����ȊO�Ȃ�ABegin��End�̊Ԃ�Middle�ȊO�����݂��邱�ƂƂȂ�A���ʂ���������
                    else
                    {
                        Debug.LogError($"Long���I������O��{type}�����݂��܂�");
                    }
                }
            }

            //LongBegin�ȍ~��End��������Ȃ�
            Debug.LogError($"Long���I������O�ɋȂ��I�����Ă��܂�");
            return (-1,-1);
        }

        /// <summary>
        /// �y�������āA���ׂĂ�div�ɑ΂��āA�N���b�N���Ԃ��L�^
        /// </summary>
        /// <param name="sheetData"></param>
        private void DecidingTiming(SheetData sheetData)
        {
            divTiming = new float[sheetData.AllBarNum][];

            //���ʂɎw�肳��Ă���l (for �^�C�~���O���킹)
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
