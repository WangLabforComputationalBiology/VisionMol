using Ximmerse.Wrapper.XDeviceService.Client;
using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.Wrapper.XDeviceService.Interface
{
    public interface IController
    {
        long GetHandle();

        int GetIndex();

        #region Info
        XConnectionStates GetConnectState();

        bool IsPaired();

        XPairingStates GetPairingState();

        int GetImuFps();
        int GetBatteryLevel();
        int GetButtonStateBitmask();
        int GetTrigger();
        XAttrImuInfo GetImu();
        XAttrTrackingInfo GetFusion(long timestampNs);
        XAttrTouchPadState GetTouchpadState();
        XAttrControllerState GetControllerState();
        int GetImu(ref XAttrImuInfo imuInfo);
        int GetFusion(long timestampNs, ref XAttrTrackingInfo trackingInfo);
        int GetTouchpadState(ref XAttrTouchPadState touchPadState);
        int GetControllerState(ref XAttrControllerState controllerState);
        int GetGetControllerTracking(ref XControllerTrackingState xControllerTrackingState);
        int GetMotionSensorState(ref XAttrMotionSensorState xAttrMotionSensorState);
        int GetTrackId();
        XControllerTypes GetDeviceType();
        XAccessoryType GetControllerType();
        string GetDeviceAddress();

        string GetDeviceName();
        string GetSoftwareRevision();
        string GetHardwareRevision();
        string GetSerialNumber();
        string GetModelName();
        string GetManufactureName();

        #endregion Info

        #region Controll
        XErrorCodes Connect();
        XErrorCodes ConnectToType(int type, bool force);
        XErrorCodes ConnectToAddress(string mac, bool force);
        XErrorCodes ConnectToRfid(int rfid, bool force);
        XErrorCodes ConnectToRfidPattern(int rfidPattern, bool force);
        XErrorCodes Disconnect();
        XErrorCodes ConfirmPair();
        XErrorCodes HoldConnection(int holdTimeSec);
        XErrorCodes Unbind();
        XErrorCodes Vibrate(int strengthPercentage, int durationMs);
        XErrorCodes ChangeDataFormat(int type);
        #endregion Controll
    }
}