using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    #region Singleton
    private static MusicPlayer instance;

    public static MusicPlayer Instance
    {
        get
        {
            MusicPlayer[] instances = null;
            if (instance == null)
            {
                instances = FindObjectsOfType<MusicPlayer>();
                if (instances.Length == 0)
                {
                    Debug.LogError("MusicPlayerのインスタンスが存在しません");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError("MusicPlayerのインスタンスが複数存在します");
                    return null;
                }
                else
                {
                    instance = instances[0];
                }
            }
            return instance;
        }
    }
    #endregion


    private Coroutine player;
    private AudioSource bgmSource;

    private void Awake()
    {
        bgmSource = this.GetComponent<AudioSource>();
    }
    public void PlayMusic(MusicFileData musicData)
    {
        player = StartCoroutine(play(musicData));
    }

    private IEnumerator play(MusicFileData musicData)
    {
        yield return new WaitWhile(() => WBTransition.TransitionManager.Instance.IsTransitioning);

        bgmSource.clip = musicData.Music[Setting.I.Course].Intro;
        bgmSource.PlayDelayed(0.5f);
    }
}
