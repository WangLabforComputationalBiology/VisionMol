using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Ximmerse.XR.Utils
{
    /// <summary>
    /// ������̣�Ŀǰ��֧��Ӣ�����룬����Ҫ���ü��̵�TMP_InputField������XRInputField�ű�����
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
                inputField.ActivateInputField();//����ѡ������򣬵��ǻ��Զ�ִ��SelectAll��δ֪ԭ��
                var color = inputField.selectionColor;
                color.a = 0;//������ʱʹ�õĸı���ɫ������SelectAll���µ���˸
                inputField.selectionColor = color;
            }
            yield return new WaitForEndOfFrame();//��Ҫ�ӳ�һ֡�����ù��Ż���Ч
            inputField.MoveTextEnd(true);
            inputField.ForceLabelUpdate();//����ǿ��ˢ�¹����ʾ,���������һ֡����Ч
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
