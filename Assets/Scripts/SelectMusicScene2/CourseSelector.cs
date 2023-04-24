using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MusicSelectScene
{
    public class CourseSelector : MonoBehaviour
    {
        [SerializeField] private Text courseText;
        [SerializeField] private MusicListController listCtrl;
       // [SerializeField] private Image frameImage;
        [SerializeField] private UserDataShower dataShower;
        [SerializeField] private AudioManager audioManager;
        //[SerializeField] private SceneChangeButton playButton;
        [SerializeField] private GameObject originalSheetPanel;
        [SerializeField]private List<CourseButton> courseButtons;
        private MusicFileData[] audioDatas;
        [SerializeField] private PlayButton playButton;
        [SerializeField] private MakeSheetButton makeSheetButton;

        /// <summary>
        /// 前回選択した、Original以外の難易度
        /// </summary>
        internal Course Course { get; private set; }

        //Dictionary<Course, (Image image, TextMeshProUGUI tmp)> button_map ;

        internal void Initialize()
        {
            //button_map = new Dictionary<Course, (Image image, TextMeshProUGUI tmp)>();
            audioDatas = Resources.LoadAll<MusicFileData>("Music");

            for (int i = 0; i < 5; i++)
            {
                Course course = (Course)i;

                //難度のボタンが押されたらCourseを変更し、Settingファイルの「最後に選んだCourse」も変更
                Button courseButton = transform.GetChild(i).GetComponent<Button>();
                //courseButtonImages[i] = transform.GetChild(i).GetComponent<Image>();
                //courseButton.onClick.AddListener(()=> {
                //    if (course != Course.Original)
                //    {
                //        Course = course;
                //    }
                //    MusicAndCourseDecided(true,course);
                //});

                //button_map.Add((Course)i, (courseButton.GetComponent<Image>(), courseButton.GetComponent<TextMeshProUGUI>()));
            }

            //最後に遊んだ難易度に変更
            Course = Setting.I.Course!=Course.Original? Setting.I.Course: Course.Easy;

            listCtrl.MusicChanged.Subscribe(_ => MusicAndCourseDecided(false, Course));
        }

        public void CourseButtonClicked(int courseInt)
        {
            Course course = (Course)courseInt;
            if (course != Course.Original)
            {
                Course = course;
            }
            MusicAndCourseDecided(true, course);
        }


        /// <summary>
        /// 難易度選択ボタンが押された時の処理
        /// </summary>
        /// <param name="buttonClicked">難易度ボタンをクリックしたとき(スクロールで決定していないとき)</param>
        /// <param name="course">押された難易度(Originalでも可)(スクロールで決定していないとき)</param>
        private void MusicAndCourseDecided(bool buttonClicked,Course course)
        {
            //選択した楽曲の難易度等のプロパティを取得
            Data.OneMusic music = listCtrl.SelectedMusic;

            //楽曲の音声ファイルを見つける
            MusicFileData audioData = Array.Find(audioDatas, a => a.MusicId == music.id);

            //最後に遊んだ難易度を記憶
            Setting.I.Course = course;

            //音楽がnullのことがある(修正したい)
            bool hasMusic = false;

            //Originalは記憶しない(毎度original選択画面が表示されるのを防ぐ)
            if (course != Course.Original)
            {
                //難易度表示の変更
                courseText.text = (Course.ToString());

                //音楽を流す
                hasMusic = audioManager.PlayMusic(audioData.Music[Course].Intro, audioData.AudioClip.volume);

                //難易度ボタンの色を変更
                courseButtons.ForEach(b => b.Show(music,hasMusic));

                dataShower.Show(music.id, Course);//, c);

            
            }
            else if(buttonClicked)
            {
                originalSheetPanel.SetActive(true);
            }

            Setting.I.LastPlayedMusic = audioData;

            Difficulty diff = music.GetDifficulty(course);

            makeSheetButton.Show(hasMusic);
            makeSheetButton.SetMusicData(audioData);

            if (course != Course.Original)
            {
                if (diff != Difficulty.NotImplimented && hasMusic)
                {
                    playButton.Show(Setting.diffColor[diff]);
                }
                else
                {
                    playButton.NotImplementedCourse();
                }
            }
            else
            {
                playButton.OriginalCourse();
            }
        }

    }
}
