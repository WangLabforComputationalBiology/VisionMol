/* Copyright (c) 2022 Crunchfish AB. All rights reserved. All information herein
 * is or may be trade secrets of Crunchfish AB.
 */
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace TouchlessA3D
{
    /// <summary>
    /// A flag for the success of a call or how it failed.
    /// </summary>
    public enum ResultType : int
    {
        RESULT_UNAVAILABLE,
        RESULT_OK,
        BUSY,
        INVALID_ARGUMENT,
        TIMEOUT,
    }

    /// <summary>
    /// Describes image transformation.
    /// </summary>
    public enum FrameTransform : int
    {
        /// no transformation
        NONE,
        /// mirror image
        FLIP_HORIZONTAL,
        /// flip image vertically
        FLIP_VERTICAL,
        /// rotate image 180 degrees
        ROTATE_180
    }

    /// <summary>
    /// A collection of calls and structs to the underlying libray used in the SDK.
    /// </summary>
    public static partial class NativeCalls
    {

        [DllImport("touchless_a3d_xr")] // ta3d_engine_t
        public static extern IntPtr ta3d_engine_acquire(System.String unique_id, System.String persistent_storage_path,
          IntPtr calibration, IntPtr callback, IntPtr callback_context);

        [DllImport("touchless_a3d_xr")]
        public static extern IntPtr ta3d_engine_get_build_id();

        [DllImport("touchless_a3d_xr")]
        public static extern void ta3d_engine_release(IntPtr ta3d_engine_t);

        [DllImport("touchless_a3d_xr")]
        public static extern ResultType ta3d_engine_handle_frame(IntPtr ta3d_engine_t, IntPtr ta3d_frame_t);

        [DllImport("touchless_a3d_xr")]
        public static extern ResultType ta3d_engine_predict_hands(IntPtr ta3d_engine_t, IntPtr left_hand_event, IntPtr right_hand_event, long timestamp_ms, long allowed_event_lifetime);

        [DllImport("touchless_a3d_xr")]
        public static extern ResultType ta3d_engine_get_events(IntPtr ta3d_engine_t, IntPtr left_hand_event, IntPtr right_hand_event);

        [DllImport("touchless_a3d_xr")]
        public static extern ResultType ta3d_engine_get_events_for(IntPtr ta3d_engine_t, IntPtr left_hand_event, IntPtr right_hand_event, int timeout_ms);

        [DllImport("touchless_a3d_xr")]
        public static extern IntPtr ta3d_event_create();

        [DllImport("touchless_a3d_xr")]
        public static extern void ta3d_event_destroy(IntPtr ta3d_event);

        [DllImport("touchless_a3d_xr")] //ta3d_frame_t
        public static extern IntPtr ta3d_frame_create_from_android_420(
          IntPtr src_y, int src_stride_y,
          IntPtr src_u, int src_stride_u,
          IntPtr src_v, int src_stride_v,
          int pixel_stride_uv, int width, int height,
          long timestamp_ms, FrameRotation rotation);

        [DllImport("touchless_a3d_xr")]
        public static extern IntPtr ta3d_frame_create_from_rgba(
          IntPtr src_rgba, int src_stride,
          int width, int height,
          long timestamp_ms, FrameRotation rotation,
          bool flip_vertically);

        [DllImport("touchless_a3d_xr")]
        public static extern IntPtr ta3d_frame_create_from_grayscale(
          IntPtr pixels, int stride, int width, int height,
          long timestamp_ms, FrameRotation rotation);

        [DllImport("touchless_a3d_xr")]
        public static extern void ta3d_frame_destroy(IntPtr ta3d_frame_t);

        [DllImport("touchless_a3d_xr")]
        public static extern GestureType ta3d_event_get_type(IntPtr ta3d_event_t);

        [DllImport("touchless_a3d_xr")]
        public static extern void ta3d_get_skeleton_3d(IntPtr ta3d_event_t, IntPtr ta3d_skeleton_3d_t);

        [DllImport("touchless_a3d_xr")]
        public static extern void ta3d_get_skeleton_2d(IntPtr ta3d_event_t, IntPtr ta3d_skeleton_2d_t);

        [DllImport("touchless_a3d_xr")]
        public static extern HandednessType ta3d_get_handedness(IntPtr ta3d_event_t);

        [DllImport("touchless_a3d_xr")]
        public static extern long ta3d_get_timestamp(IntPtr ta3d_event_t);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct ta3d_point_2_float_t
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public float[] coordinates;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct ta3d_point_3_float_t
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] coordinates;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct ta3d_size_2_float_t
        {
            public float width;
            public float height;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct ta3d_matrix_3_3_float_t
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public float[] elements;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct ta3d_matrix_1_8_float_t
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public float[] elements;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct ta3d_calibration_s
        {
            public ta3d_size_2_float_t calibration_size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public float[] camera_matrix;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public float[] distortion_coefficients;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct ta3d_skeleton_2d_s
        {
            public ResultType status;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NativePointID.END)]
            public ta3d_point_2_float_t[] points;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct ta3d_skeleton_3d_s
        {
            public ResultType status;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NativePointID.END)]
            public ta3d_point_3_float_t[] points;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NativeBoneID.END)]
            public ta3d_matrix_3_3_float_t[] rotations;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NativeBoneID.END)]
            public float[] bone_lengths;
        }

        public enum NativePointID : int
        {
            THUMB1 = 0,
            THUMB2 = 1,
            THUMB3 = 2,
            THUMB_TIP = 3,
            INDEX1 = 4,
            INDEX2 = 5,
            INDEX3 = 6,
            INDEX_TIP = 7,
            MIDDLE1 = 8,
            MIDDLE2 = 9,
            MIDDLE3 = 10,
            MIDDLE_TIP = 11,
            RING1 = 12,
            RING2 = 13,
            RING3 = 14,
            RING_TIP = 15,
            PINKY1 = 16,
            PINKY2 = 17,
            PINKY3 = 18,
            PINKY_TIP = 19,
            WRIST = 20,
            END = 21,
        }

        public enum NativeBoneID : int
        {
            WRIST_THUMB1 = 0,
            THUMB1_THUMB2 = 1,
            THUMB2_THUMB3 = 2,
            THUMB3_THUMB_TIP = 3,
            WRIST_INDEX1 = 4,
            INDEX1_INDEX2 = 5,
            INDEX2_INDEX3 = 6,
            INDEX3_INDEX_TIP = 7,
            WRIST_MIDDLE1 = 8,
            MIDDLE1_MIDDLE2 = 9,
            MIDDLE2_MIDDLE3 = 10,
            MIDDLE3_MIDDLE_TIP = 11,
            WRIST_RING1 = 12,
            RING1_RING2 = 13,
            RING2_RING3 = 14,
            RING3_RING_TIP = 15,
            WRIST_PINKY1 = 16,
            PINKY1_PINKY2 = 17,
            PINKY2_PINKY3 = 18,
            PINKY3_PINKY_TIP = 19,
            END = 20,
        }

        public static Vector3 GetPosition(this ta3d_skeleton_3d_s skeleton, NativePointID pointID, bool flipped)
        {
            var coords = skeleton.points[(int)pointID].coordinates;
            var position = flipped ? new Vector3(coords[0], -coords[1], coords[2]) : new Vector3(coords[0], coords[1], coords[2]);
            return position;
        }

        public static void SetPosition(ref this ta3d_skeleton_3d_s skeleton, int pointID, Vector3 point)
        {
            var coords = skeleton.points[pointID].coordinates;
            coords[0] = point.x;
            coords[1] = point.y;
            coords[2] = point.z;
        }

        public static Vector2 GetPosition(this ta3d_skeleton_2d_s skeleton2d, NativePointID pointID, bool flipped)
        {
            var coords = skeleton2d.points[(int)pointID].coordinates;
            var position = flipped ? new Vector2(coords[0], 1 - coords[1]) : new Vector2(coords[0], coords[1]); // convert to space : min = left bottom, max = right top
            return position;
        }

        public static void SetPosition(ref this ta3d_skeleton_2d_s skeleton2d, int pointID, Vector2 point)
        {
            var coords = skeleton2d.points[pointID].coordinates;
            coords[0] = point.x;
            coords[1] = point.y;
        }
    }
}