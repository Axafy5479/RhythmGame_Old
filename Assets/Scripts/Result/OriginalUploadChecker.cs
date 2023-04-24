using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Michsky.UI.ModernUIPack;
using TMPro;

public class OriginalUploadChecker:MonoBehaviour
{
    [SerializeField] private ModalWindowManager windowManager;
    [SerializeField] private ButtonManagerWithIcon uploadButton;
    [SerializeField] private ButtonManagerWithIcon cancelButtonText;



    private Coroutine coroutine;
    private MakeSheetScene.SheetData SheetData { get; set; }
    private int MusicId { get; set; } = -1;
    public void ShowConfirmationScreen(MakeSheetScene.SheetData sheetData, int musicId)
    {
        SheetData = sheetData;
        MusicId = musicId;
        windowManager.descriptionText =
            $"この譜面をサーバーにアップロードし、\nユーザーに公開できます。\n\nアップロードした際端末上から譜面データが削除され、\n<color=red>今後修正はできなくなります</color>。\n\nアップロードできる譜面は残り{SaveSystem.I.UserData.CanUploadNumber}枚です。";
        windowManager.UpdateUI();
        windowManager.OpenWindow();

    }

    public void ForDebugging()
    {
        //ShowConfirmationScreen(GetUserData.I.GetUsersSheet(7), 7);
        SaveSystem.I.Load();
        ShowConfirmationScreen(GetUserData.I.GetUsersSheet(7), 7);
    }

    public void OKButtonClicked()
    {
        coroutine = StartCoroutine(Check(SheetData, MusicId));
        SheetData = null;
        MusicId = -1;
        uploadButton.enabled = false;
        cancelButtonText.enabled = false;
        //OnUploadCompleated("11", 10);
    }

    public void CancelButtonClicked()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        SheetData = null;
        MusicId = -1;
        windowManager.CloseWindow();
    }

    public IEnumerator Check(MakeSheetScene.SheetData sheetData, int musicId)
    {
        yield return StartCoroutine(GetUserData.I.UploadSheet(musicId, sheetData, OnUploadCompleated));
    }

    private void OnUploadCompleated(string sheetId,int lestNumber)
    {

        uploadButton.gameObject.SetActive(false);
        cancelButtonText.enabled = true;

        windowManager.titleText = "アップロード完了";
        windowManager.descriptionText = $"<color=yellow><size=30><b>譜面Idは「{sheetId}」です</b></size></color>。\nホーム画面の「Id検索」ボタンを選択し\nこのIdを入力することで、いつでもプレイできます。\n\nアップロード可能な譜面は\n残り{lestNumber}枚です。";
        windowManager.UpdateUI();
        cancelButtonText.buttonText = "閉じる";
        cancelButtonText.UpdateUI();
    }





}

