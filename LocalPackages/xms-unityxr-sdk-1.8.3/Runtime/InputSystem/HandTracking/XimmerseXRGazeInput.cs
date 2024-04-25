using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using Ximmerse.XR.InputSystems.GazeAndGestureInteraction;


namespace Ximmerse.XR.InputSystems
{

    /// <summary>
    /// Gaze input state 
    /// </summary>
    public struct GazeInputState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('X', 'M', 'G', 'I');

        [InputControl(name = "gazeRayState", layout = "Integer")]
        public uint gazeRayState;

        [InputControl(name = "gazeRayOrigin", layout = "Vector3")]
        public Vector3 gazeRayOrigin;

        [InputControl(name = "gazeRayRotation", layout = "Quaternion")]
        public Quaternion gazeRayRotation;

        /// <summary>
        /// Gaze 视线下的确认操作
        /// </summary>
        [InputControl(name = "gazeSelect", layout = "Button")]
        public bool gazeSelect;

        /// <summary>
        /// Gaze 视线下的确认操作
        /// </summary>
        [InputControl(name = "gazeSelectValue", layout = "Axis")]
        public float gazeSelectValue;

        /// <summary>
        /// 是否正在视线交互过程?
        /// </summary>
        [InputControl(name = "isInteracting", layout = "Button")]
        public bool isInteracting;

        /// <summary>
        /// 拖动手势速度.
        /// </summary>
        [InputControl(name = "gestureDragVelocity", layout = "Vector3")]
        public Vector3 gestureDragVelocity;

        /// <summary>
        /// gesture scale factor.
        /// </summary>
        [InputControl(name = "gestureScaleFactor", layout = "Axis")]
        public float gestureScaleFactor;

