using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// Pointer position damp movement config class.
    /// </summary>
    [System.Serializable]
    public class PointerDampMovementConfig
    {
        /// <summary>
        /// anchor point 距离跳变阈值， 当target distance 大于此阈值的时候， anchor point将会跳变。
        /// </summary>
        public float JumpDistanceValve = 0.25f;

        /// <summary>
        /// 当 anchor point 小于  AnchorPointJumpDistanceValve 的时候， 此范围用于计算 normalized damp seed 
        /// </summary>
        public Vector2 DampSpeedInputRange = new Vector2(0, 0.3f);

        /// <summary>
        /// 用于控制 damp movement speed 的速率均一化控制曲线
        /// </summary>
        public AnimationCurve DampSpeedControlCurve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// 逼近速度 Min, Max
        /// </summary>
        public Vector2 ApproachSpeedRange = new Vector2(0.01f, 10);


        public void UpdatePoint(ref Vector3 point, Vector3 targetPoint)
        {
            float distanceToTargetAnchorPoint = Vector3.Distance(targetPoint, point);
            //Jump to anchor point
            if (distanceToTargetAnchorPoint >= this.JumpDistanceValve)
            {
                point = targetPoint;
            }
            else
            {
                float normalized = Mathf.Clamp01((distanceToTargetAnchorPoint - DampSpeedInputRange.x) / (DampSpeedInputRange.y - DampSpeedInputRange.x));
                float speedNormalized = this.DampSpeedControlCurve.Evaluate(normalized);
                float speed = (ApproachSpeedRange.y - ApproachSpeedRange.x) * speedNormalized + ApproachSpeedRange.x;
                point = Vector3.MoveTowards(point, targetPoint, speed * Time.deltaTime);
                // Debug.LogFormat("AnchorHandInputDeviceConfig.UpdateAnchor - normalized = {0}, speedN = {1}, speed = {2}", normalized, speedNormalized, speed);
            }
        }
    }


    /// <summary>
    /// 手势锚定点输入配置.
    /// </summary>
    [System.Serializable]
    public class AnchorHandInputDeviceConfig
    {

        public enum AnchorPointMode
        {
            ThreeDimensionSpace,

            ProjectOnFixedPlane,

            AlignToGraspPoint,
        }

        public AnchorPointMode anchorPointMode = AnchorPointMode.ThreeDimensionSpace;

        [Range(0.1f, 3)]
        public float projectionPlaneDistance = 1;

        /// <summary>
        /// anchor offset to local hand track matrix
        /// </summary>
        public Vector3 localAnchorOffset = new Vector3(0, -0.05f, 0.3f);

        /// <summary>
        /// euler offset to local hand track matrix
        /// </summary>
        public Vector3 localEulerOffset = new Vector3(0, 0, 0f);

        /// <summary>
        /// The local hand ray origin offset to launch the ray 
        /// </summary>
        public Vector3 localHandRayOriginOffset = new Vector3(0, 0, 0);

        /// <summary>
        /// If  true, hand ray is disabled when the palm normal is facing towards user.
        /// </summary>
        public bool disableHandRayWhenPalmFaceUser = true;

        /// <summary>
        /// 食指和拇指的捏合距离判定宽度阈值。
        /// </summary>
        [Tooltip("食指和拇指的捏合距离判定宽度阈值")]
        public float graspDistanceSpan = 0.02f;

        /// <summary>
        /// Anchor point damp movement config
        /// </summary>
        public PointerDampMovementConfig anchorPointerDampMovementConfig = new PointerDampMovementConfig();

        /// <summary>
        /// Grasp point damp movement config
        /// </summary>
        public PointerDampMovementConfig graspPointDampMovementConfig = new PointerDampMovementConfig();


    }
}