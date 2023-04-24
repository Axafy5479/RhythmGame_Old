using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MusicSelectScene
{
    public class CourseButton : MonoBehaviour
    {
        [SerializeField] private Image buttonFrame;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private GameObject notImplementedPanel;
        [SerializeField] private Button button;
        [SerializeField] private Course course;

        public void Show(Data.OneMusic music,bool hasMusic)
        {
            Difficulty diff = music.GetDifficulty(course);
            if (diff != Difficulty.NotImplimented && hasMusic)
            {
                button.enabled = true;
                buttonFrame.color = Setting.diffColor[diff];
                notImplementedPanel.SetActive(false);
                buttonText.color = (Color.white + Setting.diffColor[diff]) / 2;
            }
            else
            {
                button.enabled = false;
                buttonFrame.color = Color.black;
                notImplementedPanel.SetActive(true);
                buttonText.color = Color.gray;
            }
        }

    }
}
