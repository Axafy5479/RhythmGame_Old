using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeSheetScene
{
    public class SeManager : MonoBehaviour
    {

        #region Singleton
        private static SeManager instance;

        public static SeManager Instance
        {
            get
            {
                SeManager[] instances = null;
                if (instance == null)
                {
                    instances = FindObjectsOfType<SeManager>();
                    if (instances.Length == 0)
                    {
                        Debug.LogError("SeManagerのインスタンスが存在しません");
                        return null;
                    }
                    else if (instances.Length > 1)
                    {
                        Debug.LogError("SeManagerのインスタンスが複数存在します");
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



        Queue<AudioSource> player_audios = new Queue<AudioSource>();
        Queue<AudioSource> enemy_audios = new Queue<AudioSource>();
        [SerializeField] AudioClip player_se;
        [SerializeField] AudioClip enemy_se;


        [SerializeField, Range(0, 1)] private float volume = 1f;

        private float initialVol;

        void Start()
        {
            AudioSource[] audios = this.GetComponents<AudioSource>();
            for (int i = 0; i < audios.Length / 2; i++)
            {
                audios[i].clip = player_se;
                audios[i].volume = volume;
                initialVol = volume;
                player_audios.Enqueue(audios[i]);

            }

            for (int i = audios.Length / 2; i < audios.Length; i++)
            {
                audios[i].clip = enemy_se;
                audios[i].volume = volume;
                enemy_audios.Enqueue(audios[i]);
            }
        }

        public void MakeSound(bool isPlayer)
        {
            if (isPlayer)
            {
                var s = player_audios.Dequeue();
                s.volume = initialVol;
                s.Play();
                player_audios.Enqueue(s);
            }
            else
            {
                var s = enemy_audios.Dequeue();
                s.volume = initialVol;
                s.Play();
                enemy_audios.Enqueue(s);
            }

        }
    }
}
