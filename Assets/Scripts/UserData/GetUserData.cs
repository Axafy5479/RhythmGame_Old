using MakeSheetScene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
//using UserHomeScene;

namespace Data
{
	public class GetUserData //MonoSingleton<GetUserData>
	{

        #region Singleton
        private static GetUserData instance = new GetUserData();
        public static GetUserData I { 
			get 
			{
				switch (Application.internetReachability)
				{
					case NetworkReachability.NotReachable:
						Error.ErrorManager.Instance.ShowErrorScreen(Error.ErrorType.Disconnection);
						return null;
					case NetworkReachability.ReachableViaCarrierDataNetwork:
						Debug.Log("キャリアデータネットワーク経由で到達可能");
						break;
					case NetworkReachability.ReachableViaLocalAreaNetwork:
						Debug.Log("Wifiまたはケーブル経由で到達可能");
						break;
				}
				return instance; 
			} 
		}
        #endregion
        private GetUserData() { }

		#region UserData
		private UserData userData;
        public UserData UserData { get{
				if(userData == null)
                {
					Debug.LogError("userDataがnullです");
					Error.ErrorManager.Instance.ShowErrorScreen(Error.ErrorType.NoUserData);
                }
				return userData;
			} }
		public Music Music { get; private set; } = null;
		public HashSet<int> MissionUserData { get; private set; }
		#endregion

		private const string ENTRANCE_PATH = "https://framari.org/Entrance_for_unityApp/";
		public string directory => Application.persistentDataPath + "/MusicSheets2/";


        #region GetDataMethod

		/// <summary>
		/// ユーザー情報を一括で取得
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetData()
        {
			userData = null;
			Music = null;
			MissionUserData = null;

			yield return GetUserData_Connect();
			yield return GetAllMissionState();
			yield return GetAllMusic_Connect();

			yield return new WaitWhile(()=>UserData==null || Music==null||MissionUserData==null);

		}

		/// <summary>
		/// プレイヤーが作った譜面を、端末内から取得する
		/// </summary>
		/// <param name="musicId"></param>
		/// <returns></returns>
		public SheetData GetUsersSheet(int musicId)
		{
			if (Setting.SAVE_FILE)
			{
				string Path = directory + musicId + ".json";

				DirectoryInfo di = new DirectoryInfo(directory);

				if (!di.Exists)
				{
					di.Create();
					return null;
				}

				if (!File.Exists(Path))
				{
					return null;
				}

				StreamReader reader = new StreamReader(Path);
				string jsonData = reader.ReadToEnd();
				return JsonUtility.FromJson<MakeSheetScene.SheetData>(jsonData);
			}
            else
            {
				string keyForPlayerPrefs = "madeSheet_" + musicId;
                if (!PlayerPrefs.HasKey(keyForPlayerPrefs))
                {
					return null;
                }

				string jsonData = PlayerPrefs.GetString(keyForPlayerPrefs);
		
				return JsonUtility.FromJson<MakeSheetScene.SheetData>(jsonData);
			}
		}

		/// <summary>
		/// 標準の譜面をサーバーから取得
		/// </summary>
		/// <param name="musicId"></param>
		/// <param name="course"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public IEnumerator GetMusicSheet(int musicId, Course course, Action<MakeSheetScene.SheetData> action)
		{

			WWWForm form = new WWWForm();
			form.AddField("userName", SaveSystem.I.GetUserId());

			form.AddField("password", SaveSystem.I.GetPassword());
			form.AddField("musicId", musicId);
			form.AddField("difficulty", (int)course);

			string url = ENTRANCE_PATH + "GetStandardSheetFromApp.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
			}
			else
			{
				string jsonData = uwr.downloadHandler.text;

				try
				{
					SheetData sheetData = JsonUtility.FromJson<MakeSheetScene.SheetData>(jsonData);
					action(sheetData);
				}
				catch (Exception e)
				{
					Debug.LogError(jsonData);
					Debug.LogError(e);
				}
			}
		}

