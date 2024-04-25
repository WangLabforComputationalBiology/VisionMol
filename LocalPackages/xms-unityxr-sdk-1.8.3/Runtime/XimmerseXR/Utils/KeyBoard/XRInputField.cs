using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ximmerse.XR.Utils
{
    /// <summary>
    /// TMP_InputField组件上挂载可调用虚拟键盘
    /// </summary>
    public class XRInputField : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler, IBeginDragHandler,IEndDragHandler
    {
        private TMP_InputField currentInputField;
        SpriteState spriteState;
        Sprite normalSprite;
        bool isPointerEnter = false;

        public TMP_InputField CurrentInputField
        {
            get => currentInputField;
        }

        private void Start()
        {
            currentInputField = GetComponent<TMP_InputField>();
            spriteState = currentInputField.spriteState;
            normalSprite = currentInputField.image.sprite;
            currentInputField.enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {

            if (KeyboardManager.Instance!=null)
            {
                KeyboardManager.Instance.CurrentXRInputField = this;
                KeyboardManager.Instance.ShowKeyBoard();
            }
            else
            {
                Debug.LogError("KeyboardManager is null");
            }
        }

        private void OnDisable()
        {
            KeyboardManager.Instance.HideKeyBoard();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerEnter = true;
            currentInputField.image.sprite = spriteState.highlightedSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerEnter = false;
            currentInputField.image.sprite = normalSprite;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            currentInputField.image.sprite = spriteState.selectedSprite;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isPointerEnter)
            {
                currentInputField.image.sprite = spriteState.highlightedSprite;
            }
            else
            {
                currentInputField.image.sprite = normalSprite;
            }
        }
    }

}
