using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

//#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID || UNITY_WIN || UNITY_MAC
#if true
using UnityEngine;
using UnityEngine.Events;
using XDebug = UnityEngine.Debug;
#else
using XDebug = System.Diagnostics.Debug;
#endif // UNITY_EDITOR

using NativeHandle = System.IntPtr;
using NativeExHandle = System.IntPtr;
using AOT;

namespace Ximmerse.XR
{
    public partial class XDevicePlugin
    {
        public enum XSirenXWatchButtonMasks
        {
            kTouchRight  = 0x0001,
            kTouchMiddle = 0x0002,
            kTouchLeft   = 0x0004,
            kPower       = 0x0008,
            kHallSensor  = 0x0010,
            kGlove       = 0x0020, //Reserved
            kTouchStatus = 0x00C0
        };
        public enum XSirenXWatchTouch { 
            Release    = 0x00,
            SingleTap  = 0x40,
            DoubleTap  = 0x80,
            Swipe      = 0xC0
        }


        public enum XSirenXNeckButtonMasks
        {
            kPower = 0x08,
            kAPP   = 0x10
        };
        ///////////////////////////////////////////////////////
        /// @enum XControllerButtonMasks
        /// @brief Masks of Controller buttons.
        // Reference : https://msdn.microsoft.com/en-us/library/windows/apps/microsoft.directx_sdk.reference.xinput_gamepad
        public enum XControllerButtonMasks
        {
            kXControllerButton_DpapUp = 0x0001,
            kXControllerButton_DpapDown = 0x0002,
            kXControllerButton_DpadLeft = 0x0004,
            kXControllerButton_DpadRight = 0x0008,
            kXControllerButton_Start = 0x0010,
            kXControllerButton_Back = 0x0020,
            kXControllerButton_LeftThumb = 0x0040,
            kXControllerButton_RightThumb = 0x0080,
            kXControllerButton_LeftShoulder = 0x0100,
            kXControllerButton_RightShoulder = 0x0200,
            kXControllerButton_Guide = 0x0400,
            kXControllerButton_A = 0x1000,
            kXControllerButton_B = 0x2000,
            kXControllerButton_X = 0x4000,
            kXControllerButton_Y = 0x8000,
            // Emulation
            kXControllerButton_LeftThumbMove = 0x10000,
            kXControllerButton_RightThumbMove = 0x20000,
            kXControllerButton_LeftTrigger = 0x40000,
            kXControllerButton_RightTrigger = 0x80000,
            kXControllerButton_LeftThumbUp = 0x100000,
            kXControllerButton_LeftThumbDown = 0x200000,
            kXControllerButton_LeftThumbLeft = 0x400000,
            kXControllerButton_LeftThumbRight = 0x800000,
            kXControllerButton_RightThumbUp = 0x1000000,
            kXControllerButton_RightThumbDown = 0x2000000,
            kXControllerButton_RightThumbLeft = 0x4000000,
            kXControllerButton_RightThumbRight = 0x8000000,
            //
            kXControllerButton_None = 0x0,
            kXControllerButton_ANY = ~kXControllerButton_None,
        };

        public enum XContextStates
        {
            kXContextStInited, ///< context state, context enviroment allocted, but not working.
            kXContextStStarted, ///< context state, devices in context working.
            kXContextStWillRelease, ///< context state, moment before release.
        };

        public enum VpuPowerMode
        {
            Standby = 1,
            WakeUp = 2,
            Sleep = 4,
        }

        public enum AlgType
        {
            AlgTag = 0,
            AlgIrLed = 1,
            AlgStream = 2,
        }

        public enum XGlassConfigType
        {
            GlassPerspectivePercentage = 0,
            GlassVoltage = 1,
        }

        ////////////////////////////////////////////////////////
        /// @enum XContextAttributes
        /// @brief Attributes of Context
        /// @deprecated 
        public enum XContextAttributes
        {
            //kXCtxAttr_DeviceVersion,
            kXCtxAttr_Int_SdkVersion,
            kXCtxAttr_Str_SdkVersion,
            kXCtxAttr_Int_SDKALGVersion, // int
            kXCtxAttr_Int_State, ///< State value indicated context working state, 
            kXCtxAttr_Str_SdkBuildDetail,
        };


