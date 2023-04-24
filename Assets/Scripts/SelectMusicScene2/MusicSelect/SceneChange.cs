using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChange : MonoBehaviour
{
    public void SceneChangeToSheetMaking()
    {
        MusicFileData musicData = MusicDetail.Instance.MusicData;
        WBTransition.TransitionManager.Instance.StartTransition("MakeSheetScene2",onEndTransition:()=> {
            throw new System.NotImplementedException();
        });
    }

    public void HomeButtonClicked()
    {
        WBTransition.TransitionAssist.ToHome();
    }
}
