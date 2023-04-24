using Data;
using MakeSheetScene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MusicSelectScene {
    public class PlayButton :LongClickButton
    {
        [SerializeField] private Image frameImage;
        [SerializeField] private Text playText;
        [SerializeField] private Button playButton;
        [SerializeField] private GameObject notImplemetedPanel;

        private Color DiffCol { get; set; }

        public void Show(Color diffColor)
        {
            notImplemetedPanel.SetActive(false);

            playButton.enabled = true;
            frameImage.color = Color.white;
            playText.color = (Color.white+diffColor)/ 2;
            DiffCol = diffColor;
        }

        public void OriginalCourse()
        {
            notImplemetedPanel.SetActive(false);

            playButton.enabled = false;
            frameImage.color = new Color(0, 0, 0, 0);
            playText.color = new Color(0, 0, 0, 0);

            DiffCol = new Color(0, 0, 0, 0);
        }

        public void NotImplementedCourse()
        {
            notImplemetedPanel.SetActive(true);
            frameImage.color = Color.gray;
            playButton.enabled = false;
            playText.color = Color.gray / 2;
            DiffCol = new Color(0, 0, 0, 0);

        }

        protected override void LongClick()
        {
            if (!playButton.enabled) return;
            StartGettingMusicSheet(true);
        }

        protected override void NormalClick()
        {
            if (!playButton.enabled) return;
            StartGettingMusicSheet(false);
        }

        private void StartGettingMusicSheet(bool auto)
        {
            Course course = Setting.I.Course;
            MusicFileData musicFile = Setting.I.LastPlayedMusic;
            if (course != Course.Original)
            {
                StartCoroutine(GetMusicSheet(Setting.I.LastPlayedMusic, course, auto));
            }
            else
            {
                SheetData sheetData = GetUserData.I.GetUsersSheet(musicFile.MusicId);
                if (sheetData == null)
                {
                    Debug.Log("ïàñ ÇÕçÏê¨Ç≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
                    return;
                }

                StartTransitionToGameScene(musicFile, course, sheetData,auto);
            }
        }

 

        IEnumerator GetMusicSheet(MusicFileData musicFile,Course course,bool auto)
        {

            WWWForm form = new WWWForm();
            form.AddField("userName", SaveSystem.I.GetUserId());

            form.AddField("password", SaveSystem.I.GetPassword());
            form.AddField("musicId", musicFile.MusicId);
            form.AddField("difficulty", (int)course);

            string url = "https://framari.org/GetMusicSheet.php";
            UnityWebRequest uwr = UnityWebRequest.Post(url, form);
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                string jsonData = uwr.downloadHandler.text;
                Debug.Log(jsonData);
                SheetData sheetData = JsonUtility.FromJson<SheetData>(jsonData);


                StartTransitionToGameScene(musicFile, course, sheetData,auto);
            }
        }

        private void StartTransitionToGameScene(MusicFileData musicClip, Course course, SheetData sheetData, bool auto)
        {
            WBTransition.TransitionAssist.ToGameScene(musicClip, course, sheetData, auto,DiffCol);
        }

    }
}
