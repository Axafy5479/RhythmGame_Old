using Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MusicSelectScene.OriginalSheetPanel
{
    public class PlayerUploadedSheetButton : MonoBehaviour
    {
        //[SerializeField] private TextMeshProUGUI makerName;
        [SerializeField] private Image frameSprite;
        private MakeSheetScene.SheetData SheetData { get; set; }
        private int MusicId { get; set; }
        private string SheetId { get; set; }
        private int SheetNumber { get; set; }

        private Color DiffCol{get;set;}



        public void InitializeForUploaded(int musicId,string sheetId,int sheetNumber)
        {
            //makerName.color = Setting.diffColor[(Difficulty)GetUserData.I.UserData.ColorRank];
            frameSprite.color = Setting.diffColor[(Difficulty)GetUserData.I.UserData.ColorRank];
            DiffCol = Setting.diffColor[(Difficulty)GetUserData.I.UserData.ColorRank];
            MusicId = musicId;
            SheetId = sheetId;
            SheetNumber = sheetNumber;
        }

        public void PlayButtonClicked()
        {
            MusicFileData musicClip = Setting.I.LastPlayedMusic;

            StartCoroutine(GetUserData.I.GetOthersOriginalSheet(SheetId, (sheet) => WBTransition.TransitionAssist.ToGameScene(musicClip, Course.Others, sheet, false,DiffCol)));
        }
    }
}
