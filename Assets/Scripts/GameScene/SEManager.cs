using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class SEManager : MonoSingleton<SEManager>
    {


        Queue<AudioSource> player_audios = new Queue<AudioSource>();
        Queue<AudioSource> enemy_audios = new Queue<AudioSource>();
        [SerializeField] AudioClip player_se;
        [SerializeField] AudioClip enemy_se;

        [SerializeField, Range(0, 1)] private float volume = 1f;

        //private bool isWaiting = false;
        //private bool isEWaiting = false;

        private float initialVol;


        // Start is called before the first frame update
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

        public void MakeSound(bool player, float volumeMod)
        {
            var s = player ? player_audios.Dequeue() : enemy_audios.Dequeue();
            s.volume = initialVol * volumeMod;
            s.Play();
            if (player) player_audios.Enqueue(s);
            else enemy_audios.Enqueue(s);
        }

    }
}

