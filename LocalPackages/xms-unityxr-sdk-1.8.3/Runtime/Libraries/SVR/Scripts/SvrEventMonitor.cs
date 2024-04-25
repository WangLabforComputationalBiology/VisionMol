using System;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace Ximmerse.XR.Utils
{
    public class SvrEventMonitor : MonoBehaviour
    {

        public enum svrThermalLevel
        {
            kSafe,
            kLevel1,
            kLevel2,
            kLevel3,
            kCritical,
            kNumThermalLevels
        };

        public enum svrThermalZone
        {
            kCpu,
            kGpu,
            kSkin,
            kNumThermalZones
        };

        public struct svrEventData_Thermal
        {
            public svrThermalZone zone;               //!< Thermal zone
            public svrThermalLevel level;             //!< Indication of the current zone thermal level
        };

        public struct svrEventData_Proximity
        {
            public float distance;               //!< Proximity value in cm
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct svrEventData
        {
            [FieldOffset(0)]
            public svrEventData_Thermal thermal;
            [FieldOffset(0)]
            public svrEventData_Proximity proximity;
            [FieldOffset(0)]
            public Int64 data;
        }

        public enum svrEventType
        {
            kEventNone = 0,
            kEventSdkServiceStarting = 1,
            kEventSdkServiceStarted = 2,
            kEventSdkServiceStopped = 3,
            kEventControllerConnecting = 4,
            kEventControllerConnected = 5,
            kEventControllerDisconnected = 6,
            kEventThermal = 7,
            kEventVrModeStarted = 8,
            kEventVrModeStopping = 9,
            kEventVrModeStopped = 10,
            kEventSensorError = 11,
            kEventMagnometerUncalibrated = 12,
            kEventBoundarySystemCollision = 13,
            kEvent6dofRelocation = 14,
            kEvent6dofWarningFeatureCount = 15,
            kEvent6dofWarningLowLight = 16,
            kEvent6dofWarningBrightLight = 17,
            kEvent6dofWarningCameraCalibration = 18,
            kEventProximity = 19,
            kEvent6dofLowQuality = 40
        };

        public struct SvrEvent
        {
            public svrEventType eventType;      //!< Type of event
            public uint deviceId;               //!< An identifier for the device that generated the event (0 == HMD)
            public float eventTimeStamp;        //!< Time stamp for the event in seconds since the last svrBeginVr call
            public svrEventData eventData;      //!< Event specific data payload
        };
        private void Start()
        {

        }


        svrEventData eventData = new svrEventData();
        float timeWarning = 0;
        bool warningEnable = false;
        public void Inspect(Text warningText, float keepTime)
        {
            uint deviceId = 0;
            float eventTimeStamp = 0;
            int dataCount = Marshal.SizeOf(eventData) / sizeof(uint);
            uint[] dataBuffer = new uint[dataCount];
            int eventType = 0;
#if !UNITY_EDITOR
            bool isEvent = SvrPluginAndroid.SvrPollEvent(ref eventType, ref deviceId, ref eventTimeStamp, dataCount, dataBuffer);

            if (isEvent)
            {
                switch ((svrEventType)(eventType))
                {

                    case svrEventType.kEvent6dofLowQuality:
                        {
                            if (warningText != null)
                            {
                                //warningText.enabled = true;
                                warningText.color = Color.red;
                                warningText.text = string.Format("定位异常，请检查环境纹理和亮度..");
                            }
                            timeWarning = keepTime;
                            warningEnable = true;
                        };
                        break;
                }
            }

            if (warningEnable)
            {
                timeWarning -= Time.deltaTime;
                if (timeWarning <= 0)
                {
                    if (warningText != null)
                    {
                        //warningText.enabled = false;
                        warningText.color = Color.green;
                        warningText.text = "";
                    }
                    warningEnable = false;
                }
            }
#endif

        }
    }
}


