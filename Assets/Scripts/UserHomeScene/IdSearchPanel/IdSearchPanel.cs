using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Michsky.UI.ModernUIPack;
using MusicSelectScene.OriginalSheetPanel;

namespace UserHomeScene.IdSearch
{
    public class IdSearchPanel : MonoBehaviour
    {
        [SerializeField] private CustomInputField inputField;
        [SerializeField] private GameObject otherUploadedItem;
        [SerializeField] private Transform trn;
        [SerializeField] private NotificationManager idNotFoundNotification;

        private Coroutine coroutine;
        public void SearchButtonClicked()
        {
            if (trn.childCount != 0)
            {
                Destroy(trn.GetChild(0).gameObject);
            }
            coroutine = StartCoroutine(GetUserData.I.GetOriginalSheetInfoFromId(inputField.inputText.text, OnObtainingInfo));
        }

        private void OnObtainingInfo(OthersOriginalSheet info)
        {
            if (info != null)
            {
                Instantiate(otherUploadedItem, trn).GetComponent<OriginalSheetButton>().Initialize(info);
            }
            else
            {
                idNotFoundNotification.OpenNotification();
            }
        }

        public void OnCloseWindow()
        {
            inputField.inputText.text = "";
            if (trn.childCount != 0)
            {
                Destroy(trn.GetChild(0).gameObject);
            }
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}
