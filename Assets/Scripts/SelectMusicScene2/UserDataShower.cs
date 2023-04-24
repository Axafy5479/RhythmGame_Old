using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserDataShower : MonoBehaviour
{
    //[SerializeField] private Image frameImage;

    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI perfectText;
    private TextMeshProUGUI goodText;
    private TextMeshProUGUI missText;
    private TextMeshProUGUI maxComboText;
    private TextMeshProUGUI playTimesText;

    private Dictionary<int, Dictionary<Course, Data.CourseUserRecord>> record_map = new Dictionary<int, Dictionary<Course, Data.CourseUserRecord>>();


    internal void Initialize(Data.UserData userData)
    {
       

        foreach (var item in userData.MusicDatas)
        {
            if (record_map.ContainsKey(item.MusicId))
            {
                if (record_map[item.MusicId].ContainsKey(item.Course))
                {
                    record_map[item.MusicId][item.Course] = item;
                }
                else
                {
                    record_map[item.MusicId].Add(item.Course, item);
                }
            }
            else
            {
                record_map.Add(item.MusicId, new Dictionary<Course, Data.CourseUserRecord>() { { item.Course, item } });
            }
        }

    }




    private void Awake()
    {
        scoreText = this.transform.Find("_Score").GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        perfectText = this.transform.Find("_Perfect").GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        goodText = this.transform.Find("_Good").GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        missText = this.transform.Find("_Miss").GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        maxComboText = this.transform.Find("_MaxCombo").GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        playTimesText = this.transform.Find("_PlayTimes").GetChild(0).GetComponentInChildren<TextMeshProUGUI>();

    }

    internal void Show(int musicId,Course course)//,Color c);
    {
        //frameImage.color = c;

        //óVÇÒÇæãLò^Ç™Ç†ÇÈÇ©Ç»Ç¢Ç©Ç≈èÍçáï™ÇØ
        if (record_map.ContainsKey(musicId) && record_map[musicId].ContainsKey(course))
        {
            scoreText.text = record_map[musicId][course].Score.ToString();
            perfectText.text = record_map[musicId][course].Perfect1.ToString();
            goodText.text = record_map[musicId][course].Good1.ToString();
            missText.text = record_map[musicId][course].Miss1.ToString();
            maxComboText.text = record_map[musicId][course].MaxCombos1.ToString();
            playTimesText.text = record_map[musicId][course].PlayTimes1.ToString();
        }
        else
        {
            scoreText.text = "0";
            perfectText.text = "0";
            goodText.text = "0";
            missText.text = "0";
            maxComboText.text = "0";
            playTimesText.text = "0";
        }

    }
}
