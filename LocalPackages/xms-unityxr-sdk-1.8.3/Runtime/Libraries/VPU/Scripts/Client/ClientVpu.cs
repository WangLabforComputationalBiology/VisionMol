using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.Wrapper.XDeviceService.Client
{
    public class ClientVpu : Interface.Vpu
    {
        public override long GetHandle()
        {
            return XDeviceClientApi.GetVpu();
        }

        #region Info
        public override bool IsConnected()
        {
            return XDeviceClientApi.IsVpuConnected(Handle);
        }

        public override string GetBleVersion()
        {
            return XDeviceClientApi.GetVpuBleVersion(Handle);
        }

        public override int GetBleModelNumber()
        {
            return XDeviceClientApi.GetVpuBleModelNumber(Handle);
        }

        public override string GetDeviceName()
        {
            return XDeviceClientApi.GetVpuDeviceName(Handle);
        }

        public override string GetFirmwareVersion()
        {
            return XDeviceClientApi.GetVpuFirmwareVersion(Handle);
        }

        public override string GetFpgaVersion()
        {
            return XDeviceClientApi.GetVpuFpgaVersion(Handle);
        }

        public override int GetFpgaModelNumber()
        {
            return XDeviceClientApi.GetVpuFpgaModelNumber(Handle);
        }

        public override string GetHardwareVersion()
        {
            return XDeviceClientApi.GetVpuHardwareVersion(Handle);
        }

        public override string GetImuAlgVersion()
        {
            return XDeviceClientApi.GetVpuImuAlgVersion(Handle);
        }

        public override string GetModelName()
        {
            return XDeviceClientApi.GetVpuModelName(Handle);
        }

        public override int GetMcuModelNumber()
        {
            return XDeviceClientApi.GetVpuMcuModelNumber(Handle);
        }

        public override string GetSn()
        {
            return XDeviceClientApi.GetVpuSn(Handle);
        }
        #endregion Info

        #region Update
        public override int StartUpgrading(int projType, int fwType, string fwPath,
            XDeviceFirmwareUpgradeEventsDelegateFn_t listener,
            int waitMs)
        {
            return XDeviceClientApi.StartUpgradingVpuFirmware(Handle, projType, fwType, fwPath, listener, waitMs);
        }
        #endregion Update

        #region Tracking
        public override int LoadMarkerCalibrationFromFile(string file, int nofOutputElements, ref int[] outputIds)
        {
            return XDeviceClientApi.VpuLoadMarkerCalibrationFromFile(Handle, file, nofOutputElements, ref outputIds);
        }

        public override int ClearMarkerCalibrationSettings()
        {
            return XDeviceClientApi.VpuClearMarkerCalibrationSettings(Handle);
        }

        public override int GetPredictTracking(int trackId, long timestamp, ref XAttrTrackingInfo trackingInfo)
        {
            return XDeviceClientApi.VpuGetPredictTracking(Handle, trackId, timestamp, ref trackingInfo);
        }
        public override int GetTracking(int trackId, ref XAttrTrackingInfo trackingInfo)
        {
            return XDeviceClientApi.VpuGetTracking(Handle, trackId, ref trackingInfo);
        }
        #endregion Tracking

        #region Controll
        public override int SetPowerMode(VpuPowerMode powerMode)
        {
            return XDeviceClientApi.VpuSetPowerMode(Handle, powerMode);
        }

        public override int SetGlassPerspective(int value)
        {
            return XDeviceClientApi.VpuSetGlassPerspectivePercentage(Handle, value);
        }

        public override int GetGlassPerspective()
        {
            return XDeviceClientApi.VpuGetGlassPerspectivePercentage(Handle);
        }
        #endregion Controll
    }
}