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

using NativeHandle = System.IntPtr;
using NativeExHandle = System.IntPtr;
using AOT;

namespace Ximmerse.XR
{
    public partial class XDevicePlugin
    {
        public const string xopc_pluginName = "xopc_client";
        #region Native
        [DllImport(xopc_pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xopc_on_mission_begin();

        [DllImport(xopc_pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int xopc_on_mission_end();

        private delegate int opc_message_handle_callback_t(IntPtr message);
        /// Set OPCenter message handle callback.
        [DllImport(xopc_pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void xopc_set_message_callback(opc_message_handle_callback_t cb);

        #endregion

        private class OpcMessage
        {
            public string type;
        }
        [MonoPInvokeCallback(typeof(opc_message_handle_callback_t))]
        static private int OpcMessageHandleCallback(IntPtr message)
        {

            int ret = 0;
            if (OpcEventsDelegates == null)
                return ret;

            var msgJson = Marshal.PtrToStringAnsi(message);
            var msg = JsonUtility.FromJson<OpcMessage>(msgJson);
            if (msg.type != null)
            {

                if (msg.type.Equals("begin_mission"))
                {
                    if (OpcEventsDelegates.OnRequestBeginMission != null)
                    {
                        OpcEventsDelegates.OnRequestBeginMission(msgJson);
                    }
                }
                else if (msg.type.Equals("end_mission"))
                {
                    if (OpcEventsDelegates.OnRequestEndMission != null)
                    {
                        OpcEventsDelegates.OnRequestEndMission(msgJson);
                    }
                }
                else if (msg.type.Equals("exit_app"))
                {
                    if (OpcEventsDelegates.OnRequestExitApp != null)
                    {
                        OpcEventsDelegates.OnRequestExitApp(msgJson);
                    }
                }
                else
                {
                    ret = -1;
                }
            }
            return ret;
        }
        /// Delegates for handling difference messages from OP Center
        public class OpCenterMessagesDelegates
        {

            /// Invoked with mission key string argument while receiving begin mission message from OP Center.
            public Action<string> OnRequestBeginMission;
            /// Invoked with mission key string argument while receiving begin mission message from OP Center.
            public Action<string> OnRequestEndMission;
            /// Invoked with string argument while receiving begin mission message from OP Center.
            public Action<string> OnRequestExitApp;
        }

        static OpCenterMessagesDelegates OpcEventsDelegates;
        /// Set delegates to handle OpCenter messages
        public static void SetOpCenterMessageDelegates(OpCenterMessagesDelegates delegates)
        {
            OpcEventsDelegates = delegates;
            if (delegates != null)
                xopc_set_message_callback(OpcMessageHandleCallback);
            else
                xopc_set_message_callback(null);
        }
        //public void ExitOpCenter()
        //{

        //}

        /// \brief developer need to call this method before beginning the game mission, 
        ///     then start mission or not according the return value;
        ///     NOTICE: this method may block the thread.
        /// \returns Return negative value for error, otherwise return seconds for how long time allowed to play;
        public static int OnMissionBegin()
        {
            return xopc_on_mission_begin();
        }
        /// \brief Developer need to call this method before ending the game mission.
        /// \returns Return negative value for error.
        public static int OnMissionEnd()
        {
            return xopc_on_mission_end();
        }


        //public static void OnGameStateChange()
        //{
        //    return 0;
        //}


        static AndroidJavaClass OpServiceApiClass = null;

        static bool isOpcInit = false;
        static public bool OpcInit(String gameId) {
            if (isOpcInit) {
                Debug.LogError("Opc already Init");
                return true;
            }

            Debug.Log("Opc Init...");
            OpServiceApiClass = new AndroidJavaClass("com.ximmerse.opcenter.OpServiceApi");
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject obj = OpServiceApiClass.CallStatic<AndroidJavaObject>("createDefault", currentActivity, gameId);

            if (obj != null && OpServiceApiClass!=null)
            {

                isOpcInit = true;
            }
            else {
                Debug.LogError("Opc Init error...");
                isOpcInit = false;
            }
            
            return isOpcInit;
          
        }

        static public void OpcExit() {
            Debug.Log("Opc Exit...");
            if (isOpcInit) {
                OpServiceApiClass.CallStatic("releaseDefault");
            }

            isOpcInit = false;
        }
        /**
        * Get OPService version code
        */
        public static int GetOpServiceVersionCode()
        {
            if (isOpcInit) {
                return OpServiceApiClass.CallStatic<int>("getOpServiceVersionCode");
            }

            Debug.LogError("OpService not inited");
            return -1;
        }

        /**
        * Is Remote OPConsole server connected.
        */
        public static bool IsOpConsoleServerConnected()
        {
            if (isOpcInit)
            {
                return OpServiceApiClass.CallStatic<bool>("isOpConsoleServerConnected");
            }

            Debug.LogError("OpService not inited");
            return false;
        }
        /**
        *
        * \returns Return OPConsole server IP address, return null or empty if not exists.
        */
        public static string GetOpConsoleServerAddress()
        {
            if (isOpcInit)
            {
                return OpServiceApiClass.CallStatic<string>("getOpConsoleServerAddress");
            }

            Debug.LogError("OpService not inited");
            return null;

        }

        //    String[] GetGameServerAddresses(String gameId);
        //    String[] GetGameServerUUIDs(String gameId);

        /**
        * \returns return true if bound game server exists
        */
        public static bool HasBoundGameServer()
        {
            if (isOpcInit)
            {
                return OpServiceApiClass.CallStatic<bool>("hasBoundGameServer");
            }

            Debug.LogError("OpService not inited");
            return false;
        }
        /**
         * Bound game server is online
        * \returns return true if bound game server exists and online, otherwise return false.
        */
        public static bool IsBoundGameServerOnline()
        {
            if (isOpcInit)
            {
                return OpServiceApiClass.CallStatic<bool>("isBoundGameServerOnline");
            }

            Debug.LogError("OpService not inited");
            return false;
        }
        /**
        * Get bound game server address(IP or URL)
        * \returns return null or empty if not exists.
        */
        public static string GetBoundGameServerAddress()
        {
            if (isOpcInit)
            {
                return OpServiceApiClass.CallStatic<string>("getBoundGameServerAddress");
            }

            Debug.LogError("OpService not inited");
            return null;

        }
        /**
        * Get bound game server GameID
        * \returns return null or empty if not exists.
        */
        public static string GetBoundGameServerGameID()
        {
            if (isOpcInit)
            {
                return OpServiceApiClass.CallStatic<string>("getBoundGameServerGameID");
            }

            Debug.LogError("OpService not inited");
            return null;
        }
        /**
        * Get bound game server UUID
        * \returns return null or empty if not exists.
        */
        public static string GetBoundGameServerUUID()
        {
            if (isOpcInit)
            {
                return OpServiceApiClass.CallStatic<string>("getBoundGameServerUUID");
            }

            Debug.LogError("OpService not inited");
            return null;
        }
    }
}
