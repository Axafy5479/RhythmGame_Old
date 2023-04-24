using Data;
using Michsky.UI.ModernUIPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MakeSheetScene
{
    public class SheetMaker : MonoBehaviour, IMakeSheetInitializer
    {
        #region Singleton
        private static SheetMaker instance;

        public static SheetMaker I
        {
            get
            {
                SheetMaker[] instances = null;
                if (instance == null)
                {
                    instances = FindObjectsOfType<SheetMaker>();
                    if (instances.Length == 0)
                    {
                        Debug.LogError("SheetMaker�̃C���X�^���X�����݂��܂���");
                        return null;
                    }
                    else if (instances.Length > 1)
                    {
                        Debug.LogError("SheetMaker�̃C���X�^���X���������݂��܂�");
                        return null;
                    }
                    else
                    {
                        instance = instances[0];
                    }
                }
                return instance;
            }
        }
        #endregion

        [SerializeField] private GameObject oneShotPrefab;
        [SerializeField] private Transform oneBarTrn;
        [SerializeField] private GameObject oneBarPrefab;
        [SerializeField] private Toggle playerPen;
        [SerializeField] private Text barText;
        [SerializeField] private CanvasGroup menuPanelCG;
        [SerializeField] private ModalWindowManager panel;

        [SerializeField]private Text allNoteNumText_player;
        [SerializeField] private Text allNoteNumText_rival;

        [SerializeField] private Image normalImagePlayer;
        [SerializeField] private Image longBeginImagePlayer;
        [SerializeField] private Image longEndImagePlayer;

        [SerializeField] private MusicFileData dataForDebugging;


        /// <summary>
        /// ���ʍ쐬�V�[���ɑJ�ڂ����ۂɁA�n�߂ɌĂ�
        /// </summary>
        public IEnumerator Initialize(MusicFileData musicData)
        {
            //���ʂ̃Z�[�u���s���I�u�W�F�N�g�̏����� (��肩���̕��ʂ̓ǂݍ���)
            SheetSaveSystem = new MusicSheetSaveSystem(musicData);

            //�ǂݍ��񂾕��ʂ̎擾
            SheetData = SheetSaveSystem.SheetData;

            //�S�m�[�c�̐��̕\�����X�V
            ChangeAllNotesText(true, SheetData.UpdateNoteNumber(true));
            ChangeAllNotesText(false, SheetData.UpdateNoteNumber(false));

            //���߂Ɏ����Ă���y���́Aplayer�̃y��
            playerPen.isOn = true;
            PlayerPenToggleChanged();

            CurrentBar = 0;
            ShowBar(0);
            AudioManager.Instance.Initialize(musicData);
            yield return null;
        }




        //private void Start()
        //{
        //    if (Application.platform == RuntimePlatform.WindowsEditor)
        //    {
        //        SaveSystem.I.Load();
        //        StartCoroutine(Initialize(dataForDebugging));
        //    }
        //}





        private MusicSheetSaveSystem SheetSaveSystem { get; set; } 
        /// <summary>
        /// �ۑ��Ɏg�p����f�[�^
        /// </summary>
        public SheetData SheetData { get; private set; }
        public void Save()
        {
            SheetSaveSystem.Save();
        }


        public int GenerateNoteId()
        {
            return SheetData.GetNoteId();
        }

        private int currentBar = 0;
        public int CurrentBar
        {
            get => currentBar; 
            private set
            {
                if (value >= SheetData.AllBarNum) return;

                if (AudioManager.Instance.IsPlaying&&value-currentBar==1)
                {
                    StartCoroutine(ShowNextBar());
                }
                else
                {
                    ShowBar(value);
                }
                currentBar = value;

                //���ߐ��̕\����ύX����
                barText.text = currentBar.ToString();


            }
        }

        public void NextBar()
        {
            CurrentBar++;
        }


        /// <summary>
        /// number�������߂𑝂₷
        /// </summary>
        /// <param name="number">�ǉ��J�n�ʒu</param>
        public void AddBar(int number,int nextBarNum)
        {
            for (int i = 0; i < number; i++)
            {
                SheetData.AddBar();
            }

            if (nextBarNum>0)
            {
                //�ǉ�����bar�̍Ō��div��LongNoteBegin or LongNoteMillde����������ALongNoteMiddle���Ȃ���

                int divNum = SheetData.barDatas[nextBarNum - 1].DivDatas.Length - 1;
                int[][] playerNotes = SheetData.barDatas[nextBarNum - 1].DivDatas[divNum].GetNotes(true);
                int[][] rivalNotes = SheetData.barDatas[nextBarNum - 1].DivDatas[divNum].GetNotes(false);
                for (int pos = 0; pos < 4; pos++)
                {
                    NoteTypeForSheet type = (NoteTypeForSheet)playerNotes[pos][0];
                    if (type == NoteTypeForSheet.LongBegin || type == NoteTypeForSheet.LongMiddle)
                    {
                        LongBeginNoteSet(true, nextBarNum - 1, divNum, pos);
                    }

                    NoteTypeForSheet rivalType = (NoteTypeForSheet)rivalNotes[pos][0];
                    if (rivalType == NoteTypeForSheet.LongBegin || rivalType == NoteTypeForSheet.LongMiddle)
                    {
                        LongBeginNoteSet(false, nextBarNum - 1, divNum, pos);
                    }
                }

            }


            Save();
        }



        public void LongBeginNoteSet(bool isPlayer,int barNum,int divNum,int pos)
        {
            for (int i = barNum; i < SheetData.AllBarNum; i++)
            {
                int initDiv = i == barNum ? divNum + 1 : 0;
                for (int div = initDiv; div < SheetData.barDatas[i].DivDatas.Length; div++)
                {
                    int[] note = SheetData.barDatas[i].DivDatas[div].GetNotes(isPlayer)[pos];



                    if ((NoteTypeForSheet)note[0] == NoteTypeForSheet.None)
                    {
                        SetMiddleNote(isPlayer,i,div,pos);
                        //SheetMaker.I.SheetData.barDatas[i].DivDatas[div].SetNotes(isPlayer, NoteTypeForSheet.LongMiddle, -1, pos);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        public NoteTypeForSheet Pen { get; private set; } = NoteTypeForSheet.Normal;
        public bool IsPlayerPen => playerPen.isOn;


        public void PlayerPenToggleChanged()
        {
            if (playerPen.isOn)
            {
                normalImagePlayer.color = new Color(0.1764f,0.6862f,0.3333f);
                longBeginImagePlayer.color = new Color(0,0.7411f,1);
                longEndImagePlayer.color = new Color(0,0.5686f,1);
            }
            else
            {
                normalImagePlayer.color = new Color(0.5493f,0.1764f,0.6862f);
                longBeginImagePlayer.color = new Color(1, 0.3176f, 0.6509f);
                longEndImagePlayer.color = Color.red;
            }
        }

        public void ChangePen(int noteType)
        {
            Pen = (NoteTypeForSheet)noteType;
        }



        /// <summary>
        /// ���݂̏��߃I�u�W�F�N�g
        /// </summary>
        public BarObject BarObj { get; private set; } = null;

        /// <summary>
        /// �S�m�[�c�̐��̕\�����X�V����
        /// </summary>
        /// <param name="isPlayer"></param>
        /// <param name="num"></param>
        public void ChangeAllNotesText(bool isPlayer,int num)
        {
            (isPlayer ? allNoteNumText_player : allNoteNumText_rival).text = num.ToString();
        }



        public void MenuButtonClicked()
        {
            //menuPanelCG.alpha = 1;
            //menuPanelCG.blocksRaycasts = true;
            panel.OpenWindow();
        }

        public void MenuCloseButtonClicked()
        {
            //menuPanelCG.alpha = 0;
            //menuPanelCG.blocksRaycasts = false;
            panel.CloseWindow();
        }

        public void QuitButtonClicked()
        {
            WBTransition.TransitionAssist.ToMusicSelect();
        }



        private (BarObject cur, BarObject next) BarObjsForPlay = (null, null);
        public void OnStopAudio()
        {
            if(BarObjsForPlay.cur !=null)Destroy(BarObjsForPlay.cur.gameObject);
            if(BarObjsForPlay.next!=null)Destroy(BarObjsForPlay.next.gameObject);
        
            ShowBar(CurrentBar);
        }
        private IEnumerator ShowNextBar()
        {
            if(BarObjsForPlay.cur == null)
            {
                BarObjsForPlay.cur = BarObj;

                if (currentBar + 1 < SheetData.AllBarNum)
                {
                    BarObject barObj = Instantiate(oneBarPrefab, oneBarTrn).GetComponent<BarObject>();
                    barObj.Initialize(currentBar + 1, SheetData.barDatas[currentBar + 1]);
                    barObj.gameObject.name = (currentBar + 1).ToString();
                    BarObjsForPlay.next = barObj;
                }
            }

            if (BarObjsForPlay.next == null)
            {
                if (currentBar + 1 < SheetData.AllBarNum)
                {
                    BarObject barObj = Instantiate(oneBarPrefab, oneBarTrn).GetComponent<BarObject>();
                    barObj.Initialize(currentBar + 1, SheetData.barDatas[currentBar + 1]);
                    barObj.gameObject.name = (currentBar + 1).ToString();
                    BarObjsForPlay.next = barObj;
                }
            }


            int nextBar = CurrentBar + 1;
            if (SheetData.AllBarNum <= nextBar) yield break;

            BarObject cur = null;
            BarObject next = null;
            BarObjsForPlay.cur.Hide();

            cur = BarObjsForPlay.next;
            BarObjsForPlay.next.Show();
            yield return new WaitForSeconds(0.2f);
            if (BarObjsForPlay.cur != null) BarObjsForPlay.cur.DestroyBarObj();

            if (nextBar+1 < SheetData.AllBarNum)
            {
                BarObject barObj = Instantiate(oneBarPrefab, oneBarTrn).GetComponent<BarObject>();
                barObj.Initialize(nextBar+1, SheetData.barDatas[nextBar+1]);
                barObj.gameObject.name = (nextBar+1).ToString();
                next = barObj;
            }
            BarObjsForPlay = (cur,next);
        }



        /// <summary>
        /// �w��̏��߂�\������
        /// </summary>
        /// <param name="barNum">�\������������</param>
        private void ShowBar(int barNum)
        {
            //���݂̏��߂̍ŏIdiv��LongBegin or LongMiddle�����݂��邩�ۂ����m�F
            List<int> playerFingers_havingLong = new List<int>();
            List<int> rivalFingers_havingLong = new List<int>();
            int prevBarNum = -1;
            int divNum = -1;

            if (barNum > 0)
            {
                prevBarNum = BarObj.BarNumber;
                divNum = BarObj.BarData.DivDatas.Length;

                for (int i = 0; i < 4; i++)
                {
                    int pnote = BarObj.BarData.DivDatas[divNum-1].GetNotes(true)[i][0];
                    int rnote = BarObj.BarData.DivDatas[divNum-1].GetNotes(false)[i][0];

                    if (pnote == 2 || pnote == 3) playerFingers_havingLong.Add(i);
                    if (rnote == 2 || rnote == 3) rivalFingers_havingLong.Add(i);

                }
            }



            //�V�������߃I�u�W�F�N�g���C���X�^���X������
            BarObject barObj = Instantiate(oneBarPrefab, oneBarTrn).GetComponent<BarObject>();

            //sheetData�����ƂɁA�m�[�c��z�u
            barObj.Initialize(barNum, SheetData.barDatas[barNum]);

            //���ɉ�������̏��߂��\������Ă���ꍇ(�V�[���J�ڒ���ȊO)������폜
            if(BarObj!=null) BarObj.DestroyBarObj();

            //���݂̏��߃I�u�W�F�N�g���X�V
            BarObj= barObj;

            //�����O�m�[�c�̏���
            playerFingers_havingLong.ForEach(n => LongBeginNoteSet(true, prevBarNum, divNum - 1, n));
            rivalFingers_havingLong.ForEach(n => LongBeginNoteSet(false, prevBarNum, divNum - 1, n));

            //�\��
            barObj.Show();
        }

        /// <summary>
        /// ���߈ړ��{�^���������ꂽ�Ƃ�
        /// </summary>
        /// <param name="delta">�ړ���</param>
        public void ChangeBarButtonClicked(int delta)
        {
            bool bgmPlaying = AudioManager.Instance.IsPlaying;
            if (bgmPlaying)
            {
                AudioManager.Instance.StopButtonClicked();
            }

            //bool isPlaying = audioSource.isPlaying;
            //audioSource.Stop();

            //�S���ߐ�
            int allBarNum = SheetData.AllBarNum;


            int temp = CurrentBar + delta;
            if (temp >= allBarNum)
            {
                int exceed = temp + 1 - allBarNum;
                AddBar(exceed, allBarNum);
                //BPMChanged();
            }
            else if (temp < 1) temp = 0;

            CurrentBar = temp;

            if (bgmPlaying)
            {
                AudioManager.Instance.StartButtonClicked();
            }
            //audioSource.time = BarObjs[CurrentBar];
            //if (isPlaying) audioSource.Play();
        }

        /// <summary>
        /// BPM���ύX���ꂽ�ہA�ȍ~�̏�����BPM���ύX���A�Z�[�u����
        /// </summary>
        /// <param name="barNumber"></param>
        /// <param name="BPM"></param>
        public void BPMChanged(int barNumber,float BPM)
        {
            for (int i = barNumber; i < SheetData.barDatas.Count; i++)
            {
                SheetData.barDatas[i].BPM = BPM;
            }

            if (barNumber <= currentBar)
            {
                BarObj.BPMChanged(BPM);
            }
            SheetSaveSystem.Save();
        }




        public void EraseNote(bool isPlayer, int barNum, int div, int pos)
        {
            //from�������_���Ɍ��肷��
            List<int> fromList = new List<int>() { 0, 1, 2, 3 };
            fromList.Remove(pos);

            (int allNotesNumber,int parentId,int childId) = SheetMaker.I.SheetData.barDatas[barNum].DivDatas[div].Erase(isPlayer, pos);

            if (parentId != -1)
            {
                if (barNum <= 0)
                {
                    Debug.LogError("0���ߖڂ̃m�[�c�͐e�m�[�g���������Ȃ��͂��ł�");
                }

                SheetData.barDatas[barNum - 1].FindParentById_SetNewChild(isPlayer, parentId, -1);
            }

            if (childId != -1)
            {
                if (barNum >= SheetData.AllBarNum-1)
                {
                    Debug.LogError("�Ō�̏��߂̃m�[�c�͎q���������Ȃ��͂��ł�");
                }

                SheetData.barDatas[barNum - 1].FindParentById_SetNewChild(isPlayer, childId, -1);
            }

            ChangeAllNotesText(isPlayer, allNotesNumber);

            if (barNum == CurrentBar)
            {
                //�����ڂ�ς���
                BarObj.DivObjs[div].Notes[pos].CheckColor();
            }
        }


        public void SetMiddleNote(bool isPlayer, int barNum, int div, int pos)
        {
            //from�������_���Ɍ��肷��
            List<int> fromList = new List<int>() { 0, 1, 2, 3 };
            fromList.Remove(pos);

            int allNotesNumber = SheetMaker.I.SheetData.barDatas[barNum].DivDatas[div].SetLongMiddle(isPlayer, pos);
            ChangeAllNotesText(isPlayer, allNotesNumber);

            if (barNum == CurrentBar)
            {
                //�����ڂ�ς���
                BarObj.DivObjs[div].Notes[pos].CheckColor();
            }
        }

    }
}
