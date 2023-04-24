using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MakeSheetScene
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ChangeBpmPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private InputField inputField;
        private CanvasGroup cg;


        private void Awake()
        {
            cg = this.GetComponent<CanvasGroup>();
            Hide();
        }

        public void Show()
        {
            cg.blocksRaycasts = true;
            cg.alpha = 1;
        }

        private void Hide()
        {
            cg.blocksRaycasts = false;
            cg.alpha = 0;
        }

        public void ChangeInputField()
        {
            string text = inputField.text;
            try
            {
                float newBpm = float.Parse(text);
                SheetMaker.I.BPMChanged(SheetMaker.I.CurrentBar,newBpm);
            }
            catch
            {
                Debug.Log("êîílÇì¸óÕÇµÇƒÇ≠ÇæÇ≥Ç¢");
                return;
            }

            Hide();
        }
    }
}
