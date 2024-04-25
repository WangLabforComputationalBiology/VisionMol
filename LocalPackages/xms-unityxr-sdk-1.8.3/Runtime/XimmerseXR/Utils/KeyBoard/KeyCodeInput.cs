using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Ximmerse.XR.Utils
{
    public delegate void KeyCodeDel();
    public class KeyCodeInput : MonoBehaviour
    {
        public enum KeyType
        {
            /// <summary>
            /// 字母以及符号，切换
            /// </summary>
            Varied,
            /// <summary>
            /// 数字以及字符,不变
            /// </summary>
            Constant,
            /// <summary>
            /// 清除
            /// </summary>
            clear,
            /// <summary>
            /// 删除
            /// </summary>
            cancel,
            /// <summary>
            /// 确认
            /// </summary>
            enter,
            /// <summary>
            /// 大小写切换
            /// </summary>
            caps,
            /// <summary>
            /// 符号切换
            /// </summary>
            symbol
        }
        [SerializeField] protected TMP_Text currentKeyString;
        [SerializeField] protected Button keycodeBtn;
        //[SerializeField] private Image capsImage;
        //[SerializeField] Sprite defaultCaps
        //[SerializeField] public string defaultKey;
        //[SerializeField] public string capsKey;
        //[SerializeField] string symbolKey1;
        //[SerializeField] string symbolKey2;
        [SerializeField] protected KeyType keyType;

        protected KeyCodeDel keyCodeStringDel;
        private static List<KeyCodeInput> keyCodeInputs = new List<KeyCodeInput>();
        public static List<KeyCodeInput> KeyCodeInputs
        {
            get => keyCodeInputs;
            set => keyCodeInputs = value;
        }

        public TMP_Text CurrentTmp
        {
            get => currentKeyString;
            set => currentKeyString = value;
        }
        public Button KeycodeBtn
        {
            get => keycodeBtn;
            set => keycodeBtn = value;
        }

        private void Awake()
        {
            keyCodeInputs.Add(this);
        }

        private void Start()
        {
            keycodeBtn.onClick.AddListener(KeyCodeBtnAddListener);
        }

        private void OnDestroy()
        {
            keyCodeInputs.Remove(this);
        }


        protected void KeyCodeBtnAddListener()
        {
            if (keyType == KeyType.Varied || keyType == KeyType.Constant)
            {
                KeyboardManager.Instance.KeyCodeBtnToInputField(currentKeyString.text);
            }
            else if (keyType == KeyType.cancel)
            {
                KeyboardManager.Instance.KeyCodeBtnCancel();
            }
            else if (keyType == KeyType.enter)
            {
                KeyboardManager.Instance.KeyCodeBtnReturn();
            }
            else if (keyType == KeyType.clear)
            {
                KeyboardManager.Instance.KeyCodeBtnClear();
            }
            else if (keyType == KeyType.symbol)
            {
                KeyboardManager.Instance.KeyCodeBtnSymbolChange();
            }
            else if (keyType == KeyType.caps)
            {
                KeyboardManager.Instance.KeyCodeBtnCapsChange();
            }

        }

        public void ChangeCurrentKeyString()
        {
            keyCodeStringDel?.Invoke();
        }

    }
}
