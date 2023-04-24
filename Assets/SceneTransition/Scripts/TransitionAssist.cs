using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WBTransition
{
    public static class TransitionAssist
    {
        public static@void ToTitle()
        {
            Input.multiTouchEnabled = false;
            TransitionManager.Instance.StartTransition("OpenAppScene");
        }

        public static void ToHome()
        {
            Input.multiTouchEnabled = false;

            //WBTransition.TransitionManager.Instance.StartTransition("UserHome", onEndTransition: ()=> { return UserHomeScene.UserHomeManager.I.Initialize(); });

            TransitionManager.Instance.StartTransition("UserHome", onEndTransition: () => { return GameObject.Find("_Initializer").GetComponent<IInitializer>().Initialize(); });
        }

        public static void ToMusicSelect()
        {
            Input.multiTouchEnabled = false;

            TransitionManager.Instance.StartTransition("MusicSelectScene", onEndTransition: () => { return GameObject.Find("_Initializer").GetComponent<IInitializer>().Initialize(); });//, onEndTransition: () => { return MusicList.I.Initialize(); });
        }

        public static void ToResult(Record record)
        {
            Input.multiTouchEnabled = false;

            TransitionManager.Instance.StartTransition("ResultScene", onEndTransition: () => { return GameObject.Find("_Initializer").GetComponent<IResultInitializer>().Initialize(record); });
        }

        public static void ToGameScene(MusicFileData musicData, Course course, MakeSheetScene.SheetData sheetData,bool auto, Color diffCol)
        {
            Input.multiTouchEnabled = true;

            TransitionManager.Instance.StartTransition("GameScene2"
                , onEndTransition:()=> GameObject.Find("GameManager").GetComponent<IGameSceneInitializer>().Initialize(musicData, course, sheetData, auto,diffCol)
                , onEndRemoveMask: () => {  GameObject.Find("GameManager").GetComponent<IGameSceneInitializer>().MusicStart(); });
        }

        public static void ToTutorialScene(MusicFileData musicData, MakeSheetScene.SheetData sheetData,Color diffCol)
        {
            Input.multiTouchEnabled = true;

            TransitionManager.Instance.StartTransition("GameScene2"
                ,onEndTransition:()=> GameObject.Find("GameManager").GetComponent<IGameSceneInitializer>().Initialize(musicData, Course.Easy, sheetData,false,diffCol)
                , onEndRemoveMask: () => { GameObject.Find("TutorialManager").GetComponent<ITutorialInitializer>().TutorialStart(musicData, sheetData); });
        }

        public static void ToMakeScene(MusicFileData data)
        {
            Input.multiTouchEnabled = false;

            TransitionManager.Instance.StartTransition("MakeSheetScene2", onEndTransition: () => { return GameObject.Find("OneBarFrame").GetComponent<IMakeSheetInitializer>().Initialize(data); });
        }
    }
}
