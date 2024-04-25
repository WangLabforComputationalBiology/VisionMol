using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Ximmerse.RhinoX
{
    /// <summary>
    /// RhinoX virtual keyboard input field.
    /// 对于需要虚拟键盘支持的文本输入框， 添加此组件到 InputField 对象上。
    /// 如果需要支持 TextMeshPro 的 InputField 类，只需要将代码中对 UnityEngine.UI.InputField 的引用, 改成 TMPro.TMP_InputField 类即可。
    /// </summary>
    //[RequireComponent(typeof(InputField))]
    public class RxInputField : MonoBehaviour, IPointerClickHandler
    {
        void Start()
        {
            if (GetComponent<InputField>())
                GetComponent<InputField>().enabled = false;//禁用UI自带的组件，使用VR keyboard 接管。
        }
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("OnPointerClick");
            if (VirtualKeyboardManager.Instance)
            {
                if(GetComponent<InputField>())
                {
                    VirtualKeyboardManager.Instance.SetFocusInputField(GetComponent<InputField>());//设置当前焦点输入框
                }
                VirtualKeyboardManager.Instance.Display(true);
            }
            else
            {
                Debug.LogError("Virtual Keyboard Manager == Null");
            }
        }
    }
}