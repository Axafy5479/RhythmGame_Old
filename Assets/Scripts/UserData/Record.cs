using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JudgeEnum
{
    Perfect = 0,
    Good = 1,
    Miss = 2,
    None = 3
}



[System.Serializable]
public class Record
{
    [SerializeField] private int musicId;
    [SerializeField] private string musicTitle;
    [SerializeField] private Course course;
    [SerializeField] private List<OneHistory> histories;
    [SerializeField] private int score;
    [SerializeField] private int maxCombo;
    [SerializeField] private bool isPlayer;
    [SerializeField] private int[] gradeCounts;
    public int CurrentCombo { get; private set; }
    public List<OneHistory> oneHistory { get => histories; }
    public int Score { get => score; private set => score = value; }
    public int MaxCombo { get => maxCombo; private set => maxCombo = value; }
    public bool IsPlayer { get => isPlayer; private set => isPlayer = value; }
    public Dictionary<JudgeEnum, int> GradeMap => new Dictionary<JudgeEnum, int>() { { JudgeEnum.Perfect, gradeCounts[0] }, { JudgeEnum.Good, gradeCounts[1] }, { JudgeEnum.Miss, gradeCounts[2] } };

    public int MusicId { get => musicId; }
    public Course Course { get => course; }
    public string MusicTitle => musicTitle;

    private int allNote;

    public MakeSheetScene.SheetData SheetData { get; }


    public Record(int musicId,string musicTitle, Course difficulty, bool isPlayer, int allNote,MakeSheetScene.SheetData sheetData)
    {
        this.musicId = musicId;
        course = difficulty;
        IsPlayer = isPlayer;
        this.allNote = allNote;
        this.histories = new List<OneHistory>();
        gradeCounts = new int[3];
        SheetData = sheetData;
        this.musicTitle = musicTitle;
    }



    public void SetNoteGrade(int id, float time, JudgeEnum judge)
    {
        oneHistory.Add(new OneHistory(id, time, judge));
        gradeCounts[(int)judge]++;

        Score = GetScore();


        if (judge != JudgeEnum.Miss)
        {
            CurrentCombo++;
            MaxCombo = Mathf.Max(MaxCombo, CurrentCombo);
        }
        else
        {
            CurrentCombo = 0;
        }
    }

    private int GetScore()
    {
        // 1. ¸“x‚ð”ä—¦‚Æ‚µ‚ÄŒvŽZ‚·‚é  (perfect‚ð2, good‚ð1, miss‚ð0‚Æ‚µ‚ÄŒvŽZ)
        float ratio = (GradeMap[JudgeEnum.Perfect] * 2 + GradeMap[JudgeEnum.Good]) / (float)(allNote * 2);

        // 2. –ž“_‚ð10000‚É‚µ‚½‚¢
        return (int)(ratio * 10000);
    }
}

[System.Serializable]
public class OneHistory
{
    [SerializeField] private int id;
    [SerializeField] private float t;
    [SerializeField] private JudgeEnum j;

    public OneHistory(int id, float time, JudgeEnum judge)
    {
        this.id = id;
        this.t = time;
        this.j = judge;
    }

    public int Id { get => id; }
    public float Time { get => t; }
    public JudgeEnum Judge { get => j; }
}
