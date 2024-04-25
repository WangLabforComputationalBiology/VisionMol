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
    public enum MotionControllerType
    {
        kTypeAllController = 0,
        kTypeLeftController,
        kTypeRightController,
        kTypeHubController,
        kTypeNeckController,
    };
    public enum MotionAlgActionType
    {
        kActVioUpdate = 0,
        kActImuUpdate,
        kActFusionUpdate,
        kActResetChestYaw = 3,  // void
        kActSetLogLevel,    // int -1/0/1/2: off/error/info(default)/debug
        kActSetAlgMode,     // int 0, 1

        kActXDeviceEventUpdate,

        kActClearShock = 10,    // void     清除电击设置
        kActSetShockTarget,     // int      设置触发电击的控制器(保留，目前写死 kTypeNeckController)
        kActSetShockPosition,   // int      设置电击点位，参考 xdevice_client.h 中的 XShockPosition
        kActSetShockStrength,   // int      设置电击强度
        kActSetShockDuration,   // int      设置电击时长, 单位 ms
        kActDoShock,            // void     执行电击

        kActClearVibrate,       // void     清除震动设置
        kActSetVibrateTarget,   // int      设置触发震动的控制器
        kActSetVibrateStrength, // int      设置震动强度
        kActSetVibrateMode,     // int      设置震动模式
        kActSetVibrateDuration, // int      设置震动时长, 单位 ms
        kActDoVibrate,          // void     执行震动
    };
    public enum MotionAlgEventType
    {
        kEvtMotionResult,
    };


    public enum InitError
    {
        kErrMotionDataFailed,
        kErrMemoryMotionVioFailed,
        kErrMemoryMotionImuFailed,
        kErrXDeviceEventFailed,
        kErrMemoryXDeviceEventFailed,
    };


    public delegate void XRuntimeDataCallbackFunc(System.Int64 pluginHandle, int what, int size, IntPtr userData);

    public class XRuntimeClient
    {
        public const string pluginName = "xruntime_client";

        public const string Tag = "XRuntimeClient";


        #region api wrapper
        static bool isInit = false;
        static public bool Init() {

            if (!isInit) {

                int ret = xruntime_client_init();

                if (ret == 0)
                {

                    isInit = true;
                    Debug.LogFormat("{0} init successfully {1}", Tag, ret);
                }
                else {
                    isInit = false;

                    Debug.LogErrorFormat("{0} init error {1}", Tag, ret);
                }
            }

            return isInit;
        }
        static public void Exit()
        {

            if (isInit)
            {
                isInit = false;
                int ret = xruntime_client_deinit();

                if (ret == 0)
                {

                    Debug.LogFormat("{0} exit successfully {1}", Tag, ret);
                }
                else
                {
                     Debug.LogErrorFormat("{0} exit error {1}", Tag, ret);
                }
            }

        }


        #endregion api wrapper



        #region native api 
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xruntime_client_init();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xruntime_client_deinit();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern  IntPtr xruntime_client_get_memory(string key, IntPtr mem_size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeHandle xruntime_client_get_plugin(string name);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_get_version(NativeHandle plugin_handle);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_start(NativeHandle plugin_handle);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_stop(NativeHandle plugin_handle);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr xplugin_get_output_shm(NativeHandle plugin_handle, string shm_key, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int  xplugin_get_ringbuf_data(NativeHandle plugin_handle, string shm_key, int offset, int count,  System.IntPtr ptr);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_subscribe_data(NativeHandle plugin_handle, int what, XRuntimeDataCallbackFunc callback, IntPtr user_data);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_unsubscribe_data(NativeHandle plugin_handle, int what);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]


        public static extern int xplugin_do_bool(NativeHandle plugin_handle, int what, bool arg);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_do_int(NativeHandle plugin_handle, int what, int arg);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_do_long(NativeHandle plugin_handle, int what, int arg);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_do_float(NativeHandle plugin_handle, int what, float arg);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_do_double(NativeHandle plugin_handle, int what, double arg);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_do_string(NativeHandle plugin_handle, int what, System.IntPtr arg);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_do_void(NativeHandle plugin_handle, int what);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_subscribe_all_data(NativeHandle plugin_handle, XRuntimeDataCallbackFunc callback, IntPtr user_data);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int xplugin_unsubscribe_all_data(NativeHandle plugin_handle);

        #endregion native api 



    }
}
