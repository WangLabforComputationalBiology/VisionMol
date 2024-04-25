using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Ximmerse.XR
{
    public class XimSystemApi
    {
        [DllImport("ximUtils")]
        public static extern IntPtr XimNative_GetSN();


        public static string XimGetSN()
        {
            int size = Marshal.SizeOf(typeof(char)) * 13;
            IntPtr strPtr = Marshal.AllocHGlobal(size);
            strPtr = XimNative_GetSN();
            string sn = Marshal.PtrToStringAnsi(strPtr);
            return sn;
        }
    }
}

