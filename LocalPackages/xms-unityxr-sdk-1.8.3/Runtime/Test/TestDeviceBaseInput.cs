using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Ximmerse.XR.Utils;
using System.Text;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

namespace Ximmerse.XR.tests
{

    public class TestDeviceBaseInput : MonoBehaviour
    {
        public XRController left, right;

        StringBuilder leftHandMsg = new StringBuilder();

        StringBuilder rightHandMsg = new StringBuilder();

        public bool TestHaptic = true;

        public InputHelpers.Button HapticButton = InputHelpers.Button.TriggerButton;

        static InputHelpers.Button[] xrButtons = new InputHelpers.Button[]
        {
            InputHelpers.Button.MenuButton,
            InputHelpers.Button.TriggerButton,
            InputHelpers.Button.GripButton,
            InputHelpers.Button.PrimaryButton,
            InputHelpers.Button.SecondaryButton,
            InputHelpers.Button.Primary2DAxisTouch,
            InputHelpers.Button.Primary2DAxisClick,
        };

        static InputHelpers.Button[] xrAxis = new InputHelpers.Button[]
        {
            InputHelpers.Button.Trigger,
            InputHelpers.Button.Grip,
            InputHelpers.Button.PrimaryAxis2DRight,
            InputHelpers.Button.PrimaryAxis2DLeft,
            InputHelpers.Button.PrimaryAxis2DUp,
            InputHelpers.Button.PrimaryAxis2DDown,
        };

        void LateUpdate()
        {
            leftHandMsg.Clear();
            if (left)
            {
                var _inputDevice = left.inputDevice;
                if (_inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 p)
                    && _inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion q))
                {
                    // Debug.LogFormat("Hand input device p = {0}, q = {1} (Left)", p.ToString("F3"), q.eulerAngles.ToString("F2"));
                }

                foreach (var btn in xrButtons)
                {
                    if (_inputDevice.IsPressed(btn, out bool isPressed) && isPressed)
                    {
                        leftHandMsg.AppendFormat(" {0} ", btn);
                    }
                }

                foreach (var btn in xrAxis)
                {
                    if (_inputDevice.TryReadSingleValue(btn, out float val) && val != 0)
                    {
                        leftHandMsg.AppendFormat("\r\n {0}={1} ", btn, val);
                    }
                }

                if (leftHandMsg.Length > 0)
                {
                    Matrix4x4 world = Camera.main.transform.parent.localToWorldMatrix * Matrix4x4.TRS(left.currentControllerState.position, left.currentControllerState.rotation, Vector3.one);
                    RxDraw.Text3D(world.GetColumn(3), Quaternion.LookRotation(Camera.main.transform.forward), 0.01f, leftHandMsg.ToString(), Color.green);
                }


                if (TestHaptic && _inputDevice.IsPressed(this.HapticButton, out bool isPressedHapticBtn) && isPressedHapticBtn)
                {
                    bool ret = _inputDevice.TryGetHapticCapabilities(out HapticCapabilities hapticCapabilities);
                    // Debug.LogFormat("Unity.SendHandleHapticImpulse_Left , ret: {0} support haptic: {1}, supports buffer: {2}", ret, hapticCapabilities.supportsImpulse, hapticCapabilities.supportsBuffer);
                    bool sentResult = _inputDevice.SendHapticImpulse(0u, 1.0f, 1f);
                    Debug.LogFormat("Left Haptic impluse result: {0}", sentResult);
                    //  _inputDevice.SendHapticBuffer(0, new byte[10]);
                }
            }

            rightHandMsg.Clear();
            if (right)
            {
                var _inputDevice = right.inputDevice;
                if (_inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 p)
                                    && _inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion q))
                {
                    //Debug.LogFormat("Hand input device p = {0}, q = {1} (Right)", p.ToString("F3"), q.eulerAngles.ToString("F2"));
                }

                foreach (var btn in xrButtons)
                {
                    if (_inputDevice.IsPressed(btn, out bool isPressed) && isPressed)
                    {
                        rightHandMsg.AppendFormat(" {0} ", btn);
                    }
                }

                foreach (var btn in xrAxis)
                {
                    if (_inputDevice.TryReadSingleValue(btn, out float val) && val != 0)
                    {
                        rightHandMsg.AppendFormat("\r\n {0}={1} ", btn, val);
                    }
                }

                if (rightHandMsg.Length > 0)
                {
                    Matrix4x4 world = Camera.main.transform.parent.localToWorldMatrix * Matrix4x4.TRS(right.currentControllerState.position, right.currentControllerState.rotation, Vector3.one);
                    RxDraw.Text3D(world.GetColumn(3), Quaternion.LookRotation(Camera.main.transform.forward), 0.01f, rightHandMsg.ToString(), Color.green);
                }

                if (TestHaptic && _inputDevice.IsPressed(this.HapticButton, out bool isPressedHapticBtn) && isPressedHapticBtn)
                {
                    bool ret = _inputDevice.TryGetHapticCapabilities(out HapticCapabilities hapticCapabilities);
                    //  Debug.LogFormat("Unity.SendHandleHapticImpulse_Right , ret: {0} support haptic: {1}, supports buffer: {2}", ret, hapticCapabilities.supportsImpulse, hapticCapabilities.supportsBuffer);
                    bool sentResult = _inputDevice.SendHapticImpulse(0u, 1.0f, 1f);
                    Debug.LogFormat("Right Haptic impluse result: {0}", sentResult);
                    // _inputDevice.SendHapticBuffer(0, new byte[10]);
                }
            }
        }
    }
}