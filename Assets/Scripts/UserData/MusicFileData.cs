using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;



[CreateAssetMenu(fileName ="Music",menuName ="Music")]
public class MusicFileData : ScriptableObject
{
    [SerializeField] private int musicId;
    [SerializeField] private string musicName;
    [SerializeField] private float volume = 1;
    //[SerializeField] private CharacterData rival;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private List<OneMusic> music;

    [SerializeField] private string composer;
    [SerializeField] private string originalMusicName;



    public int MusicId { get => musicId; }
    public string MusicName { get => musicName; }
    public (AudioClip clip,float volume) AudioClip { get => (audioClip,volume); }
    public Dictionary<Course,OneMusic> Music => music.ToDictionary(x=>x.Cource);

    public string Composer { get => composer;  }
    public string OriginalMusicName { get => originalMusicName;  }

    //public CharacterData Rival { get => rival; }
}

[System.Serializable]
public class OneMusic
{
    [SerializeField] private Course cource;
    //[SerializeField] private Difficulty diff;
    //[SerializeField] private TextAsset score;
    [SerializeField] private AudioClip intro;
    //[SerializeField] private int musicId;

    public Course Cource { get => cource; }
    //public Difficulty Diff { get => diff; }

    public AudioClip Intro { get => intro; }
}