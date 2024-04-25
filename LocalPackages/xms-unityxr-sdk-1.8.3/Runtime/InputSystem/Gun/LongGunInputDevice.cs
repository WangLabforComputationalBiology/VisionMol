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
    /// Long gun input state descriptor.
    /// </summary>
    public struct LongGunInputState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('X', 'M', 'L', 'N');

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
        /// 保险按钮状态
        /// </summary>
        [InputControl(name = "SecureKeyState", layout = "Integer")]
        public uint SecureKeyState;

        /// <summary>
        /// 0 = loose, 1 = release
        /// </summary>
        [InputControl(name = "ChamberSlide", layout = "Axis")]
        public float ChamberSlide;

        #endregion

    }

    [InputControlLayout(commonUsages = new[] { "LongGun" }, isGenericTypeOfDevice = false, displayName = "LongGun", stateType = typeof(LongGunInputState))]
    public class LongGunInputDevice : InputDevice, IInputUpdateCallbackReceiver
    {

        static LongGunInputDevice longGunInputDevice;

        /// <summary>
        /// Adds or gets long gun input devices
        /// </summary>
        /// <returns></returns>
#if UNITY_EDITOR
        //[UnityEditor.MenuItem("Ximmerse XR SDK/Create LongGun Device",false,11)]
#endif
        public static LongGunInputDevice GetLongGunInputDevice()
        {
            if (longGunInputDevice != null)
            {
                return longGunInputDevice;
            }
            else
            {
                InputSystem.RegisterLayout<LongGunInputDevice>(matches: new InputDeviceMatcher()
   .WithInterface("LongGunInputState"));
                LongGunInputDevice _gun = (LongGunInputDevice)InputSystem.AddDevice("LongGunInputDevice", "LongGun");
                InputSystem.SetDeviceUsage(_gun, "LongGun");
                InputSystem.EnableDevice(_gun);
                LongGunInputDevice.longGunInputDevice = _gun;
                return _gun;
            }
        }

        System.Func<LongGunInputState> m_getter = null;

        public void RegisterStateGetter(System.Func<LongGunInputState> getter)
        {
            m_getter = getter;
        }


        /// <summary>
        /// Tracking state, the value mapped to UnityEngine.XR.InputTrackingState:
        /// None = 0, Position = 1, Rotation = 2,Velocity = 4,AngularVelocity = 8,
        /// Acceleration = 16,AngularAcceleration = 32, All = 63
        /// </summary>
        [InputControl(name = "LongGunInputState/TrackingState")]
        public IntegerControl TrackingState
        {
            get; internal set;
        }


        [InputControl(name = "LongGunInputState/Position")]
        public Vector3Control Position
        {
            get; internal set;
        }

        [InputControl(name = "LongGunInputState/Rotation")]
        public QuaternionControl Rotation
        {
            get; internal set;
        }


        /// <summary>
        /// Power key
        /// </summary>
        [InputControl(name = "LongGunInputState/Power")]
        public ButtonControl Power
        {
            get; internal set;
        }

        /// <summary>
        /// Trigger key
        /// </summary>
        [InputControl(name = "LongGunInputState/Trigger")]
        public ButtonControl Trigger
        {
            get; internal set;
        }

        /// <summary>
        /// Trigger value
        /// </summary>
        [InputControl(name = "LongGunInputState/TriggerValue")]
        public AxisControl TriggerValue
        {
            get; internal set;
        }


        /// <summary>
        /// MagLoad key
        /// </summary>
        [InputControl(name = "LongGunInputState/MagLoad")]
        public ButtonControl MagLoad
        {
            get; internal set;
        }

        /// <summary>
        /// MagRelease key
        /// </summary>
        [InputControl(name = "LongGunInputState/SecureKeyState")]
        public IntegerControl SecureKeyState
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

            Power = GetChildControl<ButtonControl>("Power");
            Trigger = GetChildControl<ButtonControl>("Trigger");
            TriggerValue = GetChildControl<AxisControl>("TriggerValue");
            MagLoad = GetChildControl<ButtonControl>("MagLoad");

            SecureKeyState = GetChildControl<IntegerControl>("SecureKeyState");

            ChamberSlide = GetChildControl<AxisControl>("ChamberSlide");
        }

        public void OnUpdate()
        {
            if (m_getter != null)
            {
                var _state = new LongGunInputState();
                _state = m_getter();
                InputSystem.QueueStateEvent(this, _state);
            }
            else
            {
                Debug.LogWarning("LongGun Input device: missing state getter !");
                var _state = new LongGunInputState();
                InputSystem.QueueStateEvent(this, _state);
            }
        }
    }

}