        ////////////////////////////////////////////////////////
        /// @enum XVpuAttributes
        /// @brief Attributes of VPU Device.
        /// @deprecated 
        public enum XVpuAttributes
        {
            kXVpuAttr_Str_SoftwareRevision,
            kXVpuAttr_Str_HardwareRevision,
            kXVpuAttr_Str_SerialNumber,
            kXVpuAttr_Str_DeviceName,
            kXVpuAttr_Str_ModelName,
            kXVpuAttr_Str_FPGAVersion,
            kXVpuAttr_Str_ALGVersion,

            kXVpuAttr_Int_ImuFps,
            kXVpuAttr_Int_FpgaFps,
            /// Reversed
            kXVpuAttr_Int_AlgorithmPoseFps,

            kXVpuAttr_Int_ErrorCode,

            /// Connection state of Controller, see \ref XConnectionStates
            kXVpuAttr_Int_ConnectionState,

            kXVpuAttr_Int_PowerMode,
            /// Battery Level. Invalid if battery mode is external power
            kXVpuAttr_Int_Battery,
            kXVpuAttr_Int_BatteryVoltage,
            kXVpuAttr_Int_BatteryTemperature,

            /// IMU info of device, see \ref XAttrImuInfo
            kXVpuAttr_Obj_ImuInfo,
            /// IMU info of device, Output to variable aguments
            kXVpuAttr_V_ImuInfo,

            /// 6Dof info of device, see \ref XAttr6DofInfo
            kXVpuAttr_Obj_6DofInfo,
            /// 6Dof info of device, Output to variable aguments
            kXVpuAttr_V_6DofInfo,

            /// Pressed button bits
            kXVpuAttr_Int_ButtonBits, // int 

            /// Button events, \ref XButtonEvents
            kXVpuAttr_Int_ButtonEvent = kXVpuAttr_Int_ButtonBits,

            /// Number of Paired controllers
            kXVpuAttr_Int_PairedNumber,

            /// Device infos, imu, 6dof, buttons ... see \ref XAttrControllerState
            kXVpuAttr_Obj_ControllerState,

            /// VPU cammera tracking object pose info. see \ref XAttrTrackingInfo
            kXVpuAttr_Obj_TrackingInfo,

            /// VPU cammera tracking object pose info. Output to variable aguments
            kXVpuAttr_V_TrackingInfo,

            //kXVpuAttr_Evt_TrackingPoseUpdate,

            /// VPU paired controllers, see \ref XAttrPairedInfos
            kXVpuAttr_Obj_PairedInfos,

        };


        ////////////////////////////////////////////////////////
        /// @enum XControllerAttributes
        /// @brief Attribute types of controller.
        /// @deprecated 
        public enum XControllerAttributes
        {
            kXCAttr_Str_FirmwareRevision,
            kXCAttr_Str_SoftwareRevision,
            kXCAttr_Str_HardwareRevision,
            kXCAttr_Str_SerialNumber,
            kXCAttr_Str_DeviceName,
            kXCAttr_Str_ModelName,
            kXCAttr_Str_ManufacturerName,

            /// FPS of controller device reporting IMU data
            kXCAttr_Int_ImuFps,

            kXCAttr_Int_ErrorCode,

            /// Connection state of Controller, see \ref XConnectionStates
            kXCAttr_Int_ConnectionState,

            kXCAttr_Int_PowerMode,
            /// Battery Level. Invalid if battery mode is external power
            kXCAttr_Int_Battery,
            kXCAttr_Int_BatteryVoltage,
            kXCAttr_Int_BatteryTemperature,


            /// IMU info of device, see \ref XAttrImuInfo
            kXCAttr_Obj_ImuInfo,
            /// IMU info of device, Output to variable aguments
            kXCAttr_V_ImuInfo,

            /// 6Dof info of device, see \ref XAttr6DofInfo
            kXCAttr_Obj_6DofInfo,
            /// 6Dof info of device, Output to variable aguments
            kXCAttr_V_6DofInfo,

            /// Pressed button bits
            kXCAttr_Int_ButtonBits,
            /// Button events, \ref XButtonEvents
            kXCAttr_Int_ButtonEvent = kXCAttr_Int_ButtonBits,

            kXCAttr_Int_Trigger,
            /// Device touchpad state info. see \ref XAttrTouchPadInfo
            kXCAttr_Obj_TouchPadState,


            //kXCAttr_Int_TouchPadState, // 

            /// Device infos, imu, 6dof, buttons ... see \ref XAttrControllerState
            kXCAttr_Obj_ControllerState,

            /// Get Controller device type, see \ref XControllerTypes.
            kXCAttr_Int_Type,
            /// Get Controller device address (MAC address)
            kXCAttr_Obj_Address,
            /// Get Hex String MAC Address of controller.
            kXCAttr_Str_Address,

