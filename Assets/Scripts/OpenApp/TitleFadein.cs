using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenApp
{
    public class TitleFadein : MonoBehaviour
    {
        [SerializeField] private CanvasGroup buttonCanvas;
        private Coroutine coroutine;

        // Start is called before the first frame update
        void Start()
        {
            coroutine = StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            yield return new WaitForSeconds(2f);
            this.GetComponent<CanvasGroup>().DOFade(1.0F, 2f);
            yield return new WaitForSeconds(1f);

            Sequence s = DOTween.Sequence();
            s.Append(this.GetComponent<CanvasGroup>().DOFade(1.0F, 2f))
                .AppendInterval(0.5f)
                .Append(buttonCanvas.DOFade(1f, 0.5f)
                .OnComplete(() => buttonCanvas.blocksRaycasts = true));

            s.Play();
        }

        private void OnDestroy()
        {
            if(coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }


    }
}
