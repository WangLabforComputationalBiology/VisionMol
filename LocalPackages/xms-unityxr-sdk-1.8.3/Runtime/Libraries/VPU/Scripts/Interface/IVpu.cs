using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.Wrapper.XDeviceService.Interface
{
    public interface IVpu
    {
        long GetHandle();

        #region Info
        bool IsConnected();
        string GetFirmwareVersion();
        string GetHardwareVersion();
        string GetSn();
        string GetDeviceName();
        string GetModelName();
        string GetFpgaVersion();
        string GetBleVersion();
        string GetImuAlgVersion();

        int GetFpgaModelNumber();
        int GetMcuModelNumber();
        int GetBleModelNumber();
        #endregion Info

        #region Update
        int StartUpgrading(int projType, int fwType, string fwPath,
            XDeviceFirmwareUpgradeEventsDelegateFn_t listener,
            int waitMs);
        #endregion Update

        #region Tracking
        int LoadMarkerCalibrationFromFile(string file, int nofOutputElements, ref int[] outputIds);
        int ClearMarkerCalibrationSettings();

        int GetPredictTracking(int trackId, long timestamp, ref XAttrTrackingInfo trackingInfo);
        int GetTracking(int trackId, ref XAttrTrackingInfo trackingInfo);
        #endregion Tracking

        #region Controll
        int SetPowerMode(VpuPowerMode powerMode);
        int SetGlassPerspective(int value);
        int GetGlassPerspective();
        #endregion Controll
    }
}