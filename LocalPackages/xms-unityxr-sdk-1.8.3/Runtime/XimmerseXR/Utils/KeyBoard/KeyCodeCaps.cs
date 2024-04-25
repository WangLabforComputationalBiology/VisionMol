using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Ximmerse.XR.Utils
{
    public class KeyCodeCaps : KeyCodeInput
    {
        [SerializeField] Image keyIcon;
        [SerializeField] Sprite defaultLetterSprite;
        [SerializeField] Sprite HoverLetterSprite;
        [SerializeField] Sprite defaultSymbolSprite;
        [SerializeField] Sprite HoverSymbolSprite;


        private void Start()
        {
            keycodeBtn.onClick.AddListener(KeyCodeBtnAddListener);
            keyCodeStringDel += ChangeTipState;
        }

        private void ChangeTipState()
        {
            if (KeyboardManager.Instance.IsSymbol)
            {
                if (KeyboardManager.Instance.IsCaps)
                {
                    //tipText.key = KeyboardManager.back;
                    keyIcon.sprite = HoverSymbolSprite;
                    //tipText.OnTextLanguageChange();
                }
                else
                {
                    //tipText.key = KeyboardManager.more;
                    keyIcon.sprite = defaultSymbolSprite;

                    //tipText.OnTextLanguageChange();
                }
            }
            else
            {
                if (KeyboardManager.Instance.IsCaps)
                {
                    keyIcon.sprite = HoverLetterSprite;
                }
                else
                {
                    keyIcon.sprite = defaultLetterSprite;
                }
            }
        }


    }

}
