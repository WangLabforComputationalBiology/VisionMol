using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.XR
{
    /// <summary>
    /// Track result descriptor.
    /// </summary>
    public struct TrackingResult
    {
        /// <summary>
        /// id 
        /// </summary>
        public int id;

        /// <summary>
        /// size of tracked marker.
        /// </summary>
        public float size;

        /// <summary>
        /// true for controller, cube, false for single card.
        /// </summary>
        public bool isGroup;

        /// <summary>
        /// tracked or not tracked.
        /// </summary>
        public bool tracked;

        /// <summary>
        /// Pose at world space
        /// </summary>
        public Pose worldPose;

        /// <summary>
        /// Tracked distance, valid when tracked is true.
        /// </summary>
        public float trackedDistance;

        /// <summary>
        /// Tracked confidence, range [0..1]
        /// </summary>
        public float trackedConfidence;
    }
}