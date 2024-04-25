using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;

namespace Ximmerse.XR.InputSystems
{
    [InputControlLayout(commonUsages = new[] { "DualHandGesture" }, isGenericTypeOfDevice = false, displayName = "DualHandGestureInputDevice", stateType = typeof(DualHandGestureInputState))]
    public class DualHandGestureInputDevice : InputDevice, IInputUpdateCallbackReceiver
    {

        /// <summary>
        /// Config of the dual hand input device.
        /// </summary>
        public DualHandInputDeviceConfig config
        {
            get; internal set;
        }

        public static DualHandGestureInputDevice dualHandInput;

        /// <summary>
        /// Is performing zoom or rotate gesture ?
        /// </summary>
        [InputControl(name = "DualHandGestureInputState/IsPerformed")]
        public ButtonControl IsPerformed
        {
            get; internal set;
        }

        /// <summary>
        /// 0 when isPerformed == false,
        /// 1 when performing zoom scale,
        /// 2 when performing rotateXZ scale.
        /// </summary>
        [InputControl(name = "DualHandGestureInputState/State")]
        public IntegerControl State
        {
            get; internal set;
        }

        /// <summary>
        /// Rotation gesture at XZ plane, in angular speed.
        /// </summary>
        [InputControl(name = "DualHandGestureInputState/RotateAngularXZ")]
        public AxisControl RotateAngularXZ
        {
            get; internal set;
        }

        /// <summary>
        /// Zoom gesture scale.
        /// </summary>
        [InputControl(name = "DualHandGestureInputState/ZoomScale")]
        public AxisControl ZoomScale
        {
            get; internal set;
        }

        private DualHandGestureInputState m_inputState = new DualHandGestureInputState();

        public DualHandGestureInputState InputState { get => m_inputState; }


        protected override void FinishSetup()
        {
            base.FinishSetup();
            IsPerformed = GetChildControl<ButtonControl>("IsPerformed");
            State = GetChildControl<IntegerControl>("State");
            RotateAngularXZ = GetChildControl<AxisControl>("RotateAngularXZ");
            ZoomScale = GetChildControl<AxisControl>("ZoomScale");
        }


        public void OnUpdate()
        {
            m_inputState.IsPerformed = 0;
            m_inputState.State = 0;
            UpdateDualHandState(ref m_inputState);
            InputSystem.QueueStateEvent(this, m_inputState);
        }

        private void UpdateDualHandState(ref DualHandGestureInputState _state)
        {
            var left = HandAnchorInputDevice.left;
            var right = HandAnchorInputDevice.right;

            //Update dual hand gesture input state when both hands are tracked:
            if (left.InputState_XR_Space.isTracked != 0 && right.InputState_XR_Space.isTracked != 0)
            {
                //When the two hands are griping:
                if (left.InputState_XR_Space.isGrip && right.InputState_XR_Space.isGrip)
                {
                    var lGripPointDelta = left.fingerGripPointRuntime - left.prevFingerGripPointRuntime;
                    var rGripPointDelta = right.fingerGripPointRuntime - right.prevFingerGripPointRuntime;

                    Vector3 dirL = lGripPointDelta.normalized;
                    Vector3 dirR = rGripPointDelta.normalized;

                    //Debug.LogFormat("UpdateDualHandState() : Left delta = {0}, deltaN ={1}, Right delta = {2}, deltaN = {3}", lGripPointDelta.magnitude.ToString("F5"), dirL, rGripPointDelta.magnitude.ToString("F5"), dirR);

                    bool l_isX = isXMovement(dirL);
                    bool r_isX = isXMovement(dirR);
                    if (l_isX || r_isX)
                    {
                        //signL is positive when zoom in, negative when zoom out
                        int signL = l_isX ? (int)Mathf.Sign(dirL.x) : 0;
                        //signR is negative when zoom in, positive when zoom out
                        int signR = r_isX ? (int)Mathf.Sign(dirL.y) : 0;
                        if (signL != signR)
                        {
                            //zoomDir is positive when zoom in, negative when zoom out
                            int zoomDir = signL != 0 ? -signL : signR;
                            float maxDelta = Mathf.Max(lGripPointDelta.magnitude, rGripPointDelta.magnitude);
                            if (maxDelta >= this.config.pointDeltaDistanceRange.x && maxDelta <= this.config.pointDeltaDistanceRange.y)
                            {
                                float vel = maxDelta / Time.deltaTime;
                                float scaleDelta = vel * this.config.velocityToScaleRate;
                                float scale = 1 + zoomDir * scaleDelta;
                                float clampScale = Mathf.Clamp(scale, this.config.ScaleRange.x, this.config.ScaleRange.y);
                                //  Debug.LogFormat("UpdateDualHandState - Zoom info: maxDelta = {0}, vel = {1}, scaleDelta = {2}, scale = {3}, clampScale = {4}", maxDelta.ToString("F5"), vel.ToString("F5"), scaleDelta.ToString("F5"), scale.ToString("F5"), clampScale.ToString("F5"));
                                _state.IsPerformed = 1;
                                _state.State = 1;
                                _state.ZoomScale = clampScale;
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Check if the direction alongs X axis ?
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        static bool isXMovement(Vector3 dir)
        {
            return Mathf.Abs(dir.x) >= Mathf.Abs(dir.y) * 3
                && Mathf.Abs(dir.x) >= Mathf.Abs(dir.z) * 3;
        }
    }

}