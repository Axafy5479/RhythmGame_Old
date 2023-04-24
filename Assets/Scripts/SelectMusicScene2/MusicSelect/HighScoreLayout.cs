using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreLayout : MonoBehaviour
{

    #region Singleton
    private static HighScoreLayout instance;

    public static HighScoreLayout Instance
    {
        get
        {
            HighScoreLayout[] instances = null;
            if (instance == null)
            {
                instances = FindObjectsOfType<HighScoreLayout>();
                if (instances.Length == 0)
                {
                    Debug.LogError("HighScoreLayoutのインスタンスが存在しません");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError("HighScoreLayoutのインスタンスが複数存在します");
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


    [SerializeField] private Text highScoreText;
    [SerializeField] private Text combosText;
    [SerializeField] private Text perfectText;
    [SerializeField] private Text goodText;
    [SerializeField] private Text missText;

    [SerializeField] private Text playNumberText;

    public void ShowRecord(int musicId)
    {
        Data.UserData userData = Data.GetUserData.I.UserData;

        if (userData == null) return;

        Data.CourseUserRecord musicData = userData.MusicDatas.Find(data => data.MusicId == musicId);
        if (musicData != null)
        {
            highScoreText.text = musicData.Score.ToString();

            combosText.text = musicData.MaxCombos1.ToString();
            perfectText.text = musicData.Perfect1.ToString();
            goodText.text = musicData.Good1.ToString();
            missText.text = musicData.Miss1.ToString();
            playNumberText.text = musicData.PlayTimes1.ToString();
        }
        else
        {
            highScoreText.text = "0";
            combosText.text = "0";
            perfectText.text = "0";
            goodText.text = "0";
            missText.text = "0";
            playNumberText.text = "0";
        }


    }
}
