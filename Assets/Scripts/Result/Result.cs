using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result
{
    //public Result(int id, Course course, bool fullCombo, string musicName, Dictionary<Judge, int> judgeCount, int combo, int point);//,CharacterData character)
    //{
    //    MusicName = musicName;
    //    Combo = combo;
    //    Point = point;
    //  //  JudgeCount = judgeCount;
    //    MusicId = id;
    //    FullCombo = fullCombo;
    //    Course = course;
    //   // Character = character;
    //}

    public string MusicName { get; }
   // public Dictionary<Judge, int> JudgeCount { get; }
    public int Combo { get; }
    public int Point { get; }
    public bool FullCombo { get; }
    public int MusicId { get; }
    public Course Course { get; }
    //public CharacterData Character { get; }
}
