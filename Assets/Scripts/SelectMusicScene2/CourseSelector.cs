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
        /// �O��I�������AOriginal�ȊO�̓�Փx
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

                //��x�̃{�^���������ꂽ��Course��ύX���ASetting�t�@�C���́u�Ō�ɑI��Course�v���ύX
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

            //�Ō�ɗV�񂾓�Փx�ɕύX
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
        /// ��Փx�I���{�^���������ꂽ���̏���
        /// </summary>
        /// <param name="buttonClicked">��Փx�{�^�����N���b�N�����Ƃ�(�X�N���[���Ō��肵�Ă��Ȃ��Ƃ�)</param>
        /// <param name="course">�����ꂽ��Փx(Original�ł���)(�X�N���[���Ō��肵�Ă��Ȃ��Ƃ�)</param>
        private void MusicAndCourseDecided(bool buttonClicked,Course course)
        {
            //�I�������y�Ȃ̓�Փx���̃v���p�e�B���擾
            Data.OneMusic music = listCtrl.SelectedMusic;

            //�y�Ȃ̉����t�@�C����������
            MusicFileData audioData = Array.Find(audioDatas, a => a.MusicId == music.id);

            //�Ō�ɗV�񂾓�Փx���L��
            Setting.I.Course = course;

            //���y��null�̂��Ƃ�����(�C��������)
            bool hasMusic = false;

            //Original�͋L�����Ȃ�(���xoriginal�I����ʂ��\�������̂�h��)
            if (course != Course.Original)
            {
                //��Փx�\���̕ύX
                courseText.text = (Course.ToString());

                //���y�𗬂�
                hasMusic = audioManager.PlayMusic(audioData.Music[Course].Intro, audioData.AudioClip.volume);

                //��Փx�{�^���̐F��ύX
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
