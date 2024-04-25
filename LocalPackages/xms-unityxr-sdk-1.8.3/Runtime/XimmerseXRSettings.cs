using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Ximmerse.XR.Tag;
using Ximmerse.XR.InputSystems;

namespace Ximmerse.XR
{

    public enum HandTrackDataSource
    {

        /// <summary>
        /// Route data from system service
        /// </summary>
        SystemService,

        /// <summary>
        /// Route data fron local app libraries
        /// </summary>
        AppLibrary,

    }

    /// <summary>
    /// XR Settings for Ximmerse XR Plugin.
    /// Required by XR Management package.
    /// </summary>
    [Serializable]
    [UnityEngine.XR.Management.XRConfigurationData("Ximmerse", "Ximmerse.XR.XimmerseXRSettings")]
    public class XimmerseXRSettings : ScriptableObject
    {

        /// <summary>
        /// 是否开启 single pass rendering 模式
        /// </summary>
        [Tooltip("是否开启 single pass rendering 模式")]
        public bool SinglePassRendering = false;

        //public Font debugFont;
        //[Tooltip("Display eye reticle texture or not.")]
        //public bool displayReticle = true;

        //[Tooltip("Texture of eye reticle.")]
        //public Texture2D reticleTexture;

        //[Tooltip("The default tracking profile to be loaded, when SDK starts.")]
        //public ObjectTrackingProfile defaultTrackingProfile;

        //[Tooltip("The default ground plane layout to be loaded, when SDK starts. \r\n If none, there will be no ground plane by default.")]
        //public GroundPlaneLayoutConfiguration defaultGroundPlaneLayoutConfig;

        //[Tooltip("Draw tracked marker gizmos if true.")]
        //public bool DrawTrackedMarkerGizmos = false;

        //[Tooltip("Print detail tracked info.")]
        //public bool DrawDetailTrackedInfo = false;


        //[Tooltip("if true, hand tracking is activated when application starts.")]
        //public bool HandTracking = false;

        //[Tooltip("Hand track data source from system service (recommanded) or local app library")]
        //public HandTrackDataSource handTrackDataSource = HandTrackDataSource.SystemService;

        [Tooltip("if true, hand tracking output data is smoothed per frame.")]
        public bool SmoothHandTrackingData = true;

        [Tooltip("Hand track rotation smooth angle diff range min-max")]
        public Vector2 SmoothHandTrackRotationAngleDiffRange = new Vector2(1, 1.5f);

        [Tooltip("When angle falls into the smooth range, this normalized curve controls the smooth rate ")]
        public AnimationCurve SmoothRotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Tooltip("Angular speed when smoothing hand rotation")]
        public float SmoothingAngularSpeed = 120;

        public Vector2 smoothHandPositionDiffRange = new Vector2(0.001f, 0.01f);

        public AnimationCurve SmoothHandTrackPositionSampleCurve = AnimationCurve.Linear(0, 0, 1, 1);




        [Header("--- Hand Anchor Input Config ---")]
        public AnchorHandInputDeviceConfig leftHandAnchorInputDeviceConfig = new AnchorHandInputDeviceConfig();

        public AnchorHandInputDeviceConfig rightHandAnchorInputDeviceConfig = new AnchorHandInputDeviceConfig();

        [Header("--- Zoom Config ---")]
        public DualHandInputDeviceConfig dualHandInputDeviceConfig = new DualHandInputDeviceConfig();

        public static XimmerseXRSettings instance
        {
            get; internal set;
        }

        private void Awake()
        {
            instance = this;
        }
    }
}
