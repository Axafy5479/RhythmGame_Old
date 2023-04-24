using DG.Tweening;
using NotesPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenApp
{
    public class OpenAppManager : MonoBehaviour
    {

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private MusicFileData initialMusic;

        bool endBGM = false;
        private Coroutine coroutine;

        public void GameStartButtonClicked()
        {

            Sequence s = DOTween.Sequence().Append(DOTween.To(() => audioSource.volume, num => audioSource.volume = num, 0, 1f))
                .OnComplete(()=>endBGM = true);

            s.Play();
            var _ = NotesPoolManager.Instance;

            if (SaveSystem.I.CheckNewUserData())
            {
                Setting.I.LastPlayedMusic = initialMusic;
                coroutine = StartCoroutine(NewUserTransition());
            }
            else
            {
                coroutine = StartCoroutine(GetDataAndTransition());
            }
         

        }





        private IEnumerator GetDataAndTransition()
        {
            yield return StartCoroutine(Data.GetUserData.I.GetData());
            yield return new WaitWhile(() => !endBGM);

            //登録済みユーザー
            WBTransition.TransitionAssist.ToHome();
        }

        private IEnumerator NewUserTransition()
        {
            yield return new WaitWhile(() => !endBGM);
            //新規のユーザー
            WBTransition.TransitionManager.Instance.StartTransition("Login");
        }

        public void DeleteData()
        {
            PlayerPrefs.DeleteAll();
        }

        private void OnDestroy()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}
