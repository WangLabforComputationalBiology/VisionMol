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
    /// Anchor hand input state data.
    /// 锚定点手势输入状态描述数据.
    /// </summary>
    public struct HandAnchorInputState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('X', 'H', 'A', 'R');

        #region Properties represents hand track state

        [InputControl(name = "isTracked", layout = "Button")]
        public int isTracked;

        /// <summary>
        /// 0 = none, 3 (1+2) = position + rotation
        /// </summary>
        [InputControl(name = "trackingState", layout = "Integer")]
        public uint trackingState;

        /// <summary>
        /// 锚定点位置
        /// </summary>
        [InputControl(name = "anchorPosition", layout = "Vector3")]
        public Vector3 anchorPosition;

        /// <summary>
        /// Anchor ray origin point.
        /// </summary>
        [InputControl(name = "anchorRayOrigin", layout = "Vector3")]
        public Vector3 anchorRayOrigin;

        /// <summary>
        /// Anchor ray rotation
        /// </summary>
        [InputControl(name = "anchorRayRotation", layout = "Quaternion")]
        public Quaternion anchorRayRotation;

        /// <summary>
        /// hand ray origin point.
        /// </summary>
        [InputControl(name = "handRayOrigin", layout = "Vector3")]
        public Vector3 handRayOrigin;

        /// <summary>
        /// hand ray rotation
        /// </summary>
        [InputControl(name = "handRayRotation", layout = "Quaternion")]
        public Quaternion handRayRotation;


        /// <summary>
        /// 指间捏合点位置
        /// </summary>
        [InputControl(name = "fingerGraspPoint", layout = "Vector3")]
        public Vector3 fingerGraspPoint;

        /// <summary>
        /// 手势 : 捏指动作触发器
        /// </summary>
        [InputControl(name = "isGrip", layout = "Button")]
        public bool isGrip;

        /// <summary>
        /// 手势 : 捏指幅度
        /// </summary>
        [InputControl(name = "gripValue", layout = "Axis")]
        public float gripValue;

        #endregion

    }

}