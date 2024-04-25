using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Ximmerse.XR;
using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.Wrapper.XDeviceService.Client
{
    public struct XControllerTrackingState
    {
        public long predTimestampNano;
        public int index;
        public long timestamp;
        public int state;
        public float posX;
        public float posY;
        public float posZ;
        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;
    }

    static class XDeviceClientApi
    {
        #region Base
        internal static int Init()
        {
            return xdevc_init();
        }

        internal static int Exit()
        {
            xdevc_exit();
            return 0;
        }

        internal static long GetXContext()
        {
            return xdevc_get_xcontext();
        }

        internal static long GetVpu()
        {
            return xdevc_get_vpu();
        }

        internal static long GetController(int index)
        {
            return xdevc_get_controller(index);
        }

        internal static int GetNumberOfControllers()
        {
            return (int)xdevc_get_number_of_controllers();
        }

        internal static void StartEventCallback()
        {

        }

        internal static void StopEventCallback()
        {

        }
        #endregion Base

        #region Context
        internal static string GetClientVersion()
        {
            return ConvertIntPtrToString(
                xdevc_get_client_version());
        }

        internal static int GetClientBuildNumber()
        {
            return xdevc_get_client_build_number();
        }

        internal static string GetClientBuildInformation()
        {
            return ConvertIntPtrToString(
                xdevc_get_client_build_information());
        }

        internal static int GetClientAlgVersion(AlgType algType)
        {
            return xdevc_get_client_alg_version((int)algType);
        }

        internal static string GetServerVersion()
        {
            return ConvertIntPtrToString(
                xdevc_get_server_version());
        }

        internal static int GetServerBuildNumber()
        {
            return xdevc_get_server_build_number();
        }

        internal static string GetServerBuildInformation()
        {
            return ConvertIntPtrToString(
                xdevc_get_server_build_information());
        }

        internal static int GetServerAlgVersion(AlgType algType)
        {
            return (int)xdevc_get_server_alg_version((int)algType);
        }
        #endregion Context

        #region Vpu
        internal static bool IsVpuConnected(long vpuHandle)
        {
            return xdevc_vpu_is_connected(vpuHandle);
        }

        internal static int UnpairAndDisconnectAllControllers(long vpuHandle)
        {
            return xdevc_vpu_unpair_and_disconnect_all_controllers(vpuHandle);
        }

        internal static string GetVpuFirmwareVersion(long vpuHandle)
        {
            return ConvertIntPtrToString(
                xdevc_vpu_get_firmware_version(vpuHandle));
        }

        internal static string GetVpuHardwareVersion(long vpuHandle)
        {
            return ConvertIntPtrToString(
                xdevc_vpu_get_hardware_version(vpuHandle));
        }

        internal static string GetVpuSn(long vpuHandle)
        {
            return ConvertIntPtrToString(
                xdevc_vpu_get_sn(vpuHandle));
        }

        internal static string GetVpuDeviceName(long vpuHandle)
        {
            return ConvertIntPtrToString(
                xdevc_vpu_get_device_name(vpuHandle));
        }

        internal static string GetVpuModelName(long vpuHandle)
        {
            return ConvertIntPtrToString(
                xdevc_vpu_get_model_name(vpuHandle));
        }

        internal static string GetVpuFpgaVersion(long vpuHandle)
        {
            return ConvertIntPtrToString(
                xdevc_vpu_get_fpga_version(vpuHandle));
        }

        internal static string GetVpuBleVersion(long vpuHandle)
        {
            return ConvertIntPtrToString(
                xdevc_vpu_get_ble_firmware_version(vpuHandle));
        }

        internal static string GetVpuImuAlgVersion(long vpuHandle)
        {
            return ConvertIntPtrToString(
                xdevc_vpu_get_imu_alg_version(vpuHandle));
        }

        internal static int GetVpuFpgaModelNumber(long vpuHandle)
        {
            return xdevc_vpu_get_fpga_model_number(vpuHandle);
        }

        internal static int GetVpuMcuModelNumber(long vpuHandle)
        {
            return xdevc_vpu_get_mcu_model_number(vpuHandle);
        }

        internal static int GetVpuBleModelNumber(long vpuHandle)
        {
            return xdevc_vpu_get_ble_model_number(vpuHandle);
        }

        internal static int StartUpgradingVpuFirmware(
            long vpuHandle, int projectType, int fwType, string fwPath,
            XDeviceFirmwareUpgradeEventsDelegateFn_t eventDelegate,
            int waitMs)
        {
            return xdevc_vpu_start_upgrading(
                vpuHandle, projectType, fwType, fwPath, eventDelegate, IntPtr.Zero, waitMs);
        }

        internal static int VpuLoadMarkerCalibrationFromFile(long vpuHandle, string file, int nofOutputElements, ref int[] outputIds)
        {
            return xdevc_vpu_load_marker_calibration_from_file(vpuHandle, file, nofOutputElements, ref outputIds);
        }

        internal static int VpuClearMarkerCalibrationSettings(long vpuHandle)
        {
            return xdevc_vpu_clear_marker_calibration_settings(vpuHandle);
        }

        internal static int VpuGetPredictTracking(long vpuHandle, int trackId, long timestamp, ref XAttrTrackingInfo trackingInfo)
        {
            int result = xdevc_vpu_get_predict_tracking(vpuHandle, trackId, timestamp, ref trackingInfo);
            return result;
        }

        internal static int VpuGetTracking(long vpuHandle, int trackId, ref XAttrTrackingInfo trackingInfo)
        {
            int result = xdevc_vpu_get_tracking(vpuHandle, trackId, ref trackingInfo);
            return result;
        }

        internal static int VpuSetPowerMode(long vpuHandle, VpuPowerMode powerMode)
        {
            return xdevc_vpu_set_power_mode(vpuHandle, (int)powerMode);
        }

        internal static int VpuSetGlassPerspectivePercentage(long vpuHandle, int val)
        {
            if (val >= 0 || val <= 100)
            {
                return xdevc_vpu_set_glass_perspective(vpuHandle, (int)XGlassConfigType.GlassPerspectivePercentage, val);
            }
            else
            {
                return -1;
            }
        }

        internal static int VpuGetGlassPerspectivePercentage(long vpuHandle)
        {
            int val = 0;
            XGlassConfigType xGlassConfigType = XGlassConfigType.GlassPerspectivePercentage;
            int glass = xdevc_vpu_get_glass_perspective(vpuHandle, ref xGlassConfigType, ref val);
            return glass;

        }

        #endregion Vpu

        #region Controller
        internal static bool CheckHandleValid(long handle) {

            if (handle <= 0 || handle == 0xffffffff) {
                return false;
            }

            return true;
        }

        internal static XConnectionStates GetControllerConnectState(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle)) {
                Debug.LogError("Error controllerHandle "+ controllerHandle);
                return XConnectionStates.kXConnSt_Disconnected;
            }

            return (XConnectionStates)xdevc_ctrl_get_connect_state(controllerHandle);
        }

        internal static bool IsControllerPaired(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                Debug.LogError("Error controllerHandle " + controllerHandle);
                return false;
            }

            return xdevc_ctrl_is_paired(controllerHandle);
        }

        internal static XPairingStates GetControllerPairingState(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return XPairingStates.kXPairStUnpaired;
            }

            return (XPairingStates)xdevc_ctrl_get_pairing_state(controllerHandle);
        }

        internal static int GetControllerImuFps(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_get_imu_fps(controllerHandle);
        }

        internal static int GetControllerBatteryLevel(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_get_battery_level(controllerHandle);
        }

        internal static int GetControllerButtonStateBitmask(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_get_button_state_bitmask(controllerHandle);
        }

        internal static int GetControllerTrigger(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_get_trigger(controllerHandle);
        }

        internal static int GetControllerImu(long controllerHandle, ref XAttrImuInfo imuInfo)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            int result = xdevc_ctrl_get_imu(controllerHandle, ref imuInfo);
            return result;

        }

        internal static int GetControllerFusion(long controllerHandle, long timestampNs, ref XAttrTrackingInfo trackingInfo)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            int result = xdevc_ctrl_get_fusion(controllerHandle, timestampNs, ref trackingInfo);
            return result;
        }

        internal static int GetControllerTouchpadState(long controllerHandle, ref XAttrTouchPadState touchPadState)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            int result = xdevc_ctrl_get_touchpad_state(controllerHandle, ref touchPadState);
            return result;
        }

        internal static int GetControllerState(long controllerHandle, ref XAttrControllerState controllerState)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            int result = xdevc_ctrl_get_controller_state(controllerHandle, ref controllerState);
            return result;
        }

        internal static int GetControllerTracking(int device_type, ref XControllerTrackingState xControllerTrackingState)
        {
            NativePluginApi.Unity_getControllerTrackingByType(device_type, 0,
                       ref xControllerTrackingState.index, ref xControllerTrackingState.timestamp, ref xControllerTrackingState.state,
                       ref xControllerTrackingState.posX, ref xControllerTrackingState.posY, ref xControllerTrackingState.posZ,
                       ref xControllerTrackingState.rotX, ref xControllerTrackingState.rotY, ref xControllerTrackingState.rotZ, ref xControllerTrackingState.rotW);
            return 0;
        }

        internal static int GetMotionSensorState(long controllerHandle, ref XAttrMotionSensorState xAttrMotionSensorState)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }

            XAccessoryType xAccessoryType = GetControllerType(controllerHandle);

            if ((xAccessoryType == XAccessoryType.XAccessoryType_SirenXWatchL) || (xAccessoryType == XAccessoryType.XAccessoryType_SirenXWatchR))
            {
                int result = xdevc_ctrl_get_motion_sensor_state(controllerHandle, ref xAttrMotionSensorState);
                return result;
            }
            else
            {
                return -1;
            }
        }

        internal static int GetControllerTrackId(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_get_track_id(controllerHandle);
        }

        internal static XControllerTypes GetControllerDeviceType(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return XControllerTypes.kControllerType_Unknow;
            }
            return (XControllerTypes)xdevc_ctrl_get_device_type(controllerHandle);
        }

        internal static XAccessoryType GetControllerType(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return XAccessoryType.XAccessoryType_Unknow;
            }
            int type = xdevc_ctrl_get_type(controllerHandle);
            if (Enum.IsDefined(typeof(XAccessoryType), type))
            {
                return (XAccessoryType)type;
            }
            else
            {
                Debug.LogWarning("QmQ controllerHandle :" + controllerHandle + "type : " + type);
                return XAccessoryType.XAccessoryType_Unknow;
            }
        }

        internal static string GetControllerDeviceAddress(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return null;
            }
            return ConvertIntPtrToString(xdevc_ctrl_get_device_address(controllerHandle));
        }

        internal static string GetControllerDeviceName(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return null;
            }
            return ConvertIntPtrToString( xdevc_ctrl_get_device_name(controllerHandle));
        }

        internal static string GetControllerSoftwareRevision(long controllerHandle)
        {
            return ConvertIntPtrToString(xdevc_ctrl_get_software_revision(controllerHandle));
        }

        internal static string GetControllerHardwareRevision(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return null;
            }
            return ConvertIntPtrToString(xdevc_ctrl_get_hardware_revision(controllerHandle));
        }

        internal static string GetControllerSerialNumber(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return null;
            }
            return ConvertIntPtrToString(xdevc_ctrl_get_serial_number(controllerHandle));
        }

        internal static string GetControllerModelName(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return null;
            }
            return ConvertIntPtrToString(xdevc_ctrl_get_model_name(controllerHandle));
        }

        internal static string GetControllerManufactureName(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return null;
            }
            return ConvertIntPtrToString(xdevc_ctrl_get_manufacture_name(controllerHandle));
        }

        internal static int ControllerConnect(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_connect(controllerHandle);
        }

        internal static int ControllerConnectToType(long controllerHandle, int type, bool force)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_connect_to_type(controllerHandle, type, force);
        }

        internal static int ControllerConnectToAddress(long controllerHandle, string mac, bool force)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_connect_to_address(controllerHandle, mac, force);
        }

        internal static int ControllerConnectToRfid(long controllerHandle, int rfid, bool force)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_connect_to_rfid(controllerHandle, rfid, force);
        }

        internal static int ControllerConnectToRfidPattern(long controllerHandle, int rfidPattern, bool force)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_connect_to_rfid_pattern(controllerHandle, rfidPattern, force);
        }

        internal static int ControllerDisconnect(long controllerHandle)
        {
            return xdevc_ctrl_disconnect(controllerHandle);
        }

        internal static int ControllerConfirmPair(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_confirm_pair(controllerHandle);
        }

        internal static int ControllerHoldConnection(long controllerHandle, int holdTimeSec)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_hold_connection(controllerHandle, holdTimeSec);
        }

        internal static int ControllerUnbind(long controllerHandle)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_unbind(controllerHandle);
        }

        internal static int ControllerVibrate(long controllerHandle, int strenghPercentage, int durMs)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_vibrate(controllerHandle, strenghPercentage, durMs);
        }

        internal static int ChangeDataFormat(long controllerHandle, int type)
        {
            if (!CheckHandleValid(controllerHandle))
            {
                return -1;
            }
            return xdevc_ctrl_change_data_format(controllerHandle, type);
        }

        #endregion Controller

        #region Util
        internal static string ConvertIntPtrToString(IntPtr ptr)
        {
            return ptr == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(ptr);
        }
        #endregion
    }
}