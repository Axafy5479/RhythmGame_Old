using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScene
{
    public class BGMSource : MonoSingleton<BGMSource>
    {
        [SerializeField] private AudioSource source;

        public float CurrentTime => source.time;

        public AudioSource Initialize(AudioClip clip)
        {
            source.clip = clip;
            source.time = 0.1f;
            return source;
        }

        public void Play()
        {
            source.Play();
        }
    }
}