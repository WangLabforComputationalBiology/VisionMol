using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

//#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID || UNITY_WIN || UNITY_MAC
#if true
using UnityEngine;
using UnityEngine.Events;
using XDebug = UnityEngine.Debug;
#else
using XDebug = System.Diagnostics.Debug;
#endif // UNITY_EDITOR

using NativeHandle = System.Int64;
using NativeExHandle = System.Int64;
using AOT;
using Unity.Collections;
using System.IO;
using System.Linq;
using Ximmerse.XR.Collections;
using System.Text;
using TouchlessA3D;
using static TouchlessA3D.NativeCalls;
namespace Ximmerse.XR
{

    public enum HandType
    {
        LEFT_HAND = 0,
        RIGHT_HAND = 1

    };
    public struct HandClientData
    {

        public int timestamp;  //时间戳
        public int id;  //左、右手
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 66)]
        public float[] points;  //关节点
        public int Gestureflag;  //手势

        public ta3d_skeleton_3d_s skeleton3D;

        public ta3d_skeleton_2d_s skeleton2D;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points">66 float</param>
        /// <param name="bones">22 vector3</param>
        public HandClientData(float[] points)
        {
            this.points = points;
            this.timestamp = 0;
            this.id = 0;
            this.Gestureflag = 0;
            skeleton3D = new ta3d_skeleton_3d_s();
            GetDefaultSkeleton3DData(ref skeleton3D);
            skeleton2D = new ta3d_skeleton_2d_s();
            GetDefaultSkeleton2DData(ref skeleton2D);
        }

        /// <summary>
        /// Writes default value to NativeCalls.ta3d_skeleton_3d_s structure.
        /// </summary>
        /// <param name="skel3D"></param>
        private static void GetDefaultSkeleton3DData(ref NativeCalls.ta3d_skeleton_3d_s skel3D)
        {
            skel3D.status = ResultType.RESULT_UNAVAILABLE;
            skel3D.points = new NativeCalls.ta3d_point_3_float_t[(int)NativeCalls.NativePointID.END];
            for (int i = 0, iMax = skel3D.points.Length; i < iMax; i++)
            {
                skel3D.points[i] = new NativeCalls.ta3d_point_3_float_t()
                {
                    coordinates = new float[3],
                };
            }

            skel3D.rotations = new NativeCalls.ta3d_matrix_3_3_float_t[(int)NativeCalls.NativePointID.END];
            for (int i = 0, iMax = skel3D.rotations.Length; i < iMax; i++)
            {
                skel3D.rotations[i] = new NativeCalls.ta3d_matrix_3_3_float_t()
                {
                    elements = new float[9],
                };
            }

            skel3D.bone_lengths = new float[(int)NativeCalls.NativePointID.END];
        }

        /// <summary>
        /// Writes default value to NativeCalls.ta3d_skeleton_2d_s structure.
        /// </summary>
        /// <param name="skel2D"></param>
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("HandClientData:");
            sb.AppendLine($"Timestamp: {timestamp}");
            sb.AppendLine($"ID: {id}");
            sb.AppendLine("Points:");

            const int pointsPerLine = 3;
            for (int i = 0; i < points.Length; i++)
            {
                if (i % pointsPerLine == 0)
                    sb.Append("  ");

                sb.Append($"{points[i]} ");

                if ((i + 1) % pointsPerLine == 0)
                    sb.AppendLine();
            }

            sb.AppendLine($"Gesture Flag: {Gestureflag}");
            return sb.ToString();
        }

        /// <summary>
        /// Copy data to raw skeleton 3d data
        /// </summary>
        /// <param name="rawSkel3D"></param>
        public void CopyTo(Vector3[] bones)
        {
            for (int boneIndex = 0, bonesLength = 22; boneIndex < bonesLength; boneIndex++)
            {
                bones[boneIndex] = new Vector3
                (
                    points[boneIndex * 3],
                    points[boneIndex * 3 + 1],
                    -points[boneIndex * 3 + 2]
                );
            }
        }

        /// <summary>
        /// Get point position of the index.
        /// </summary>
        /// <param name="NodeIndex"></param>
        /// <returns></returns>
        public Vector3 GetPosition(int NodeIndex)
        {
            return new Vector3(
                points[NodeIndex * 3],
                points[NodeIndex * 3 + 1],
                points[NodeIndex * 3 + 2]
                );
        }

        /// <summary>
        /// Gets hand track client service data in skeleton 3d.
        /// </summary>
        /// <param name="skel3D">Output skel 3d data.</param>
        /// <param name="skel2D">Output skel 2d data.</param>
        /// <param name="raw2view">Matrix converts raw to view space, for gettng skeleton 2d data. </param>
        public void GetSkeleton(out ta3d_skeleton_3d_s skel3D,
            out ta3d_skeleton_2d_s skel2D,
            Matrix4x4 raw2view)
        {
            this.skeleton3D.status = this.timestamp != 0 ? ResultType.RESULT_OK : ResultType.RESULT_UNAVAILABLE;

            if (skeleton3D.status == ResultType.RESULT_OK)
            {
                for(int i = 0; i < 21; i++)
                {
                    var rawPoint = GetPosition(i);
                    this.skeleton3D.SetPosition(i, rawPoint);

                    Vector3 viewSpacePosition = raw2view.MultiplyPoint3x4(rawPoint);
                    this.skeleton2D.SetPosition(i, viewSpacePosition);
                }
            }
            skel3D = skeleton3D;
            skel2D = skeleton2D;
        }

        public void Reset()
        {
            Gestureflag = -1;
            //for (int i = 0, iMax = points.Length; i < iMax; i++)
            //{
            //    points[i] = default;
            //}
        }
    };

    public class HandTrackClient
    {
        public const string Tag = "HandTrackClient";

        static System.Int64 sPlugin = 0;
        static string PluginName = "handDataService";
        static string SharedMemNameL = "skeleton_left_mem";
        static string SharedMemNameR = "skeleton_right_mem";

        static bool isInited = false;


        static public int Init()
        {

            if (isInited) {
                return 0;
            }
            XRuntimeClient.Init();
            sPlugin = XRuntimeClient.xruntime_client_get_plugin(PluginName);
            Debug.LogFormat("get plugin: {0}\n", sPlugin);
            if (sPlugin > 0)
            {
                XRuntimeClient.xplugin_start(sPlugin);
            }
            else
            {
                return -1;
            }

            shared_memL = Marshal.AllocHGlobal(Marshal.SizeOf<HandClientData>());
            shared_memR = Marshal.AllocHGlobal(Marshal.SizeOf<HandClientData>());

            isInited = true;
            return 0;
        }


        static public void Exit()
        {
            isInited = false;

            if (sPlugin > 0)
            {
                XRuntimeClient.xplugin_stop(sPlugin);
            }
            sPlugin = 0;
            XRuntimeClient.Exit();
            Marshal.FreeHGlobal(shared_memL);
            Marshal.FreeHGlobal(shared_memR);
        }


        static public int SetCmd(int type, IntPtr param)
        {
            return 0;
        }

        static System.IntPtr shared_memL;
        static System.IntPtr shared_memR;
        HandClientData pose_data;
        static public bool GetEvent(HandType handID, ref HandClientData pose_data)
        {


            if (!isInited) {
                return false;
            }

            System.IntPtr shared_mem;
            String SharedMemName;

            if (handID == HandType.LEFT_HAND)
            {
                shared_mem = shared_memL;
                SharedMemName = SharedMemNameL;
            }
            else
            {
                shared_mem = shared_memR;
                SharedMemName = SharedMemNameR;
            }


            int ret = XRuntimeClient.xplugin_get_ringbuf_data(sPlugin, SharedMemName, 0, 1, shared_mem);


            if (ret < 0)
            {
                return false;
            }

            //RB 0 2 1 64
            pose_data = Marshal.PtrToStructure<HandClientData>(shared_mem);
            //Debug.LogFormat("{0} hand:  ({1}, {2}, {3})  ({4}, {5}, {6}) ({7},{8}, {9}) ({10}, {11},{12})\n",
            //         handID,
            //         pose_data.points[0], pose_data.points[1], pose_data.points[2], pose_data.points[3],
            //         pose_data.points[4], pose_data.points[5], pose_data.points[6], pose_data.points[7],
            //         pose_data.points[8], pose_data.points[9], pose_data.points[10], pose_data.points[11]);
            return true;
        }
    }
}
