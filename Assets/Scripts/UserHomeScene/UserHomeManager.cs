using Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UserHomeScene
{
    public class UserHomeManager : MonoSingleton<UserHomeManager>,IInitializer
    {
        [SerializeField] private TextMeshProUGUI userNameText;
        [SerializeField] private Image userColorRankFill;
        [SerializeField] private Image userColorRankFrame;
        [SerializeField] private TextMeshProUGUI scoreSumText;
        [SerializeField] private TextMeshProUGUI percentageText;





        public void MusicSelectSceneButton()
        {
            WBTransition.TransitionAssist.ToMusicSelect();
        }
        public IEnumerator Initialize()
        {

            yield return StartCoroutine(GetData());
        }

        private IEnumerator GetData()
        {
            userNameText.text = SaveSystem.I.UserId;
           // yield return StartCoroutine(GetUserData.I.GetUserData_Connect());
            Data.UserData userData = GetUserData.I.UserData;

            userNameText.color = Setting.diffColor[(Difficulty)userData.ColorRank];
            userColorRankFrame.color = Setting.diffColor[(Difficulty)userData.ColorRank];
            userColorRankFill.color = Setting.diffColor[(Difficulty)userData.ColorRank];
            userColorRankFill.fillAmount = userData.CurColPercentage / 100;
            scoreSumText.text = userData.ScoreSum.ToString();
            percentageText.text = userData.Percentage.ToString("f1")+"%";

            //  yield return StartCoroutine(GetUserData.I.GetAllMissionState());
            yield return null;





           

      

            
























        }


    }
}
