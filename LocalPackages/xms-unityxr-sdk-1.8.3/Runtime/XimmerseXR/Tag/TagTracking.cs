using UnityEngine;

namespace Ximmerse.XR.Tag
{
    /// <summary>
    /// The state after losing the Tag
    /// </summary>
    public enum LostState
    {
        Stay,
        FollowHead,
    }

    /// <summary>
    /// Tag tracking function
    /// </summary>
    [AddComponentMenu("Ximmerse XR/Tag Tracking")]
    public class TagTracking : XRTracking
    {
        #region Property
        /// <summary>
        /// Id
        /// </summary>
        public int TrackId
        {
            get => trackID;
            set => trackID = value;
        }
        /// <summary>
        /// Whether to enable debug mode
        /// </summary>
        public bool DebugView
        {
            get => m_debugView;
            set => m_debugView = value;
        }
        /// <summary>
        /// Axis dimensions in debug mode
        /// </summary>
        public float Size
        {
            get => m_size;
            set => m_size = value;
        }

        /// <summary>
        /// tracking state
        /// </summary>
        public bool isTracking
        {
            get => IsTracking();
        }
        /// <summary>
        /// Stay or FollowHead when lost tracking.
        /// </summary>
        public LostState TrackingIsLost
        {
            get => trackingIsLost;
            set => trackingIsLost = value;
        }
        #endregion
    }

}
