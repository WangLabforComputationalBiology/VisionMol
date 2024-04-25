using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Ximmerse.XR;
using static Ximmerse.XR.XDevicePlugin;
using NativeHandle = System.Int64;

namespace Ximmerse.Wrapper.XDeviceService.Client
{
    public static class XDeviceClientWrapper
    {
        public delegate void OnControllerConnection();
        public static OnControllerConnection onControllerConnection;
        public static OnControllerConnection onControllerConnectState;
        public static OnControllerConnection onControllerDisconnect;
        public static OnControllerConnection onControllerBatteryStateChange;
        private static bool _isInit = false;
        public static bool isPairing;
        public static XControllerTypes pairingType;

        private static Dictionary<int, ClientController> clientControllerDic = new Dictionary<int, ClientController>();

        private static List<ClientController> clientControllers = new List<ClientController>();

        public static Dictionary<int, ClientController> ClientControllerDic
        {
            get => clientControllerDic;
        }
        public static bool IsInit
        {
            get { return _isInit; }
        }

        public static int Init()
        {
            if (_isInit)
            {
                Debug.Log("inited");
                return 0;
            }

            int ret = XDeviceClientApi.Init();
            Debug.Log("init: " + ret);

            if (ret == 0)
            {
                _isInit = true;
                int numberOfControllers = XDeviceClientApi.GetNumberOfControllers();
                if (numberOfControllers > 0)
                {
                    for (int i = 0; i < numberOfControllers; i++)
                    {
                        ClientController clientController = new ClientController(i);
                        clientControllerDic.Add(i, clientController); 
                        clientControllers.Add(clientController);
                    }
                }

            }
            else
            {
                Debug.LogError("init failed: " + ret);
            }

            XDevicePlugin.RegisterXEventCallbackDelegate(OnXEvent);
            return ret;
        }

        public static void Connect(int index, XControllerTypes xControllerTypes)
        {
            clientControllerDic[index].ConnectToType((int)xControllerTypes, true);
        }

        public static void DisConnect(int index)
        {
            clientControllerDic[index].Unbind();
            clientControllerDic[index].Disconnect();
        }


        public static int Exit()
        {
            if (!_isInit)
            {
                Debug.Log("Not inited");
                return 0;
            }

            Debug.Log("exit");
            XDeviceClientApi.StopEventCallback();
            XDeviceClientApi.Exit();
            _isInit = false;
            return 0;
        }

        [MonoPInvokeCallback(typeof(XDevicePlugin.XDeviceClientEventDelegate))]
        private static void OnXEvent(XDevicePlugin.XEvent evt, IntPtr handle, IntPtr ud)
        {
            Debug.Log($"[Ximmerse XR] OnXEvent: [{evt}] [{handle}] [{ud}]");
            switch (evt)
            {
                case XDevicePlugin.XEvent.kXEvtConnectionStateChange:

                    foreach (var controller in clientControllers)
                    {
                        var nativeExHandle = controller.GetHandle();
                        if (nativeExHandle == (long)handle)
                        {
                            XDevicePlugin.XConnectionStates state = controller.GetConnectState();
                            bool isPaired = controller.IsPaired();
                            Debug.Log($"[Ximmerse XR] OnXEvent: kXEvtControllerStateChange [{evt}] [{handle}] [{state}] [{isPaired}]");
                            if (state == XDevicePlugin.XConnectionStates.kXConnSt_Connected)
                            {
                                if (!isPaired&& isPairing)
                                {
                                    controller.ConfirmPair();
                                    onControllerConnection?.Invoke();
                                }
                            }
                        }
                    }
                    onControllerConnectState?.Invoke();
                    break;


                case XDevicePlugin.XEvent.kXEvtDeviceBatteryStateChange:
                    Debug.Log($"[Ximmerse XR] kXEvtDeviceBatteryStateChange: [{evt}] [{handle}] [{ud}]");

                    onControllerBatteryStateChange?.Invoke();

                    break;


                //case XDevicePlugin.XEvent.kXEvtControllerStateChange:
                //    foreach (var controller in clientControllers)
                //    {
                //        if (controller.GetHandle() == (long)handle)
                //        {
                //            bool isPaired = controller.IsPaired();
                //            bool isConnected = controller.IsPaired();
                //            Debug.Log($"[RhinoX] OnXEvent: kXEvtControllerStateChange [{evt}] [{handle}] [{isPaired}]");
                //            if (isConnected)
                //            {
                //                if (isPaired)
                //                {
                //                    controller.WaitForPair = false;
                //                    OnControllerPairingCb(handle, XDevicePlugin.XControllerPairingEvents.kPairingEventPaired, 0);
                //                }
                //                else
                //                {
                //                    OnControllerPairingCb(handle, XDevicePlugin.XControllerPairingEvents.kPairingEventUnpaired, 0);
                //                }
                //            }
                //        }
                //    }
                //    break;
                default:
                    Debug.Log($"Unhandled event [{evt}] [{handle}] [{ud}]");
                    break;
            }
        }
    }
}