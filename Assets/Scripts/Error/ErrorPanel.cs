using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Error
{
    internal class ErrorPanel : MonoBehaviour
    {
        [SerializeField] private ModalWindowManager windowManager;

        public void Show(ErrorType eType)
        {
            windowManager.titleText = ErrorManager.MesagesMap[eType].title;
            windowManager.descriptionText = ErrorManager.MesagesMap[eType].content;

            windowManager.OpenWindow();
        }
    }

}
