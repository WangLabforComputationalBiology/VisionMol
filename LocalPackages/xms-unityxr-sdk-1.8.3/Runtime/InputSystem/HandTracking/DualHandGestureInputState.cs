using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;
namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// Dual hand gesture input state
    /// 双手手势输入状态描述数据
    /// </summary>
    public struct DualHandGestureInputState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('X', 'D', 'H', 'I');

        /// <summary>
        /// If zoom or twist gesture is performed
        /// </summary>
        [InputControl(name = "IsPerformed", layout = "Button")]
        public int IsPerformed;

        /// <summary>
        /// 0 when isPerformed == false,
        /// 1 when performing zoom scale,
        /// 2 when performing rotateXZ scale.
        /// </summary>
        [InputControl(name = "State", layout = "Integer")]
        public uint State;

        /// <summary>
        /// Rotation gesture at XZ plane, in angular speed.
        /// </summary>
        [InputControl(name = "RotateAngularXZ", layout = "Axis")]
        public float RotateAngularXZ;

        /// <summary>
        /// Zoom gesture scale.
        /// </summary>
        [InputControl(name = "ZoomScale", layout = "Axis")]
        public float ZoomScale;
    }

}