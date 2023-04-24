using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class UserData
{
    //[SerializeField] private List<MusicRecord> musicRecords = new List<MusicRecord>();
    [SerializeField] private string password = "";
    [SerializeField] private string userId = "";
    [SerializeField] private int canUploadNumber = 50;


    //public List<MusicRecord> MusicRecords { get => musicRecords;  }
    public string Password { get => password; set => password = value; }
    public string UserId { get => userId; set => userId = value; }
    public int CanUploadNumber { get => canUploadNumber; set => canUploadNumber = value; }
}

[System.Serializable]
public class MusicRecord
{
    [SerializeField] private int musicId;

    [SerializeField] private MusicRecordOfCourse[] records;

    public MusicRecord(int musicId)
    {
        //this.musicId = musicId;
        //this.records = new MusicRecordOfCourse[5]
        //{
        //    new MusicRecordOfCourse(musicId,Course.Easy,false,0,0,0,0,0,0,false),
        //    new MusicRecordOfCourse(musicId,Course.Normal,false,0,0,0,0,0,0,false),
        //    new MusicRecordOfCourse(musicId,Course.Hard,false,0,0,0,0,0,0,false),
        //    new MusicRecordOfCourse(musicId,Course.Lunatic,false,0,0,0,0,0,0,false),
        //    new MusicRecordOfCourse(musicId,Course.Original,false,0,0,0,0,0,0,false),
        //};
    }

    public int MusicId { get => musicId; }
    public MusicRecordOfCourse[] Records { get => records; }
}

[System.Serializable]
public class MusicRecordOfCourse
{
    [SerializeField] private int musicId;
   // [SerializeField] private Course course;
    [SerializeField] private bool fullCombo;
    [SerializeField] private int score;
    [SerializeField] private int maxCombo;
    [SerializeField] private int playNumber;
    [SerializeField] private int perfect;
    [SerializeField] private int good;
    [SerializeField] private int miss;
    [SerializeField] private bool playable = false;

    public MusicRecordOfCourse(int musicId, int course, bool fullCombo, int score, int maxCombo, int playNumber, int perfect, int good, int miss, bool playable)
    {
        this.musicId = musicId;
        //this.course = course;
        this.score = score;
        this.maxCombo = maxCombo;
        this.playNumber = playNumber;
        this.perfect = perfect;
        this.good = good;
        this.miss = miss;
        this.fullCombo = fullCombo;
        this.playable = playable;
    }

    public int MusicId { get => musicId; }
    //public Course Course { get => course; }
    public int Score { get => score; }
    public int PlayNumber { get => playNumber; }
    public int MaxCombos { get => maxCombo; }
    public int Perfect { get => perfect; }
    public int Good { get => good; }
    public int Miss { get => miss; }
    public bool FullCombo => fullCombo;
    //public bool Playable { get {

    //        if (Course != Course.Original) return playable;
    //        else
    //        {
    //            string Path = Application.persistentDataPath + "/MusicSheets2/" + MusicId + ".json";
    //            return File.Exists(Path);
    //        }
        
    //    } }

    public void SetPlayable()
    {
        playable = true;
    }

    public void AddPlayNumner()
    {
        playNumber++;
    }
}