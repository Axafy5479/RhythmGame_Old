using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Data;
using UnityEngine.UI;

namespace MusicSelectScene.OriginalSheetPanel
{
    public class OriginalSheetButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI makerName;
        [SerializeField] private Image frameSprite;
        
        private OthersOriginalSheet SheetInfo { get; set; }
        private Color DiffCol { get; set; }


        public void Initialize(OthersOriginalSheet sheetInfo)
        {
            SheetInfo = sheetInfo;
            makerName.text = sheetInfo.UserName;
            makerName.color = Setting.diffColor[(Difficulty)sheetInfo.UserColor];
            frameSprite.color = Setting.diffColor[(Difficulty)sheetInfo.UserColor];
            DiffCol = Setting.diffColor[(Difficulty)GetUserData.I.UserData.ColorRank];
        }

        public void PlayButtonClicked()
        {
            MusicFileData musicClip = Setting.I.LastPlayedMusic;

            StartCoroutine(GetUserData.I.GetOthersOriginalSheet(SheetInfo.SheetId,(sheet) => WBTransition.TransitionAssist.ToGameScene(musicClip, Course.Others, sheet, false,DiffCol)));
        }
    }
}
