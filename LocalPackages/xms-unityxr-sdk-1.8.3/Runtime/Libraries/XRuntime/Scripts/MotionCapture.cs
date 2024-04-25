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
namespace Ximmerse.XR
{

    public enum BodyPoint
    {
        Pelvis = 0,  // 骨盆
        LEFT_HIP = 1,  // 左侧臀部
        RIGHT_HIP = 2,  // 右侧臀部
        SPINE1 = 3,  // 脊柱
        LEFT_KNEE = 4,  // 左腿膝盖
        RIGHT_KNEE = 5,  // 右腿膝盖
        SPINE2 = 6,  // 脊柱
        LEFT_ANKLE = 7,  // 左脚踝
        RIGHT_ANKLE = 8,  // 右脚踝
        SPINE3 = 9,  // 脊柱
        LEFT_FOOT = 10,  // 左脚
        RIGHT_FOOT = 11,  // 右脚
        NECK = 12,  // 颈部
        LEFT_COLLAR = 13,  // 左侧锁骨
        RIGHT_COLLAR = 14,  // 右侧锁骨
        HEAD = 15,  // 头部
        LEFT_SHOULDER = 16,  // 左肩膀
        RIGHT_SHOULDER = 17,  // 右肩膀
        LEFT_ELBOW = 18,  // 左手肘
        RIGHT_ELBOW = 19,  // 右手肘
        LEFT_WRIST = 20,  // 左手腕
        RIGHT_WRIST = 21,  // 右手腕
        LEFT_HAND = 22,  // 左手
        RIGHT_HAND = 23  // 右手
    }

    public enum DevicePonit {

        LEFT_HAND = 0,
        RIGHT_HAND = 1,
        LEFT_LEG = 2,
        RIGHT_LEG = 3, 
        HEAD = 4,
        CHEST = 5,
        DEV_NUM = 6
    }



  
    public struct BodyTrackerData
    {
        public uint timestamp;          // timestamp, in second
        public uint timestamp_nsec;     // timestamp, in nanosecond

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public Quaternion[] data;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
        public int[] device_states;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
        public Vector3[] device_accs;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
        public Vector3[] device_gyros;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
        public Vector3[] device_locs;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
        public Vector3[] device_vels;

        public BodyTrackerData(Quaternion[] data = null)
        {
            this.data = data == null ? new Quaternion[24] : data;
            this.device_states = new int[6];
            this.device_accs = new Vector3[6];
            this.device_gyros = new Vector3[6];
            this.device_locs = new Vector3[6];
            this.device_vels = new Vector3[6];
            this.timestamp = 0;
            this.timestamp_nsec = 0;
        }
    }
    public class MotionCapture
    {
        public const string Tag = "MotionCapture";

        static System.Int64 sPlugin = 0;
        static string PluginName = "MotionAlg";
        static string SharedMemName = "motion_result";
        static System.IntPtr shared_mem;

