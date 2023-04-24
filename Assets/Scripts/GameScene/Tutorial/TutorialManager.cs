using Data;
using MakeSheetScene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Tutorial
{
    public class TutorialManager : MonoBehaviour,ITutorialInitializer
    {
        [SerializeField] private MusicFileData audioClip;
        [SerializeField] private Typing typer;

        public void TutorialStart(MusicFileData musicData, SheetData sheetData)
        {


                SaveSystem.I.Load();
                StartCoroutine(GetUserData.I.GetMusicSheet(1, Course.Easy, sheet => GameObject.Find("GameManager").GetComponent<IGameSceneInitializer>().Initialize(audioClip, Course.Easy, sheet,false,Color.gray)));
                GameObject.Find("GameManager").GetComponent<IGameSceneInitializer>().MusicStart();


            typer.StartTyping();
        }


    }
}
