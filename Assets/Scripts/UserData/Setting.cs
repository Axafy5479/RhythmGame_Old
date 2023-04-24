using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum Course
{
    Easy = 0,
    Normal = 1,
    Hard = 2,
    Lunatic = 3,
    Original = 4,
    Others = 5
}

public enum Difficulty
{
    NotImplimented = -1,
    Gray = 0,
    Brown = 1,
    Green = 2,
    Cyan = 3,
    Blue = 4,
    Yellow = 5,
    Orange = 6,
    Red = 7,
}

[CreateAssetMenu(fileName ="Setting",menuName ="Setting")]
public class Setting : ScriptableObject
{


    public const bool SAVE_FILE = true;

    #region Singleton
    private static Setting instance;
    public static Setting I
    {
        get
        {
            if(instance == null)
            {
                instance = Resources.Load<Setting>("Setting");
            }
            return instance;
        }
    }
    #endregion

    [SerializeField] private MusicFileData initialMusicFile;

    public static Dictionary<Difficulty, Color> diffColor = new Dictionary<Difficulty, Color>()
    {
        {Difficulty.Gray,Color.gray },
        {Difficulty.Brown,new Color(145f/255, 82f/255, 36f/255) },
        {Difficulty.Green,new Color(0,0.6039f,0) },
        {Difficulty.Cyan,Color.cyan },
        {Difficulty.Blue,Color.blue },
        {Difficulty.Yellow,Color.yellow },
        {Difficulty.Orange,new Color(255f/255, 145f/255, 0) },
        {Difficulty.Red,Color.red },
    };


    public bool auto = false;
    public bool portrate = false;
    public Course Course = Course.Easy;
    public MusicFileData LastPlayedMusic;

    private void OnEnable()
    {
        if(LastPlayedMusic == null)
        {
            LastPlayedMusic = initialMusicFile;
        }
    }

    //private CharacterData characterData;
    //public CharacterData CharacterData
    //{
    //    get
    //    {
    //        if(characterData == null)
    //        {
    //            characterData = Resources.Load<CharacterData>("Characters/Sanae");
    //        }
    //        return characterData;
    //    }
    //    set => characterData = value;
    //}
}