            /// Get Bind ID of connected controller.
            kXCAttr_Int_BindID,

        };

        public enum XPowerModes
        {
            kXPowerModeExternal = 0, ///< External power supply
            kXPowerModeBattery = 1, ///< battery 
        };

        /// /////////////////////////////////////////////////////////////////////
        /// \enum XControllerTypes
        ///  \bref Types defined of Ximmerse Controllers
        public enum XControllerTypes
        {
            kControllerType_Saber = 0x0000,
            kControllerType_Kylo = 0x0001,
            kControllerType_DType = 0x0002,
            kControllerType_PickUp = 0x0003,

            kControllerType_3Dof = 0x0200,
            kControllerType_Tag = 0x0100,

            kControllerType_TagLeft = kControllerType_Tag,
            kControllerType_TagRight = 0x0101,
            kControllerType_TagRfid = 0x0102,
            kControllerType_Cube = 0x0103,
            kControllerType_TouchPad = 0x0104,

            kControllerType_ShortGun = 0x0105,
            kControllerType_LongGun = 0x0106,

            kControllerType_CircularLeft = 0x010A,
            kControllerType_CircularRight = 0x010B,

            kControllerType_LedLeft = 0x010C,
            kControllerType_LedRight = 0x010D,

            kControllerType_GunElec_95_1 = 0x0115,
            kControllerType_GunAir_95_1 = 0x0116,
            kControllerType_GunElec_92G = 0x0117,
            kControllerType_GunAir_92G = 0x0118,
            kControllerType_GunElec_191 = 0x0119,
            kControllerType_GunAir_191 = 0x011A,

            kControllerType_Recorder = 0x0110,
            kControllerType_TearGas = 0x0111,
            kControllerType_StunGun = 0x0112,
            kControllerType_Batons = 0x0113,
            kControllerType_FlashLight = 0x0114,

            kControllerType_Snipe = 0x011B,
            kControllerType_SnipeGrenade = 0x011C,
            kControllerType_SirenXWatchL = 0x011D,
            kControllerType_SirenXWatchR = 0x011E,
            kControllerType_SirenXHub = 0x011F,
            kControllerType_SirenXNeck = 0x0120,

            kControllerType_FlexEndoscopy = 0x0121,
            kControllerType_ThrWallRadar = 0x0122,

            kControllerType_Unknow = 0xffff,
            kControllerType_BitMask = 0xffff,
        }

        public enum XAccessoryType
        {
            XAccessoryType_Saber = 0X3030,
            XAccessoryType_Kylo = 0X3031,
            XAccessoryType_PickUp = 0X3032,
            XAccessoryType_Universal = 0X3032,
            XAccessoryType_3DofXimController = 0X3033,

            //XAccessoryType_TagControllerLeft = 0X3130,
            //XAccessoryType_TagControllerRight = 0X3131,

            XAccessoryType_TagControllerL = 0x313A,
            XAccessoryType_TagControllerR = 0x313B,

            XAccessoryType_LEDControllerLeft = 0x313C,
            XAccessoryType_LEDControllerRight = 0x313D,

            XAccessoryType_Cube = 0x3133,
            XAccessoryType_TouchPad = 0x3134,

            XAccessoryType_ShortGun = 0x3135,
            XAccessoryType_LongGun = 0x3136,

            XAccessoryType_Recorder = 0x3140,
            XAccessoryType_TearTrap = 0x3141,
            XAccessoryType_StunGun = 0x3142,
            XAccessoryType_Batons = 0x3143,
            XAccessoryType_FlashLight = 0x3144,

            XAccessoryType_Elec951 = 0x3145,
            XAccessoryType_Air951 = 0x3146,

            XAccessoryType_Elec92G = 0x3147,
            XAccessoryType_Air92G = 0x3148,

            XAccessoryType_Elec191 = 0x3149,
            XAccessoryType_Air191 = 0x314A,

            XAccessoryType_Snipe = 0x314B,
            XAccessoryType_SnipeGrenade = 0x314C,

            XAccessoryType_SirenXWatchL = 0x314D,
            XAccessoryType_SirenXWatchR = 0x314E,
            XAccessoryType_SirenXHub = 0x314F,
            XAccessoryType_SirenXNeck = 0x3150,

            XAccessoryType_FlexEndoscopy = 0x3151,
            XAccessoryType_ThrWallRadar = 0x3152,

            XAccessoryType_Unknow = 0xffff,
        }

