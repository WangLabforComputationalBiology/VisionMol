using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Ximmerse.XR.Utils
{
    /// <summary>
    /// 虚拟键盘，目前仅支持英文输入，在需要调用键盘的TMP_InputField上增加XRInputField脚本即可
    /// </summary>
    public class KeyboardManager : MonoBehaviour
    {

        string inputText;
        [SerializeField] TMP_InputField inputField;
        private XRInputField xRInputField;
        private RectTransform inputFieldTextRect;


        private bool isCaps;
        private bool isSymbol;
        public bool IsCaps
        {
            get => isCaps;
            set => isCaps = value;
        }
        public bool IsSymbol
        {
            get => isSymbol;
            set => isSymbol = value;
        }


        public XRInputField CurrentXRInputField
        {
            get => xRInputField;
            set => xRInputField = value;
        }

        #region Instance
        private static KeyboardManager instance;
        public static KeyboardManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<KeyboardManager>();
                }
                return instance;
            }
        }
        private void Awake()
        {
            instance = this;
        }
        #endregion

        private void Start()
        {
            //inputField.keyboardType = TouchScreenKeyboardType.NintendoNetworkAccount;
            inputFieldTextRect = inputField.textComponent.GetComponent<RectTransform>();
            inputField.onValueChanged.AddListener(InputSetting);
            HideKeyBoard();
        }

        private void OnEnable()
        {
            if (xRInputField!=null)
            {
                inputText = xRInputField.CurrentInputField.text;
                inputField.text = inputText;
            }
        }

        public void KeyCodeBtnToInputField(string keyCodeInput)
        {
            inputText = inputField.text;
            if (xRInputField.CurrentInputField.contentType == TMP_InputField.ContentType.IntegerNumber)
            {
                int input;
                if (int.TryParse(keyCodeInput, out input))
                {
                    inputText = inputText += input.ToString();
                    inputField.text = inputText;
                    xRInputField.CurrentInputField.text = inputText;
                }
            }
            else
            {
                inputText = inputText += keyCodeInput;
                inputField.text = inputText;
                xRInputField.CurrentInputField.text = inputText;
            }
            //StartCoroutine(ResetInputFieldCaret());
        }
        IEnumerator ResetInputFieldCaret()
        {
            if (!inputField.isFocused)
            {
                inputField.ActivateInputField();//主动选中输入框，但是会自动执行SelectAll，未知原因
                var color = inputField.selectionColor;
                color.a = 0;//这里暂时使用的改变颜色来避免SelectAll导致的闪烁
                inputField.selectionColor = color;
            }
            yield return new WaitForEndOfFrame();//需要延迟一帧后设置光标才会生效
            inputField.MoveTextEnd(true);
            inputField.ForceLabelUpdate();//立即强制刷新光标显示,否则会在下一帧才生效
            inputField.selectionColor = inputField.selectionColor;
        }

        public void KeyCodeBtnCancel()
        {
            if (inputText.Length>0)
            {
                inputText = inputText.Substring(0, inputText.Length - 1);
                inputField.text = inputText;
                xRInputField.CurrentInputField.text = inputText;
            }
        }

        public void KeyCodeBtnReturn()
        {
            HideKeyBoard();
        }

        public void KeyCodeBtnClear()
        {
            inputText = "";
            inputField.text = inputText;
            xRInputField.CurrentInputField.text = inputText;
        }

        public void KeyCodeBtnSymbolChange()
        {
            isCaps = false;
            isSymbol = !isSymbol;
            RefreshKey();
        }

        public void KeyCodeBtnCapsChange()
        {
            isCaps = !isCaps;
            RefreshKey();
        }

        public void RefreshKey()
        {
            foreach (var item in KeyCodeInput.KeyCodeInputs)
            {
                item.ChangeCurrentKeyString();
            }
        }

        public void ShowKeyBoard()
        {
            if (xRInputField!=null)
            {
                inputField.text = xRInputField.CurrentInputField.text;
                gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("XRInputField is null");
            }
        }

        public void HideKeyBoard()
        {
            gameObject.SetActive(false);
            xRInputField = null;
        }

        private void InputSetting(string text)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(inputFieldTextRect);
            if (inputFieldTextRect.sizeDelta.x> inputField.textViewport.sizeDelta.x)
            {
                inputFieldTextRect.anchoredPosition = inputField.textViewport.sizeDelta - inputFieldTextRect.sizeDelta;
            }
            else
            {
                inputFieldTextRect.anchoredPosition = Vector2.zero;
            }
        }

    }
}
