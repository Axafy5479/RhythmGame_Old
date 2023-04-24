using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MakeSheetScene {
    public class NoteForDecidingParent : MonoBehaviour
    {
        [SerializeField] private Button button;
        public void Activate(bool clickedNoteIsPlayer,int clickedNoteId,int newParentId)
        {
            button.onClick.AddListener(() =>
            {
                int currentBar = SheetMaker.I.CurrentBar;
                SheetMaker.I.SheetData.barDatas[currentBar].FindChildById_SetNewParent(clickedNoteIsPlayer, clickedNoteId, newParentId);
                SheetMaker.I.SheetData.barDatas[currentBar-1].FindParentById_SetNewChild(!clickedNoteIsPlayer, newParentId, clickedNoteId);
                SheetMaker.I.Save();
                DecideParentWindow.I.Close();

            }
            );
        }
    }
}