        /// <summary>
        /// gesture rotate factor.
        /// </summary>
        [InputControl(name = "gestureRotateFactor", layout = "Axis")]
        public float gestureRotateFactor;

    }

    /// <summary>
    ///  Gaze input device
    /// </summary>
    [InputControlLayout(isGenericTypeOfDevice = false, displayName = "Ximmerse XR Gaze", stateType = typeof(GazeInputState))]
    public class XimmerseXRGazeInput : InputDevice, IInputUpdateCallbackReceiver
    {
        public static XimmerseXRGazeInput gazeInput
        {
            get; internal set;
        }

        [InputControl(name = "GazeInputState/gazeRayState")]
        public IntegerControl gazeRayState
        {
            get; internal set;
        }

        [InputControl(name = "GazeInputState/gazeRayOrigin")]
        public Vector3Control gazeRayOrigin
        {
            get; internal set;
        }

        [InputControl(name = "GazeInputState/gazeRayRotation")]
        public QuaternionControl gazeRayRotation
        {
            get; internal set;
        }

        /// <summary>
        /// Gaze 视线下的确认操作
        /// </summary>
        [InputControl(name = "GazeInputState/gazeSelect")]
        public ButtonControl gazeSelect
        {
            get; internal set;
        }

        [InputControl(name = "GazeInputState/gazeSelectValue")]
        public AxisControl gazeSelectValue
        {
            get; internal set;
        }

        /// <summary>
        /// 是否正在视线交互过程?
        /// </summary>
        [InputControl(name = "GazeInputState/isInteracting")]
        public ButtonControl isInteracting;

        /// <summary>
        /// 拖动手势速度.
        /// </summary>
        [InputControl(name = "GazeInputState/gestureDragVelocity")]
        public Vector3Control gestureDragVelocity
        {
            get; internal set;
        }

        /// <summary>
        /// 双手缩放操作
        /// </summary>
        [InputControl(name = "GazeInputState/gestureScaleFactor")]
        public AxisControl gestureScaleFactor
        {
            get; internal set;
        }


        /// <summary>
        /// 双手旋转操作
        /// </summary>
        [InputControl(name = "GazeInputState/gestureRotateFactor")]
        public AxisControl gestureRotateFactor
        {
            get; internal set;
        }

        private Vector3 prevDuralPalmCenterForRotationAlg;

        /// <summary>
        /// Internal gaze input state.
        /// </summary>
        public GazeInputState state
        {
            get; private set;
        }

        public static event System.Action OnPostUpdate;

        public XimmerseXRGazeInput() : base()
        {
            displayName = "Ximmerse XR Gaze";
        }


        protected override void FinishSetup()
        {
            base.FinishSetup();
            gazeRayState = GetChildControl<IntegerControl>("gazeRayState");
            gazeRayOrigin = GetChildControl<Vector3Control>("gazeRayOrigin");
            gazeRayRotation = GetChildControl<QuaternionControl>("gazeRayRotation");
            gazeSelect = GetChildControl<ButtonControl>("gazeSelect");
            gazeSelectValue = GetChildControl<AxisControl>("gazeSelectValue");
            isInteracting = GetChildControl<ButtonControl>("isInteracting");
            gestureDragVelocity = GetChildControl<Vector3Control>("gestureDragVelocity");

            gestureScaleFactor = GetChildControl<AxisControl>("gestureScaleFactor");
            gestureRotateFactor = GetChildControl<AxisControl>("gestureRotateFactor");
        }
        public void OnUpdate()
        {
            var _state = new GazeInputState();
            GazeAndHandInteractionSystem.GetEyeReticleLocalPose(out Pose eyeReticlePose);
            _state.gazeRayState = 3u;
            _state.gazeRayOrigin = eyeReticlePose.position;
            _state.gazeRayRotation = eyeReticlePose.rotation;
            //Debug.LogFormat("Internal get gaze pose : {0}/{1}", _state.gazeRayOrigin, _state.gazeRayRotation);

            //Check gaze interacting:
            {
                if (HandTracking.LeftHandTrackInfo.IsValid || (HandTracking.RightHandTrackInfo.IsValid))
                {
                    _state.isInteracting = true;
                }
            }
            //Check gaze confirm :
            {
                //Left or right hand grisp = gaze confirm action:
                if (HandTracking.LeftHandTrackInfo.IsValid && HandTracking.LeftHandTrackInfo.gestureGrisp == GestureType_Grisp.GraspClosed || HandTracking.LeftHandTrackInfo.NativeGestureType == (int)TouchlessA3D.GestureType.CLOSED_PINCH)
                {
                    _state.gazeSelect = true;
                    _state.gazeSelectValue = 1;
                    _state.gestureDragVelocity = HandTracking.LeftHandTrackInfo.PalmProjectionVelocity;
                }

                if (HandTracking.RightHandTrackInfo.IsValid && HandTracking.RightHandTrackInfo.gestureGrisp == GestureType_Grisp.GraspClosed || HandTracking.RightHandTrackInfo.NativeGestureType == (int)TouchlessA3D.GestureType.CLOSED_PINCH)
                {
                    _state.gazeSelect = true;
                    _state.gazeSelectValue = 1;
                    float vel01 = _state.gestureDragVelocity.sqrMagnitude;
                    float vel02 = HandTracking.RightHandTrackInfo.PalmProjectionVelocity.sqrMagnitude;
                    _state.gestureDragVelocity = vel02 > vel01 ? HandTracking.RightHandTrackInfo.PalmProjectionVelocity : _state.gestureDragVelocity;//Note:取速度高的拖拽
                }
            }

            if (HandTracking.LeftHandTrackInfo.IsValid && HandTracking.RightHandTrackInfo.IsValid && HandTracking.PrevLeftHandTrackInfo.IsValid && HandTracking.PrevRightHandTrackInfo.IsValid)
            {
                UpdateDualHandGestureState(ref _state, HandTracking.LeftHandTrackInfo, HandTracking.RightHandTrackInfo, HandTracking.PrevLeftHandTrackInfo, HandTracking.PrevRightHandTrackInfo);
            }

            InputSystem.QueueStateEvent(this, _state);

            state = _state;

            OnPostUpdate?.Invoke();
        }





        /// <summary>
        /// Update dual hand gesture state info.
        /// </summary>
        /// <param name="_state"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private void UpdateDualHandGestureState(ref GazeInputState _state, HandTrackingInfo left, HandTrackingInfo right, HandTrackingInfo prevLeft, HandTrackingInfo prevRight)
        {
            //判读 Scale Factor 和 Rotate Factor:
            if (TryCalculateScaleFactor(left, right, out float scaleFactor))
            {
                // Debug.LogFormat("UpdateDualHandGestureState : scaleFactor = {0}", scaleFactor.ToString("F3"));
                _state.gestureScaleFactor = scaleFactor;
            }
            else
            {
                _state.gestureScaleFactor = 1; //default scale value when no scale gesture is performing
            }

            if (TryCalculateRotationFactor(left, right, out float rotationAngular))
            {
                _state.gestureRotateFactor = rotationAngular;
              //  Debug.LogFormat("UpdateDualHandGestureState {0} / {1} @1", rotationAngular, Time.frameCount);
            }
            else
            {
                _state.gestureRotateFactor = 0; //no rotation
               // Debug.LogFormat("UpdateDualHandGestureState {0} / {1} @2", rotationAngular, Time.frameCount);
            }
        }

        /// <summary>
        /// 检测是否满足 缩放手势的判断标准，如果是，则输出缩放系数。 大于1为放大， 小于1为缩小
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="scaleFactor">缩放系数, 大于1为放大， 小于1为缩小.</param>
        /// <returns></returns>
        bool TryCalculateScaleFactor(HandTrackingInfo left, HandTrackingInfo right, out float scaleFactor)
        {
            var leftVel = left.PalmProjectionVelocity;
            var rightVel = right.PalmProjectionVelocity;
            //判断左右手的水平位移大于Y和Z位移:
            bool leftH = (Mathf.Abs(leftVel.x) >= 3 * Mathf.Abs(leftVel.y) && Mathf.Abs(leftVel.x) >= 3 * Mathf.Abs(leftVel.z));
            bool rightH = (Mathf.Abs(rightVel.x) >= 3 * Mathf.Abs(rightVel.y) && Mathf.Abs(rightVel.x) >= 3 * Mathf.Abs(rightVel.z));
            bool flip = Mathf.Sign(leftVel.x) != Mathf.Sign(rightVel.x);
            float vel = 0;
            int sign = 1; //1 : 放大 ， -1 缩小

            //双手水平缩放，反向运动:
            if (leftH && rightH && flip)
            {
                vel = Mathf.Max(Mathf.Abs(leftVel.x), Mathf.Abs(rightVel.x)) / 10;//10是测算出的一个附加系数
                sign = leftVel.x < 0 ? 1 : -1;//用左手的运动判断是放大还是缩小
            }
            //单左手水平移动:
            else if (leftH && !rightH)
            {
                vel = Mathf.Abs(leftVel.x) / 10;//10是测算出的一个附加系数
                sign = leftVel.x < 0 ? 1 : -1;//用左手的运动判断是放大还是缩小
            }
            //单右手水平移动:
            else if (!leftH && rightH)
            {
                vel = Mathf.Abs(rightVel.x) / 10;//10是测算出的一个附加系数
                sign = leftVel.x < 0 ? -1 : 1;//用右手的运动判断是放大还是缩小
            }

            //人为限定缩放操作的上下限L
            const float kMaxScaleFactor = 1.25f, kMinScaleFactor = 0.75f;
            //如果满足上述条件:
            if (vel > 0)
            {
                if (sign > 0) //放大
                {
                    scaleFactor = Mathf.Clamp(1 + vel, 1, kMaxScaleFactor);
                }
                else //缩小
                {
                    scaleFactor = Mathf.Clamp(1 - vel, kMinScaleFactor, 1);
                }
                return true;
            }
            else
            {
                scaleFactor = 0;
                return false;
            }
        }


        /// <summary>
        /// 计算旋转操作系数.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="rotationAngularSpeed">如果是顺时针操作，rotationAngularSpeed 为正数； 如果是逆时针操作， rotationAngularSpeed 是负数</param>
        /// <returns></returns>
        bool TryCalculateRotationFactor(HandTrackingInfo left, HandTrackingInfo right, out float rotationAngularSpeed)
        {
            Vector3 center = (left.PalmProjectionPosition + right.PalmProjectionPosition) / 2;
            Vector3 prevCenter = prevDuralPalmCenterForRotationAlg;
            prevDuralPalmCenterForRotationAlg = center;//save center point for next frame

            //  Debug.LogFormat("UpdateDualHandGestureState : center1 : {0}, center2: {1}, mid : {2}", center.ToString("F3"), prevCenter.ToString("F3"), ((center + prevCenter) / 2).ToString("F3"));

            float radian01 = GetDeltaRadians(left.PalmProjectionPosition, center, prevCenter);
            float radian02 = GetDeltaRadians(right.PalmProjectionPosition, center, prevCenter);
            float angularSpeed01 = Mathf.Rad2Deg * radian01 / Time.deltaTime;
            float angularSpeed02 = Mathf.Rad2Deg * radian02 / Time.deltaTime;
            //   Debug.LogFormat("UpdateDualHandGestureState : L vel : {0}, R vel : {1}, L angular: {2}, R angular: {3}", left.PalmProjectionVelocity.ToString("F3"), right.PalmProjectionVelocity.ToString("F3"), angularSpeed01.ToString("F2"), angularSpeed02.ToString("F2"));

            float absA01 = Mathf.Abs(angularSpeed01);
            float absA02 = Mathf.Abs(angularSpeed02);

            int sign = 1;
            float absAngularSpeed = 0;
            const float kAngularSpeedDiffEndurance = 100; //100度是左右手旋转容忍度
            const float kAcceptableAngularSpeedMin = 10;//低于此数字的 angular speed 被认为是静止:

            //两手静止:
            if (absA01 <= kAcceptableAngularSpeedMin && absA02 <= kAcceptableAngularSpeedMin)
            {
                absAngularSpeed = 0;
                sign = 0;
            }
            else if (Mathf.Abs(absA02 - absA01) <= kAngularSpeedDiffEndurance)//100角度差以内，说明两手旋转角度差距不大
            {
                absAngularSpeed = Mathf.Max(Mathf.Abs(angularSpeed01), Mathf.Abs(angularSpeed02));
                sign = angularSpeed02 > 0 ? -1 : 1; //在双手同时旋转的情况下，旋转符号由右手定义
            }
            else if ((Mathf.Abs(absA02) - Mathf.Abs(absA01)) > kAngularSpeedDiffEndurance)//右手比左手快100 : 使用右手作为旋转数据
            {
                absAngularSpeed = angularSpeed02;
                sign = angularSpeed02 > 0 ? -1 : 1; //旋转符号由右手定义
            }
            else if ((Mathf.Abs(absA01) - Mathf.Abs(absA02)) > kAngularSpeedDiffEndurance)//右手比左手快100 : 使用右手作为旋转数据
            {
                absAngularSpeed = angularSpeed01;
                sign = angularSpeed01 > 0 ? 1 : -1; //旋转符号由右手定义
            }

            if (absAngularSpeed > 0)
            {
                rotationAngularSpeed = absAngularSpeed * sign;
                return true;
            }
            else
            {
                rotationAngularSpeed = 0;
                return false;
            }
        }

        float GetDeltaRadians(Vector3 point, Vector3 center, Vector3 previousCenter)
        {
            float prevRadian = Mathf.Atan2(point.x - previousCenter.x, point.z - previousCenter.z);
            float radian = Mathf.Atan2(point.x - center.x, point.z - center.z);
            var d = Mathf.Repeat(prevRadian - radian, Mathf.PI * 2.0f);
            if (d > Mathf.PI)
            {
                d -= Mathf.PI * 2.0f;
            }
            return d;
        }


        /// <summary>
        /// Clear dual hand state info fields.
        /// </summary>
        private void ClearDualHandGestureState(ref GazeInputState _state)
        {
            _state.gestureRotateFactor = 0;
            _state.gestureScaleFactor = 0;
        }
    }
}