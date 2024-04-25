using System.Collections.Generic;
using UnityEngine;
using static Ximmerse.XR.PluginVioFusion;

namespace Ximmerse.XR.Tag
{
    /// <summary>
    /// A component that implements the large spatial positioning function, including the field properties required by the function.
    /// </summary>
    [AddComponentMenu("Ximmerse XR/Tag Ground Plane")]
    public class TagGroundPlane : GroundSpace
    {
        #region Property
        internal static List<TagGroundPlane> tagGroundPlaneList = new List<TagGroundPlane>();
        private int m_beaconCoordSystemFlag = 1;

        private float timer = 0;
        public int TrackId
        {
            get => trackID;
            set => trackID = value;
        }

        public bool DebugView
        {
            get => m_debugView;
            set => m_debugView = value;
        }

        public float Size
        {
            get => m_size;
            set => m_size = value;
        }

        public float BeaconDriftRecenterDistanceThreshold
        {
            get => m_distance;
            set => m_distance = value;
        }

        public float BeaconDriftRecenterAngleThreshold
        {
            get => m_angle;
            set => m_angle = value;
        }

        public float BeaconConfidenceThresh
        {
            get => m_Confidence;
            set => m_Confidence = value;
        }

        public float BeaconMinDistanceThresh
        {
            get => m_minDistance;
            set => m_minDistance = value;
        }

        public float BeaconMaxDistanceThresh
        {
            get => m_maxDistance;
            set => m_maxDistance = value;
        }

        public int BeaconCoordSystemFlag
        {
            get => m_beaconCoordSystemFlag;
            set => m_beaconCoordSystemFlag = value;
        }

        public int GroupId
        {
            get => groupId;
            set => groupId = value;
        }

        private XAttrBeaconInWorldInfo SetBeaconFusion()
        {
            //< 设置beacon 到 vio fusion，平放
            Vector3 pos = TagProfileLoading.Instance.XrOffsetTransform.InverseTransformPoint(transform.position);
            Quaternion rot = Quaternion.Inverse(TagProfileLoading.Instance.XrOffsetTransform.rotation) * transform.rotation;
            XAttrBeaconInWorldInfo beacon_in_world_info = new XAttrBeaconInWorldInfo(trackID);
            beacon_in_world_info.drift_recenter_angle_threshold = BeaconDriftRecenterAngleThreshold;
            beacon_in_world_info.drift_recenter_distance_threshold = BeaconDriftRecenterDistanceThreshold;
            beacon_in_world_info.group_id = groupId;
            beacon_in_world_info.coord_system_flag = BeaconCoordSystemFlag; //0=right hand coord system, 1 = left hand coord system
            beacon_in_world_info.confidence_thresh = BeaconConfidenceThresh;
            beacon_in_world_info.max_distance_thresh = BeaconMaxDistanceThresh;
            beacon_in_world_info.min_distance_thresh = BeaconMinDistanceThresh;

            beacon_in_world_info.beacon_id = trackID;
            beacon_in_world_info.rotation[0] = rot.x;
            beacon_in_world_info.rotation[1] = rot.y;
            beacon_in_world_info.rotation[2] = rot.z;
            beacon_in_world_info.rotation[3] = rot.w;
            beacon_in_world_info.position[0] = pos.x;
            beacon_in_world_info.position[1] = pos.y;
            beacon_in_world_info.position[2] = pos.z;
            return beacon_in_world_info;
        }
        public XAttrBeaconInWorldInfo beacon_info
        {
            get => SetBeaconFusion();
        }

        /// <summary>
        /// tracking state
        /// </summary>
        public bool isTracking
        {
            get => IsTracking();
        }

        /// <summary>
        /// fusion whether valid
        /// </summary>
        public bool isValid
        {
            get => isvalid;
        }

        protected override void Awake()
        {
            tagGroundPlaneList.Add(this);
        }
        protected override void OnDestroy()
        {
            tagGroundPlaneList.Remove(this);
        }
        #endregion
    }
}

