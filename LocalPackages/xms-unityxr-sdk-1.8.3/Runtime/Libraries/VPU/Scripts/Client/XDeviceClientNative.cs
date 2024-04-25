//using System;
//using System.Runtime.InteropServices;

//namespace Ximmerse.Wrapper.XDeviceService.Client
//{
//    static class XDeviceClientNative
//    {
//        private const string LIB_XDEVICE_CLIENT = "xdevice_client";

//        #region Base

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_init();

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern void xdevc_exit();

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern bool xdevc_is_service_connected();

//        public delegate void XDeviceClientEventDelegate(Contract.XEvent evt, IntPtr handle, IntPtr ud);
//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        public static extern void xdevc_set_events_cb(XDeviceClientEventDelegate callback);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_get_event_name(Contract.XEvent evt);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_get_error_name(int errorCode);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern long xdevc_get_xcontext();

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern long xdevc_get_vpu();

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern long xdevc_get_controller(int index);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_get_number_of_controllers();
//        #endregion Base

//        #region Context
//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_get_client_version();

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_get_client_build_number();

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_get_client_build_information();

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_get_client_alg_version(int algType);


//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_get_server_version();

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_get_server_build_number();

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_get_server_build_information();

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_get_server_alg_version(int algType);
//        #endregion Context

//        #region Vpu
//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern bool xdevc_vpu_is_connected(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_vpu_unpair_and_disconnect_all_controllers(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_vpu_get_firmware_version(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_vpu_get_hardware_version(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_vpu_get_sn(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_vpu_get_device_name(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_vpu_get_model_name(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_vpu_get_fpga_version(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_vpu_get_ble_firmware_version(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_vpu_get_imu_alg_version(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_vpu_get_fpga_model_number(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_vpu_get_mcu_model_number(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_vpu_get_ble_model_number(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_vpu_start_upgrading(
//            long vpuHandle,
//            int projType,
//            int fwType,
//            string fwPath,
//            Contract.XDeviceFirmwareUpgradeEventsDelegate eventDelegate,
//        IntPtr ud,
//        int waitMs);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_vpu_load_marker_calibration_from_file(long vpuHandle, string file, int nofOutputElements, ref int[] outputIds);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_vpu_clear_marker_calibration_settings(long vpuHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_vpu_get_predict_tracking(long vpuHandle, int trackId, long timestamp, ref Info.TrackingInfo trackingInfo);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_vpu_get_tracking(long vpuHandle, int trackId, ref Info.TrackingInfo trackingInfo);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_vpu_set_power_mode(long vpuHandle, int power_mode);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_vpu_set_glass_perspective(long vpuHandle, int what, int val);
//        #endregion Vpu

//        #region Controller
//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_connect_state(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern bool xdevc_ctrl_is_paired(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_pairing_state(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_imu_fps(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_battery_level(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_button_state_bitmask(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_trigger(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_imu(long controllerHandle, ref XAttrImuInfo imuInfo);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_fusion(long controllerHandle, long timestampNs, ref Info.TrackingInfo trackingInfo);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_touchpad_state(long controllerHandle, ref Info.TouchPadState touchPadState);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_controller_state(long controllerHandle, ref Info.ControllerState controllerState);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_track_id(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern int xdevc_ctrl_get_device_type(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_ctrl_get_device_address(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_ctrl_get_device_name(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_ctrl_get_software_revision(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_ctrl_get_hardware_revision(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_ctrl_get_serial_number(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_ctrl_get_model_name(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        internal static extern IntPtr xdevc_ctrl_get_manufacture_name(long controllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        public static extern int xdevc_ctrl_connect(long ControllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        public static extern int xdevc_ctrl_connect_to_type(long ControllerHandle, int type, bool force);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        public static extern int xdevc_ctrl_connect_to_address(long ControllerHandle, string mac, bool force);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        public static extern int xdevc_ctrl_connect_to_rfid(long ControllerHandle, int rfid, bool force);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        public static extern int xdevc_ctrl_connect_to_rfid_pattern(long ControllerHandle, int rfidPattern, bool force);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        public static extern int xdevc_ctrl_disconnect(long ControllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        public static extern int xdevc_ctrl_confirm_pair(long ControllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        public static extern int xdevc_ctrl_hold_connection(long ControllerHandle, int holdTimeSec);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        public static extern int xdevc_ctrl_unbind(long ControllerHandle);

//        [DllImport(LIB_XDEVICE_CLIENT, CallingConvention = CallingConvention.Cdecl)]
//        public static extern int xdevc_ctrl_vibrate(long ControllerHandle, int strenghPercentage, int durMs);
//        #endregion Controller
//    }
//}