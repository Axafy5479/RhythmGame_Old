using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Login
{
	public class DecidingUserName : MonoBehaviour
	{
		[SerializeField] private TMPro.TMP_InputField inputField;

		[SerializeField] private NotificationManager noticeLayout;
		[SerializeField] private NotificationManager dupplicatedNotice;
		[SerializeField] private NotificationManager numberNotice;
		[SerializeField] private NotificationManager typeNotice;

		[SerializeField] private GoToTutorialScene goTutorial;



        private void Start()
        {
    //        if (Application.platform == RuntimePlatform.WindowsEditor)
    //        {
				//if (!SaveSystem.I.CheckNewUserData())
				//{
				//	SaveSystem.I.Load();
				//	goTutorial.StartTutorial();
				//}
    //        }
        }

		public void UserNameDecided()
		{
			StartCoroutine(HttpConnect(inputField.text));
		}

		private IEnumerator TryMakeNewUser(string newUserName)
        {
			yield return StartCoroutine(CheckUserNameDupplication(inputField.text));
		}

		public void ShowNotice()
        {
			if (inputField.text.Length < 1)
			{
				Debug.LogError("文字数が1文字以上になるように修正してください");
				numberNotice.OpenNotification();
				return;
			}

			if (inputField.text.Length > 17)
			{
				Debug.LogError("文字数が16文字以下になるように修正してください");
				numberNotice.OpenNotification();
				return;
			}


			if (Regex.IsMatch(inputField.text, @"[^a-zA-z0-9_]"))
			{
				Debug.LogError("半角英数とアンダーバーで入力してください");
				typeNotice.OpenNotification();
				return;
			}
			StartCoroutine(TryMakeNewUser(inputField.text));

		}

		public void CloseNotice()
		{
			noticeLayout.CloseNotification();
			inputField.enabled = true;
		}


		private IEnumerator CheckUserNameDupplication(string newUserName)
		{

			WWWForm form = new WWWForm();
			form.AddField("newUserName", newUserName);


			string url = "https://framari.org/Entrance_for_unityApp/CheckUserNameDupplicationFromApp.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
				SaveSystem.I.DeleteUserIdAndPass();
			}
			else
			{

				string result = uwr.downloadHandler.text;

				if (result == "UserNameDupplicated")
				{
					//Debug.LogError("このユーザー名は既に存在しています");
					dupplicatedNotice.OpenNotification();
				}
				else
				{
					noticeLayout.OpenNotification();
					inputField.enabled = false;
				}
			}
		}

		IEnumerator HttpConnect(string newUserName)
		{

			WWWForm form = new WWWForm();
			form.AddField("newUserName", newUserName);

			string password = StringUtils.GeneratePassword(32);
			form.AddField("password", password);
			SaveSystem.I.SetUserId(newUserName,password);


			string url = "https://framari.org/Entrance_for_unityApp/RegisterPlayerAsNewUser.php";
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
			{
				Debug.Log(uwr.error);
				SaveSystem.I.DeleteUserIdAndPass();
			}
			else
			{
				string result = uwr.downloadHandler.text;
				if (result == "UserNameDupplicated")
				{
					Debug.Log("UserNameDupplicated");
				}
				else
				{
					Debug.Log(result);
					yield return Data.GetUserData.I.GetData();
					
					goTutorial.StartTutorial();
				}
			}
		}
	}
}


public static class StringUtils
{
	private const string PASSWORD_CHARS =
		"0123456789abcdefghijklmnopqrstuvwxyz";

	public static string GeneratePassword(int length)
	{
		var sb = new System.Text.StringBuilder(length);
		var r = new System.Random();

		for (int i = 0; i < length; i++)
		{
			int pos = r.Next(PASSWORD_CHARS.Length);
			char c = PASSWORD_CHARS[pos];
			sb.Append(c);
		}

		return sb.ToString();
	}
}