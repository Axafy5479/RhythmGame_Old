using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameScene
{

    public class GameManager : MonoSingleton<GameManager>, IGameSceneInitializer
    {
        private MusicFileData musicData;
        private Course course;
        private Color DiffCol { get; set; }
        public bool Auto { get; private set; }
        public Course Course { get => course; private set => course = value; }

        [SerializeField] private GameObject playerNormalNotePrefab;
        [SerializeField] private GameObject rivalNormalNotePrefab;
        [SerializeField] private GameObject playerLongNotePrefab;
        [SerializeField] private GameObject rivalLongNotePrefab;


        [SerializeField] private Transform playerCharaTrn;
        [SerializeField] private Transform rivalCharaTrn;
        [SerializeField] private Camera landscapeCamera;
        [SerializeField] private Camera portrateCamera;


        [SerializeField] private TextMeshPro musicNameText;
        [SerializeField] private TextMeshPro originalMusicName;
        [SerializeField] private TextMeshPro courseText;


        //[SerializeField] private LaneInputManager[] playerLanes;
        //[SerializeField] private LaneInputManager[] rivalLanes;


        [SerializeField] private Transform[] playerMakeNotePoints;
        [SerializeField] private Transform[] rivalMakeNotePoints;

        //for debugging
        [SerializeField] private GameObject makeNotePoint0;


        private MakeSheetScene.SheetData sheetData;
        private AudioSource bgmSource;

        // Start is called before the first frame update
        //void Start()

        public void MusicStart()
        {
 

            BGMSource.I.Play();

            //タイムキーパーにAudioSourceを渡し、timeを監視させる
            TimeKeeper.I(true).StartKeeping(bgmSource);
            TimeKeeper.I(false).StartKeeping(bgmSource);
        }

        public IEnumerator Initialize(MusicFileData musicFileData,Course course,MakeSheetScene.SheetData sheetData,bool auto,Color diffCol)
        {
            Auto = auto;
            this.sheetData = sheetData;
            diffCol = new Color(diffCol.r, diffCol.g, diffCol.b, 0.25f);
            DiffCol = diffCol;
            DOTween.SetTweensCapacity(512, 512);

            musicNameText.text = musicFileData.MusicName + " / " + musicFileData.Composer;
            musicNameText.color = diffCol;
            originalMusicName.text = musicFileData.OriginalMusicName + " / ZUN";
            originalMusicName.color = diffCol;
            courseText.text = course.ToString();
            courseText.color = diffCol;



            //Transform[] playerBeatPoints = Array.ConvertAll(playerLanes,x=>x.transform);
            //Transform[] rivalBeatPoints = Array.ConvertAll(rivalLanes, x => x.transform);


            this.musicData = musicFileData;
            this.course = course;

            Debug.Log(TimeKeeper.I(true));
            //すべてのNoteControllerを作成し、発射時間を計算する
            TimeKeeper.I(true).Initialize(sheetData,auto);
            TimeKeeper.I(false).Initialize(sheetData,true);

            //allNotes =
            //(
            //    (new NotesFactory(true, playerNormalNotePrefab, playerLongNotePrefab)).MakeAllNotes(sheetData, playerMakeNotePoints, playerBeatPoints),
            //    (new NotesFactory(false, rivalNormalNotePrefab, rivalLongNotePrefab)).MakeAllNotes(sheetData, rivalMakeNotePoints, rivalBeatPoints)
            //);

            //Recorder(成績記録係)の初期化
            Recorder.I.Initialize(musicFileData.MusicId, musicFileData.MusicName, course, sheetData);

            //BGMSourceに音声ファイルを渡す&AudioSourceを取得
            (AudioClip clip, float volume) = musicData.AudioClip;
            bgmSource = BGMSource.I.Initialize(clip);
            bgmSource.volume = volume;

            yield return null;
        }

        public void Finish()
        {
            DOTween.KillAll();
            Record record = SaveRecord();

            if (Auto)
            {
                Quit();
            }
            else
            {
                WBTransition.TransitionAssist.ToResult(record);
            }

        }


        public void Pause(bool pause)
        {
            if (pause)
            {
                bgmSource.Pause();
                Time.timeScale = 0;
            }
            else
            {
                bgmSource.Play();
                Time.timeScale = 1;
            }
        }


        private Record SaveRecord()
        {
            Record record = Recorder.I.GetRecord(true);
            SaveSystem_GameRecord saveSystem = new SaveSystem_GameRecord(musicData, course);
            saveSystem.Save(record.Score, record);
            return record;
        }

        public void TryAgain()
        {
            WBTransition.TransitionAssist.ToGameScene(musicData, course, sheetData,Auto,DiffCol);
        }

        public void Quit()
        {
            WBTransition.TransitionAssist.ToMusicSelect();
        }
    }
}