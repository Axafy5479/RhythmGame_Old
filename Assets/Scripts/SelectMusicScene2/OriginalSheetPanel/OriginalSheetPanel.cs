using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace MusicSelectScene.OriginalSheetPanel
{
    public class OriginalSheetPanel : MonoBehaviour
    {
        [SerializeField] private GameObject othersMusicButton;
        [SerializeField] private GameObject unuploadedButton;
        [SerializeField] private GameObject uploadedButton;



        [SerializeField] private Transform othersMusicButtonTrn;
        [SerializeField] private Transform playerUnuploadedButtonTrn;
        [SerializeField] private Transform playerUploadedButtonTrn;

        //private void Awake()
        //{
        //    CloseButtonClicked();
        //}

        public void CloseButtonClicked()
        {
            Clear();
            this.gameObject.SetActive(false);
        }

        private void Clear()
        {
            foreach (Transform item in othersMusicButtonTrn)
            {
                Destroy(item.gameObject);
            }

            foreach (Transform item in playerUnuploadedButtonTrn)
            {
                Destroy(item.gameObject);
            }

            foreach (Transform item in playerUploadedButtonTrn)
            {
                Destroy(item.gameObject);
            }
        }

        private void OnEnable()
        {
            //�v���C���[�̖��A�b�v���[�h�̕��ʂ�����Ε\��
            var musicData = GetUserData.I.GetUsersSheet(Setting.I.LastPlayedMusic.MusicId);
            if(musicData!=null)
            {
                PlayerUnuploadedSheetButton pButton = Instantiate(unuploadedButton, playerUnuploadedButtonTrn).GetComponent<PlayerUnuploadedSheetButton>();
                pButton.InitializeForUnuploaded(musicData);
            }

            //�T�[�o�[��ɂ���v���C���[�̍�i
            StartCoroutine(GetUploadedPlayerSheet());

            //�T�[�o�[��́A�ق��̃��[�U�[�̍�i
            StartCoroutine(GetOthersSheet());
        }

        private IEnumerator GetOthersSheet()
        {
            //SaveSystem.I.Load();
            //yield return GetUserData.I.GetData();
            yield return GetUserData.I.GetOthersOriginalSheetInfo(Setting.I.LastPlayedMusic.MusicId, MakeSheetInfoList);
        }

        private IEnumerator GetUploadedPlayerSheet()
        {
            //SaveSystem.I.Load();
            //yield return GetUserData.I.GetData();
            yield return GetUserData.I.GetPlayerOriginalSheetInfo(Setting.I.LastPlayedMusic.MusicId, OnObtainingPlayerSheet);
        }

        private void MakeSheetInfoList(OthersOriginalSheetList list)
        {
            foreach (var item in list.SheetInfos)
            {
                OriginalSheetButton sheetButton = Instantiate(othersMusicButton, othersMusicButtonTrn).GetComponent<OriginalSheetButton>();
                sheetButton.Initialize(item);

            }
        }

        private void OnObtainingPlayerSheet((int musicId,string sheetId,int sheetNumber)[] infos)
        {
            foreach (var item in infos)
            {
                PlayerUploadedSheetButton sheetButton = Instantiate(uploadedButton, playerUploadedButtonTrn).GetComponent<PlayerUploadedSheetButton>();
                sheetButton.InitializeForUploaded(item.musicId,item.sheetId,item.sheetNumber);
            }
        }
    }
}
