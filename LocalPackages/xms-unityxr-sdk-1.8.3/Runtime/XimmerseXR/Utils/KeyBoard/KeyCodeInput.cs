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
            /// ��ĸ�Լ����ţ��л�
            /// </summary>
            Varied,
            /// <summary>
            /// �����Լ��ַ�,����
            /// </summary>
            Constant,
            /// <summary>
            /// ���
            /// </summary>
            clear,
            /// <summary>
            /// ɾ��
            /// </summary>
            cancel,
            /// <summary>
            /// ȷ��
            /// </summary>
            enter,
            /// <summary>
            /// ��Сд�л�
            /// </summary>
            caps,
            /// <summary>
            /// �����л�
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