        public struct PoseData
        {
            public uint timestamp;          // timestamp, in second
            public uint timestamp_nsec;     // timestamp, in nanosecond
            public int pose_state;          // 0/1: invalid/valid


            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 96)] 
            public double[] pose;           // (x,y,z,w) * 24 nodes,
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] device_states;     // lhand, rhand, lleg, rleg, head, chest
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18)]
            public double[] accs;           // acc in local space, (acc_x, acc_y, acc_z) * 6 devices
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18)]
            public double[] gyros;          // gyro in local space, (gyro_x, gyro_y, gyro_z) * 6 devices
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18)]
            public double[] locs;           // location in QVR space,
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18)]
            public double[] vels;           // velocity in QVR space, (vel_x, vel_y, vel_z) * 6 devices
        };



        static public int Init()
        {
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

            shared_mem = Marshal.AllocHGlobal(Marshal.SizeOf<PoseData>());

            return 0;
        }


        static public void Exit()
        {
            if (sPlugin > 0)
            {
                XRuntimeClient.xplugin_stop(sPlugin);
            }

            XRuntimeClient.Exit();
            Marshal.FreeHGlobal(shared_mem);
        }


        static public int SetCmd(int type, IntPtr param)
        {
            return 0;
        }

        static public bool ResetChestYaw() {

            if (sPlugin > 0) {
                XRuntimeClient.xplugin_do_void(sPlugin, (int)MotionAlgActionType.kActResetChestYaw);

                return true;
            }

            return false;
        }


        static public bool SetLogLevel(int level)
        {

            if (sPlugin > 0)
            {
                XRuntimeClient.xplugin_do_int(sPlugin, (int)MotionAlgActionType.kActSetLogLevel, level);

                return true;
            }

            return false;
        }
        /// <summary>
        /// 设置动捕算法模式
        /// </summary>
        /// <param name="mode">int 0, 1</param>
        /// <returns></returns>
        static public bool SetAlgMode(int mode)
        {

            if (sPlugin > 0)
            {
                XRuntimeClient.xplugin_do_int(sPlugin, (int)MotionAlgActionType.kActSetAlgMode, mode);

                return true;
            }

            return false;
        }
        /// <summary>
        /// 设置动捕设备的电击
        /// </summary>
        /// <param name="controller_type">设置触发电击的设备</param>
        /// <param name="position">设置电击点位，参考 xdevice_client.h 中的 XShockPosition</param>
        /// <param name="strength">设置电击强度，百分比范围0~100</param>
        /// <param name="duration">设置电击时长, 单位 ms</param>
        /// <returns></returns>
        static public bool SetShock(MotionControllerType controller_type, int position, int strength, int duration)
        {
            if (sPlugin > 0)
            {
                XRuntimeClient.xplugin_do_void(sPlugin, (int)MotionAlgActionType.kActClearShock);
                XRuntimeClient.xplugin_do_int(sPlugin,  (int)MotionAlgActionType.kActSetShockTarget, (int)controller_type);
                XRuntimeClient.xplugin_do_int(sPlugin,  (int)MotionAlgActionType.kActSetShockPosition, position);
                XRuntimeClient.xplugin_do_int(sPlugin,  (int)MotionAlgActionType.kActSetShockStrength, strength);
                XRuntimeClient.xplugin_do_int(sPlugin,  (int)MotionAlgActionType.kActSetShockDuration, duration);
                XRuntimeClient.xplugin_do_void(sPlugin, (int)MotionAlgActionType.kActDoShock);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置动捕设备震动
        /// </summary>
        /// <param name="controller_type">设置触发震动的控制器</param>
        /// <param name="mode">设置震动模式</param>
        /// <param name="strength">设置震动强度，百分比范围0~100</param>
        /// <param name="duration">设置震动时长, 单位 ms</param>
        /// <returns>true: 成功  false:失败</returns>
        static public bool SetVibrate(MotionControllerType controller_type, int mode, int strength, int duration)
        {
            if (sPlugin > 0)
            {
                XRuntimeClient.xplugin_do_void(sPlugin, (int)MotionAlgActionType.kActClearVibrate);
                XRuntimeClient.xplugin_do_int(sPlugin,  (int)MotionAlgActionType.kActSetVibrateTarget, (int)controller_type);
                XRuntimeClient.xplugin_do_int(sPlugin,  (int)MotionAlgActionType.kActSetVibrateMode, mode);
                XRuntimeClient.xplugin_do_int(sPlugin,  (int)MotionAlgActionType.kActSetVibrateStrength, strength);
                XRuntimeClient.xplugin_do_int(sPlugin,  (int)MotionAlgActionType.kActSetVibrateDuration, duration);
                XRuntimeClient.xplugin_do_void(sPlugin, (int)MotionAlgActionType.kActDoVibrate);

                return true;
            }

            return false;
        }

        //static public bool SetVibrate(MotionControllerType device, MotionAlgActionType actionType) {

        //    if (sPlugin > 0)
        //    {
        //        XRuntimeClient.xplugin_do_int(sPlugin, (int)actionType, (int)device);

        //        return true;
        //    }

        //    return false;
        //}

        float px, py, pz;
        float rx, ry, rz, rw;

        static BodyTrackerData lastBodyData = new BodyTrackerData(null);
        static public bool GetEvent(ref BodyTrackerData bodyData)
        {

            PoseData pose_data;
            int ret = XRuntimeClient.xplugin_get_ringbuf_data(sPlugin, SharedMemName, 0, 1, shared_mem);
            //Debug.LogFormat("get shm p={0}\n", shared_mem);

            if (ret < 0)
            {
                return false;
            }

            //RB 0 2 1 64
            pose_data = Marshal.PtrToStructure<PoseData>(shared_mem);
            //Debug.LogFormat("Motion result: head ({0}, {1}, {2}, {3}), lhand ({4}, {5}, {6}, {7}), rhand({8}, {9}, {10}, {11})\n",
            //         pose_data.pose[15 * 4 + 0], pose_data.pose[15 * 4 + 1], pose_data.pose[15 * 4 + 2], pose_data.pose[15 * 4 + 3],
            //         pose_data.pose[20 * 4 + 0], pose_data.pose[20 * 4 + 1], pose_data.pose[20 * 4 + 2], pose_data.pose[20 * 4 + 3],
            //         pose_data.pose[21 * 4 + 0], pose_data.pose[21 * 4 + 1], pose_data.pose[21 * 4 + 2], pose_data.pose[21 * 4 + 3]);

            bodyData.timestamp = pose_data.timestamp;
            bodyData.timestamp_nsec = pose_data.timestamp_nsec;

            Quaternion quat_rotate_z = new Quaternion();
            quat_rotate_z.eulerAngles = new Vector3(0, 0, 90f);
            for (int i = 0; i < 24; i++)
            {
                //QVR to Unity
                //if (i == 0)
                //{

                //    Vector3 pos00 = new Vector3(0, 0, 0);
                //    Quaternion rot00 = new Quaternion((float)pose_data.pose[i * 4 + 1],
                //                                      (float)pose_data.pose[i * 4 + 2],
                //                                      (float)pose_data.pose[i * 4 + 3],
                //                                      (float)pose_data.pose[i * 4 + 0]);
                //    NativePluginApi.QVR2UnityCoordinate(pos00, rot00);

                //    bodyData.data[i] = rot00;

                //    continue;
                //}
                bodyData.data[i].x = (float)pose_data.pose[i * 4 + 1];
                bodyData.data[i].y = -(float)pose_data.pose[i * 4 + 2];
                bodyData.data[i].z = -(float)pose_data.pose[i * 4 + 3];
                bodyData.data[i].w = (float)pose_data.pose[i * 4 + 0];


                // pose[i] = quat_rotate_z *pose[i];
            }
            Quaternion rot_tmp = new Quaternion();

            for (int i = 0; i < (int)DevicePonit.DEV_NUM; i++) {

                bodyData.device_states[i] = pose_data.device_states[i];

                bodyData.device_accs[i].x = (float)pose_data.accs[i * 3 + 0];
                bodyData.device_accs[i].y = (float)pose_data.accs[i * 3 + 1];
                bodyData.device_accs[i].z = (float)pose_data.accs[i * 3 + 2];

                bodyData.device_gyros[i].x = (float)pose_data.gyros[i * 3 + 0];
                bodyData.device_gyros[i].y = (float)pose_data.gyros[i * 3 + 1];
                bodyData.device_gyros[i].z = (float)pose_data.gyros[i * 3 + 2];


                bodyData.device_vels[i].x = (float)pose_data.vels[i * 3 + 0];
                bodyData.device_vels[i].y = (float)pose_data.vels[i * 3 + 1];
                bodyData.device_vels[i].z = (float)pose_data.vels[i * 3 + 2];

                //NativePluginApi.QVR2UnityCoordinate(bodyData.device_vels[i], rot_tmp);

                bodyData.device_locs[i].x = (float)pose_data.locs[i * 3 + 0];
                bodyData.device_locs[i].y = (float)pose_data.locs[i * 3 + 1];
                bodyData.device_locs[i].z = (float)pose_data.locs[i * 3 + 2];

              
                //NativePluginApi.QVR2UnityCoordinate(bodyData.device_locs[i], rot_tmp);

            }

            lastBodyData = bodyData;

            return true;
        }


        static public Vector3 GetDevicePosition(DevicePonit ponit) {

            if (sPlugin < 0) {
                return new Vector3(0, 0, 0);
            }

            return lastBodyData.device_locs[(int)ponit];
        }

        static public Vector3 GetDeviceAcc(DevicePonit ponit)
        {

            if (sPlugin < 0)
            {
                return new Vector3(0, 0, 0);
            }

            return lastBodyData.device_accs[(int)ponit];
        }

        static public Vector3 GetDeviceGyro(DevicePonit ponit)
        {

            if (sPlugin < 0)
            {
                return new Vector3(0, 0, 0);
            }

            return lastBodyData.device_gyros[(int)ponit];
        }

        static public Vector3 GetDeviceVelocity(DevicePonit ponit)
        {

            if (sPlugin < 0)
            {
                return new Vector3(0, 0, 0);
            }

            return lastBodyData.device_vels[(int)ponit];
        }

        static public bool GetDeviceStates(DevicePonit ponit)
        {

            if (sPlugin < 0)
            {
                return false;
            }

            if ((lastBodyData.device_states[(int)ponit] & (int)ponit) != 0) {
                return true;
            }

            return false;
        }

    }
}
