using System;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Profiling;
namespace TouchlessA3D
{
    using static NativeCalls;
    /**
     * An engine for processing frames.
     */
    public class Engine
    {
        private delegate void CallbackFromNative(IntPtr context, IntPtr ta3d_event_t);
        private IntPtr m_ta3d_engine_t;
        /// <summary>
        /// leftEvent for the engine to update
        /// </summary>
        private IntPtr leftEventPtr = IntPtr.Zero;

        /// <summary>
        /// rightEvent for the engine to update
        /// </summary>
        private IntPtr rightEventPtr = IntPtr.Zero;

        public IntPtr LeftEventPtr { get => leftEventPtr; }
        public IntPtr RightEventPtr { get => rightEventPtr; }


        private IntPtr leftSkeletonDataIntPtr, rightSkeletonDataIntPtr;

        private IntPtr leftSkeletonData2dIntPtr, rightSkeletonData2dIntPtr;

        private ta3d_skeleton_3d_s leftSkeleton3DData, rightSkeleton3DData;

        private ta3d_skeleton_2d_s leftSkeleton2DData, rightSkeleton2DData;

        public Frame rgbaFrame;

        public Engine(string unique_id, string persistent_storage_path, ICalibration calibration)
        {
            IntPtr nativeCalibration = calibration == null ? IntPtr.Zero : calibration.getNativeCalibration();
            m_ta3d_engine_t = NativeCalls.ta3d_engine_acquire(unique_id, persistent_storage_path, nativeCalibration, IntPtr.Zero, IntPtr.Zero);

            //Create left and right hand event pointer :
            leftEventPtr = ta3d_event_create();
            rightEventPtr = ta3d_event_create();

            leftSkeletonDataIntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ta3d_skeleton_3d_s)));
            rightSkeletonDataIntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ta3d_skeleton_3d_s)));

            leftSkeletonData2dIntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ta3d_skeleton_2d_s)));
            rightSkeletonData2dIntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ta3d_skeleton_2d_s)));

            GetDefaultSkeleton3DData(ref leftSkeleton3DData);
            GetDefaultSkeleton3DData(ref rightSkeleton3DData);
            GetDefaultSkeleton2DData(ref leftSkeleton2DData);
            GetDefaultSkeleton2DData(ref rightSkeleton2DData);
        }

        private void GetDefaultSkeleton3DData(ref ta3d_skeleton_3d_s skel3D)
        {
            skel3D.status = ResultType.RESULT_UNAVAILABLE;
            skel3D.points = new ta3d_point_3_float_t[(int)NativeCalls.NativePointID.END];
            for (int i = 0, iMax = skel3D.points.Length; i < iMax; i++)
            {
                skel3D.points[i] = new ta3d_point_3_float_t()
                {
                    coordinates = new float[3],
                };
            }

            skel3D.rotations = new ta3d_matrix_3_3_float_t[(int)NativeCalls.NativePointID.END];
            for (int i = 0, iMax = skel3D.rotations.Length; i < iMax; i++)
            {
                skel3D.rotations[i] = new ta3d_matrix_3_3_float_t()
                {
                     elements = new float[9],
                };
            }

            skel3D.bone_lengths = new float[(int)NativeCalls.NativePointID.END];
        }

        private void GetDefaultSkeleton2DData(ref ta3d_skeleton_2d_s skel2D)
        {
            skel2D.status = ResultType.RESULT_UNAVAILABLE;
            skel2D.points = new ta3d_point_2_float_t[21];
            for (int i = 0, iMax = skel2D.points.Length; i < iMax; i++)
            {
                skel2D.points[i] = new ta3d_point_2_float_t()
                {
                    coordinates = new float[2],
                };
            }
        }

        ~Engine()
        {
            Marshal.FreeHGlobal(leftSkeletonDataIntPtr);
            Marshal.FreeHGlobal(rightSkeletonDataIntPtr);

            Marshal.FreeHGlobal(leftSkeletonData2dIntPtr);
            Marshal.FreeHGlobal(rightSkeletonData2dIntPtr);

            ta3d_event_destroy(leftEventPtr);
            ta3d_event_destroy(rightEventPtr);
            NativeCalls.ta3d_engine_release(m_ta3d_engine_t);
        }

        public void handleFrame(IFrame frame)
        {
            NativeCalls.ta3d_engine_handle_frame(m_ta3d_engine_t, frame.getNativeFrame());
        }
        /// <summary>
        /// Avoid GC : handleFrame cause a boxing when IFrame is a struct
        /// </summary>
        /// <param name="frame"></param>
        public void handleFrame2(Frame frame)
        {
            NativeCalls.ta3d_engine_handle_frame(m_ta3d_engine_t, frame.getNativeFrame());
        }

        /// <summary>
        /// Introduced at 2.1.0
        /// </summary>
        public void UpdateHands(
            out ta3d_skeleton_3d_s LeftHandSkeletonData,
            out ta3d_skeleton_2d_s LeftHandSkeleton2dData,
            out GestureType gesture_left,
            out ta3d_skeleton_3d_s RightHandSkeletonData,
            out ta3d_skeleton_2d_s RightHandSkeleton2dData,
            out GestureType gesture_right,
            out long latestEventFrameTime,
            bool predict = true, long predictEventLifeTime = 150)
        {
            latestEventFrameTime = -1;
            ResultType result = ta3d_engine_get_events_for(m_ta3d_engine_t, leftEventPtr, rightEventPtr, 0);
            if (result == ResultType.RESULT_OK)
            {
                var leftTime = ta3d_get_timestamp(leftEventPtr);
                var rightTime = ta3d_get_timestamp(rightEventPtr);
                latestEventFrameTime = leftTime > rightTime ? leftTime : rightTime;
            }
            if (predict)
            {
                long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                result = ta3d_engine_predict_hands(m_ta3d_engine_t,
                    leftEventPtr, rightEventPtr, timestamp, predictEventLifeTime);
            }

            if (GetSkeleton(leftEventPtr, leftSkeletonDataIntPtr, ref this.leftSkeleton3DData, out long ts_left, out gesture_left))
            {
                GetSkeleton2D(leftEventPtr, leftSkeletonData2dIntPtr, ref this.leftSkeleton2DData);
            }
            else
            {
                LeftHandSkeleton2dData = default;
            }
            if (GetSkeleton(rightEventPtr, rightSkeletonDataIntPtr, ref this.rightSkeleton3DData, out long ts_right, out gesture_right))
            {
                GetSkeleton2D(rightEventPtr, rightSkeletonData2dIntPtr, ref this.rightSkeleton2DData);
            }
            else
            {
                RightHandSkeleton2dData = default;
            }

            LeftHandSkeletonData = this.leftSkeleton3DData;
            RightHandSkeletonData = this.rightSkeleton3DData;
            LeftHandSkeleton2dData = this.leftSkeleton2DData;
            RightHandSkeleton2dData = this.rightSkeleton2DData;
        }


        private bool GetSkeleton(in IntPtr handEvent, IntPtr skeletonPtr, ref ta3d_skeleton_3d_s skeletonData, out long timestamp, out GestureType gestureType)
        {
            skeletonData.status = ResultType.RESULT_UNAVAILABLE;//assigns default result type
            timestamp = 0;
            gestureType = GestureType.NO_HAND;
            if (IntPtr.Zero == handEvent || GestureType.NO_HAND == ta3d_event_get_type(handEvent))
            {
                return false;
            }
            ta3d_get_skeleton_3d(handEvent, skeletonPtr);
            //   skeletonData = (ta3d_skeleton_3d_s)Marshal.PtrToStructure(skeletonPtr, typeof(ta3d_skeleton_3d_s));
            NativeCalls.GetSkeleton3DPose(skeletonPtr, ref skeletonData);
            if (skeletonData.status != ResultType.RESULT_OK)
            {
                return false;
            }

            timestamp = ta3d_get_timestamp(handEvent);
            gestureType = ta3d_event_get_type(handEvent);

            return true;
        }

        private bool GetSkeleton2D(in IntPtr handEvent, IntPtr skeleton2DPtr, ref ta3d_skeleton_2d_s skeleton2dData)
        {
            skeleton2dData.status = ResultType.RESULT_UNAVAILABLE;
            ta3d_get_skeleton_2d(handEvent, skeleton2DPtr);
            NativeCalls.GetSkeleton2DPose(skeleton2DPtr, ref skeleton2dData);
            //Debug.LogFormat("Wrist2D point: {0}", skeleton2dData.GetPosition(NativePointID.WRIST).ToString("F3"));
            return skeleton2dData.status == ResultType.RESULT_OK;
        }
    }
}