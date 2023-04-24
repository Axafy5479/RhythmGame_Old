using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene
{



    public class Recorder : MonoSingleton<Recorder>
    {
        [SerializeField] private TextMeshPro playerScore;
        [SerializeField] private TextMeshPro playerCombo;
        [SerializeField] private TextMeshPro rivalScore;
        [SerializeField] private TextMeshPro rivalCombo;
        [SerializeField] private Slider scoreSlider;

        private Record playerRecord;
        private Record rivalRecord;



        public void Initialize(int musicId,string musicTitle,Course course,MakeSheetScene.SheetData sheetData)
        {
            playerRecord = new Record(musicId, musicTitle, course, true,sheetData.AllNoteNumber_player,sheetData);
            rivalRecord = new Record(musicId, musicTitle, course, false,sheetData.AllNoteNumber_rival, sheetData);

            playerScore.text = "0";
            rivalScore.text = "0";
            playerCombo.text = "0";
            rivalCombo.text = "0";
            scoreSlider.value = 0.5f;
        }

        public void AddRecord(bool isPlayer, int noteId,float time, JudgeEnum judge)
        {
            (isPlayer ? playerRecord : rivalRecord).SetNoteGrade(noteId, time, judge);

            Record r = isPlayer ? playerRecord : rivalRecord;
            TextMeshPro sc = isPlayer ? playerScore : rivalScore;
            TextMeshPro combo = isPlayer ? playerCombo : rivalCombo;

            sc.text = r.Score.ToString();
            combo.text = r.CurrentCombo.ToString();
            ShowJudge.I.Show(isPlayer, judge);
            if (playerRecord.Score + rivalRecord.Score == 0)
            {
                scoreSlider.value = 0.5f;
            }
            else
            {
                scoreSlider.value = playerRecord.Score / (float)(playerRecord.Score + rivalRecord.Score);
            }
        }

        public Record GetRecord(bool isPlayer)
        {
            return isPlayer ? playerRecord : rivalRecord;

         
        }

    }


}
