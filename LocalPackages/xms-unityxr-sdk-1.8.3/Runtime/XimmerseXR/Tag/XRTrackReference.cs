using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace Ximmerse.XR.Tag
{
    /// <summary>
    /// Track reference. Anchor set transform to tracked marker.
    /// </summary>
    //[AddComponentMenu("Ximmerse XR/Track reference")]
    public class XRTrackReference : MonoBehaviour
    {
        public int referenceID = 0;

        private void OnEnable()
        {
            XRManager.OnTrackUpdate += XRManager_OnTrackUpdate;
        }

        private void XRManager_OnTrackUpdate(NativeArray<TrackingResult> tracked, NativeArray<TrackingResult> unTracked, NativeArray<TrackingResult> added, NativeArray<TrackingResult> lost)
        {
            for (int i = 0; i < tracked.Length; i++)
            {
                TrackingResult t = tracked[i];
                if (t.id == this.referenceID)
                {
                    transform.SetPositionAndRotation(t.worldPose.position, t.worldPose.rotation);
                    break;
                }
            }
        }

        private void OnDisable()
        {
            XRManager.OnTrackUpdate -= XRManager_OnTrackUpdate;
        }
    }
}