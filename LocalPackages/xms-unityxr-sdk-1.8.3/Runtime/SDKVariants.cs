using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ximmerse.XR.Tag;

namespace Ximmerse.XR
{
    /// <summary>
    /// SDK variants
    /// </summary>
    public static class SDKVariants
    {
        /// <summary>
        /// The tracking data directory (external)
        /// </summary>
        public const string kTrackingDataDir_External = "/sdcard/vpusdk/marker_calib";

        /// <summary>
        /// The tracking data directory (internal)
        /// This is Application.persisentDocument at android
        /// </summary>
        public static string kTrackingDataDir_Internal
        {
            get; internal set;
        }

        /// <summary>
        /// Is SDK supported at the current platform.
        /// </summary>
        public static bool IsSupported
        {
            get; internal set;
        }

        /// <summary>
        /// The currently active ground plane layout.
        /// </summary>
        public static GroundPlaneLayout groundPlaneLayout
        {
            get; internal set;
        }

        /// <summary>
        /// VPU shift positional offset
        /// </summary>
        public static readonly Vector3 kVPU_Shift = new Vector3(0.025f - 0.0238f, 0.0809f - 0.03f, 0.1412f);

        /// <summary>
        /// VPU tilt euler offset
        /// </summary>
        public static readonly Vector3 kVPU_TiltEuler = new Vector3(35, 0, 0);

        /// <summary>
        /// tracking anchor offset to eye.
        /// </summary>
        public static Matrix4x4 TrackingAnchor
        {
            get; internal set;
        }

        /// <summary>
        /// If true, tracked marker's gizmos is draw per frame.
        /// </summary>
        public static bool DrawTrackedMarkerGizmos
        {
            get; internal set;
        }

        public static bool DrawDetailTrackedInfo
        {
            get; internal set;
        }
    }
}