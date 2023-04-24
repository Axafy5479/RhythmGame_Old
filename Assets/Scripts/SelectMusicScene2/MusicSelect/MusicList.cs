using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MusicList : MonoSingleton<MusicList>
{
    [SerializeField] private Text debugText;
    [SerializeField] private Transform content;
    private SimpleScrollSnap scrollSnap;
    [SerializeField] private GameObject musicItemPrefab;
    private List<(GameObject go, Data.OneMusic oneMusic, Dictionary<Course,Data.CourseUserRecord> records)> allMusic = new List<(GameObject, Data.OneMusic oneMusic, Dictionary<Course, Data.CourseUserRecord>)>();

    private HashSet<int> idChecker = new HashSet<int>();

    int lastPlayedMusicIndex;
    //public IEnumerator Initialize()
    //{
    //    debugText.text += Time.time + "\n";

    //    yield return Data.GetUserData.I.GetAllMusic_Connect();
    //    yield return Data.GetUserData.I.GetUserData_Connect();
    //    debugText.text += Time.time + "\n";

    //    Data.Music music = Data.GetUserData.I.Music;
    //    Data.UserData userData = Data.GetUserData.I.UserData;


    //    Dictionary<(int musicId, Course course), Data.CourseUserRecord> course_data_map = new Dictionary<(int musicId, Course course), Data.CourseUserRecord>();

    //    foreach (var item in userData.MusicDatas)
    //    {
    //        course_data_map.Add((item.MusicId, item.Course), item);
    //    }




    //    //表示
    //    scrollSnap = this.GetComponent<SimpleScrollSnap>();


    //    MusicData[] musicDatas = Resources.LoadAll<MusicData>("Music");

    //    //最後に遊んだ曲
    //    int lastPlayedMusic = Setting.Instance.LastPlayedMusic;


    //    foreach (var oneMusic in music.music)
    //    {



    //        int id = oneMusic.id;
    //        MusicData musicData = Array.Find(musicDatas, x => x.MusicId == id);

    //        if (idChecker.Contains(id))
    //        {
    //            Debug.LogError("IDが重複しています");
    //        }
    //        if (id == lastPlayedMusic)
    //        {
    //            lastPlayedMusicIndex = id;
    //        }

    //        idChecker.Add(id);

    //        Dictionary<Course, Data.CourseUserRecord> records = new Dictionary<Course, Data.CourseUserRecord>()
    //        {
    //            //{Course.Original,userData.MusicDatas.Find(m=>m.MusicId == id && m.Difficulty == Course.Original)},
    //            //{Course.Easy,userData.MusicDatas.Find(m=>m.MusicId == id && m.Difficulty == Course.Easy)},
    //            //{Course.Normal,userData.MusicDatas.Find(m=>m.MusicId == id && m.Difficulty == Course.Normal)},
    //            //{Course.Hard,userData.MusicDatas.Find(m=>m.MusicId == id && m.Difficulty == Course.Hard)},
    //            //{Course.Lunatic,userData.MusicDatas.Find(m=>m.MusicId == id && m.Difficulty == Course.Lunatic)},   
    //            {Course.Original,course_data_map.ContainsKey((id,Course.Original))? course_data_map[(id,Course.Original)]:null},
    //            {Course.Easy,course_data_map.ContainsKey((id,Course.Easy))? course_data_map[(id,Course.Easy)]:null},
    //            {Course.Normal,course_data_map.ContainsKey((id,Course.Normal))? course_data_map[(id,Course.Normal)]:null},
    //            {Course.Hard,course_data_map.ContainsKey((id,Course.Hard))? course_data_map[(id,Course.Hard)]:null},
    //            {Course.Lunatic,course_data_map.ContainsKey((id,Course.Lunatic))? course_data_map[(id,Course.Lunatic)]:null},
    //        };

    //        GameObject listObj = Instantiate(musicItemPrefab, content);
    //        //listObj.GetComponent<MusicItem>().Initialize(oneMusic, records, musicData); 
    //        allMusic.Add((listObj, oneMusic, records));

          

    //    }
    //    debugText.text += Time.time + "\n";

    //    MusicDetail.Instance.Initialize();
    //    if (music.music.Count < 1) yield break;
    //    //for (int i = 0; i < (20 / allMusic.Count); i++)
    //    //{
    //    //    foreach (var item in allMusic)
    //    //    {
    //    //        Instantiate(item.go, content).GetComponent<MusicItem>().Initialize(item.oneMusic, item.records, Array.Find(musicDatas,x=>x.MusicId==item.oneMusic.id));
    //    //    }
    //    //}

    //    scrollSnap.CurrentPanel = lastPlayedMusicIndex;
    //    scrollSnap.startingPanel = lastPlayedMusicIndex;
    //    debugText.text += Time.time + "\n";

    //}
}
