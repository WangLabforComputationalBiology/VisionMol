using UnityEngine;
using Ximmerse.XR;
using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.Wrapper.XDeviceService.Client
{
    public class ClientController : Interface.Controller
    {
        public ClientController(int index) : base(index)
        {
        }

        public ClientController(XDevicePlugin.XControllerTypes type) : base(type)
        { 
        
        }

        public override long GetHandle()
        {
            return XDeviceClientApi.GetController(Index);
        }

        #region Info
        public override XConnectionStates GetConnectState()
        {
            return XDeviceClientApi.GetControllerConnectState(Handle);
        }

        public override bool IsPaired()
        {
            return XDeviceClientApi.IsControllerPaired(Handle);
        }

        public override XPairingStates GetPairingState()
        {
            return XDeviceClientApi.GetControllerPairingState(Handle);
        }

        public override int GetImuFps()
        {
            return XDeviceClientApi.GetControllerImuFps(Handle);
        }

        public override int GetBatteryLevel()
        {
            return XDeviceClientApi.GetControllerBatteryLevel(Handle);
        }

        public override int GetButtonStateBitmask()
        {
            return XDeviceClientApi.GetControllerButtonStateBitmask(Handle);
        }

        public override int GetTrigger()
        {
            return XDeviceClientApi.GetControllerTrigger(Handle);
        }

        public override XAttrImuInfo GetImu()
        {
            XAttrImuInfo imuInfo = new XAttrImuInfo();
            int result = XDeviceClientApi.GetControllerImu(Handle, ref imuInfo);
            if (result != 0)
            {
                Debug.LogError("Controller.GetImu: failed: " + result);
            }
            return imuInfo;
        }

        public override XAttrTrackingInfo GetFusion(long timestampNs)
        {
            XAttrTrackingInfo trackingInfo = new XAttrTrackingInfo();
            int result = XDeviceClientApi.GetControllerFusion(Handle, timestampNs, ref trackingInfo);
            if (result != 0)
            {
                Debug.LogError("Controller.GetFusion: failed: " + result);
            }
            return trackingInfo;
        }

        public override XAttrTouchPadState GetTouchpadState()
        {
            XAttrTouchPadState touchPadState = new XAttrTouchPadState();
            int result = XDeviceClientApi.GetControllerTouchpadState(Handle, ref touchPadState);
            if (result != 0)
            {
                Debug.Log("Controller.GetTouchpadState: failed: " + result);
            }
            return touchPadState;
        }

        public override XAttrControllerState GetControllerState()
        {
            XAttrControllerState controllerState = new XAttrControllerState();
            int result = XDeviceClientApi.GetControllerState(Handle, ref controllerState);
            if (result != 0)
            {
                Debug.Log("Controller.GetControllerState: failed: " + result);
            }
            return controllerState;
        }

        public override int GetImu(ref XAttrImuInfo imuInfo)
        {
            return XDeviceClientApi.GetControllerImu(Handle, ref imuInfo);
        }

        public override int GetFusion(long timestampNs, ref XAttrTrackingInfo trackingInfo)
        {
            return XDeviceClientApi.GetControllerFusion(Handle, timestampNs, ref trackingInfo);
        }

        public override int GetTouchpadState(ref XAttrTouchPadState touchPadState)
        {
            return XDeviceClientApi.GetControllerTouchpadState(Handle, ref touchPadState);
        }

        public override int GetControllerState(ref XAttrControllerState controllerState)
        {
            return XDeviceClientApi.GetControllerState(Handle, ref controllerState);
        }

        public override int GetGetControllerTracking(ref XControllerTrackingState xControllerTrackingState)
        {
            DevieType = GetDeviceType();
            return XDeviceClientApi.GetControllerTracking((int)DevieType, ref xControllerTrackingState);
        }

        public override int GetMotionSensorState(ref XAttrMotionSensorState motionSensorState)
        {
            return XDeviceClientApi.GetMotionSensorState(Handle, ref motionSensorState);
        }

        public override int GetTrackId()
        {
            return XDeviceClientApi.GetControllerTrackId(Handle);
        }

        public override XControllerTypes GetDeviceType()
        {
            return XDeviceClientApi.GetControllerDeviceType(Handle);
        }

        public override XAccessoryType GetControllerType()
        {
            return XDeviceClientApi.GetControllerType(Handle);
        }

        public override string GetDeviceAddress()
        {
            return XDeviceClientApi.GetControllerDeviceAddress(Handle);
        }

        public override string GetDeviceName()
        {
            return XDeviceClientApi.GetControllerDeviceName(Handle);
        }

        public override string GetSoftwareRevision()
        {
            return XDeviceClientApi.GetControllerSoftwareRevision(Handle);
        }

        public override string GetHardwareRevision()
        {
            return XDeviceClientApi.GetControllerHardwareRevision(Handle);
        }

        public override string GetSerialNumber()
        {
            return XDeviceClientApi.GetControllerSerialNumber(Handle);
        }

        public override string GetModelName()
        {
            return XDeviceClientApi.GetControllerModelName(Handle);
        }

        public override string GetManufactureName()
        {
            return XDeviceClientApi.GetControllerManufactureName(Handle);
        }
        #endregion Info

        #region Controll
        public override XErrorCodes Connect()
        {
            return (XErrorCodes)XDeviceClientApi.ControllerConnect(Handle);
        }

        public override XErrorCodes ConnectToType(int type, bool force)
        {
            return (XErrorCodes)XDeviceClientApi.ControllerConnectToType(Handle, type, force);
        }

        public override XErrorCodes ConnectToAddress(string mac, bool force)
        {
            return (XErrorCodes)XDeviceClientApi.ControllerConnectToAddress(Handle, mac, force);
        }

        public override XErrorCodes ConnectToRfid(int rfid, bool force)
        {
            return (XErrorCodes)XDeviceClientApi.ControllerConnectToRfid(Handle, rfid, force);
        }

        public override XErrorCodes ConnectToRfidPattern(int rfidPattern, bool force)
        {
            return (XErrorCodes)XDeviceClientApi.ControllerConnectToRfidPattern(Handle, rfidPattern, force);
        }

        public override XErrorCodes Disconnect()
        {
            return (XErrorCodes)XDeviceClientApi.ControllerDisconnect(Handle);
        }

        public override XErrorCodes ConfirmPair()
        {
            return (XErrorCodes)XDeviceClientApi.ControllerConfirmPair(Handle);
        }

        public override XErrorCodes HoldConnection(int holdTimeSec)
        {
            return (XErrorCodes)XDeviceClientApi.ControllerHoldConnection(Handle, holdTimeSec);
        }

        public override XErrorCodes Unbind()
        {
            return (XErrorCodes)XDeviceClientApi.ControllerUnbind(Handle);
        }

        public override XErrorCodes Vibrate(int strengthPercentage, int durationMs)
        {
            return (XErrorCodes)XDeviceClientApi.ControllerVibrate(Handle, strengthPercentage, durationMs);
        }

        public override XErrorCodes ChangeDataFormat(int type)
        {
            return (XErrorCodes)XDeviceClientApi.ChangeDataFormat(Handle, type);
        }

        public void RegisterXDeviceDeviceMessageDelegate(XDeviceDeviceMessageDelegate dlg)
        {
            xdevc_set_device_msg_cb(Handle, dlg);
        }
        #endregion Controll
    }
}