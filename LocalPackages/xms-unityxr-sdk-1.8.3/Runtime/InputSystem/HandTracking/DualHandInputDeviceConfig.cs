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
    /// Dual hand input device config
    /// </summary>
    [System.Serializable]
    public class DualHandInputDeviceConfig
    {
        /// <summary>
        ///  抓取点移动接受范围.
        /// </summary>
        [Tooltip("抓取点移动接受范围")]
        public Vector2 pointDeltaDistanceRange = new Vector2(0.001f, 0.01f);

        /// <summary>
        /// 速度对缩放的比率
        /// </summary>
        [Tooltip("速度对缩放的比率")]
        public float velocityToScaleRate = 1;

        /// <summary>
        /// 缩放范围
        /// </summary>
        [Tooltip("每帧允许的缩放范围")]
        public Vector2 ScaleRange = new Vector2(0.9f, 1.1f);

    }
}