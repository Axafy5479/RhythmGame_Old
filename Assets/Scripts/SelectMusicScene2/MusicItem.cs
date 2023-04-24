using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MusicItem : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private GameObject fullComboText;
    [SerializeField] private Text playNumText;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Image frameImage;


   // public Dictionary<Course, Data.CourseUserRecord> UserData { get; private set; }
    public Data.OneMusic Music { get; private set; }
    public MusicFileData MusicData { get; private set; }
    //public Dictionary<Course, MusicRecordClone> RecordClone { get; private set; }


    public void Initialize(Data.OneMusic music)
    {
        text.text = music.name;
        Music = music;

    }

    public void Selected()
    {
        frameImage.sprite = selectedSprite;
    }

    public void UnSelected()
    {
        if (frameImage.sprite == selectedSprite)
        {
            frameImage.sprite = normalSprite;
        }
    }

}