        public struct XAttrMotionSensorState
        {
            public UInt32 held_item_rfid;
            public float barometric_pressure;
            public int heart_rate;
            public int blood_oxygen;
        };

        ////////////////////////////////////////////////////////
        /// @enum XControllerTrackModes
        /// @brief Algorithm tracking types of controller.
        public enum XControllerTrackModes
        {
            kControllerTrackModeNone,
            kControllerTrackModeVisable,
            kControllerTrackModeTagMark,
        };
     
        //////////////////////////////////////
        /// @enum XButtonEvents
        /// @brief Button events
        public enum XButtonEvents
        {
            kXBtnEvt_Down,
            kXBtnEvt_Up,
            //kXBtnEvt_LongPress,
        };
      
        ///////////////////////////////////////
        /// @struct XButtonEventParam
        /// @brief Parameters for button event notification
        [StructLayout(LayoutKind.Sequential)]
        public struct XButtonEventParam
        {
            /// Button key value.
            public int btn;
            /// Event value. see \ref XButtonEvents
            public int evt;

            public XButtonEventParam(int btn, int evt)
            {
                this.btn = btn;
                this.evt = evt;
            }
        };
      
        ////////////////////////////////////////
        /// @enum XConnectionStates
        /// @brief Connection state of device
        public enum XConnectionStates
        {
            kXConnSt_Disconnected = 0,
            kXConnSt_Scanning = 1,
            kXConnSt_Connecting = 2,
            kXConnSt_Connected = 3,
            kXConnSt_Disconnecting = 4,
            //kXConnSt_Disconnected = ?
        };

        public enum XPairingStates
        {
            kXPairStUnpaired = 0,
            kXPairStPairing = 1,
            kXPairStPaired = 2,
            kXPairStUnpairing = 3,
        };

        public enum XTrackResults
        {
            kXTrackResultNone = 0,
            kXTrackResultPose = 1,
            kXTrackResultPosition = 2,

            kXTrackResultAll = kXTrackResultPosition | kXTrackResultPose,
        };
        ///////////////////////////////////////////////
        /// @struct XAttr6DofInfo
        /// @brief Structure for gettting 6Dof information
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttr6DofInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position;  /// < float buffer for gettting position values
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation; /// < float buffer for getting quaternions of rotation.
            public UInt64 timestamp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] euler;  /// < float buffer for gettting euler rotation.

