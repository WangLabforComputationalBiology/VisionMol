//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Ximmerse.XR
{
    /// <summary>
    /// Interface for PluginVIO fusion API.
    /// </summary>
	public class PluginVioFusion
    {

        private const string kPlugin_FusionAPI = "fusion_api";


        public enum ParamType {

            TYPE_POSE_VELOCITY_FLOAT = 0,
            TYPE_POSE_QUALITY_FLOAT = 1,
            TYPE_SENSOR_QUALITY_FLOAT = 2,
            TYPE_CAMERA_QUALITY_FLOAT = 3,
            TYPE_TRACKING_WARNING_INT = 4,
            TYPE_ANGLE_WITH_GROUND_FLOAT = 5

        };



        public enum TrackingWarningType
        {
            VIO_LOW_FEATURE_COUNT_ERROR = 0x0001,
            VIO_LOW_LIGHT_ERROR = 0x0002,
            VIO_BRIGHT_LIGHT_ERROR = 0x0004,
            VIO_STEREO_CAMERA_CALIBRATION_ERROR = 0x0008,
            VIO_NOT_READY_ERROR = 0x1000,
        };
        /////////////////////////////////////////////
        /// @struct XAttrVIOWorldToBeaconInfo
        /// @brief 6DOF Information of VIO world-origin in beacon coordinate.
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrVIOWorldInBeaconInfo
        {

            public int beacon_id;
            public UInt64 timestamp;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position ;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation;

            public float tracking_confidence;
            public float min_distance;
            public float correct_weight;


            public static XAttrVIOWorldInBeaconInfo Obtain()
            {
                return new XAttrVIOWorldInBeaconInfo(-1);
            }

            public XAttrVIOWorldInBeaconInfo(int beacon_id)
            {
                this.beacon_id = beacon_id;
                this.timestamp = 0;
                this.position = new float[3] { 0, 0, 0 } ;
                this.rotation = new float[4] { 0, 0, 0, 0 };
                this.tracking_confidence = 0;
                this.min_distance = 0;
                this.correct_weight = 0;
            }
        };


        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrVIOInfo
        {
            public UInt64 timestamp;                    // timestamp of current frame

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation;

            public float pose_quality;
            public float sensor_quality;
            public float camera_quality;

            public static XAttrVIOInfo Obtain()
            {
                return new XAttrVIOInfo(-1);
            }


            public XAttrVIOInfo(int index)
            {
                this.timestamp = 0;
                this.position =  new float[3] { 0, 0, 0 };
                this.rotation = new float[4] { 0, 0, 0, 0 };

                this.pose_quality = 0;
                this.sensor_quality = 0;
                this.camera_quality = 0;
            }
        };

        /////////////////////////////////////////////
        /// @struct XAttrTrackingInfo
        /// @brief VPU cammera tracking object pose info.
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrTrackingInfo
        {
            public int index;
            public int state;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation;
            public UInt64 timestamp;
            public UInt64 recognized_markers_mask;
            public double confidence;
            public double marker_distance;

            public static XAttrTrackingInfo Obtain()
            {
                return new XAttrTrackingInfo(-1);
            }


            public XAttrTrackingInfo(int index)
            {
                this.timestamp = 0;
                this.index = index;
                this.state = 0;
                this.position = new float[3] { 0, 0, 0 };
                this.rotation = new float[4] { 0, 0, 0, 0 }; 
                this.recognized_markers_mask = 0;
                this.confidence = 0;
                this.marker_distance = -1.0;
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrBeaconInWorldInfo
        {
            public float drift_recenter_distance_threshold;    // distance threshold for detect vio drift and recenter, default: 1.0f
            public float drift_recenter_angle_threshold;			// angle threshold for detect vio drift and recenter, default: 1.0f
            public int beacon_id;                              // beacon id
            public int group_id;                                   // beacon group id, = -1 when this beacon is not belong to a group
            public int coord_system_flag;             // true/false means left/right hand coordinates system
            public float confidence_thresh;            // beacon tracking confidence threshold
            public float min_distance_thresh;      // min beacon tracking distance threshold
            public float max_distance_thresh;      // max beacon tracking distance threshold
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position;                      // beacon position in world 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation;                      // beacon rotation int world  x, y, z, w. Since beacon can only be horizontal or vertical, the rotation can only be happened around y axis

            public static XAttrBeaconInWorldInfo Obtain()
            {
                return new XAttrBeaconInWorldInfo(0);
            }

            public XAttrBeaconInWorldInfo(int beacon_id)
            {

                this.drift_recenter_distance_threshold = 1.0f;
                this.drift_recenter_angle_threshold = 1.0f;
                this.beacon_id = 0;
                this.group_id = 0;
                this.coord_system_flag = 1;
                this.confidence_thresh = 0;
                this.min_distance_thresh = 0;
                this.max_distance_thresh = 0;
                this.position = new float[3] { 0, 0, 0 };
                this.rotation = new float[4] { 0, 0, 0, 1 };
            }

        };


        public static bool plugin_vio_fusion_get_frame(UInt64 predictedTimeNs,
                                    ref XAttrVIOWorldInBeaconInfo beacon_Info,
                                    ref XAttrTrackingInfo marker_info,
                                    ref XAttrVIOInfo vio_info)
        {

            int beacon_id = 0;
            UInt64 beacon_timestamp = 0;
            float beacon_pos0 = 0;
            float beacon_pos1 = 0;
            float beacon_pos2 = 0;
            float beacon_rot0 = 0;
            float beacon_rot1 = 0;
            float beacon_rot2 = 0;
            float beacon_rot3 = 0;
            float beacon_tracking_confidence = 0;
            float beacon_min_distance = 0;
            float beacon_correct_weight = 0;

            int track_index = 0;
            int track_state = 0;
            UInt64 track_timestamp = 0;
            float track_pos0 = 0;
            float track_pos1 = 0;
            float track_pos2 = 0;
            float track_rot0 = 0;
            float track_rot1 = 0;
            float track_rot2 = 0;
            float track_rot3 = 0;
            UInt64 track_recognized_markers_mask = 0;
            double track_confidence = 0;
            double track_marker_distance = 0;

            UInt64 vio_timestamp = 0;
            float vio_pos0 = 0;
            float vio_pos1 = 0;
            float vio_pos2 = 0;
            float vio_rot0 = 0;
            float vio_rot1 = 0;
            float vio_rot2 = 0;
            float vio_rot3 = 0;
            float vio_pose_quality = 0;
            float vio_sensor_quality = 0;
            float vio_camera_quality = 0;


            bool ret = plugin_vio_fusion_get2(predictedTimeNs, ref beacon_id,
                                               ref beacon_timestamp,
                                               ref beacon_pos0, ref beacon_pos1, ref beacon_pos2,
                                               ref beacon_rot0, ref beacon_rot1, ref beacon_rot2, ref beacon_rot3,
                                               ref beacon_tracking_confidence,
                                               ref beacon_min_distance,
                                               ref beacon_correct_weight,
                                               ref track_index,
                                               ref track_state,
                                               ref track_timestamp,
                                                ref track_pos0,
                                                ref track_pos1,
                                                ref track_pos2,
                                                ref track_rot0,
                                                ref track_rot1,
                                                ref track_rot2,
                                                ref track_rot3,
                                                ref track_recognized_markers_mask,
                                                ref track_confidence,
                                                ref track_marker_distance,
                                                ref vio_timestamp,
                                                ref vio_pos0,
                                                ref vio_pos1,
                                                ref vio_pos2,
                                                ref vio_rot0, ref vio_rot1, ref  vio_rot2, ref vio_rot3,
                                                ref vio_pose_quality,
                                                ref vio_sensor_quality,
                                                ref vio_camera_quality);

            if (ret) {
                beacon_Info.beacon_id = beacon_id;
                beacon_Info.timestamp = beacon_timestamp;
                beacon_Info.position[0] = beacon_pos0;
                beacon_Info.position[1] = beacon_pos1;
                beacon_Info.position[2] = beacon_pos2;
                beacon_Info.rotation[0] = beacon_rot0;
                beacon_Info.rotation[1] = beacon_rot1;
                beacon_Info.rotation[2] = beacon_rot2;
                beacon_Info.rotation[3] = beacon_rot3;
                beacon_Info.tracking_confidence = beacon_tracking_confidence;
                beacon_Info.min_distance = beacon_min_distance;
                beacon_Info.correct_weight = beacon_correct_weight;
  
                marker_info.index = track_index;
                marker_info.state = track_state;
                marker_info.timestamp = track_timestamp;
                marker_info.position[0] = track_pos0;
                marker_info.position[1] = track_pos1;
                marker_info.position[2] = track_pos2;
                marker_info.rotation[0] = track_rot0;
                marker_info.rotation[1] = track_rot1;
                marker_info.rotation[2] = track_rot2;
                marker_info.rotation[3] = track_rot3;
                marker_info.recognized_markers_mask = track_recognized_markers_mask;
                marker_info.confidence = track_confidence;
                marker_info.marker_distance = track_marker_distance;

                vio_info.timestamp = vio_timestamp;
                vio_info.position[0] = vio_pos0;
                vio_info.position[1] = vio_pos1;
                vio_info.position[2] = vio_pos2;
                vio_info.rotation[0] = vio_rot0;
                vio_info.rotation[1] = vio_rot1;
                vio_info.rotation[2] = vio_rot2;
                vio_info.rotation[3] = vio_rot3;
                vio_info.pose_quality = vio_pose_quality;
                vio_info.sensor_quality = vio_sensor_quality;
                vio_info.camera_quality = vio_camera_quality;
            }
            
            return ret;

        }

        public static bool plugin_vio_fusion_set_param(ref XAttrBeaconInWorldInfo beacon_in_world_info)
        {
            // beacon in world ptr
            System.IntPtr ptr_info = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(XAttrBeaconInWorldInfo)));
            Marshal.StructureToPtr(beacon_in_world_info, ptr_info, false);
            bool ret = plugin_vio_fusion_set(ptr_info);
            Marshal.FreeHGlobal(ptr_info);
            return ret;
        }


        public static void plugin_vio_fusion_set_off() {
            XAttrBeaconInWorldInfo beacon_in_world_info =new XAttrBeaconInWorldInfo();
            beacon_in_world_info.beacon_id = -1;
            plugin_vio_fusion_set_param(ref beacon_in_world_info);
        }

        [DllImport(kPlugin_FusionAPI, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool plugin_vio_fusion_init(bool bQvrStart);

        [DllImport(kPlugin_FusionAPI, CallingConvention = CallingConvention.Cdecl)]
        public static extern void plugin_vio_fusion_exit();


        [DllImport(kPlugin_FusionAPI, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool plugin_vio_fusion_set(IntPtr beacon_in_world_info);

        [DllImport(kPlugin_FusionAPI, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool plugin_vio_fusion_get(UInt64 predictedTimeNs,
                                    IntPtr beacon_Info,
                                    IntPtr marker_info,
                                    IntPtr vio_info);

        [DllImport(kPlugin_FusionAPI, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool plugin_vio_fusion_run(int unused);

        [DllImport(kPlugin_FusionAPI, CallingConvention = CallingConvention.Cdecl)]
        public static extern float plugin_get_vio_state_float(int data_type);


        [DllImport(kPlugin_FusionAPI, CallingConvention = CallingConvention.Cdecl)]
        public static extern int plugin_get_vio_state_int(int data_type);


        [DllImport(kPlugin_FusionAPI, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool plugin_vio_fusion_reset(int param);

        [DllImport(kPlugin_FusionAPI, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool plugin_vio_fusion_get2(UInt64 predictedTimeNs,
                                    ref int beacon_id,
                                    ref UInt64 beacon_timestamp,
                                    ref float beacon_pos0, ref float beacon_pos1, ref float beacon_pos2,
                                    ref float beacon_rot0, ref float beacon_rot1, ref float beacon_rot2, ref float beacon_rot3,
                                    ref float beacon_tracking_confidence,
                                    ref float beacon_min_distance,
                            ref float beacon_correct_weight,
                            ref int   track_index,
                            ref int   track_state,
                            ref UInt64 track_timestamp,
                            ref float track_pos0, ref float track_pos1, ref float track_pos2,
                            ref float track_rot0, ref float track_rot1, ref float track_rot2, ref float track_rot3,
                            ref UInt64 track_recognized_markers_mask,
                            ref double track_confidence,
                            ref double track_marker_distance,
                            ref UInt64 vio_timestamp,
                            ref float vio_pos0, ref float vio_pos1, ref float vio_pos2,
                            ref float vio_rot0, ref float vio_rot1, ref float vio_rot2, ref float vio_rot3,
                            ref float vio_pose_quality,
                            ref float vio_sensor_quality,
                            ref float vio_camera_quality);
    }
}
