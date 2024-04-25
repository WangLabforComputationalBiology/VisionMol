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

    public struct RingBuf
    {
        public volatile int index;
        public volatile int nof_el;
        public volatile int mask;
        public volatile int el_size;
        public System.IntPtr buf_ptr;

        public System.IntPtr GetCurrent() { 
            return buf_ptr + index * el_size;
        }
    }
}
