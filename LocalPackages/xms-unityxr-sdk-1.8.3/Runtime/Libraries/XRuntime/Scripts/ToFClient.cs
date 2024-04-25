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

namespace Ximmerse.XR
{
    public struct ToFFrame
    {
        public UInt64 ts;
        public byte width;
        public byte height;
        public byte stride;
        public byte pixel_format;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 29)]
        public UInt32[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 328 * 240 * 2)] 
        public byte[] data; // pos = 128
    };




    public class ToFClient
    {
        public const string Tag = "ToFClient";

        static System.Int64 sPlugin = 0;
        static string PluginName = "xplugin_tofalg";
        static string SharedMemName = "TOF_DEPTH_DATA_MEM";

        static bool isInited = false;

        static System.IntPtr shared_mem_ptr;

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

            shared_mem_ptr = Marshal.AllocHGlobal(Marshal.SizeOf<ToFFrame>());

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
            Marshal.FreeHGlobal(shared_mem_ptr);
        }


        static public int SetCmd(int type, IntPtr param)
        {
            return 0;
        }

        static public bool GetEvent(ref ToFFrame frame)
        {

            if (!isInited) {
                return false;
            }
            //ToFFrame toFFrame;
            int ret = XRuntimeClient.xplugin_get_ringbuf_data(sPlugin, SharedMemName, 0, 1, shared_mem_ptr);

            if (ret < 0)
            {
                //Debug.LogError(Tag+ ":GetEvent ret="+ ret);
                return false;
            }

            frame = Marshal.PtrToStructure<ToFFrame>(shared_mem_ptr);


            //Debug.Log("tof ts=" + frame.ts + ",width=" + frame.width + ",height=" + frame.height + ",stride="+frame.stride+",size=" + Marshal.SizeOf<ToFFrame>());

            //String aaa = "data:";
            //for (int i = 0; i < 50; i++)
            //{
            //    aaa += String.Format("{0},", frame.data[328 * 120 + 120 + i]);
            //}
            //Debug.Log(aaa);

            return true;
        }
    }
}
