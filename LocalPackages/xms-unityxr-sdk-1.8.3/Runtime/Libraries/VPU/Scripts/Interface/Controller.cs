using UnityEngine;
using Ximmerse.Wrapper.XDeviceService.Client;
using Ximmerse.XR;
using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.Wrapper.XDeviceService.Interface
{
    public abstract class Controller : IController
    {
  
        protected int Index;
        protected XControllerTypes DevieType;
        protected long handleTemp;
        protected long Handle
        {
            get {
                if (Index == -1) {
                    handleTemp = NativePluginApi.Unity_getControllerHandleByType((int)DevieType);
                }
                return handleTemp; 
            }
            set { handleTemp = value; }
            
        }
        public Controller(int index)
        {
            Index = index;
            DevieType = XControllerTypes.kControllerType_Unknow;
            Handle = GetHandle();
        }

        public Controller(XControllerTypes devieType)
        {
            DevieType = devieType;
            Index = -1;
        }

        public int GetIndex()
        {
            return Index;
        }

        public abstract long GetHandle();

        #region Info
        public abstract XConnectionStates GetConnectState();

        public abstract bool IsPaired();

        public abstract XPairingStates GetPairingState();

        public abstract int GetImuFps();
        public abstract int GetBatteryLevel();
        public abstract int GetButtonStateBitmask();
        public abstract int GetTrigger();
        public abstract XAttrImuInfo GetImu();
        public abstract XAttrTrackingInfo GetFusion(long timestampNs);
        public abstract XAttrTouchPadState GetTouchpadState();
        public abstract XAttrControllerState GetControllerState();
        public abstract int GetImu(ref XAttrImuInfo imuInfo);
        public abstract int GetFusion(long timestampNs, ref XAttrTrackingInfo trackingInfo);
        public abstract int GetTouchpadState(ref XAttrTouchPadState touchPadState);
        public abstract int GetControllerState(ref XAttrControllerState controllerState);
        public abstract int GetGetControllerTracking(ref XControllerTrackingState xControllerTrackingState);
        public abstract int GetMotionSensorState(ref XAttrMotionSensorState xAttrMotionSensorState);
        public abstract int GetTrackId();
        public abstract XControllerTypes GetDeviceType();
        public abstract XAccessoryType GetControllerType();
        public abstract string GetDeviceAddress();

        public abstract string GetDeviceName();
        public abstract string GetSoftwareRevision();
        public abstract string GetHardwareRevision();
        public abstract string GetSerialNumber();
        public abstract string GetModelName();
        public abstract string GetManufactureName();
        #endregion Info

        #region Controll
        public abstract XErrorCodes Connect();
        public abstract XErrorCodes ConnectToType(int type, bool force);
        public abstract XErrorCodes ConnectToAddress(string mac, bool force);
        public abstract XErrorCodes ConnectToRfid(int rfid, bool force);
        public abstract XErrorCodes ConnectToRfidPattern(int rfidPattern, bool force);
        public abstract XErrorCodes Disconnect();
        public abstract XErrorCodes ConfirmPair();
        public abstract XErrorCodes HoldConnection(int holdTimeSec);
        public abstract XErrorCodes Unbind();
        public abstract XErrorCodes Vibrate(int strengthPercentage, int durationMs);
        public abstract XErrorCodes ChangeDataFormat(int type);
        #endregion Controll
    }
}