using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using Unity.Collections;
using Unity.Jobs;
using System.IO;
using System;
using Ximmerse.XR.Utils;
using Ximmerse.XR.Internal;
using Ximmerse.XR.Collections;

namespace Ximmerse.XR
{

    /// <summary>
    ///  Track event.
    /// </summary>
    /// <param name="tracked">Array of all tracked markers.</param>
    /// <param name="unTracked">Array of all untracked markers.</param>
    /// <param name="added">Array of newly tracked markers.</param>
    /// <param name="lost">Array of lost tracked markers.</param>
    public delegate void TrackEvent(NativeArray<TrackingResult> tracked, NativeArray<TrackingResult> unTracked, NativeArray<TrackingResult> added, NativeArray<TrackingResult> lost);

    /// <summary>
    /// Ximmerse XR manager.
    /// </summary>
    [Preserve]
    [DefaultExecutionOrder(-20000)]
    [AddComponentMenu(".")]//dont add this component manually, it will be added by XR loader.
    public sealed class XRManager : MonoBehaviour
    {

        static XRManager instance;

        public static XRManager Instance
        {
            get => instance;
        }

        /// <summary>
        /// Event is fired per frame, describes the markers' tracking status 
        /// </summary>
        public static event TrackEvent OnTrackUpdate;

        /// <summary>
        /// Event is fired per frame, after processing the tracking data.
        /// </summary>
        public static event Action OnPostTrackUpdate;

        xNativeList<TrackingResult> trackingResultsPerFrame = xNativeList<TrackingResult>.Create(64);

        Camera m_mainCam;

        public Camera mainCam
        {
            get
            {
                if (!m_mainCam)
                {
                    m_mainCam = Camera.main;
                }
                return m_mainCam;
            }
        }

        //Matrix4x4 mainCameraAwakePose;

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(instance, this))
            {
                instance = null;
            }
        }

        private IEnumerator Start()
        {
            StartCoroutine(CleanJob());
            while (!mainCam)
            {
                yield return null;
            }
            //mainCameraAwakePose = Matrix4x4.TRS(mainCam.transform.position, mainCam.transform.rotation, Vector3.one);
        }

        private void Update()
        {
            if (!SDKVariants.IsSupported || !Application.isPlaying || !mainCam)
            {
                return;
            }
            SvrPluginAndroid.Unity_setFrame(Time.frameCount);
            //Matrix4x4 trans = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            //if (mainCam.transform.parent)
            //{
            //    var m1 = mainCam.transform.root.localToWorldMatrix;
            //    var m2 = mainCam.transform.GetChild(0).localToWorldMatrix;
            //    trans = m1 * m2;
            //}


            //XDevicePlugin.UpdateTracking(mainCam.transform.parent ? mainCam.transform.parent.localToWorldMatrix : mainCameraAwakePose, out TrackingSummary summary);
            //trackingResultsPerFrame.Clear();
            //trackingResultsPerFrame.AddRange(summary.tracked);
            //try
            //{
            //    //Debug.LogFormat("Summary of tracking: {0}, unTracked = {1}, newTracked = {2}, lost = {3}", summary.tracked.Length, summary.unTracked.Length, summary.newTracked.Length, summary.lostTracked.Length);
            //    OnTrackUpdate?.Invoke(summary.tracked, summary.unTracked, summary.newTracked, summary.lostTracked);
            //}
            //catch (Exception e)
            //{
            //    Debug.LogException(e);
            //}
            ////Draw gizmos of tracked marker:
            //if (SDKVariants.DrawTrackedMarkerGizmos)
            //{
            //    foreach (var tracked in summary.tracked)
            //    {
            //        float size = tracked.size;
            //        //if(markerCfgInfo.markerType == ConfigMarkerType.MarkerGroup_Submarker || markerCfgInfo.markerType == ConfigMarkerType.SingleMarker)
            //        {
            //            RxDraw.DrawWirePlane(tracked.worldPose.position, tracked.worldPose.rotation, size, size, new Color(0.2f, 0.88f, 0.2f, 1));

            //            var textRotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
            //            textRotation = textRotation.PitchNYaw();

            //            string debugTxt = tracked.id.ToString();
            //            if (SDKVariants.DrawDetailTrackedInfo)
            //            {
            //                Vector3 p = tracked.worldPose.position;
            //                Vector3 e = tracked.worldPose.rotation.eulerAngles.PrettyAngle();
            //                float confidence = tracked.trackedConfidence;
            //                debugTxt =
            //                    $"{tracked.id.ToString()}\r\n P={p.ToString("F2")} \r\n Q={e.ToString("F2")} \r\n Distance={tracked.trackedDistance.ToString("F2")}m \r\n Confidence = {confidence.ToString("F3")}";
            //            }
            //            RxDraw.Text3D(tracked.worldPose.position, tracked.worldPose.rotation, 0.012f, debugTxt, new Color(0.96f, 0.9f, 0.93f, 1));
            //        }

            //        RxDraw.DrawTranslateGizmos(tracked.worldPose.position, tracked.worldPose.rotation, size * 0.85f);
            //    }
            //}
            //summary.Dispose();

            OnPostTrackUpdate?.Invoke();
        }

        IEnumerator CleanJob()
        {
            var eof = new WaitForEndOfFrame();
            while (true)
            {
                yield return eof;
                trackingResultsPerFrame.Clear();
            }
        }

        public static TrackingResult GetTrackingResult(int id)
        {
            if (!instance)
            {
                return default(TrackingResult);
            }
            var l = instance.trackingResultsPerFrame;
            foreach (var t in l)
            {
                if (t.id == id)
                {
                    return t;
                }
            }
            return default(TrackingResult);
        }
    }
}