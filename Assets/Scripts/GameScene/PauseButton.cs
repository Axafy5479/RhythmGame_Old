using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WBTransition;

namespace GameScene
{
    public class PauseButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private GameObject pauseScreen;

        bool pause = false;
        private void Start()
        {
            pauseScreen.SetActive(false);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            pause = true;
            pauseScreen.SetActive(true);
            GameManager.I.Pause(true);
        }

        public void ResumeButtonClicked()
        {
            pause = false;
            pauseScreen.SetActive(false);
            GameManager.I.Pause(false);

        }

        public void QuitButtonClicked()
        {
            Time.timeScale = 1;
            DOTween.KillAll();
            //TransitionManager.Instance.StartTransition("MusicSelectScene");

            GameManager.I.Quit();
        }

        public void TryAgainButtonClicked()
        {

            Time.timeScale = 1;
            DOTween.KillAll();

            GameManager.I.TryAgain();


            //       TransitionManager.Instance.StartTransition("GameScene", 0.3f, 1, 0.5f, null, () => {
            //           GameManager.Instance.LoadChart(musicData, new PlayMode(0, Setting.Instance.auto), Setting.Instance.CharacterData,Setting.Instance.diff);
            //       }
            //, () => GameManager.Instance.play());
        }

    }
}
