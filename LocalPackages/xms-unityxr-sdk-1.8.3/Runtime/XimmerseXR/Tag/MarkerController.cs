using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.XR.Tag
{
    [RequireComponent(typeof(TagTracking))]
    public class MarkerController : MonoBehaviour
    {
        private enum Controller
        {
            left = 0,
            right = 1,
        }

        [SerializeField] 
        private Controller m_marker = Controller.left;

        private bool isTriggerButtonDown;

        private bool isTouchButtonDown;

        private bool isAppButtonDown;

        private bool isHomeButtonDown;

        private int triggerValue;
        /// <summary>
        /// Whether Trigger Button is pressed
        /// </summary>
        public bool TriggerButtonDown
        {
            get => isTriggerButtonDown;
        }
        /// <summary>
        /// Whether Touch Button is pressed
        /// </summary>
        public bool TouchButtonDown
        {
            get => isTouchButtonDown;
        }
        /// <summary>
        /// Whether App Button is pressed
        /// </summary>
        public bool AppButtonDown
        {
            get => isAppButtonDown;
        }
        /// <summary>
        /// Whether Home Button is pressed
        /// </summary>
        public bool HomeButtonDown
        {
            get => isHomeButtonDown;
        }



        private void OnValidate()
        {
            if (m_marker == Controller.left)
            {
                gameObject.GetComponent<TagTracking>().TrackId = 82;
            }
            if (m_marker == Controller.right)
            {
                gameObject.GetComponent<TagTracking>().TrackId = 81;
            }
        }

        public bool IsTriggerButtonDown()
        {
#if !UNITY_EDITOR
            if (XDevicePlugin.xdevc_ctrl_get_button_state_bitmask(XDevicePlugin.xdevc_get_controller((int)m_marker)) == 32)
            {
                isTriggerButtonDown = true;
            }
            else
            {
                isTriggerButtonDown = false;
            }
#endif
            return isTriggerButtonDown;
        }

        public void IsGetAllButtonDown()
        {
            #if !UNITY_EDITOR
            if (XDevicePlugin.xdevc_ctrl_get_button_state_bitmask(XDevicePlugin.xdevc_get_controller((int)m_marker)) == 32)
            {
                isTriggerButtonDown = true;
                Debug.Log("isTriggerButtonDown +32");
            }
            else
            {
                isTriggerButtonDown = false;
            }

            if (XDevicePlugin.xdevc_ctrl_get_button_state_bitmask(XDevicePlugin.xdevc_get_controller((int)m_marker)) == 4)
            {
                isTouchButtonDown = true;
                Debug.Log("isTouchButtonDown +4");
            }
            else
            {
                isTouchButtonDown = false;
            }

            if (XDevicePlugin.xdevc_ctrl_get_button_state_bitmask(XDevicePlugin.xdevc_get_controller((int)m_marker)) == 16)
            {
                isAppButtonDown = true;
                Debug.Log("isAppButtonDown +16");
            }
            else
            {
                isAppButtonDown = false;
            }

            if (XDevicePlugin.xdevc_ctrl_get_button_state_bitmask(XDevicePlugin.xdevc_get_controller((int)m_marker)) == 8)
            {
                isHomeButtonDown = true;
                Debug.Log("isHomeButtonDown +8");
            }
            else
            {
                isHomeButtonDown = false;
            }
            #endif
        }

        public int TriggerValue()
        {
#if !UNITY_EDITOR
            triggerValue = XDevicePlugin.xdevc_ctrl_get_trigger(XDevicePlugin.xdevc_get_controller((int)m_marker));
#endif
            return triggerValue;
        }

        private void Update()
        {
            IsGetAllButtonDown();
        }
    }
}

