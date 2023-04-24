using Data;
using MakeSheetScene;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace MusicSelectScene
{
    public class SceneChangeButton : MonoBehaviour
    {
        public void GoHomeButtonClicked()
        {
            WBTransition.TransitionAssist.ToHome();
        }

        public void MakeSceneButtonClicked()
        {
            WBTransition.TransitionAssist.ToMakeScene(Setting.I.LastPlayedMusic);
        }
    }



}
