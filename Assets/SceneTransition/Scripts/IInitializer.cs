using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitializer
{
    public IEnumerator Initialize();
}

public interface IResultInitializer
{
    public IEnumerator Initialize(Record record);
}

public interface IGameSceneInitializer
{
    public IEnumerator Initialize(MusicFileData musicData, Course course, MakeSheetScene.SheetData sheetData,bool auto, Color diffCol);
    public void MusicStart();
}

public interface ITutorialInitializer
{
    public void TutorialStart(MusicFileData musicData, MakeSheetScene.SheetData sheetData);
}

public interface IMakeSheetInitializer
{
    public IEnumerator Initialize(MusicFileData musicData);
}
