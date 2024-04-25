/***
 * Author: Yunhan Li
 * Any issue please contact yunhn.lee@gmail.com
 ***/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace Ximmerse.RhinoX
{
    /// <summary>
    /// 虚拟键盘管理器。 
    /// </summary>
    public class VirtualKeyboardManager : MonoBehaviour
    {
        #region Public Variables
        [Header("User defined")]
        [Tooltip("If the character is uppercase at the initialization")]
        public bool isUppercase = false;
        public int maxInputLength;

        [Header("UI Elements")]
        public Text inputText;

        [Header("Essentials")]
        public Transform keys;
        #endregion

        #region Private Variables
        private string Input
        {
            get {
                return inputText.text;
            }
            set {
                inputText.text = value;
            }
        }
        private Key[] keyList;
        private bool capslockFlag;

        CanvasGroup m_CanvasGroup;

        Collider[] keyColliders = new Collider[] { };

        Coroutine onDisplayCoroutine;

        #endregion

        public static VirtualKeyboardManager Instance
        {
            get; private set;
        }

        InputField m_Focus_InputField;

        /// <summary>
        /// 使用此事件，监听用户的输入改变行为。
        /// </summary>
        public static System.Action<string> OnInputTextIsChanged;

        #region Monobehaviour Callbacks
        void Awake()
        {
            Instance = this;
            keyList = keys.GetComponentsInChildren<Key>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
            keyColliders = m_CanvasGroup.GetComponentsInChildren<Collider>();
        }

        void Start()
        {
            for (int i = 0; i < keyList.Length; i++)
            {
                Key key = keyList[i];
                key.OnKeyClicked += GenerateInput;
            }
            capslockFlag = isUppercase;
            CapsLock();

            Display(false, true);//默认隐藏
        }
        #endregion

        #region Public Methods
        public void Backspace()
        {
            if (Input.Length > 0)
            {
                Input = Input.Remove(Input.Length - 1);
                if(m_Focus_InputField)
                {
                    m_Focus_InputField.text = Input;
                }
                OnInputTextIsChanged?.Invoke(Input);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 设置初始化输入内容文本。
        /// 此方法不会调用 OnInputTextIsChanged 事件。
        /// </summary>
        public void ResetToText(string text)
        {
            Input = text;
        }

        /// <summary>
        /// 清除当前输入框文本。
        /// 同时清除 Focus InputField
        /// </summary>
        public void Clear()
        {
            Input = string.Empty;
            if (m_Focus_InputField)
            {
                m_Focus_InputField.text = Input;
            }
            OnInputTextIsChanged?.Invoke(Input);
        }

        public void CapsLock()
        {
            for (int i = 0; i < keyList.Length; i++)
            {
                Key key = keyList[i];
                if (key is Alphabet)
                {
                    key.CapsLock(capslockFlag);
                }
            }
            capslockFlag = !capslockFlag;
        }

        public void Shift()
        {
            for (int i = 0; i < keyList.Length; i++)
            {
                Key key = keyList[i];
                if (key is Shift)
                {
                    key.ShiftKey();
                }
            }
        }

        public void GenerateInput(string s)
        {
            if (Input.Length > maxInputLength)
            {
                return;
            }
            Input += s;
            if (m_Focus_InputField)
            {
                m_Focus_InputField.text = Input;
            }
            OnInputTextIsChanged?.Invoke(Input);
        }
        #endregion

        /// <summary>
        /// 展示虚拟键盘
        /// </summary>
        /// <param name="displayed"></param>
        /// <param name="immediately">是否渐变显示</param>
        public void Display(bool displayed, bool immediately = false)
        {
            if (onDisplayCoroutine != null)
            {
                StopCoroutine(onDisplayCoroutine);
            }
            this.onDisplayCoroutine = StartCoroutine(OnDisplay(displayed, immediately));
        }

        /// <summary>
        /// 渐变消失。
        /// 此方法用于外部Button事件调用。
        /// </summary>
        public void FadeOut()
        {
            Display(false, true);
        }

        IEnumerator OnDisplay(bool displayed, bool immediately)
        {
            float st = Time.time;
            const float fadeTime = .2f;
            //toggle collider of keys:
            for (int i = 0; i < keyColliders.Length; i++)
            {
                Collider kc = keyColliders[i];
                kc.enabled = displayed;
            }
            if(!immediately)//不需要渐变
            {
                m_CanvasGroup.alpha = displayed ? 1 : 0;
                yield break;
            }
            //fading:
            while ((Time.time - st) <= fadeTime)
            {
                float t = displayed ? (Time.time - st) / fadeTime : 1 - ((Time.time - st) / fadeTime);
                m_CanvasGroup.alpha = t;
                yield return null;
            }

            m_CanvasGroup.alpha = displayed ? 1 : 0;
        }

        /// <summary>
        /// 设置当前的外部焦点输入框。
        /// </summary>
        /// <param name="Focus"></param>
        public void SetFocusInputField(InputField Focus)
        {
            m_Focus_InputField = Focus;
            ResetToText(Focus.text);
        }
    }
}