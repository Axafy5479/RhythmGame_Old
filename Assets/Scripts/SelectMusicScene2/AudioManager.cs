using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSelectScene
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        private AudioSource source;

        private Coroutine coroutine;

        private void Awake()
        {
            source = this.GetComponent<AudioSource>();
            source.Stop();

        }
        internal bool PlayMusic(AudioClip clip,float volume)
        {
            //音楽ファイルがnullの物があるため(修正したい)
            if (clip == null) return false;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            source.Stop();
            coroutine = StartCoroutine(PlayDelayed(clip, volume));
            return true;
        }

        private IEnumerator PlayDelayed(AudioClip clip,float volume)
        {
            yield return new WaitWhile(() => WBTransition.TransitionManager.Instance.IsTransitioning);
            yield return new WaitForSeconds(0.5f);
            source.clip = clip;
            source.volume = volume; 
            source.Play();
        }
    }
}
