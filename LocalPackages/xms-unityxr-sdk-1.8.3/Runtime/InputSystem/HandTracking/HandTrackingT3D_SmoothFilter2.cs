using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// HandTrackingT3D_SmoothFilter2.cs
    /// </summary>
    public partial class HandTrackingT3D
    {
        /// <summary>
        /// Tries to smooth hand track info.
        /// </summary>
        /// <param name="handTrackInfo"></param>
        void SmoothHandTrackInfo2(ref HandTrackingInfo handTrackInfo, float smoothRate = 0.3f)
        {
            HandnessType handnessType = handTrackInfo.Handness;
            Vector3 kDefaultLeft = new Vector3(-0.08612f, 0.82626f, 0.55666f);//the default left and right palm rotation, in head space
            Vector3 kDefaultRight = new Vector3(0.08612f, 0.82626f, 0.55666f);
            Quaternion kDefaultRawQ = handnessType == HandnessType.Left ? Quaternion.LookRotation(kDefaultLeft) : Quaternion.LookRotation(kDefaultRight);

            var t = Camera.main.transform;
            var headSpacePalmQ = Quaternion.Inverse(t.rotation) * handTrackInfo.PalmRotation;
            var smoothQ = Quaternion.Lerp(kDefaultRawQ, headSpacePalmQ, smoothRate);

            var smoothPalmQ = Quaternion.LookRotation((handTrackInfo.PalmPosition - new Vector3(t.position.x, t.position.y - 0.2f, t.position.z)).normalized); //* smoothQ;

            //handTrackInfo.PalmRotation = t.rotation * smoothQ;
            handTrackInfo.PalmRotation = smoothPalmQ;
            handTrackInfo.PalmLocalRotation = Quaternion.Inverse(t.parent.rotation) * handTrackInfo.PalmRotation;

        }



        void SmoothHandTrackInfo3(ref HandTrackingInfo current, HandTrackingInfo prev)
        {
            var wristLocalDiff = Vector3.Distance(prev.WristRawPosition, current.WristRawPosition);
            var ring0LocalDiff = Vector3.Distance(prev.RingFinger.RawPositions[0], current.RingFinger.RawPositions[0]);
            var middle0LocalDiff = Vector3.Distance(prev.MiddleFinger.RawPositions[0], current.MiddleFinger.RawPositions[0]);
            //Debug.LogFormat("SmoothHandTrackInfo3 : {0} diff: {1}/{2}/{3}", current.Handness, wristLocalDiff.ToString("F6"), ring0LocalDiff.ToString("F6"), middle0LocalDiff.ToString("F6"));
        }
    }
}