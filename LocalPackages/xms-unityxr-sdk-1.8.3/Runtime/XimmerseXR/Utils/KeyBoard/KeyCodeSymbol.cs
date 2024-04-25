using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Ximmerse.XR.Utils
{
    public class KeyCodeSymbol : KeyCodeInput
    {
        [SerializeField] Image tipImage;
        [SerializeField] Sprite letterImage;
        [SerializeField] Sprite symbolImage;


        private void Start()
        {
            keycodeBtn.onClick.AddListener(KeyCodeBtnAddListener);
            keyCodeStringDel += ChangeTipState;
        }

        private void ChangeTipState()
        {
            if (KeyboardManager.Instance.IsSymbol)
            {
                tipImage.sprite = letterImage;
            }
            else
            {
                tipImage.sprite = symbolImage;
            }
        }


    }

}