            public XAttr6DofInfo(UInt64 timestamp = 0, float[] position = null, float[] rotation = null)
            {
                this.timestamp = timestamp;
                this.position = position == null ? new float[3] { 0, 0, 0 } : position;
                this.rotation = rotation == null ? new float[4] { 0, 0, 0, 0 } : rotation;
                this.euler = new float[3];
            }
        };

        //////////////////////////////////////////////
        /// @struct XAttrImuInfo
        /// @brief Structure for getting IMU information.
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrImuInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] accelerometer; ///< (out) float buffer for getting accelerometer values.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] gyroscope; ///< float buffer for getting gyroscope values.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] magnetism; /// < float buffer for getting magnetism values.
            public UInt64 timestamp;

            public XAttrImuInfo(
                UInt64 timestamp = 0,
                float[] accelerometer = null,
                float[] gyroscope = null, float[] magnetism = null)
            {
                this.timestamp = timestamp;
                this.accelerometer = accelerometer == null ? new float[3] { 0, 0, 0 } : accelerometer;
                this.gyroscope = gyroscope == null ? new float[3] { 0, 0, 0 } : gyroscope;
                this.magnetism = magnetism == null ? new float[3] { 0, 0, 0 } : magnetism;
            }
        };
 

        /////////////////////////////////////////////
        /// @struct XAttrTrackingInfo
        /// @brief VPU cammera tracking object pose info.
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrTrackingInfo
        {
            public int index;
            public int state;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation;
            public UInt64 timestamp;
            public UInt64 recognized_markers_mask;
            public double confidence;
            public double marker_distance;

            public XAttrTrackingInfo(
                UInt64 timestamp = 0,
                int index = 0,
                int state = 0,
                float[] position = null,
                float[] rotation = null,
                double confidence = 0,
                double marker_distance = 0)
            {
                this.timestamp = timestamp;
                this.index = index;
                this.state = state;
                this.position = position == null ? new float[3] { 0, 0, 0 } : position;
                this.rotation = rotation == null ? new float[4] { 0, 0, 0, 0 } : rotation;
                this.recognized_markers_mask = 0;
                this.confidence = 0;
                this.marker_distance = 0;
            }

            public override string ToString() {
                Quaternion q = new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);
                return string.Format("id:{0},st:{1},time:{2},P:{3},{4},{5}, R:{6},{7},{8}", 
                    index, state, timestamp,
                    position[0], position[1], position[2], 
                    q.eulerAngles.x, q.eulerAngles.y, q.eulerAngles.z);
            }
        };

        //////////////////////////////////////
        /// @struct XAttrControllerState
        /// @brief Structure for getting controller state informations.
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrControllerState
        {

            /// quaternion 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] rotation;

            /// x, y, z
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] position;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] accelerometer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] gyroscope;

            public UInt64 timestamp;

            /// bit map indicating button pressed state of controller.
            public UInt32 button_state;

            //uint8_t valid_flag; /// 
            // TouchPad, reverve
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public float[] axes;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] euler;  /// < float buffer for gettting euler rotation.

            public int trigger;
            public int grip;
            public UInt32 held_item_rfid;
            public float barometric_pressure;
            public int heart_rate;
            public int blood_oxygen;

            public XAttrControllerState(
                UInt64 timestamp = 0,
                UInt32 button_state = 0,
                float[] rotation = null,
                float[] position = null,
                float[] accelerometer = null,
                float[] gyroscope = null,
                float[] axes = null,
                int trigger = 0)
            {
                this.timestamp = timestamp;
                this.button_state = button_state;
                this.rotation = rotation == null ? new float[4] { 0, 0, 0, 0 } : rotation;
                this.position = position == null ? new float[3] { 0, 0, 0 } : position;
                this.accelerometer = accelerometer == null ? new float[3] { 0, 0, 0 } : accelerometer;
                this.gyroscope = gyroscope == null ? new float[3] { 0, 0, 0 } : gyroscope;
                this.axes = axes == null ? new float[6] { 0, 0, 0, 0, 0, 0 } : axes;
                this.euler = new float[3];
                this.trigger = trigger;
                this.grip = 0;
                this.held_item_rfid = 0;
                this.barometric_pressure = 0;
                this.heart_rate = 0;
                this.blood_oxygen = 0;
            }
        };


        public enum XAttrTouchPadEvents
        {
            kAttrTpadEvt_Idle = 0x00,
            kAttrTpadEvt_Press = 0x01,
            kAttrTpadEvt_Release = 0x02,
            kAttrTpadEvt_Move = 0x03,
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrTouchPadState
        {
            public bool pressed; ///< indicated touchpad is touching.
            public float x; ///< x coordinate value, from 0 to 1.
            public float y; ///< y coordinate value, from 0 to 1.
        };

        public struct XAttrMacAddress
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] mac;
        }
        /////////////////////////////
        /// \struct XAttrPairedInfos
        /// \bref Structure for getting infomation of paired controllers.
        [StructLayout(LayoutKind.Sequential)]
        public struct XAttrPairedInfos
        {
            public int c; ///< Number of paired controller.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public int[] bind_id_a; ///< An array containing bind ID of paired controllers.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6 * 16)]
            public XAttrMacAddress[] mac_a; ///< An array containing MAC address of paired controllers.
        };


        public enum XErrorCodes
        {
            kErrNoError = 0,
            kErrUnknonw = -1,

            kErrBegin = -1000,

            kErrOperationFail,
            kErrDeviceNotFound,
            kErrInvalidArgument,
            kErrVpuNotArrived,
            kErrFileNotExist,
            kErrFileEmpty,
            kErrChecksumFail,
            kErrInWrongState,

            kErrVpuAuthFailed,

            kErrReadMarkerConfigFailed,

            kErrNotFound, // no usage

            kErrApiInvalid,

            kErrVpuCommandFail,

            kErrNoDeviceToConnect,
            kErrWrongConnectState,
            kErrWrongPairingState,

            kErrPairListIsFull,

            kErrWaitTimeout,
            kErrBindIdInvalid,

            kErrOutoffMemory,

            kErrServiceNotRunning,
            kErrServiceInvalid,
            kErrCodeError,

            // kErrOpClient = -2000,

            kErrEnd,
        };
        public enum XEvents{
    kXEvtVpuRequestSuccess,
    kXEvtVpuRequestFailed,

    kXEvtVpuFwUpgradingStart = 100,
    kXEvtVpuFwUpgradingProgress,
    kXEvtVpuFwUpgradingEndSuccess,
    kXEvtVpuFwUpgradingEndFail,

    kXEvtAuthResult = 200,
    kXEvtJavaBroadcast = 201,
    kXEvtJavaOpcReport = 202,
    };
  }
}
