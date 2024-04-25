using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// Hand tracking smooth filter alg 03.
    /// </summary>
    public partial class HandTrackingT3D
    {

        struct SmoothedHandTrackInfo
        {
            public Vector3 PalmPosition;

            public Quaternion PalmRotation;

            public Vector3 PalmNormal;
        }


        /// <summary>
        /// Apply smoothing filter (ALG03)
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="smooth"></param>
        bool SmoothHandTrackInfo03(in HandTrackingInfo frame, in HandTrackingInfo prevFrame, out SmoothedHandTrackInfo smooth)
        {
            smooth = new SmoothedHandTrackInfo();
            if (!frame.IsValid || !prevFrame.IsValid)
            {
                return false;
            }
            //For most common case, static hand track data has below diff :
            //Positional diff : 0.00137, rotational diff angle: 1.72

           // Debug.LogFormat("Positional diff : {0}, rotational diff angle: {1}", Vector3.Distance(frame.PalmPosition, prevFrame.PalmPosition).ToString("F5"), Quaternion.Angle(frame.PalmRotation, prevFrame.PalmRotation).ToString("F2"));

            var translationCurve = XimmerseXRSettings.instance.SmoothHandTrackPositionSampleCurve;
            float tDiff = Vector3.Distance(frame.PalmPosition, prevFrame.PalmPosition);
            float sampleT = Mathf.Clamp01((tDiff - XimmerseXRSettings.instance.smoothHandPositionDiffRange.x) / (XimmerseXRSettings.instance.smoothHandPositionDiffRange.y - XimmerseXRSettings.instance.smoothHandPositionDiffRange.x));
            float tMultiplier = translationCurve.Evaluate(sampleT);
            smooth.PalmPosition = Vector3.MoveTowards(prevFrame.PalmPosition, frame.PalmPosition, tMultiplier * tDiff);

            var qSmoothCurve = XimmerseXRSettings.instance.SmoothRotationCurve;
            float qDiff = Quaternion.Angle(frame.PalmRotation, prevFrame.PalmRotation);
            float sampleQ = Mathf.Clamp01((qDiff - XimmerseXRSettings.instance.SmoothHandTrackRotationAngleDiffRange.x) / (XimmerseXRSettings.instance.SmoothHandTrackRotationAngleDiffRange.y - XimmerseXRSettings.instance.SmoothHandTrackRotationAngleDiffRange.x));
            float qMultiplier = qSmoothCurve.Evaluate(sampleQ);

            //  float maxDegreeDelta = qMultiplier * qDiff;                                                                                                                                                              
            float maxDegreeDelta = XimmerseXRSettings.instance.SmoothingAngularSpeed * qMultiplier * Time.deltaTime;
            Debug.LogFormat("qMultiplier = {0}, qDiff = {1}, sampleQ = {2}, maxDegreeDelta = {3}", qMultiplier, qDiff, sampleQ, maxDegreeDelta);
            smooth.PalmRotation = Quaternion.RotateTowards(prevFrame.PalmRotation, frame.PalmRotation, maxDegreeDelta);
            smooth.PalmNormal = smooth.PalmRotation * Quaternion.Euler(-90, 0, 0) * Vector3.forward;
            return true;
        }
    }
}