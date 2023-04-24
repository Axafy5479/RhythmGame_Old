//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using MakeSheetScene;

//namespace GameScene
//{
//    /// <summary>
//    /// Notes����邾���B �v���C���[�̍H������C�o���̍H����A�����Notes��TimeKeeper(Singleton)�ɓn��
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
//        /// ���ʃf�[�^
//        /// </summary>
//        private SheetData sheetData { get; set; }

//        /// <summary>
//        /// ���ׂĂ�Div�Ɋւ��āA���@�����ׂ�����ێ�
//        /// </summary>
//        private List<float> divTiming = new List<float>();



//        private bool IsPlayer { get; }
//        public bool AUTO => IsPlayer;

//        public List<List<Time_NoteCtrl_Pair>> MakeAllNotes(SheetData sheetData, Transform[] makeNotePoints, Transform[] beatPoints)
//        {
//            //notesMap = new Dictionary<int, NoteController>();
//            int totalDiv = 0;

//            //TimeKeeper�ɓn���������(���̃��\�b�h��ʂ��č쐬����)
//            List<List<Time_NoteCtrl_Pair>> noteQueue = new List<List<Time_NoteCtrl_Pair>>();

//            //���ʃf�[�^
//            this.sheetData = sheetData;

//            //���߂ɁA���ׂĂ�div�̎������v�Z���Ă���
//            DecidingTiming();

//            //���ׂĂ̏���
//            for (int barIndex = 0; barIndex < sheetData.AllBarNum; barIndex++)
//            {
//                BarData barData = sheetData.barDatas[barIndex];
//                List<Time_NoteCtrl_Pair> pairs_bar = new List<Time_NoteCtrl_Pair>();

//                //���ׂĂ�div
//                for (int div = 0; div < barData.DivDatas.Length; div++)
//                {
//                    float beatingTime = divTiming[totalDiv] - 60 * (4 / barData.BPM);
//                    Time_NoteCtrl_Pair pair = new Time_NoteCtrl_Pair(beatingTime);
//                    int[][] notesData = barData.DivDatas[div].GetNotes(IsPlayer);

//                    //���ׂẴ|�W�V����
//                    for (int pos = 0; pos < 4; pos++)
//                    {
//                        int[] note = notesData[pos];
                       
//                        //NoteType�� normal��long�̎��ɁANote�𐶐�
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
                 
//                                //NoteController�̏�����
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

//                                //�v���X���Ԃ̌v�Z
//                                float endTime = CalculatePressTime(totalDiv, barIndex, div, pos);
//                                //NoteController�̏�����
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

//                        //�p�x�̒���
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

//            //���ׂĂ�NoteCtrl�Ɣ��Ԏ��Ԃ̃f�[�^��TimeKeeper�ɑ���
//            //TimeKeeper.I.SetNoteCtrlData(IsPlayer,noteQueue);
//            return noteQueue;
//        }


//        /// <summary>
//        /// �y�������āA���ׂĂ�div�ɑ΂��āA�N���b�N���Ԃ��L�^
//        /// </summary>
//        /// <param name="sheetData"></param>
//        private void DecidingTiming()
//        {
//            //���ʂɎw�肳��Ă���l (for �^�C�~���O���킹)
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
//        /// LongNote(���݈ʒu�ɂ��w��)�̃v���X���Ԃ��v�Z����
//        /// </summary>
//        /// <param name="beginTime">LongNote�̃^�b�v����</param>
//        /// <param name="barIndex">LongBegin�̏��ߐ�</param>
//        /// <param name="divIndex">LongBegin��Div</param>
//        /// <param name="posIndex">LongBegin��Pos</param>
//        /// <returns></returns>
//        private float CalculatePressTime(int beginTotalDiv,int barIndex,int divIndex,int posIndex)
//        {
//            //�w�肳�ꂽ�ʒu(���߁Adiv�APos)��Long���z�u����Ă��邱�Ƃ��m�F�@�@(�ꉞ)
//            var type_start = (MakeSheetScene.NoteTypeForSheet)sheetData.barDatas[barIndex].DivDatas[divIndex].GetNotes(IsPlayer)[posIndex][0];
//            if (type_start!= MakeSheetScene.NoteTypeForSheet.LongBegin)
//            {
//                Debug.LogError($"���̈ʒu��Note��{type_start}�ł���ALongBegin�ł͂���܂���");
//            }

//            //LongBegin�̏��LongEnd��������܂ł��ǂ��Ă���
//            //���for����p���āAdiv�A���߂�i��ł���
//            int endTotalDiv = beginTotalDiv;
//            for (int bar = barIndex; bar < sheetData.AllBarNum; bar++)
//            {

//                int initialIndex = bar == barIndex ? divIndex + 1 : 0;

//                for (int div = initialIndex; div < sheetData.barDatas[bar].DivDatas.Length; div++)
//                {
//                    endTotalDiv++;
//                    //�����ł́A�ʒu����ӂɌ���ł��Ă���B
//                    //�z�u����Ă���Note��Type���擾
//                    var type = (MakeSheetScene.NoteTypeForSheet)sheetData.barDatas[bar].DivDatas[div].GetNotes(IsPlayer)[posIndex][0];

//                    //Middle�Ȃ�A�T�����p��
//                    if (type == MakeSheetScene.NoteTypeForSheet.LongMiddle)
//                    {
//                        continue;
//                    }

//                    //LongEnd�Ȃ�T���I���B�v���X���Ԃ�����ł���
//                    else if(type == MakeSheetScene.NoteTypeForSheet.LongEnd)
//                    {
//                        return divTiming[endTotalDiv];
//                    }

//                    //����ȊO�Ȃ�ABegin��End�̊Ԃ�Middle�ȊO�����݂��邱�ƂƂȂ�A���ʂ���������
//                    else
//                    {
//                        Debug.LogError($"Long���I������O��{type}�����݂��܂�");
//                    }
//                }
//            }

//            //LongBegin�ȍ~��End��������Ȃ�
//            Debug.LogError($"Long���I������O�ɋȂ��I�����Ă��܂�");
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
