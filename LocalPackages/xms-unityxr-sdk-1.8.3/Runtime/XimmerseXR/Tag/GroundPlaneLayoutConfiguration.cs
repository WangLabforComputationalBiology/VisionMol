using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ximmerse.XR.Tag
{

    public enum GroundPlaneMode : byte
    {
        ground = 0, wall,
    }

    [System.Serializable]

    public class GroundPlaneLayout
    {
        /// <summary>
        /// Ground plane items.
        /// </summary>
        public GroundPlaneGroup[] groundPlaneGroups;

        public bool IsValid()
        {
            return groundPlaneGroups != null && groundPlaneGroups.Length > 0;
        }
    }

    /// <summary>
    /// Ground Plane groups, contains many ground plane items.
    /// </summary>
    [System.Serializable]
    public class GroundPlaneGroup
    {
        public int groupIndex;

        /// <summary>
        /// 组下的每个 ground plane item
        /// </summary>
        public GroundPlaneLayoutItem[] groundPlanes;
    }

    [System.Serializable]
    public class SingleGroundPlaneTechnicalParameter
    {
        /// <summary>
        /// drift recenter angle threshold
        /// </summary>
        public float drift_recenter_angle_threshold = 1.0f;

        /// <summary>
        /// drift recenter distance threshold
        /// </summary>
        public float drift_recenter_distance_threshold = 1.0f;

        /// <summary>
        /// confidence threshold
        /// </summary>
        public float confidence_thresh = 0.9f;

        /// <summary>
        /// min distance threshold
        /// </summary>
        public float max_distance_thresh = 1.8f;

        /// <summary>
        /// max distance threshold
        /// </summary>
        public float min_distance_thresh = 0.1f;
    }

    /// <summary>
    /// Single ground plane item data.
    /// </summary>
    [System.Serializable]
    public class GroundPlaneLayoutItem
    {
        public int track_id;

        public GroundPlaneMode mode;

        public Vector3 position;

        public Vector3 euler;

        public SingleGroundPlaneTechnicalParameter technicalParameter = new SingleGroundPlaneTechnicalParameter();
    }


    /// <summary>
    /// 用于 Fusoin模式下，Ground Plane 的矩阵配置
    /// </summary>
    //[CreateAssetMenu(menuName = "Ximmerse/Ground Plane Layout Config", fileName = "Ground Plane Layout Config")]
    public sealed class GroundPlaneLayoutConfiguration : ScriptableObject
    {
        /// <summary>
        /// The ground plane matrix.
        /// </summary>
        public GroundPlaneLayout layout = new GroundPlaneLayout()
        {
            groundPlaneGroups = new GroundPlaneGroup[]
            {
                new GroundPlaneGroup()
                {
                     groupIndex = 0,
                     groundPlanes = new GroundPlaneLayoutItem[]
                     {

                     }
                }
            }
        };


        [ContextMenu ("Test : load this config")]
        void TestLoadMe()
        {
            SDKVariants.groundPlaneLayout = layout;
            XimmerseXR.LoadGroundPlaneLayout(this.layout);
        }
    }
}