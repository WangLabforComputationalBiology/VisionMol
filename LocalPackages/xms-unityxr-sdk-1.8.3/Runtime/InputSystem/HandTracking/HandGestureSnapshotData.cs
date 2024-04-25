using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ximmerse.XR.InputSystems
{
    [System.Serializable]
    public class FingerSnapshotDescriptor
    {
        [Range(0, 1f)]
        public float bendness = 0;
    }

    [System.Serializable]
    public class HandGestureSnapshot
    {
        public FingerSnapshotDescriptor ThumbFinger = new FingerSnapshotDescriptor();

        public FingerSnapshotDescriptor IndexFinger = new FingerSnapshotDescriptor();

        public FingerSnapshotDescriptor MiddleFinger = new FingerSnapshotDescriptor();

        public FingerSnapshotDescriptor RingFinger = new FingerSnapshotDescriptor();

        public FingerSnapshotDescriptor LittleFinger = new FingerSnapshotDescriptor();
    }

    /// <summary>
    /// Hand gesture snapshot data.
    /// </summary>
    //[CreateAssetMenu(menuName = "Ximmerse XR/Hand Tracking/Pose Snapshot Data", fileName = "Hand Pose Snapshot")]
    public class HandGestureSnapshotData : ScriptableObject
    {

        [Multiline(3)]
        public string description;

        /// <summary>
        /// Confidence valve.
        /// 1 = Strict, 0 = loosen.
        /// </summary>
        [SerializeField, Range(0, 1f)]
        float m_ConfidenceValve = 0.8f;

        /// <summary>
        /// Confidence valve to measure the gesture data
        /// </summary>
        public float ConfidenceValve
        {
            get => m_ConfidenceValve;
            set
            {
                m_ConfidenceValve = value;
            }
        }

        [SerializeField]
        HandGestureSnapshot m_HandGestureSnapshot = new HandGestureSnapshot();

        public HandGestureSnapshot snapshot
        {
            get => m_HandGestureSnapshot;
        }

        /// <summary>
        /// Confidence time.
        /// </summary>
        public float ConfidenceTime
        {
            get; set;
        }
    }
}