		/// <summary>
		/// ほかのユーザーの譜面を、sheetId指定で取得する
		/// </summary>
		/// <param name="sheetId"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public IEnumerator GetOthersOriginalSheet(string sheetId, Action<MakeSheetScene.SheetData> action)
        {
			
			WWWForm form = new WWWForm();
			form.AddField("userName", SaveSystem.I.GetUserId());

			form.AddField("password", SaveSystem.I.GetPassword());
			form.AddField("sheetId", sheetId);

			string url = ENTRANCE_PATH + "GetUsersSheetData.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
			}
			else
			{
				string jsonData = uwr.downloadHandler.text;
				try
				{
					SheetData sheetData = JsonUtility.FromJson<MakeSheetScene.SheetData>(jsonData);
					action(sheetData);
				}
				catch (Exception e)
				{
					Debug.LogError(jsonData);
					Debug.LogError(e);
				}
			}
		}


		/// <summary>
		/// ほかのユーザーの譜面を、sheetId指定で取得する
		/// </summary>
		/// <param name="sheetId"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public IEnumerator GetOriginalSheetInfoFromId(string sheetId, Action<OthersOriginalSheet> action)
		{

			WWWForm form = new WWWForm();
			form.AddField("userName", SaveSystem.I.GetUserId());

			form.AddField("password", SaveSystem.I.GetPassword());
			form.AddField("sheetId", sheetId);

			string url = ENTRANCE_PATH + "GetSheetInfoFromId_FromUnitApp.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
			}
			else
			{
				string jsonData = uwr.downloadHandler.text;
				if (!jsonData.Contains("NoData"))
				{
					try
					{
						OthersOriginalSheet sheetInfo = JsonUtility.FromJson<OthersOriginalSheet>(jsonData);
						action(sheetInfo);
					}
					catch (Exception e)
					{
						Debug.LogError(jsonData);
						Debug.LogError(e);
					}
				}
                else
                {
					action(null);
				}
			}
		}


		/// <summary>
		/// ほかのユーザーの譜面を、musicId指定で4つ取得する
		/// </summary>
		/// <param name="musicId"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public IEnumerator GetOthersOriginalSheetInfo(int musicId,Action<OthersOriginalSheetList> action)
		{

			WWWForm form = new WWWForm();
			form.AddField("userName", SaveSystem.I.GetUserId());

			form.AddField("password", SaveSystem.I.GetPassword());
			form.AddField("musicId", musicId);

			string url = ENTRANCE_PATH + "/GetUserRanadomSheet.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
			}
			else
			{
				string jsonData = uwr.downloadHandler.text;

				try
				{
					if (!jsonData.Contains("NoData"))
					{
						OthersOriginalSheetList sheetInfos = JsonUtility.FromJson<OthersOriginalSheetList>(jsonData);
						action(sheetInfos);
					}
				}
				catch (Exception e)
				{
					Debug.LogError(jsonData);
					Debug.LogError(e);
				}
			}
		}

		/// <summary>
		/// プレイヤーが作成した、musicIdの譜面をすべて取得する
		/// </summary>
		/// <param name="musicId">musicId</param>
		/// <param name="action">引数は(musicID,"譜面id",譜面番号)のリスト</param>
		/// <returns></returns>
		public IEnumerator GetPlayerOriginalSheetInfo(int musicId, Action<(int,string,int)[]> action)
		{

			WWWForm form = new WWWForm();
			form.AddField("userName", SaveSystem.I.GetUserId());

			form.AddField("password", SaveSystem.I.GetPassword());
			form.AddField("musicId", musicId);

			string url = ENTRANCE_PATH + "/GetUserUploadedMusic_FromUniApp.php";

			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
			}
			else
			{
				string jsonData = uwr.downloadHandler.text;

				try
				{
					if (!jsonData.Contains("NoData"))
					{
						OthersOriginalSheetList sheetInfos = JsonUtility.FromJson<OthersOriginalSheetList>(jsonData);
						action(Array.ConvertAll(sheetInfos.SheetInfos, info => (info.MusicId, info.SheetId, info.Number)));
					}
				}
				catch (Exception e)
				{
					Debug.LogError(jsonData);
					Debug.LogError(e);
				}
			}
		}

		private IEnumerator GetUserData_Connect()
		{

			WWWForm form = new WWWForm();
			form.AddField("userName", SaveSystem.I.GetUserId());

			form.AddField("password", SaveSystem.I.GetPassword());

			string url = ENTRANCE_PATH+"GetUserDataFromApp.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
			}
			else
			{
				string jsonData = uwr.downloadHandler.text;
				try
				{


					userData = JsonUtility.FromJson<UserData>(jsonData);
					UserData.MusicDatas.ForEach(x => Debug.Log(x));
				}
                catch (Exception e)
                {
					Debug.LogError(jsonData);
					Debug.LogError(e);
				}
			}
		}
		private IEnumerator GetAllMissionState()
        {
			WWWForm form = new WWWForm();
			form.AddField("userName", SaveSystem.I.GetUserId());

			form.AddField("password", SaveSystem.I.GetPassword());

			string url = ENTRANCE_PATH+"ControlMissionFromApp.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
			}
			else
			{
				string jsonData = uwr.downloadHandler.text;

				try
				{
					jsonData = "{\"clearedIds\":" + jsonData + "}";
					MissionUserData d = JsonUtility.FromJson<MissionUserData>(jsonData);
					MissionUserData = new HashSet<int>(d.ClearedIds);
				}
				catch (Exception e)
				{
					Debug.LogError(jsonData);
					Debug.LogError(e);
				}
			}
		}
		private IEnumerator GetAllMusic_Connect()
		{

			WWWForm form = new WWWForm();
			form.AddField("userName", SaveSystem.I.GetUserId());

			form.AddField("password", SaveSystem.I.GetPassword());

			string url = ENTRANCE_PATH+"GetAllMusicInfosFromApp.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
			}
			else
			{
				string jsonData = uwr.downloadHandler.text;

				try
				{
					jsonData = "{\"music\":" + jsonData + "}";
					Music music = JsonUtility.FromJson<Music>(jsonData);
					music.music.ForEach(x => Debug.Log(x));
					Music = music;
				}
				catch (Exception e)
				{
					Debug.LogError(jsonData);
					Debug.LogError(e);
				}
			}
		}
		#endregion

		#region ChangeData
		public IEnumerator ChangeMissionClear(int id,Action clearAction)
        {
			WWWForm form = new WWWForm();
			form.AddField("userName", SaveSystem.I.GetUserId());
			form.AddField("password", SaveSystem.I.GetPassword());
			form.AddField("missionId", id);

			string url = ENTRANCE_PATH+"ControlMissionFromApp.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
			}
			else
			{
				string response = uwr.downloadHandler.text;
				if(response == "already")
				{
					Debug.LogError("このミッションはクリア済みです");
					yield break;
                }
				else
                {
                    try
                    {
						string jsonData = uwr.downloadHandler.text;
						jsonData = "{\"clearedIds\":" + jsonData + "}";
						MissionUserData d = JsonUtility.FromJson<MissionUserData>(jsonData);
						MissionUserData = new HashSet<int>(d.ClearedIds);
						clearAction();
					}
                    catch (Exception e)
                    {

						Debug.LogError("予期しない応答 : " + response);
						Debug.LogError(e);
					}
					
		
                }
			}
		}
		public IEnumerator UpdateUserData(Record record)
		{

			WWWForm form = new WWWForm();
			form.AddField("userName", SaveSystem.I.GetUserId());
			form.AddField("password", SaveSystem.I.GetPassword());
			form.AddField("musicId", record.MusicId);
			form.AddField("difficulty", (int)record.Course);
			form.AddField("score", record.Score);
			form.AddField("perfect", record.GradeMap[JudgeEnum.Perfect]);
			form.AddField("good", record.GradeMap[JudgeEnum.Good]);
			form.AddField("miss", record.GradeMap[JudgeEnum.Miss]);
			form.AddField("maxCombo", record.MaxCombo);


			string url = ENTRANCE_PATH+"ChangeUserScore_From_TBApp.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
			}
			else
			{
				string jsonData = uwr.downloadHandler.text;

				try
				{
					userData = JsonUtility.FromJson<UserData>(jsonData);
				}
				catch (Exception e)
				{
					Debug.LogError(jsonData);
					Debug.LogError(e);
				}
			}

		}
		public IEnumerator UploadSheet(int musicId,SheetData sheetData,Action<string,int> onUploadCompleated)
		{

			WWWForm form = new WWWForm();
			form.AddField("userName", SaveSystem.I.GetUserId());
			form.AddField("password", SaveSystem.I.GetPassword());
			form.AddField("musicId", musicId);
			form.AddField("sheetData", JsonUtility.ToJson(sheetData));


			string url = ENTRANCE_PATH+"UploadSheetFromApp.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
			}
			else
			{
				string result = uwr.downloadHandler.text;

				try
				{
			
					if (result == "full")
					{
						Debug.Log($"上限に達しているためアップロードできません");
					}
					else
					{
						ResultForUploadSheet sheetUploadingStatus = JsonUtility.FromJson<ResultForUploadSheet>(result);
						Debug.Log($"現在{sheetUploadingStatus.CurrentUploadedNum}個の譜面がサーバー上に存在します");
						Debug.Log($"譜面idは{sheetUploadingStatus.OriginalSheetId}です");
						Debug.Log($"残りのアップロード可能回数は{sheetUploadingStatus.RestUploadNum}です");

						if (Setting.SAVE_FILE)
						{
							string Path = directory + musicId + ".json";
							SaveSystem.I.SetCanUploadNumber(sheetUploadingStatus.RestUploadNum);
							File.Delete(Path);
						}
                        else
                        {
							string keyForPlayerPrefs = "madeSheet_" + musicId;
							SaveSystem.I.SetCanUploadNumber(sheetUploadingStatus.RestUploadNum);
							PlayerPrefs.DeleteKey(keyForPlayerPrefs);
						}
						onUploadCompleated(sheetUploadingStatus.OriginalSheetId, sheetUploadingStatus.RestUploadNum);
					}
				}
				catch (Exception e)
				{
					Debug.LogError(result);
					Debug.LogError(e);
				}
			}

		}
		#endregion

	}

	#region class for json
	[System.Serializable]
	public class UserData
	{
		[SerializeField] private List<CourseUserRecord> musicData;
		[SerializeField] private int colorRank = 0;
		[SerializeField] private float curColPercentage = 0;
		[SerializeField] private float percentage = 0;

		[SerializeField] private int scoreSum = 0;
		public List<CourseUserRecord> MusicDatas
		{
			get
			{

				return musicData;
			}
		}

        public int ColorRank { get => colorRank; }
        public float CurColPercentage { get => curColPercentage; }
        public float Percentage { get => percentage; }
        public int ScoreSum { get => scoreSum;  }
    }

    /// <summary>
    /// ユーザーが残した、あるCourseの最高記録
    /// </summary>
    [System.Serializable]
	public class CourseUserRecord
    {
		[SerializeField] private string name;
		[SerializeField] private int musicId;
		[SerializeField] private Course difficulty;
		[SerializeField] private int score;

		[SerializeField] private int Perfect;
		[SerializeField] private int Good;
		[SerializeField] private int Miss;
		[SerializeField] private int MaxCombos;
		[SerializeField] private int PlayTimes;





		[SerializeField] private string playbackData;

		public string Name { get => name; }
		public int MusicId { get => musicId;  }
		public Course Course { get => difficulty; }
		public int Score { get => score;  }
        public int Perfect1 { get => Perfect; }
        public int Good1 { get => Good; }
        public int Miss1 { get => Miss; }
        public int MaxCombos1 { get => MaxCombos; }
        public int PlayTimes1 { get => PlayTimes; }

        public override string ToString()
        {
			return $"name:{Name},MusicId:{MusicId},dificulty:{Course},Score:{Score}";
        }
    }

	[System.Serializable]
	public class PlayBackData
    {
		[SerializeField] private List<DivJudge> DivJudgeList;
    }

	[System.Serializable]
	public class DivJudge
	{
		[SerializeField] private int[] judges;
	}



	[System.Serializable]
	public class Music
    {
		public List<OneMusic> music = new List<OneMusic>();
    }

	[System.Serializable]
	public class OneMusic
    {
		public int id;
		public string name;
		public int diff_easy;
		public int diff_normal;
		public int diff_hard;
		public int diff_lunatic;
		public bool fullcombo;

        public override string ToString()
        {
			return $"name:{name}";
        }

		public Difficulty GetDifficulty(Course course)
        {
			return course switch
			{
				Course.Easy => (Difficulty)diff_easy,
				Course.Normal => (Difficulty)diff_normal,
				Course.Hard => (Difficulty)diff_hard,
				Course.Lunatic => (Difficulty)diff_lunatic,
				Course.Original=> Difficulty.Gray,
				_ => throw new System.NotImplementedException(),
			};

		}
    }

	[Serializable]
	public class ResultForUploadSheet
    {
		[SerializeField] private string[] sheetUploadingStatus;

		public string OriginalSheetId => sheetUploadingStatus[0];
		public int RestUploadNum => int.Parse(sheetUploadingStatus[2]);
		public int CurrentUploadedNum => int.Parse(sheetUploadingStatus[1]);
    }

    [Serializable]
    public class MissionUserData
    {
		[SerializeField] private int[] clearedIds;

        public int[] ClearedIds { get => clearedIds; }
    }

	[Serializable]
	public class OthersOriginalSheetList
    {
		[SerializeField] private OthersOriginalSheet[] sheetInfos;
		public OthersOriginalSheet[] SheetInfos => sheetInfos;
	}

	[Serializable]
	public class OthersOriginalSheet
    {
		[SerializeField] private string id;
		[SerializeField] private string userName;
		[SerializeField] private int userColorAtMake;
		[SerializeField] private int musicId;
		[SerializeField] private int number;
		[SerializeField] private int userColor;

        public string SheetId { get => id;  }
        public string UserName { get => userName; }
        public int UserColorAtMake { get => userColorAtMake;  }
        public int MusicId { get => musicId; }

		/// <summary>
		/// ユーザーは同じ曲に対して難度も譜面を作れるが、これが何番目に相当するか
		/// </summary>
		public int Number { get => number; }
        public int UserColor { get => userColor; }
    }

    #endregion
}