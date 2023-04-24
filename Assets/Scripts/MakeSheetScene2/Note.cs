using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MakeSheetScene
{


    public class Note : MonoBehaviour
    {
        #region Serializable private
        [SerializeField] private CanvasGroup playerNoneCG;
        [SerializeField] private CanvasGroup rivalNoneCG;
        [SerializeField] private CanvasGroup playerNormalCG;
        [SerializeField] private CanvasGroup rivalNormalCG;
        [SerializeField] private CanvasGroup playerLongBeginCG;
        [SerializeField] private CanvasGroup rivalLongBeginCG;
        [SerializeField] private CanvasGroup playerLongMiddleCG;
        [SerializeField] private CanvasGroup rivalLongMiddleCG;
        [SerializeField] private CanvasGroup playerLongEndCG;
        [SerializeField] private CanvasGroup rivalLongEndCG;

        [SerializeField] private Text playerFromText;
        [SerializeField] private Text rivalFromText;
        #endregion

        public int BarNumber { get; private set; }
        public int DivNumber { get; private set; }
        public int Pos { get; private set; }


        public Dictionary<bool, int[]> noteDataMap => new Dictionary<bool, int[]>()
    {
        {true, SheetMaker.I.SheetData.barDatas[BarNumber].DivDatas[DivNumber].GetNotes(true)[Pos] },
        {false, SheetMaker.I.SheetData.barDatas[BarNumber].DivDatas[DivNumber].GetNotes(false)[Pos]}
    };

        private Dictionary<bool, Dictionary<NoteTypeForSheet, (CanvasGroup cg, Text text)>> gridCGMap;

        public void Initialize(int barNumber, int divNumber, int pos)
        {
            gridCGMap = new Dictionary<bool, Dictionary<NoteTypeForSheet, (CanvasGroup, Text)>>()
    {
        {true,new Dictionary<NoteTypeForSheet, (CanvasGroup,Text)>() },
        {false,new Dictionary<NoteTypeForSheet, (CanvasGroup,Text)>() }
    };

            #region Making Dictionary
            gridCGMap[true].Add(NoteTypeForSheet.None, (playerNoneCG, playerFromText));
            gridCGMap[false].Add(NoteTypeForSheet.None, (rivalNoneCG, playerFromText));

            gridCGMap[true].Add(NoteTypeForSheet.Normal, (playerNormalCG, playerFromText));
            gridCGMap[true].Add(NoteTypeForSheet.LongBegin, (playerLongBeginCG, playerFromText));
            gridCGMap[true].Add(NoteTypeForSheet.LongMiddle, (playerLongMiddleCG, playerFromText));
            gridCGMap[true].Add(NoteTypeForSheet.LongEnd, (playerLongEndCG, playerFromText));

            gridCGMap[false].Add(NoteTypeForSheet.Normal, (rivalNormalCG, rivalFromText));
            gridCGMap[false].Add(NoteTypeForSheet.LongBegin, (rivalLongBeginCG, rivalFromText));
            gridCGMap[false].Add(NoteTypeForSheet.LongMiddle, (rivalLongMiddleCG, playerFromText));
            gridCGMap[false].Add(NoteTypeForSheet.LongEnd, (rivalLongEndCG, playerFromText));
            #endregion

            BarNumber = barNumber;
            DivNumber = divNumber;
            Pos = pos;

            CheckColor();
        }

        public void CheckColor()
        {

            ShowElement(noteDataMap[true], true);
            ShowElement(noteDataMap[false], false);
        }




        public void DeleatNote()
        {
            NoteTypeForSheet playerAlreadyType = NoteTypeForSheet.None;
            NoteTypeForSheet rivalAlreadyType = NoteTypeForSheet.None;

            if (noteDataMap[true] != null)
            {
                playerAlreadyType = (NoteTypeForSheet)noteDataMap[true][0];
            }
            if (noteDataMap[false] != null)
            {
                rivalAlreadyType = (NoteTypeForSheet)noteDataMap[false][0];
            }

            if (playerAlreadyType != NoteTypeForSheet.LongMiddle)
            {
                ChangeNote(true, NoteTypeForSheet.None);
                NoteErased(true, BarNumber, DivNumber, Pos, playerAlreadyType);
            }


            if (rivalAlreadyType != NoteTypeForSheet.LongMiddle)
            {
                ChangeNote(false, NoteTypeForSheet.None);
                NoteErased(false, BarNumber, DivNumber, Pos, rivalAlreadyType);
            }
        }
        public void NoteClicked()
        {
            NoteTypeForSheet penType = SheetMaker.I.Pen;
            bool isPlayerPen = SheetMaker.I.IsPlayerPen;
            NoteTypeForSheet alreadyType = NoteTypeForSheet.None;

            if (noteDataMap[isPlayerPen] != null)
            {
                alreadyType = (NoteTypeForSheet)noteDataMap[isPlayerPen][0];
            }



            //ペンが消去でなく、NormalかBeginをクリックした場合、親ノートの追加画面を表示する
            if (penType!= NoteTypeForSheet.None && (alreadyType == NoteTypeForSheet.Normal || alreadyType == NoteTypeForSheet.LongBegin))
            {
                return;
                if (penType != NoteTypeForSheet.LongEnd || alreadyType != NoteTypeForSheet.LongMiddle)
                {
                    int currentBar = SheetMaker.I.CurrentBar;
                    if (currentBar > 0) {
                        DecideParentWindow.I.Show(isPlayerPen, SheetMaker.I.SheetData.barDatas[currentBar - 1], noteDataMap[isPlayerPen][2]);
                       Debug.Log("親決定の画面");
                        return;
                    }
                }
            }



            switch (penType)
            {
                case NoteTypeForSheet.None:
                    if (alreadyType == NoteTypeForSheet.None || alreadyType == NoteTypeForSheet.LongMiddle)
                    {
                        return;
                    }
                    ChangeNote(SheetMaker.I.IsPlayerPen, penType);
                    NoteErased(isPlayerPen, BarNumber, DivNumber, Pos,alreadyType);
                    break;
                case NoteTypeForSheet.Normal:
                    if (alreadyType != NoteTypeForSheet.None)
                    {
                        return;
                    }
                    ChangeNote(SheetMaker.I.IsPlayerPen, penType);

                    break;
                case NoteTypeForSheet.LongBegin:
                    if (alreadyType != NoteTypeForSheet.None)
                    {
                        return;
                    }
                    ChangeNote(SheetMaker.I.IsPlayerPen, penType);
                    LongBeginNoteSet(isPlayerPen, BarNumber, DivNumber, Pos);
                    break;
                case NoteTypeForSheet.LongMiddle:
                    Debug.LogError("ペンで書くことはできないはずです");
                    return;
                case NoteTypeForSheet.LongEnd:
                    if (alreadyType != NoteTypeForSheet.LongMiddle)
                    {
                        return;
                    }
                    ChangeNote(SheetMaker.I.IsPlayerPen, penType);
                    LongEndNoteSet(isPlayerPen, BarNumber, DivNumber, Pos);
                    break;
                case NoteTypeForSheet.BothNormal:
                    NoteTypeForSheet theother_already = NoteTypeForSheet.None;
                    if (noteDataMap[!isPlayerPen] != null)
                    {
                        theother_already = (NoteTypeForSheet)noteDataMap[!isPlayerPen][0];
                    }
                    if (alreadyType != NoteTypeForSheet.None && alreadyType != NoteTypeForSheet.Normal)
                    {
                        return;
                    }
                    if (theother_already != NoteTypeForSheet.None && theother_already != NoteTypeForSheet.Normal)
                    {
                        return;
                    }
                    ChangeNote(SheetMaker.I.IsPlayerPen, penType);

                    break;
                default:
                    throw new System.NotImplementedException();
            }



            SheetMaker.I.Save();
        }

        private void ChangeNote(bool isPlayer, NoteTypeForSheet penType)
        {
            //fromをランダムに決定する
            List<int> fromList = new List<int>() { 0, 1, 2, 3 };
            fromList.Remove(Pos);
            int from1 = fromList[Random.Range(0, fromList.Count)];
            int from2 = fromList[Random.Range(0, fromList.Count)];




            if (penType != NoteTypeForSheet.BothNormal) {
                int id = penType == NoteTypeForSheet.LongMiddle || penType == NoteTypeForSheet.None ? -1 : SheetMaker.I.GenerateNoteId();
                int allNotesNumber = SheetMaker.I.SheetData.barDatas[BarNumber].DivDatas[DivNumber].SetNotes(isPlayer, penType, from1, Pos,id);
                SheetMaker.I.ChangeAllNotesText(isPlayer , allNotesNumber);
            }
            else
            {
                int allNotesNumber_player = SheetMaker.I.SheetData.barDatas[BarNumber].DivDatas[DivNumber].SetNotes(true, NoteTypeForSheet.Normal, from1, Pos, SheetMaker.I.GenerateNoteId());
                int allNotesNumber_rival = SheetMaker.I.SheetData.barDatas[BarNumber].DivDatas[DivNumber].SetNotes(false, NoteTypeForSheet.Normal, from2, Pos, SheetMaker.I.GenerateNoteId());
                SheetMaker.I.ChangeAllNotesText(true, allNotesNumber_player);
                SheetMaker.I.ChangeAllNotesText(false, allNotesNumber_rival);

            }

            //見た目を変える
            CheckColor();
        }



        private void ShowElement(int[] noteData, bool isPlayer)
        {
            NoteTypeForSheet type = (NoteTypeForSheet)noteData[0];


            if (type == NoteTypeForSheet.None)
            {
                Text text = isPlayer ? playerFromText : rivalFromText;
                text.text = "-1";
                text.color = new Color(0, 0, 0, 0);
            }
            else
            {
                gridCGMap[isPlayer][type].cg.alpha = 0;
                if (type == NoteTypeForSheet.LongMiddle || type == NoteTypeForSheet.None)
                {
                    Text text = isPlayer ? playerFromText : rivalFromText;
                    text.text = "-1";
                    text.color = new Color(0, 0, 0, 0);
                }
                else if (type == NoteTypeForSheet.LongEnd)
                {
                    Text text = isPlayer ? playerFromText : rivalFromText;
                    text.text = "Long End";
                    text.color = Color.black;
                }
                else
                {
                    gridCGMap[isPlayer][type].text.text = noteData[1].ToString();
                    gridCGMap[isPlayer][type].text.color = Color.black;
                }
            }

            foreach (var item in gridCGMap[isPlayer])
            {
                gridCGMap[isPlayer][item.Key].cg.alpha = (item.Key == type) ? 1 : 0;
            }
        }

        #region Longの処理
        public SheetData SheetData => SheetMaker.I.SheetData;

        public void LongBeginNoteSet(bool isPlayer, int barNum, int division, int pos)
        {
            SheetMaker.I.LongBeginNoteSet(isPlayer, barNum, division, pos);
        }

        /// <summary>
        /// 余ったLongMiddleを削除する
        /// </summary>
        /// <param name="isPlayer"></param>
        /// <param name="division"></param>
        /// <param name="pos"></param>
        private void LongEndNoteSet(bool isPlayer, int barNumber, int division, int pos)
        {
            for (int i = barNumber; i < SheetData.AllBarNum; i++)
            {
                int initDiv = i == barNumber ? division + 1 : 0;
                for (int div = initDiv; div < SheetData.barDatas[i].DivDatas.Length; div++)
                {
                    int[] note = SheetData.barDatas[i].DivDatas[div].GetNotes(isPlayer)[pos];

                    var type = (NoteTypeForSheet)note[0];
                    if (type == NoteTypeForSheet.LongMiddle || type == NoteTypeForSheet.LongEnd)
                    {
                        //SheetMaker.I.BarObjs[i].DivObjs[div].Notes[pos].ChangeNote(isPlayer, NoteTypeForSheet.None);
                        SheetMaker.I.EraseNote(isPlayer, i, div, pos);
                        //SheetMaker.I.SheetData.barDatas[i].DivDatas[div].SetNotes(isPlayer, NoteTypeForSheet.None, -1, pos);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void NoteErased(bool isPlayer, int bar, int div, int pos, NoteTypeForSheet erasedType)
        {
            if (erasedType == NoteTypeForSheet.LongBegin)
            {
                LongEndNoteSet(isPlayer, bar, div, pos);
            }
            else// if (erasedType == NoteTypeForSheet.LongEnd)
            {
                //LongEndが消され、一個手前がLongMiddle or Beginの時は、さらに伸ばす

                //一個手前
                NoteTypeForSheet prevType = NoteTypeForSheet.None;
                if (div != 0)
                {
                    prevType = (NoteTypeForSheet)SheetData.barDatas[bar].DivDatas[div - 1].GetNotes(isPlayer)[pos][0];

                }
                else if (bar != 0)
                {
                    
                    int shotNumber = SheetData.barDatas[bar - 1].DivDatas.Length;
                    prevType = (NoteTypeForSheet)SheetData.barDatas[bar - 1].DivDatas[shotNumber - 1].GetNotes(isPlayer)[pos][0];
                }

                if (prevType == NoteTypeForSheet.LongMiddle || prevType == NoteTypeForSheet.LongBegin)
                {
                    LongBeginNoteSet(isPlayer, bar, div - 1, pos);
                }
            }
        }
        #endregion
    }
}