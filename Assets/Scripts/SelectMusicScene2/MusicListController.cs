using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

namespace MusicSelectScene
{
    public class MusicListController : MonoBehaviour
    {
        [SerializeField] private GameObject musicItemPrefab;

        [SerializeField] private SimpleScrollSnap scrollSnap;
        Dictionary<int, Data.OneMusic> oneMusic_map;
        //Dictionary<(int musicId, Course course), Data.CourseUserRecord> course_data_map = new Dictionary<(int musicId, Course course), Data.CourseUserRecord>();

        internal Data.OneMusic SelectedMusic { get; private set; }
        internal Subject<Unit> MusicChanged = new Subject<Unit>();


        /// <summary>
        /// Scroll Layoutに楽曲を追加する
        /// </summary>
        /// <param name="music"></param>
        /// <param name="userData"></param>
        internal void Initialize(Data.Music music)
        {
            oneMusic_map = new Dictionary<int, Data.OneMusic>();

            //オーディオファイル等の読み込み
            MusicFileData[] musicDatas = Resources.LoadAll<MusicFileData>("Music");

            Dictionary<int, MusicFileData> music_map = musicDatas.ToDictionary(x=>x.MusicId);

            //最後に遊んだ曲
            int lastPlayedMusic = Setting.I.LastPlayedMusic.MusicId;


            //各楽曲のデータをまとめる
            foreach (var item in music.music)
            {
                //この楽曲の項目を作り、SimpleSnapに追加する
                GameObject musicItemObj = Instantiate(musicItemPrefab);
                musicItemObj.name = item.id.ToString();
                scrollSnap.Add(musicItemObj, item.id);

                oneMusic_map.Add(item.id, item);
                musicItemObj.GetComponent<MusicItem>().Initialize(item);
            }

            scrollSnap.PublicInitialize();
            //for (int i = 0; i < scrollSnap.Panels.Length; i++)
            //{
            //    if(scrollSnap.Panels[i].name == lastPlayedMusic.ToString())
            //    {
            //        scrollSnap.CurrentPanel = i;
            //        scrollSnap.GoToPanel(i);
            //        break;
            //    }
            //}

            //Show();
        }


        public void Show()
        {
            if (!int.TryParse(scrollSnap.Panel.name, out int currentMusicId))
            {
                Debug.LogError($"musicItemの名前({scrollSnap.Panel.name})をmusicIdにparseできませんでした");
                return;
            }

            SelectedMusic = oneMusic_map[currentMusicId];
            MusicChanged.OnNext(Unit.Default);

            scrollSnap.Panel.Selected();
        }

        public void OnBeginSelecting()
        {
            foreach (var item in scrollSnap.Panels)
            {
                item.UnSelected();
            }
        }
  
   
    }
}
