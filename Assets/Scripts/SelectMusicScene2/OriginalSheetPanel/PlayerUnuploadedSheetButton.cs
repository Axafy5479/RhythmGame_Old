using Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MusicSelectScene.OriginalSheetPanel
{
    public class PlayerUnuploadedSheetButton : MonoBehaviour
    {
        //[SerializeField] private TextMeshProUGUI makerName;
        [SerializeField] private Image frameSprite;
        private MakeSheetScene.SheetData SheetData { get; set; }
        private Color DiffCol { get; set; }
        public void InitializeForUnuploaded(MakeSheetScene.SheetData sheetData)
        {
            //makerName.color = Setting.diffColor[(Difficulty)GetUserData.I.UserData.ColorRank];
            frameSprite.color = Setting.diffColor[(Difficulty)GetUserData.I.UserData.ColorRank];
            DiffCol = Setting.diffColor[(Difficulty)GetUserData.I.UserData.ColorRank];
            SheetData = sheetData;
        }

        public void PlayButtonClicked()
        {
            MusicFileData musicClip = Setting.I.LastPlayedMusic;

            WBTransition.TransitionAssist.ToGameScene(musicClip, Course.Original, SheetData, false,DiffCol);
        }
    }
}
