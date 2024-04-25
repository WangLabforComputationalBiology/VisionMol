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
    /// <summary>
    ///  Short gun input state descriptor.
    /// </summary>
    public struct HandGunInputState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('X', 'M', 'G', 'N');

        #region Properties 

        /// <summary>
        /// 0 = none, 3 (1+2) = position + rotation
        /// </summary>
        [InputControl(name = "TrackingState", layout = "Integer")]
        public uint TrackingState;

        /// <summary>
        /// Palm position
        /// </summary>
        [InputControl(name = "Position", layout = "Vector3")]
        public Vector3 Position;

        /// <summary>
        /// 手掌沿手指前向的法线
        /// </summary>
        [InputControl(name = "Rotation", layout = "Quaternion")]
        public Quaternion Rotation;

        /// <summary>
        /// Function key
        /// </summary>
        [InputControl(name = "Function", layout = "Button")]
        public bool Function;


        /// <summary>
        /// Power key
        /// </summary>
        [InputControl(name = "Power", layout = "Button")]
        public bool Power;

        /// <summary>
        /// Trigger key
        /// </summary>
        [InputControl(name = "Trigger", layout = "Button")]
        public bool Trigger;

        /// <summary>
        /// Trigger value
        /// </summary>
        [InputControl(name = "TriggerValue", layout = "Axis")]
        public float TriggerValue;

        /// <summary>
        /// MagLoad key
        /// </summary>
        [InputControl(name = "MagLoad", layout = "Button")]
        public bool MagLoad;

        /// <summary>
        /// MagLoad key
        /// </summary>
        [InputControl(name = "MagRelease", layout = "Button")]
        public bool MagRelease;

        /// <summary>
        /// Grip key
        /// </summary>
        [InputControl(name = "Grip", layout = "Button")]
        public bool Grip;

        /// <summary>
        /// TriggerFingerDetection key
        /// </summary>
        [InputControl(name = "TriggerFingerDetection", layout = "Button")]
        public bool TriggerFingerDetection;


        /// <summary>
        /// 0 = loose, 1 = release
        /// </summary>
        [InputControl(name = "ChamberSlide", layout = "Axis")]
        public float ChamberSlide;

        #endregion

    }

    [InputControlLayout(commonUsages = new[] { "HandGun" }, isGenericTypeOfDevice = false, displayName = "HandGun", stateType = typeof(HandGunInputState))]
    public class HandGunInputDevice : InputDevice, IInputUpdateCallbackReceiver
    {

        static HandGunInputDevice handGunInputDevice;

        /// <summary>
        /// Adds or gets hand gun input devices
        /// </summary>
        /// <returns></returns>
#if UNITY_EDITOR
        //[UnityEditor.MenuItem("Ximmerse XR SDK/Create HandGun Device", false, 11)]
#endif
        public static HandGunInputDevice GetHandGunInputDevice()
        {
            if (handGunInputDevice != null)
            {
                return handGunInputDevice;
            }
            else
            {
                InputSystem.RegisterLayout<HandGunInputDevice>(matches: new InputDeviceMatcher()
   .WithInterface("HandGunInputState"));
                HandGunInputDevice _gun = (HandGunInputDevice)InputSystem.AddDevice("HandGunInputDevice", "HandGun");
                InputSystem.SetDeviceUsage(_gun, "HandGun");
                InputSystem.EnableDevice(_gun);
                HandGunInputDevice.handGunInputDevice = _gun;
                return _gun;
            }
        }

        System.Func<HandGunInputState> m_getter = null;

        public void RegisterStateGetter(System.Func<HandGunInputState> getter)
        {
            m_getter = getter;
        }


        /// <summary>
        /// Tracking state, the value mapped to UnityEngine.XR.InputTrackingState:
        /// None = 0, Position = 1, Rotation = 2,Velocity = 4,AngularVelocity = 8,
        /// Acceleration = 16,AngularAcceleration = 32, All = 63
        /// </summary>
        [InputControl(name = "HandGunInputState/TrackingState")]
        public IntegerControl TrackingState
        {
            get; internal set;
        }


        [InputControl(name = "HandGunInputState/Position")]
        public Vector3Control Position
        {
            get; internal set;
        }

        [InputControl(name = "HandGunInputState/Rotation")]
        public QuaternionControl Rotation
        {
            get; internal set;
        }


        /// <summary>
        /// Function key
        /// </summary>
        [InputControl(name = "HandGunInputState/Function")]
        public ButtonControl Function
        {
            get; internal set;
        }


        /// <summary>
        /// Power key
        /// </summary>
        [InputControl(name = "HandGunInputState/Power")]
        public ButtonControl Power
        {
            get; internal set;
        }

        /// <summary>
        /// Trigger key
        /// </summary>
        [InputControl(name = "HandGunInputState/Trigger")]
        public ButtonControl Trigger
        {
            get; internal set;
        }

        /// <summary>
        /// Trigger value
        /// </summary>
        [InputControl(name = "HandGunInputState/TriggerValue")]
        public AxisControl TriggerValue
        {
            get; internal set;
        }


        /// <summary>
        /// MagLoad key
        /// </summary>
        [InputControl(name = "HandGunInputState/MagLoad")]
        public ButtonControl MagLoad
        {
            get; internal set;
        }

        /// <summary>
        /// MagRelease key
        /// </summary>
        [InputControl(name = "HandGunInputState/MagRelease")]
        public ButtonControl MagRelease
        {
            get; internal set;
        }

        /// <summary>
        /// Grip key
        /// </summary>
        [InputControl(name = "HandGunInputState/Grip")]
        public ButtonControl Grip
        {
            get; internal set;
        }

        /// <summary>
        /// TriggerFingerDetection key
        /// </summary>
        [InputControl(name = "HandGunInputState/TriggerFingerDetection")]
        public ButtonControl TriggerFingerDetection
        {
            get; internal set;
        }

        /// <summary>
        /// ChamberSlide value
        /// </summary>
        [InputControl(name = "HandGunInputState/ChamberSlide")]
        public AxisControl ChamberSlide
        {
            get; internal set;
        }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            TrackingState = GetChildControl<IntegerControl>("TrackingState");
            Position = GetChildControl<Vector3Control>("Position");
            Rotation = GetChildControl<QuaternionControl>("Rotation");

            Function = GetChildControl<ButtonControl>("Function");
            Power = GetChildControl<ButtonControl>("Power");
            Trigger = GetChildControl<ButtonControl>("Trigger");
            TriggerValue = GetChildControl<AxisControl>("TriggerValue");
            MagLoad = GetChildControl<ButtonControl>("MagLoad");

            MagRelease = GetChildControl<ButtonControl>("MagRelease");
            Grip = GetChildControl<ButtonControl>("Grip");
            TriggerFingerDetection = GetChildControl<ButtonControl>("TriggerFingerDetection");

            ChamberSlide = GetChildControl<AxisControl>("ChamberSlide");
        }

        public void OnUpdate()
        {
            if (m_getter != null)
            {
                var _state = new HandGunInputState();
                _state = m_getter();
                InputSystem.QueueStateEvent(this, _state);
            }
            else
            {
                Debug.LogWarning("HandGun Input device: missing state getter !");
                var _state = new HandGunInputState();
                InputSystem.QueueStateEvent(this, _state);
            }
        }
    }

}