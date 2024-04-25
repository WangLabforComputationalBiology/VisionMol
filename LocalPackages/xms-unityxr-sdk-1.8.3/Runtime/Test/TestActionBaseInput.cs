using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Ximmerse.XR.Utils;
using System.Text;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using UnityEngine.InputSystem;


namespace Ximmerse.XR.tests
{
    /// <summary>
    /// Action base input test script.
    /// </summary>

    public class TestActionBaseInput : MonoBehaviour
    {
        public ActionBasedController left, right;

        StringBuilder leftHandMsg = new StringBuilder();

        StringBuilder rightHandMsg = new StringBuilder();

        InputActionProperty[] leftActions, leftActionValues;

        InputActionProperty[] rightActions, rightActionValues;

        // Start is called before the first frame update
        void Start()
        {
            leftActions = new InputActionProperty[]
            {
                left.selectAction, left.selectAction, left.uiPressAction,
            };

            leftActionValues = new InputActionProperty[]
            {
                left.selectActionValue, left.activateActionValue, left.uiPressActionValue,
            };

            rightActions = new InputActionProperty[]
{
                right.selectAction, right.selectAction, right.uiPressAction,
};

            rightActionValues = new InputActionProperty[]
            {
                right.selectActionValue, right.activateActionValue, right.uiPressActionValue,
            };
        }

        // Update is called once per frame
        void Update()
        {
            leftHandMsg.Clear();
            foreach (var a in leftActions)
            {
                if (a.action.IsPressed())
                {
                    leftHandMsg.AppendFormat(" {0} ", a.action.name);
                }
            }
            foreach (var a in leftActionValues)
            {
                if (a.action.IsPressed())
                {
                    leftHandMsg.AppendFormat("\r\n {0} = {1}", a.action.name, a.action.ReadValue<float>());
                }
            }

            rightHandMsg.Clear();
            foreach (var a in rightActions)
            {
                if (a.action.IsPressed())
                {
                    rightHandMsg.AppendFormat(" {0} ", a.action.name);
                }
            }
            foreach (var a in rightActionValues)
            {
                if (a.action.IsPressed())
                {
                    rightHandMsg.AppendFormat("\r\n {0} = {1}", a.action.name, a.action.ReadValue<float>());
                }
            }

            if (leftHandMsg.Length > 0)
            {
                Matrix4x4 world = Matrix4x4.TRS(left.transform.position, left.transform.rotation, Vector3.one);
                RxDraw.Text3D(world.GetColumn(3), Quaternion.LookRotation(Camera.main.transform.forward), 0.01f, leftHandMsg.ToString(), Color.green);
            }

            if (rightHandMsg.Length > 0)
            {
                Matrix4x4 world = Matrix4x4.TRS(right.transform.position, right.transform.rotation, Vector3.one);
                RxDraw.Text3D(world.GetColumn(3), Quaternion.LookRotation(Camera.main.transform.forward), 0.01f, rightHandMsg.ToString(), Color.green);
            }
        }
    }
}