using Coffee.UIExtensions;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WBTransition
{
    public class TransitionManager : MonoBehaviour
    {
        #region Singleton
        private static TransitionManager instance;
        public static TransitionManager Instance
        {
            get
            {
                if (instance == null) instance = FindObjectOfType<TransitionManager>();
                if (instance == null) instance = Instantiate(Resources.Load<GameObject>("TransitionManager"))
                                                      .GetComponent<TransitionManager>();
                return instance;
            }
        }
        #endregion

        [SerializeField] private GameObject filter;
        private RectTransform unmaskRect;
        private bool isWaiting = false;
        private Vector2 initSize;
        private List<GameObject> decoInstances = new List<GameObject>();

        public bool IsTransitioning { get; private set; } = false;


        private void Awake()
        {
            unmaskRect = this.GetComponentInChildren<Unmask>().GetComponent<RectTransform>();
            initSize = unmaskRect.sizeDelta;
            filter.SetActive(false);
            DontDestroyOnLoad(this.gameObject);
        }

        public void StartTransition(string newScene ,float closeTime = 0.5f
            ,float waitTime = 0.1f,float openTime = 0.5f,List<GameObject> decorations = null, Func<IEnumerator> onEndTransition = null,Action onEndRemoveMask = null)
        {
            IsTransitioning = true;
            filter.SetActive(true);

            KeyInputAssist.Instance.Enable = false;

            if (decorations != null)
            {
                foreach (var deco in decorations)
                {
                    decoInstances.Add(Instantiate(deco, this.transform));
                }
            }

            
            Sequence sequence = DOTween.Sequence()
                //マスクを小さくする
                .Append(unmaskRect.DOSizeDelta(Vector2.zero,closeTime).SetEase(Ease.OutQuad))

                //シーン遷移開始
                .AppendCallback(()=> {
                    isWaiting = true;
                    StartCoroutine(Transition(newScene, openTime,onEndTransition,onEndRemoveMask));
                })

                //指定時間背景を表示
                .AppendInterval(waitTime)

                //指定時間待ったことを通知
                .OnComplete(()=>isWaiting = false);
            sequence.Play();
        }

        private IEnumerator Transition(string newScene,float openTime,Func<IEnumerator> onEndTransition,Action onEndRemoveMask)
        {
            yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);

            if (isWaiting) yield return new WaitWhile(() => isWaiting);
            if(onEndTransition!=null)yield return StartCoroutine(onEndTransition());
            yield return new WaitForSeconds(1);
            unmaskRect.DOSizeDelta(initSize, openTime).SetEase(Ease.InQuad)
                .OnComplete(() => { 
                    KeyInputAssist.Instance.Enable = true;

                    //装飾を削除したい
                    foreach (var deco in decoInstances)
                    {
                        Destroy(deco);
                    }
                    if (onEndRemoveMask != null) onEndRemoveMask();
                    filter.SetActive(false);

                    IsTransitioning = false;
                }) ;
            Application.targetFrameRate = (newScene == "GameScene_tutorial"|| newScene == "GameScene2" || newScene == "MakeSheetScene2") ?60:30;

        }


    }
}
