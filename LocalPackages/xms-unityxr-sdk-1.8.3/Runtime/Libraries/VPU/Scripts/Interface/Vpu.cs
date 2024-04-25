using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.Wrapper.XDeviceService.Interface
{
    public abstract class Vpu : IVpu
    {
        protected long Handle;

        public Vpu()
        {
            Handle = GetHandle();
        }

        public abstract long GetHandle();

        #region Info
        public abstract bool IsConnected();
        public abstract string GetFirmwareVersion();
        public abstract string GetHardwareVersion();
        public abstract string GetSn();
        public abstract string GetDeviceName();
        public abstract string GetModelName();
        public abstract string GetFpgaVersion();
        public abstract string GetBleVersion();
        public abstract string GetImuAlgVersion();

        public abstract int GetFpgaModelNumber();
        public abstract int GetMcuModelNumber();
        public abstract int GetBleModelNumber();
        #endregion Info

        #region Update
        public abstract int StartUpgrading(int projType, int fwType, string fwPath, XDeviceFirmwareUpgradeEventsDelegateFn_t listener, int waitMs);
        #endregion Update

        #region Tracking
        public abstract int LoadMarkerCalibrationFromFile(string file, int nofOutputElements, ref int[] outputIds);

        public abstract int ClearMarkerCalibrationSettings();

        public abstract int GetPredictTracking(int trackId, long timestamp, ref XAttrTrackingInfo trackingInfo);
        public abstract int GetTracking(int trackId, ref XAttrTrackingInfo trackingInfo);
        #endregion Tracking

        #region Controll
        public abstract int SetPowerMode(VpuPowerMode powerMode);
        public abstract int SetGlassPerspective(int value);
        public abstract int GetGlassPerspective();
        #endregion Controll
    }
}