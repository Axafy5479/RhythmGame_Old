using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Error
{
    public enum ErrorType
    {
        Unknown = 0,
        Disconnection = 1,
        NoUserData = 2
    }

    public class ErrorManager
    {

        #region Singleton
        private static ErrorManager instance = new ErrorManager();
        public static ErrorManager Instance => instance;
        #endregion
        private ErrorManager() { }

        public void ShowErrorScreen(ErrorType eType)
        {
            GameObject ePanel = GameObject.Instantiate(Resources.Load<GameObject>("ErrorPanel/ErrorPanel"));
            ePanel.transform.SetParent(GameObject.Find("Canvas").transform,false);
            ePanel.GetComponent<ErrorPanel>().Show(eType); 

        }


        internal static Dictionary<ErrorType, (string title, string content)> MesagesMap
             = new Dictionary<ErrorType, (string title, string content)>()
             {
                 {ErrorType.Disconnection,("インターネット接続エラー","インターネット接続に失敗しました。タイトルに戻ります") },
                 {ErrorType.NoUserData,("ユーザー情報取得エラー","ユーザー情報取得に失敗しました。タイトルに戻ります") },
                 {ErrorType.Unknown,("不明なエラー","タイトルに戻ります") },
             };
    }
}
