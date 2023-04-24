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
        /// Scroll Layout�Ɋy�Ȃ�ǉ�����
        /// </summary>
        /// <param name="music"></param>
        /// <param name="userData"></param>
        internal void Initialize(Data.Music music)
        {
            oneMusic_map = new Dictionary<int, Data.OneMusic>();

            //�I�[�f�B�I�t�@�C�����̓ǂݍ���
            MusicFileData[] musicDatas = Resources.LoadAll<MusicFileData>("Music");

            Dictionary<int, MusicFileData> music_map = musicDatas.ToDictionary(x=>x.MusicId);

            //�Ō�ɗV�񂾋�
            int lastPlayedMusic = Setting.I.LastPlayedMusic.MusicId;


            //�e�y�Ȃ̃f�[�^���܂Ƃ߂�
            foreach (var item in music.music)
            {
                //���̊y�Ȃ̍��ڂ����ASimpleSnap�ɒǉ�����
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
                Debug.LogError($"musicItem�̖��O({scrollSnap.Panel.name})��musicId��parse�ł��܂���ł���");
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
