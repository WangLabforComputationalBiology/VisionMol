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
    /// gesture state for input system to couple with input action properties system.
    /// </summary>
    public struct HandState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('X', 'M', 'G', 'T');

        #region Properties represents hand track state

        [InputControl(name = "isTracked", layout = "Button")]
        public bool isTracked;

        /// <summary>
        /// 0 = none, 3 (1+2) = position + rotation
        /// </summary>
        [InputControl(name = "trackingState", layout = "Integer")]
        public uint trackingState;

        /// <summary>
        /// Palm position
        /// </summary>
        [InputControl(name = "palmPosition", layout = "Vector3")]
        public Vector3 palmPosition;

        /// <summary>
        /// 手掌沿手指前向的法线
        /// </summary>
        [InputControl(name = "palmRotation", layout = "Quaternion")]
        public Quaternion palmRotation;

        /// <summary>
        /// 手掌的尺寸大小.
        /// </summary>
        [InputControl(name = "palmScale", layout = "Vector3")]
        public Vector3 palmScale;

        /// <summary>
        /// 掌心向上法线
        /// </summary>
        [InputControl(name = "palmNormal", layout = "Vector3")]
        public Vector3 palmNormal;

        /// <summary>
        /// 手掌的世界空间速度
        /// </summary>
        [InputControl(name = "palmWorldVelocity", layout = "Vector3")]
        public Vector3 palmWorldVelocity;

        /// <summary>
        /// 手掌相对于用户头部空间的速度.X = 水平轴， Y = 纵向轴， Z = 深度轴
        /// </summary>
        [InputControl(name = "palmProjectionVelocity", layout = "Vector3")]
        public Vector3 palmProjectionVelocity;

        /// <summary>
        /// Palm position 2d in view space.
        /// </summary>
        [InputControl(name = "palmViewSpacePosition", layout = "Vector2")]
        public Vector2 palmViewSpacePosition;

        /// <summary>
        /// 手势类型 : 抓取
        /// </summary>
        [InputControl(name = "isGrasp", layout = "Button")]
        public bool isGrasp;

        /// <summary>
        /// 手势类型 : 张开手掌
        /// </summary>
        [InputControl(name = "isOpenHand", layout = "Button")]
        public bool isOpenHand;

        /// <summary>
        /// 手势类型 : 握拳
        /// </summary>
        [InputControl(name = "isClosedHand", layout = "Button")]
        public bool isClosedHand;

        /// <summary>
        /// 手势类型 : 握拳
        /// </summary>
        [InputControl(name = "gripValue", layout = "Axis")]
        public float gripValue;

        /// <summary>
        /// 手掌射线方向
        /// </summary>
        [InputControl(name = "handRayDirection", layout = "Quaternion")]
        public Quaternion handRayDirection;

        #endregion

    }

    /// <summary>
    /// Hand gesture input device
    /// </summary>
    [InputControlLayout(commonUsages = new[] { "LeftHandGesture", "RightHandGesture" }, isGenericTypeOfDevice = false, displayName = "Ximmerse XR Hand", stateType = typeof(HandState))]
    public class XimmerseXRHandInput : InputDevice, IInputUpdateCallbackReceiver
    {
        public static XimmerseXRHandInput leftHand
        {
            get; internal set;
        }

        public static XimmerseXRHandInput rightHand
        {
            get; internal set;
        }

        /// <summary>
        /// 0 = left, 1 = right
        /// </summary>
        public int handness
        {
            get; internal set;
        }

        [InputControl(name = "HandState/isTracked")]
        public ButtonControl isTracked
        {
            get; internal set;
        }

        /// <summary>
        /// Tracking state, the value mapped to UnityEngine.XR.InputTrackingState:
        /// None = 0, Position = 1, Rotation = 2,Velocity = 4,AngularVelocity = 8,
        /// Acceleration = 16,AngularAcceleration = 32, All = 63
        /// </summary>
        [InputControl(name = "HandState/trackingState")]
        public IntegerControl trackingState
        {
            get; internal set;
        }


        [InputControl(name = "HandState/palmPosition")]
        public Vector3Control palmPosition
        {
            get; internal set;
        }

        [InputControl(name = "HandState/palmRotation")]
        public QuaternionControl palmRotation
        {
            get; internal set;
        }

        [InputControl(name = "HandState/palmScale")]
        public Vector3Control palmScale
        {
            get; internal set;
        }

        [InputControl(name = "HandState/palmNormal")]
        public Vector3Control palmNormal
        {
            get; internal set;
        }

        /// <summary>
        /// 手掌的世界空间速度
        /// </summary>
        [InputControl(name = "HandState/palmWorldVelocity")]
        public Vector3Control palmWorldVelocity
        {
            get; internal set;
        }
        /// <summary>
        /// 手掌相对于用户头部空间的速度.X = 水平轴， Y = 纵向轴， Z = 深度轴
        /// </summary>
        [InputControl(name = "HandState/palmProjectionVelocity")]
        public Vector3Control palmProjectionVelocity
        {
            get; internal set;
        }


        /// <summary>
        /// Palm position 2d in view space.
        /// </summary>
        [InputControl(name = "HandState/palmViewSpacePosition")]
        public Vector2Control palmViewSpacePosition
        {
            get; internal set;
        }

        [InputControl(name = "HandState/isGrasp")]
        public ButtonControl isGrasp
        {
            get; internal set;
        }

        [InputControl(name = "HandState/isOpenHand")]
        public ButtonControl isOpenHand
        {
            get; internal set;
        }

        [InputControl(name = "HandState/isClosedHand")]
        public ButtonControl isClosedHand
        {
            get; internal set;
        }

        [InputControl(name = "HandState/gripValue")]
        public AxisControl gripValue
        {
            get; internal set;
        }


        /// <summary>
        /// 手掌射线方向
        /// </summary>
        [InputControl(name = "HandState/handRayDirection")]
        public QuaternionControl handRayDirection
        {
            get; internal set;
        }

        /// <summary>
        /// The native hand track info data.
        /// </summary>
        public HandTrackingInfo handTrackInfo
        {
            get => HandTracking.LeftHandTrackInfo;
        }

        public XimmerseXRHandInput() : base()
        {
            displayName = "Ximmerse Gesture Input Device";
        }

        protected override void FinishSetup()
        {
            base.FinishSetup();
            isTracked = GetChildControl<ButtonControl>("isTracked");
            trackingState = GetChildControl<IntegerControl>("trackingState");
            palmPosition = GetChildControl<Vector3Control>("palmPosition");
            palmRotation = GetChildControl<QuaternionControl>("palmRotation");
            palmScale = GetChildControl<Vector3Control>("palmScale");
            palmNormal = GetChildControl<Vector3Control>("palmNormal");
            palmWorldVelocity = GetChildControl<Vector3Control>("palmWorldVelocity");
            palmProjectionVelocity = GetChildControl<Vector3Control>("palmProjectionVelocity");
            palmViewSpacePosition = GetChildControl<Vector2Control>("palmViewSpacePosition");

            isGrasp = GetChildControl<ButtonControl>("isGrasp");
            isOpenHand = GetChildControl<ButtonControl>("isOpenHand");
            isClosedHand = GetChildControl<ButtonControl>("isClosedHand");
            gripValue = GetChildControl<AxisControl>("gripValue");

            handRayDirection = GetChildControl<QuaternionControl>("handRayDirection");
        }

        public void OnUpdate()
        {
            var _state = new HandState();
            UpdateHandState(ref _state, handness == 0 ? HandTracking.LeftHandTrackInfo : HandTracking.RightHandTrackInfo);
            InputSystem.QueueStateEvent(this, _state);
        }

        private void UpdateHandState(ref HandState _state, HandTrackingInfo _handTrackInfo)
        {
            _state.isTracked = _handTrackInfo.IsValid;

            _state.trackingState = _handTrackInfo.IsValid ? 3u : 0u;
            _state.palmPosition = _handTrackInfo.PalmLocalPosition;
            _state.palmRotation = _handTrackInfo.PalmLocalRotation;
            _state.palmNormal = _handTrackInfo.PalmLocalNormal;
            _state.palmScale = _handTrackInfo.PalmScale;
            _state.palmWorldVelocity = _handTrackInfo.PalmVelocity;
            _state.palmProjectionVelocity = _handTrackInfo.PalmProjectionVelocity;
            _state.palmViewSpacePosition = _handTrackInfo.PalmViewSpacePosition;
            _state.handRayDirection = _handTrackInfo.handRayDirection;

            //if (_handTrackInfo.IsValid)
            //{
            //    //Debug.LogFormat("{0} vel: {1}, in-head vel: {2}", _handTrackInfo.Handness, _handTrackInfo.PalmVelocity.ToString("F3"), _handTrackInfo.PalmProjectionVelocity.ToString("F3"));
            //    Debug.LogFormat("{0} view pos2d : {1}", _handTrackInfo.Handness, _state.palmViewSpacePosition.ToString("F3"));
            //}

            //如果 gaze interaction system 存在， 采用其抛出的优化值:
            bool isclosepinch = _handTrackInfo.NativeGestureType == (int)TouchlessA3D.GestureType.CLOSED_PINCH;
            bool isclosehand = _handTrackInfo.gestureFistOpenHand == GestureType_Fist_OpenHand.Fist;

            //Debug.LogFormat("On hand {0} update !, tacked:{1}", this.name + "," + this.usages[0], _state.isTracked);
            if (_handTrackInfo.IsValid)
            {
                if (isclosepinch || isclosehand)
                {
                    _state.isGrasp = true;
                }
                else
                {
                    _state.isGrasp = false;
                }
                _state.isOpenHand = _handTrackInfo.gestureFistOpenHand == GestureType_Fist_OpenHand.Opened;
                _state.isClosedHand = _handTrackInfo.gestureFistOpenHand == GestureType_Fist_OpenHand.Fist;
                _state.gripValue = _state.isGrasp ? 1 : 0;
            }
            else
            {
                _state.isGrasp = false;
                _state.isOpenHand = true;
                _state.isClosedHand = false;
                _state.gripValue = 0;
            }
        }
    }
}