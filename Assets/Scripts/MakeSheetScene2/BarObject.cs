using MakeSheetScene.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MakeSheetScene
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BarObject : MonoBehaviour
    {
       // [SerializeField] private GameObject oneShotPrefab;
        [SerializeField] private Text bpmText;
        [SerializeField] private Transform shotLayout;
        [SerializeField] private InputField divisionInput;
        private CanvasGroup cg;
        public BarData BarData { get; private set; }
        public int BarNumber { get; private set; }
        public DivObj[] DivObjs { get; private set; }
        private int divisionNumber;

        public void BPMChanged(float BPM)
        {
          
            bpmText.text = BPM.ToString();
        }

        public void Initialize(int barNumber,BarData barData)
        {
            bpmText.text = barData.BPM.ToString();
            cg = this.GetComponent<CanvasGroup>();
            BarNumber = barNumber;

            //プロパティーを呼ぶと、この小説のデータが飛ぶため注意
            divisionNumber = barData.DivDatas.Length;
            MakeDivs(barData);

            Hide();
        }

        public void Show()
        {
            cg.blocksRaycasts = true;
            cg.alpha = 1;
        }

        public void Hide()
        {
            cg.blocksRaycasts = false;
            cg.alpha = 0;
        }

        public int DivisionNumber
        {
            get => divisionNumber;
            set
            {
                foreach (var divs in DivObjs)
                {
                    foreach (var notes in divs.Notes)
                    {
                        notes.DeleatNote();
                    }
                }



                divisionNumber = value;
                divisionInput.text = value.ToString();
                float prevBpm = SheetMaker.I.SheetData.barDatas[BarNumber].BPM;
                //SheetMaker.I.SheetData.barDatas[BarNumber] = new BarData(value, prevBpm);
                SheetMaker.I.SheetData.InitializeOneBar(BarNumber, value);

                foreach (Transform child in shotLayout)
                {
                    Destroy(child.gameObject);
                }


                MakeDivs(SheetMaker.I.SheetData.barDatas[BarNumber]);

                if (BarNumber > 0)
                {
                    //追加時のbarの最後のdivにLongNoteBegin or LongNoteMilldeがあったら、LongNoteMiddleをつなげる

                    int divNum = SheetMaker.I.SheetData.barDatas[BarNumber - 1].DivDatas.Length-1;
                    int[][] playerNotes = SheetMaker.I.SheetData.barDatas[BarNumber - 1].DivDatas[divNum].GetNotes(true);
                    int[][] rivalNotes = SheetMaker.I.SheetData.barDatas[BarNumber - 1].DivDatas[divNum].GetNotes(false);
                    for (int pos = 0; pos < 4; pos++)
                    {
                        NoteTypeForSheet playerType = (NoteTypeForSheet)playerNotes[pos][0];
                        NoteTypeForSheet rivalType = (NoteTypeForSheet)rivalNotes[pos][0];

                        if (playerType == NoteTypeForSheet.LongBegin || playerType == NoteTypeForSheet.LongMiddle)
                        {
                            SheetMaker.I.LongBeginNoteSet(true, BarNumber - 1, divNum, pos);
                        }

                        if (rivalType == NoteTypeForSheet.LongBegin || rivalType == NoteTypeForSheet.LongMiddle)
                        {
                            SheetMaker.I.LongBeginNoteSet(false, BarNumber - 1, divNum, pos);
                        }
                    }
                }

                SheetMaker.I.Save();

            }
        }

        private void MakeDivs(BarData barData)
        {
            DivObjs = new DivObj[barData.DivDatas.Length];
            bpmText.text = "BPM : " + barData.BPM;
            BarData = barData;
           


            for (int i = 0; i < barData.DivDatas.Length; i++)
            {
                // DivObj divObj = Instantiate(oneShotPrefab, shotLayout).GetComponentInChildren<DivObj>();
                DivObj divObj = DivsPoolManager.I.Rent().GetComponent<DivObj>();
                divObj.transform.SetParent(shotLayout, false);
                divObj.Initialize(BarNumber, i);
                divObj.transform.SetAsFirstSibling();
                DivObjs[i] = divObj;
            }


        }


        public void DivisionInputChange()
        {
            try
            {
                int temp = int.Parse(divisionInput.text);
                if (temp == 0)
                {
                    Debug.Log("自然数を入力してください");
                }
                else
                {
                    DivisionNumber = temp;
                }
            }
            catch
            {
                Debug.Log("自然数を入力してください");
            }

        }

        public void DivisionButtonClicked(int d)
        {
            divisionInput.text = d.ToString();
            DivisionNumber = d;
        }


      
        public void DestroyBarObj()
        {
            foreach (var item in DivObjs)
            {
                DivsPoolManager.I.Return(item);
            }

            Destroy(this.gameObject);
        }
    }
}