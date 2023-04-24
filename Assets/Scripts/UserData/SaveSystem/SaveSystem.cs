using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem
{
  
    private const string USER_DATA_KEY = "userData";

    #region Singleton
    private static SaveSystem i = new SaveSystem();
    public static SaveSystem I => i;
    #endregion
    private SaveSystem() 
    {
    }

    public string UserId => UserData.UserId;

    public string Path => Application.persistentDataPath + "/data.json";

    public UserData UserData { get 
        {
            if (userData == null)
            {
                Error.ErrorManager.Instance.ShowErrorScreen(Error.ErrorType.NoUserData);
                return null;
            }
            else return userData;
             
        } 
        private set => userData = value; }

    private UserData userData;

    public void SetUserId(string userName,string pass)
    {
        UserData.Password = pass;
        UserData.UserId = userName;

        Save();
    }

    /// <summary>
    /// ユーザー登録時にエラーが発生した場合
    /// </summary>
    public void DeleteUserIdAndPass()
    {
        UserData.Password = "";
        UserData.UserId = "";

        Save();
    }

    
    public string GetPassword()
    {
        return UserData.Password;
    }

    public string GetUserId()
    {
        return UserData.UserId;
    }

    public void SetCanUploadNumber(int number)
    {
        UserData.CanUploadNumber = number;
        Save();
    }


    //public MusicRecordClone GetData(int id,Course course)
    //{
    //    MusicRecord data= userData.MusicRecords.Find(x => x.MusicId == id);

    //    if (data == null)
    //    {
    //        data = new MusicRecord(id);
    //        return new MusicRecordClone(Array.Find(data.Records,r=>r.Course == course));

    //    }
    //    else
    //    {
    //        return new MusicRecordClone(Array.Find(data.Records,r=>r.Course==course));
        
    //    }
    //}



    public void SetPlayable(int id, int course)
    {


        //MusicRecord data = userData.MusicRecords.Find(x => x.MusicId == id);

        //if (data == null)
        //{
        //    data = new MusicRecord(id);
        //    userData.MusicRecords.Add(data);
        //}

        ////Debug.Log(userData.MusicRecords.Count);
        ////Debug.Log(data.MusicId);
        ////Debug.Log(data.Records);
        ////Debug.Log(JsonUtility.ToJson(userData));

        //Array.Find(data.Records, c => c.Course == course).SetPlayable();
        //Save();
    }


    //public void NewRecord(Result result)
    //{

    //    MusicRecord record = userData.MusicRecords.Find(x => x.MusicId == result.MusicId);
    //    int oldScoreIndex = Array.FindIndex(record.Records, r => r.Course == result.Course);
    //    MusicRecordOfCourse oldScore = record.Records[oldScoreIndex];
    //    if (oldScore != null)
    //    {
    //        //一応確認
    //        if (oldScore.Score >= result.Point)
    //        {
    //            Debug.LogError("スコアを更新していません");
    //            return;
    //        }


    //    }
    //    //userData.MusicRecords.Remove(oldScore);

    //    if (oldScoreIndex != (int)result.Course - 1)
    //    {
    //        Debug.LogError("セーブする要素の箇所が異なります");
    //    }

    //    record.Records[oldScoreIndex] = new MusicRecordOfCourse(result.MusicId,result.Course, result.FullCombo?true:oldScore.FullCombo, result.Point, result.Combo, oldScore.PlayNumber, result.JudgeCount[Judge.Perfect], result.JudgeCount[Judge.Good], result.JudgeCount[Judge.Miss],true);

    //    Save();
    //}

    //public void AddPlayNumber(int id,Course course)
    //{
    //    MusicRecord record = userData.MusicRecords.Find(x => x.MusicId == id);
    //    if (record == null)
    //    {
    //        record = new MusicRecord(id);
    //        userData.MusicRecords.Add(record);
    //    }
    //    MusicRecordOfCourse recordCourse = Array.Find(record.Records, r => r.Course == course);
    //    recordCourse.AddPlayNumner();

    //    Save();
    //}

    public void Save()
    {

        string jsonData = JsonUtility.ToJson(UserData);

        if (Setting.SAVE_FILE)
        {
            StreamWriter writer = new StreamWriter(Path, false);
            writer.WriteLine(jsonData);
            writer.Flush();
            writer.Close();
        }
        else
        {
            PlayerPrefs.SetString(USER_DATA_KEY, jsonData);
            PlayerPrefs.Save();
        }
    }

    public void Load()
    {
        if (Setting.SAVE_FILE)
        {

            if (!File.Exists(Path))
            {
                Debug.LogError("ユーザーデータが見つかりません。先にデータの生成を行ってください");
                return;
            }

            StreamReader reader = new StreamReader(Path);
            string jsonData = reader.ReadToEnd();
            UserData = JsonUtility.FromJson<UserData>(jsonData);
            reader.Close();
        }
        else
        {
 

            if (!PlayerPrefs.HasKey(USER_DATA_KEY))
            {
                Debug.LogError("ユーザーデータが見つかりません。先にデータの生成を行ってください");
                return;
            }

            string jsonData = PlayerPrefs.GetString(USER_DATA_KEY, "");
            UserData = JsonUtility.FromJson<UserData>(jsonData);
        }
    }

    public bool CheckNewUserData()
    {
        if (Setting.SAVE_FILE)
        {
            if (!File.Exists(Path))
            {
                UserData = new UserData();
                Save();
                return true;
            }

            Load();

            if (UserData.UserId == "" || UserData.Password == "")
            {
                UserData = new UserData();
                Save();
                return true;
            }

            return false;
        }
        else
        {
            
            if( !PlayerPrefs.HasKey(USER_DATA_KEY))
            {
                UserData = new UserData();
                Save();
                Load();
                return true;
            }
            else
            {
                Load();
                return false;
            }
        }
    }


}


//public struct MusicRecordClone
//{
//    public MusicRecordClone(MusicRecordOfCourse record)
//    {
//        Id = record.MusicId;
//        Course = record.Course;
//        FullCombo = record.FullCombo;
//        Score = record.Score;
//        MaxCombos = record.MaxCombos;
//        PlayNumber = record.PlayNumber;
//        Judges = new Dictionary<Judge, int>() { { Judge.Perfect, record.Perfect }, { Judge.Good, record.Good }, { Judge.Miss, record.Miss } };
//        Playable = record.Playable;
//    }
//    public Course Course { get; }
//    public int Id { get; }
//    public bool FullCombo { get; }
//    public int Score { get; }
//    public int MaxCombos { get; }
//    public int PlayNumber { get; }
//    public bool Playable { get; }
//    public Dictionary<Judge, int> Judges { get; }
//}

