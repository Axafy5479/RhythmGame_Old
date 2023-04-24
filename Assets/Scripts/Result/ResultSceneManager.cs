
using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ResultScene
{
    public class ResultSceneManager : MonoSingleton<ResultSceneManager>,IResultInitializer
    {
        const int MIN_SCORE_FOR_UPLOAD = 9000;

        [SerializeField] private OriginalUploadChecker uploadChecker;

        [SerializeField] private Text scoreText;
        [SerializeField] private Text comboText;
        [SerializeField] private Text perfectText;
        [SerializeField] private Text goodText;
        [SerializeField] private Text missText;

        [SerializeField] private Text musicTitleText;
        [SerializeField] private Text courseText;
        [SerializeField] private GameObject highScoreText;




        public IEnumerator Initialize(Record record)
        {

            musicTitleText.text = record.MusicTitle;
            courseText.text = record.Course.ToString();

            scoreText.text = record.Score.ToString();
            comboText.text = record.MaxCombo.ToString();
            var gradeMap = record.GradeMap;

            perfectText.text = gradeMap[JudgeEnum.Perfect].ToString();
            goodText.text = gradeMap[JudgeEnum.Good].ToString();
            missText.text = gradeMap[JudgeEnum.Miss].ToString();

            CourseUserRecord previousRecord = GetUserData.I.UserData.MusicDatas.Find(m => m.Course == record.Course && m.MusicId == record.MusicId);
            bool highScore = previousRecord != null ? previousRecord.Score < record.Score : record.Score>0;

            highScoreText.SetActive(highScore);

            if (record.Course == Course.Original && record.Score>=8000)
            {
                uploadChecker.ShowConfirmationScreen(record.SheetData, record.MusicId);
            }
            else if(record.Course != Course.Others && record.Course != Course.Original)
            {
                yield return StartCoroutine(Data.GetUserData.I.UpdateUserData(record));
            }

            foreach (var item in Resources.LoadAll<Data.Mission.MissionData_Base>("Missions"))
            {
                if (!Data.GetUserData.I.MissionUserData.Contains(item.MissionId))
                {
                    if (item.Check(Data.GetUserData.I.UserData))
                    {
                        StartCoroutine(Data.GetUserData.I.ChangeMissionClear(item.MissionId,()=>Debug.Log("ミッションクリア")));
                    }
                }
            }

            if (record.Course == Course.Original && record.Score >= MIN_SCORE_FOR_UPLOAD)
            {
                if (record.SheetData.GetSheetId() == "")
                {
                    uploadChecker.ShowConfirmationScreen(record.SheetData, record.MusicId);
                }
            }

   
        }



        public void ToHomeButtonClicked()
        {
            WBTransition.TransitionAssist.ToHome();
        }

  

    }
}
