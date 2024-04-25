using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using System.Runtime.InteropServices;
using TouchlessA3D;
using System;
using System.Xml;
using System.Text;
using Unity.Collections;

namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// Hand tracking module : hand tracked pose smoothing filter.
    /// </summary>
    public partial class HandTrackingT3D
    {

        /// <summary>
        /// the pose record for hand track pose smoothing.
        /// </summary>
        struct PoseRecord
        {
            /// <summary>
            /// wrist point
            /// </summary>
            public Vector3 wristPoint;

            /// <summary>
            /// Palm rotation
            /// </summary>
            public Quaternion palmQ;

            /// <summary>
            /// This is the unity real-scene-time.
            /// </summary>
            public float time;
        }

        /// <summary>
        /// wrist speed to filter hand track pose, frame independent 
        /// </summary>
        public float kFilterDeltaSpeedFrameIndenpendentMin = 0.0015f, kFilterDeltaSpeedFrameIndenpendentMax = 0.0075f;

        /// <summary>
        /// wrist angular speed to filter hand track pose, frame independent 
        /// </summary>
        public float kFilterDeltaAngleFrameIndenpendentMin = 30, kFilterDeltaAngleFrameIndenpendentMax = 60;

        /// <summary>
        /// weight of smoothing alg by translation vs rotation.
        /// </summary>
        public float kFilterAlgTranslationWeight = 0.6f, kFilterAlgRotationWeight = 0.4f;

        /// <summary>
        /// 上一帧记录.
        /// </summary>
        PoseRecord prevLeft, prevRight;

        /// <summary>
        /// 前向追溯帧计数。默认是前向1帧.
        /// </summary>
        int reviewFrameCount = 1;

        /// <summary>
        /// 可用的平滑时间阀值。
        /// </summary>
        public float kSmoothTimeValve = 0.025f;

        Quaternion GetRawPalmRotation(HandTrackingInfo handTrackInfo)
        {
            Vector3 wristPos = handTrackInfo.WristRawPosition;
            Vector3 wristToRing = handTrackInfo.RingFinger.RawPositions[0] - wristPos;
            Vector3 wristToMiddle = handTrackInfo.MiddleFinger.RawPositions[0] - wristPos;


            Vector3 crs = Vector3.Cross(wristToRing, wristToMiddle);
            //Make palm normal always facing UP upon the palm surface:
            if (handTrackInfo.Handness == HandnessType.Left)//left
            {
                crs = -crs;
            }
            Quaternion PalmRawRotation = Quaternion.LookRotation(wristToRing, crs);
            return PalmRawRotation;
        }



        /// <summary>
        /// Tries to smooth hand track info.
        /// </summary>
        /// <param name="handTrackInfo"></param>
        /// <param name="previousFrameHandTrackInfo"></param>
        //void SmoothHandTrackInfo(ref HandTrackingInfo handTrackInfo, HandTrackingInfo previousFrameHandTrackInfo)
        //{
        //    var prev = handTrackInfo.Handness == HandnessType.Left ? prevLeft : prevRight;
        //    float timeDiff = Time.timeSinceLevelLoad - prev.time;

        //    //如果时间阀值大于可接受值,则上一帧不可采信,只是记录值然后退出:
        //    if (timeDiff >= kSmoothTimeValve)
        //    {
        //        //记录上一帧数据:
        //        if (handTrackInfo.Handness == HandnessType.Left)
        //        {
        //            prevLeft.wristPoint = handTrackInfo.WristRawPosition;
        //            prevLeft.time = Time.timeSinceLevelLoad;
        //            prevLeft.palmQ = GetRawPalmRotation(handTrackInfo);
        //        }
        //        else
        //        {
        //            prevRight.wristPoint = handTrackInfo.WristRawPosition;
        //            prevRight.time = Time.timeSinceLevelLoad;
        //            prevRight.palmQ = GetRawPalmRotation(handTrackInfo);
        //        }
        //        Debug.LogFormat("Too big time to smooth : {0}/{1}", prev.time, timeDiff);
        //        return;
        //    }

        //    var palmQ = GetRawPalmRotation(handTrackInfo);

        //    float wristPointDiff = Vector3.Distance(handTrackInfo.WristRawPosition, prev.wristPoint) / (Time.timeSinceLevelLoad - prev.time);
        //    float palmQDiff = Quaternion.Angle(palmQ, prev.palmQ) / (Time.timeSinceLevelLoad - prev.time);
        //    float smoothingCofficientT = 1 - Mathf.Clamp01(Mathf.Abs(wristPointDiff - kFilterDeltaSpeedFrameIndenpendentMin) / (kFilterDeltaSpeedFrameIndenpendentMax - kFilterDeltaSpeedFrameIndenpendentMin));
        //    float smoothingCofficientQ = 1 - Mathf.Clamp01(Mathf.Abs(palmQDiff - kFilterDeltaAngleFrameIndenpendentMin) / (kFilterDeltaAngleFrameIndenpendentMax - kFilterDeltaAngleFrameIndenpendentMin));

        //    //平滑系数: 0 = 不平滑, 1 = 完全平滑
        //    float smoothingCofficient = kFilterAlgTranslationWeight * smoothingCofficientT + kFilterAlgRotationWeight * smoothingCofficientQ;
        //    //Apply smoothing alg:
        //    handTrackInfo.PalmLocalPosition = Vector3.Lerp(handTrackInfo.PalmLocalPosition, previousFrameHandTrackInfo.PalmLocalPosition, smoothingCofficient);
        //    handTrackInfo.PalmLocalRotation = Quaternion.Lerp(handTrackInfo.PalmLocalRotation, previousFrameHandTrackInfo.PalmLocalRotation, smoothingCofficient);
        //    handTrackInfo.PalmPosition = Vector3.Lerp(handTrackInfo.PalmPosition, previousFrameHandTrackInfo.PalmPosition, smoothingCofficient);
        //    handTrackInfo.PalmRotation = Quaternion.Lerp(handTrackInfo.PalmRotation, previousFrameHandTrackInfo.PalmRotation, smoothingCofficient);

        //    handTrackInfo.PalmNormal = Vector3.Lerp(handTrackInfo.PalmLocalNormal, previousFrameHandTrackInfo.PalmLocalNormal, smoothingCofficient).normalized;
        //    handTrackInfo.PalmLocalNormal = Vector3.Lerp(handTrackInfo.PalmLocalNormal, previousFrameHandTrackInfo.PalmLocalNormal, smoothingCofficient).normalized;


        //    //记录上一帧数据:
        //    if (handTrackInfo.Handness == HandnessType.Left)
        //    {
        //        prevLeft.wristPoint = handTrackInfo.WristRawPosition;
        //        prevLeft.time = Time.timeSinceLevelLoad;
        //        prevLeft.palmQ = GetRawPalmRotation(handTrackInfo);
        //    }
        //    else
        //    {
        //        prevRight.wristPoint = handTrackInfo.WristRawPosition;
        //        prevRight.time = Time.timeSinceLevelLoad;
        //        prevRight.palmQ = GetRawPalmRotation(handTrackInfo);
        //    }

        //    Debug.LogFormat("Apply smoothing cofficient: {0}", smoothingCofficient);
        //}



    }
}