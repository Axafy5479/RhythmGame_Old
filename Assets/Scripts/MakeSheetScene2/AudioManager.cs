using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MakeSheetScene
{
    public class AudioManager : MonoBehaviour
    {

        #region Singleton
        private static AudioManager instance;

        public static AudioManager Instance
        {
            get
            {
                AudioManager[] instances = null;
                if (instance == null)
                {
                    instances = FindObjectsOfType<AudioManager>();
                    if (instances.Length == 0)
                    {
                        Debug.LogError("AudioManagerのインスタンスが存在しません");
                        return null;
                    }
                    else if (instances.Length > 1)
                    {
                        Debug.LogError("AudioManagerのインスタンスが複数存在します");
                        return null;
                    }
                    else
                    {
                        instance = instances[0];
                    }
                }
                return instance;
            }
        }
        #endregion


        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private TMPro.TMP_InputField offsetField;
        //[SerializeField] private AudioClip clip;
        public bool IsPlaying => bgmSource.isPlaying;

        public void Initialize(MusicFileData musicData)
        {
            (AudioClip clip,float volume) = musicData.AudioClip;
            bgmSource.clip = clip;
            bgmSource.volume = volume;
            offsetField.text = SheetMaker.I.SheetData.offset.ToString();
        }

        private SheetData SheetData => SheetMaker.I.SheetData;

        private Queue<float> playerSoundTime;
        private Queue<float> rivalSoundTime;

        private Queue<float> barTime;


        public void StopButtonClicked()
        {
            bgmSource.Stop();
            SheetMaker.I.OnStopAudio();
        }
        public void StartButtonClicked()
        {
            int barNumber = SheetMaker.I.CurrentBar;
            playerSoundTime = new Queue<float>();
            rivalSoundTime = new Queue<float>();
            barTime = new Queue<float>();
            float time = SheetData.offset;

            //音を鳴らす時間を決定する
            for (int i = 0;i< SheetData.barDatas.Count;i++)
            {
                if (SheetMaker.I.CurrentBar == i)
                {
                    bgmSource.time = time;
                }
                BarData bar = SheetData.barDatas[i];
 
                float deltaTime = 60 * (4 / bar.BPM) / bar.DivDatas.Length;

                foreach (var div in bar.DivDatas)
                {
                    int[][] playerNotes = div.GetNotes(true);
                    int[][] rivalNotes = div.GetNotes(false);

                    if(playerNotes.Any(x=>x[0]!=0 && x[0] != 3))
                    {
                        if (SheetMaker.I.CurrentBar <= i)
                        {
                            playerSoundTime.Enqueue(time+ 0.0005f);
                        }
                    }
                    if (rivalNotes.Any(x => x[0] != 0 && x[0] != 3))
                    {
                        if (SheetMaker.I.CurrentBar <= i)
                        {
                            rivalSoundTime.Enqueue(time+ 0.0005f);
                        }
                    }
                    time += deltaTime;
                }

                if (SheetMaker.I.CurrentBar <= i)
                {
                    barTime.Enqueue(time + 0.0005f);
                }


            }


            bgmSource.Play();

            StartCoroutine(TimeKeeping());
        }


        private IEnumerator TimeKeeping()
        {
            while (bgmSource.isPlaying)
            {
                float time = bgmSource.time;

                if (playerSoundTime.Count>0&&playerSoundTime.Peek() < time)
                {
                    playerSoundTime.Dequeue();
                    MakeSound(true);
                }
                if (rivalSoundTime.Count>0&&rivalSoundTime.Peek() < time)
                {
                    rivalSoundTime.Dequeue();
                    MakeSound(false);
                }


                if (barTime.Count>0&&barTime.Peek() < time)
                {
                    barTime.Dequeue();
                    SheetMaker.I.NextBar();
                }

                yield return new WaitForSeconds(1f / 60);
            }
        }

        private void MakeSound(bool isPlayer)
        {
            SeManager.Instance.MakeSound(isPlayer);
        }


        public void OffsetField()
        {
            try
            {
                float offsetFloat = float.Parse(offsetField.text);
                if (offsetFloat < 0)
                {
                    Debug.Log("正の数値を入力してください");
                }
                SheetMaker.I.SheetData.offset = offsetFloat;
                SheetMaker.I.Save();
            }
            catch
            {
                Debug.Log("数値を入力してください");
            }
        }
    }
}
