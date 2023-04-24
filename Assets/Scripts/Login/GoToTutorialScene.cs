using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Login
{
    public class GoToTutorialScene : MonoBehaviour
    {
        [SerializeField] private MusicFileData musicClip;

        internal void StartTutorial()
        {
            StartCoroutine(GetUserData.I.GetMusicSheet(8,Course.Easy,sheet=> WBTransition.TransitionAssist.ToTutorialScene(musicClip,sheet,Color.gray)));
        }
    }
}
