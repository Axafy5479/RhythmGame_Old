using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeSheetButton : MonoBehaviour
{

    private MusicFileData musicClip;


    [SerializeField] private GameObject notImplementPanel;
    [SerializeField] private Button button;

    public void Show(bool hasClip)
    {
        notImplementPanel.SetActive(!hasClip);
        button.enabled = hasClip;
    }

    internal void SetMusicData(MusicFileData musicClip)
    {
        this.musicClip = musicClip;
        //this.musicData = musicData;
    }

    public void ButtonClicked()
    {
        WBTransition.TransitionAssist.ToMakeScene(musicClip);
    }
}
