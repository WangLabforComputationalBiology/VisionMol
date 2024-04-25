using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ximmerse.XR.Utils
{
    public class KeyCodeInputVaried : KeyCodeInput
    {
        [SerializeField] public string defaultKey;
        [SerializeField] public string capsKey;
        [SerializeField] string symbolKey1;
        [SerializeField] string symbolKey2;

        private void Start()
        {
            keycodeBtn.onClick.AddListener(KeyCodeBtnAddListener);
            keyCodeStringDel += ChangeKey;
        }

        public void ChangeKey()
        {
            if (keyType == KeyType.Varied)
            {
                if (KeyboardManager.Instance.IsSymbol)
                {
                    if (KeyboardManager.Instance.IsCaps)
                    {
                        currentKeyString.text = symbolKey2;
                    }
                    else
                    {
                        currentKeyString.text = symbolKey1;
                    }
                }
                else
                {
                    if (KeyboardManager.Instance.IsCaps)
                    {
                        currentKeyString.text = capsKey;
                    }
                    else
                    {
                        currentKeyString.text = defaultKey;
                    }
                }

                if (string.IsNullOrEmpty(currentKeyString.text))
                {
                    keycodeBtn.interactable = false;
                }
                else
                {
                    keycodeBtn.interactable = true;
                }

            }
            else if (keyType == KeyType.caps)
            {
                if (KeyboardManager.Instance.IsSymbol)
                {
                    if (KeyboardManager.Instance.IsCaps)
                    {
                        currentKeyString.text = symbolKey2;
                    }
                    else
                    {
                        currentKeyString.text = symbolKey1;
                    }
                }
                else
                {
                    if (KeyboardManager.Instance.IsCaps)
                    {
                        currentKeyString.text = capsKey;
                    }
                    else
                    {
                        currentKeyString.text = defaultKey;
                    }
                }
            }
        }


    }

}
