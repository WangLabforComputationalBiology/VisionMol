using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using Ximmerse.Wrapper.XDeviceService.Client;
using Ximmerse.XR.Tag;

namespace Ximmerse.XR.InputSystems
{
    public class MarkerControllerInputSystem : MonoBehaviour
    {
        public static MarkerControllerInput ximmerseMarkerControllerInputDevice
        {
            get; private set;
        }

        public static void ResetControllerRotation(int controllerIndex,Quaternion quaternion)
        {
            if (controllerIndex==0)
            {
                ximmerseMarkerControllerInputDevice.currentLeftLerp = quaternion * Quaternion.Inverse(ximmerseMarkerControllerInputDevice.LeftImuRot);
            }
            else if (controllerIndex==1)
            {
                ximmerseMarkerControllerInputDevice.currentRightLerp = quaternion * Quaternion.Inverse(ximmerseMarkerControllerInputDevice.RightImuRot);
            }
        }

        public void EnabelMarkerController()
        {
            if (ximmerseMarkerControllerInputDevice == null)
            {
#if !UNITY_EDITOR
                XDeviceClientWrapper.Init();
#endif
                //Adds a virtural input device for gesture input:
                MarkerControllerInput MarkerControllerInputDevice = (MarkerControllerInput)InputSystem.AddDevice(new InputDeviceDescription
                {
                    interfaceName = "MarkerController",
                });
                InputSystem.EnableDevice(MarkerControllerInputDevice);
                XDeviceClientWrapper.onControllerConnectState += MarkerControllerInputDevice.RefreshMarkerControllerIndex;
                //MarkerControllerInputDevice.xr = FindObjectOfType<XROrigin>();
                ximmerseMarkerControllerInputDevice = MarkerControllerInputDevice;
                MarkerControllerInputDevice.RefreshMarkerControllerIndex();
            }
        }
        static bool IsHeadsetDeviceLayoutRegistered = false;
        private static void RegisterXRCameraPointLayout()
        {
            InputSystem.RegisterLayout<MarkerControllerInput>(matches: new InputDeviceMatcher()
                .WithInterface("MarkerController"));
            Debug.LogFormat("MarkerController input device layout has been registered.");
            //
            IsHeadsetDeviceLayoutRegistered = true;
        }

        private void Start()
        {
            RegisterXRCameraPointLayout();
            EnabelMarkerController();
        }

        private void OnDestroy()
        {
            XDeviceClientWrapper.onControllerConnectState -= ximmerseMarkerControllerInputDevice.RefreshMarkerControllerIndex;
            if (ximmerseMarkerControllerInputDevice != null)
            {
                InputSystem.RemoveDevice(ximmerseMarkerControllerInputDevice);
            }
        }
    }

}
