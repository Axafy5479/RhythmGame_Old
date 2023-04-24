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
                        Debug.LogError("SheetMakerのインスタンスが存在しません");
                        return null;
                    }
                    else if (instances.Length > 1)
                    {
                        Debug.LogError("SheetMakerのインスタンスが複数存在します");
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
        /// 譜面作成シーンに遷移した際に、始めに呼ぶ
        /// </summary>
        public IEnumerator Initialize(MusicFileData musicData)
        {
            //譜面のセーブを行うオブジェクトの初期化 (作りかけの譜面の読み込み)
            SheetSaveSystem = new MusicSheetSaveSystem(musicData);

            //読み込んだ譜面の取得
            SheetData = SheetSaveSystem.SheetData;

            //全ノーツの数の表示を更新
            ChangeAllNotesText(true, SheetData.UpdateNoteNumber(true));
            ChangeAllNotesText(false, SheetData.UpdateNoteNumber(false));

            //初めに持っているペンは、playerのペン
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
        /// 保存に使用するデータ
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

                //小節数の表示を変更する
                barText.text = currentBar.ToString();


            }
        }

        public void NextBar()
        {
            CurrentBar++;
        }


        /// <summary>
        /// numberだけ小節を増やす
        /// </summary>
        /// <param name="number">追加開始位置</param>
        public void AddBar(int number,int nextBarNum)
        {
            for (int i = 0; i < number; i++)
            {
                SheetData.AddBar();
            }

            if (nextBarNum>0)
            {
                //追加時のbarの最後のdivにLongNoteBegin or LongNoteMilldeがあったら、LongNoteMiddleをつなげる

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
        /// 現在の小節オブジェクト
        /// </summary>
        public BarObject BarObj { get; private set; } = null;

        /// <summary>
        /// 全ノーツの数の表示を更新する
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
        /// 指定の小節を表示する
        /// </summary>
        /// <param name="barNum">表示したい小節</param>
        private void ShowBar(int barNum)
        {
            //現在の小節の最終divにLongBegin or LongMiddleが存在するか否かを確認
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



            //新しく小節オブジェクトをインスタンス化する
            BarObject barObj = Instantiate(oneBarPrefab, oneBarTrn).GetComponent<BarObject>();

            //sheetDataをもとに、ノーツを配置
            barObj.Initialize(barNum, SheetData.barDatas[barNum]);

            //既に何かしらの小節が表示されている場合(シーン遷移直後以外)それを削除
            if(BarObj!=null) BarObj.DestroyBarObj();

            //現在の小節オブジェクトを更新
            BarObj= barObj;

            //ロングノーツの処理
            playerFingers_havingLong.ForEach(n => LongBeginNoteSet(true, prevBarNum, divNum - 1, n));
            rivalFingers_havingLong.ForEach(n => LongBeginNoteSet(false, prevBarNum, divNum - 1, n));

            //表示
            barObj.Show();
        }

        /// <summary>
        /// 小節移動ボタンが押されたとき
        /// </summary>
        /// <param name="delta">移動回数</param>
        public void ChangeBarButtonClicked(int delta)
        {
            bool bgmPlaying = AudioManager.Instance.IsPlaying;
            if (bgmPlaying)
            {
                AudioManager.Instance.StopButtonClicked();
            }

            //bool isPlaying = audioSource.isPlaying;
            //audioSource.Stop();

            //全小節数
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
        /// BPMが変更された際、以降の小説のBPMも変更し、セーブする
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
            //fromをランダムに決定する
            List<int> fromList = new List<int>() { 0, 1, 2, 3 };
            fromList.Remove(pos);

            (int allNotesNumber,int parentId,int childId) = SheetMaker.I.SheetData.barDatas[barNum].DivDatas[div].Erase(isPlayer, pos);

            if (parentId != -1)
            {
                if (barNum <= 0)
                {
                    Debug.LogError("0小節目のノーツは親ノートを持ちえないはずです");
                }

                SheetData.barDatas[barNum - 1].FindParentById_SetNewChild(isPlayer, parentId, -1);
            }

            if (childId != -1)
            {
                if (barNum >= SheetData.AllBarNum-1)
                {
                    Debug.LogError("最後の小節のノーツは子を持ちえないはずです");
                }

                SheetData.barDatas[barNum - 1].FindParentById_SetNewChild(isPlayer, childId, -1);
            }

            ChangeAllNotesText(isPlayer, allNotesNumber);

            if (barNum == CurrentBar)
            {
                //見た目を変える
                BarObj.DivObjs[div].Notes[pos].CheckColor();
            }
        }


        public void SetMiddleNote(bool isPlayer, int barNum, int div, int pos)
        {
            //fromをランダムに決定する
            List<int> fromList = new List<int>() { 0, 1, 2, 3 };
            fromList.Remove(pos);

            int allNotesNumber = SheetMaker.I.SheetData.barDatas[barNum].DivDatas[div].SetLongMiddle(isPlayer, pos);
            ChangeAllNotesText(isPlayer, allNotesNumber);

            if (barNum == CurrentBar)
            {
                //見た目を変える
                BarObj.DivObjs[div].Notes[pos].CheckColor();
            }
        }

    }
}
