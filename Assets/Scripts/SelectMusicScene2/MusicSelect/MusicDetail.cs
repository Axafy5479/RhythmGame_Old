using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WBTransition;

public class MusicDetail : MonoBehaviour
{

    #region Singleton
    private static MusicDetail instance;

    public static MusicDetail Instance
    {
        get
        {
            MusicDetail[] instances = null;
            if (instance == null)
            {
                instances = FindObjectsOfType<MusicDetail>();
                if (instances.Length == 0)
                {
                    Debug.LogError("MusicDetailのインスタンスが存在しません");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError("MusicDetailのインスタンスが複数存在します");
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


    [SerializeField] private List<GameObject> decorations;
    [SerializeField] private SimpleScrollSnap scroll;
    [SerializeField] private Image Image;
    [SerializeField] private Button button;
    [SerializeField] private Text musicNameText;

    [SerializeField] private Image musicNameImage;
    [SerializeField] private Image recordImage;
    [SerializeField] private Image playButtonImage;

    [SerializeField] private HighScoreLayout highScoreLayout;

    [SerializeField] private Toggle portrateToggle;
    [SerializeField] private Toggle autoToggle;


    [SerializeField] private Image OButtonImage;
    [SerializeField] private Image EButtonImage;
    [SerializeField] private Image NButtonImage;
    [SerializeField] private Image HButtonImage;
    [SerializeField] private Image LButtonImage;

    [SerializeField] private Button OButtonButton;
    [SerializeField] private Button EButtonButton;
    [SerializeField] private Button NButtonButton;
    [SerializeField] private Button HButtonButton;
    [SerializeField] private Button LButtonButton;

    [SerializeField] private Text PlayButtonText;

    private Course course;
    private void ColorChange(Color color)
    {
        musicNameImage.color = color;

        recordImage.color = color;
        playButtonImage.color = color;
        musicNameText.GetComponent<GradationController>().ChangeColor(Color.white, color);
        PlayButtonText.text = "";
        PlayButtonText.text = "Play";
        PlayButtonText.GetComponent<GradationController>().ChangeColor(Color.white, color);
    }

    private Transform content => scroll.Content.transform;

    public void Initialize()
    {
        portrateToggle.isOn = Setting.I.portrate;

        autoToggle.isOn = Setting.I.auto;
       // content = ;



        DifficultySelected((int)Setting.I.Course);
    }

    private MusicFileData musicData;
    public MusicFileData MusicData => musicData;
    public void ChangeMusicData(MusicFileData musicData)
    {
        this.musicData = musicData;
    }

    public void ButtonClicked()
    {
        //if (!Setting.Instance.auto)
        //{
        //    SaveSystem.Instance.AddPlayNumber(musicData.MusicId,Setting.Instance.diff);
        //}

        StartCoroutine(GetMusicSheet());

    }

    public void PortrateToggleChange()
    {
        Setting.I.portrate = portrateToggle.isOn;
    }
    public void AutoToggleChange()
    {
        Setting.I.auto = autoToggle.isOn;
    }





    public void DifficultySelected(int diff)
    {


        if (diff == -1) diff = (int)Setting.I.Course;
        MusicItem musicItem = content.GetChild(scroll.CurrentPanel).GetComponent<MusicItem>();
        musicData = musicItem.MusicData;
        Data.OneMusic musicProp = musicItem.Music;

        course = (Course)diff;

        //Setting.Instance.Course = course;
        ColorChange(Setting.diffColor[musicProp.GetDifficulty(course)]);

        musicNameText.text = "";//テキストの色を変えるには、textが変化しなければならない
        musicNameText.text = Setting.I.Course.ToString();

        highScoreLayout.ShowRecord(musicData.MusicId);

        MusicPlayer.Instance.PlayMusic(musicData);

        //bgmSource.clip = musicData.Music[Setting.Instance.diff].Intro;
        //bgmSource.PlayDelayed(0.5f)



        OButtonImage.color = Color.gray / 2;
        EButtonImage.color = Color.gray / 2;
        NButtonImage.color = Color.gray / 2;
        HButtonImage.color = Color.gray / 2;
        LButtonImage.color = Color.gray / 2;

        OButtonImage.color = Color.white;
        EButtonImage.color = Setting.diffColor[musicProp.GetDifficulty(Course.Easy)];
        NButtonImage.color = Setting.diffColor[musicProp.GetDifficulty(Course.Normal)];
        HButtonImage.color = Setting.diffColor[musicProp.GetDifficulty(Course.Hard)];
        LButtonImage.color = Setting.diffColor[musicProp.GetDifficulty(Course.Lunatic)];
        OButtonButton.enabled = true;
        EButtonButton.enabled = true;
        NButtonButton.enabled = true;
        HButtonButton.enabled = true;
        LButtonButton.enabled = true;
    }





    IEnumerator GetMusicSheet()
    {

        WWWForm form = new WWWForm();
        form.AddField("userName", SaveSystem.I.GetUserId());

        form.AddField("password", SaveSystem.I.GetPassword());
        form.AddField("musicId", musicData.MusicId);
        form.AddField("difficulty", (int)course);

        string url = "https://framari.org/GetMusicSheet.php";
        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();
        if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            string jsonData = uwr.downloadHandler.text;
            Debug.Log(jsonData);
            MakeSheetScene.SheetData sheetData = JsonUtility.FromJson<MakeSheetScene.SheetData>(jsonData);
            Setting.I.LastPlayedMusic = musicData;
            Setting.I.portrate = portrateToggle.isOn;

            //TransitionManager.Instance.StartTransition("GameScene2", 0.3f, 1, 0.5f, decorations,onEndRemoveMask: () => GameScene.GameManager.I.GameStart(musicData, course, sheetData));

            Debug.LogError("シーン遷移はまだ作られていません");
        }
    }

}
