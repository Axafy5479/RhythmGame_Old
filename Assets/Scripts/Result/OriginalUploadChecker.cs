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
            $"���̕��ʂ��T�[�o�[�ɃA�b�v���[�h���A\n���[�U�[�Ɍ��J�ł��܂��B\n\n�A�b�v���[�h�����ے[���ォ�畈�ʃf�[�^���폜����A\n<color=red>����C���͂ł��Ȃ��Ȃ�܂�</color>�B\n\n�A�b�v���[�h�ł��镈�ʂ͎c��{SaveSystem.I.UserData.CanUploadNumber}���ł��B";
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

        windowManager.titleText = "�A�b�v���[�h����";
        windowManager.descriptionText = $"<color=yellow><size=30><b>����Id�́u{sheetId}�v�ł�</b></size></color>�B\n�z�[����ʂ́uId�����v�{�^����I����\n����Id����͂��邱�ƂŁA���ł��v���C�ł��܂��B\n\n�A�b�v���[�h�\�ȕ��ʂ�\n�c��{lestNumber}���ł��B";
        windowManager.UpdateUI();
        cancelButtonText.buttonText = "����";
        cancelButtonText.UpdateUI();
    }





}

