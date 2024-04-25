using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class SvrManager : MonoBehaviour
{
    public static SvrManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<SvrManager>();
            if (instance == null) Debug.LogError("SvrManager object component not found");
            return instance;
        }
    }
    private static SvrManager instance;

    [Serializable]
    public class SvrSettings
    {
   
        public enum eVSyncCount
        {
            k1 = 1,
            k2 = 2,
        };

        public enum eMasterTextureLimit
        {
            k0 = 0, // full size
            k1 = 1, // half size
            k2 = 2, // quarter size
            k3 = 3, // ...
            k4 = 4  // ...
        };

        public enum ePerfLevel
        {
            Minimum = 1,
            Medium = 2,
            Maximum = 3
        };

        [Flags] public enum eOptionFlags
        {
            ProtectedContent = (1 << 0),
            MotionAwareFrames = (1 << 1),
            FoveationSubsampled = (1 << 2),
            EnableCameraLayer = (1 << 3),
            Enable3drOcclusion = (1 << 4),
        }

        public enum eFoveationLevel
        {
            None = -1,
            Low = 0,
            Med = 1,
            High = 2
        }

        public enum eCameraPassThruVideo
        {
            Disabled = 0,
            Enabled = 1,
        }

        public enum eTrackingRecenterMode
        {
            Disabled,
            Application,
            Device
        }



        [Tooltip("Select platform type")]
        public SvrPlugin.DeviceModel deviceModel = SvrPlugin.DeviceModel.Default;
        [Tooltip("Use position tracking (if available)")]
        public bool trackPosition = true;
        [Tooltip("Limit refresh rate")]
        public eVSyncCount vSyncCount = eVSyncCount.k1;
       [Tooltip("QualitySettings TextureQuality FullRes, HalfRes, etc.")]
        public eMasterTextureLimit masterTextureLimit = eMasterTextureLimit.k0;
        [Tooltip("CPU performance level")]
        public ePerfLevel cpuPerfLevel = ePerfLevel.Medium;
        [Tooltip("GPU performance level")]
        public ePerfLevel gpuPerfLevel = ePerfLevel.Medium;
        [Tooltip("Tracking recenter mode")]
        public eTrackingRecenterMode trackingRecenterMode = eTrackingRecenterMode.Application;
    }
    [SerializeField]
    public SvrSettings settings;

    private SvrPlugin plugin = null;


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

    public void Initialize()
	{
        Debug.Log("SvrManager.Initialize()");
        SvrPlugin.deviceModel = settings.deviceModel;
        plugin = SvrPlugin.Instance;

        Input.backButtonLeavesApp = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = -1;

        int trackingMode = (int)SvrPlugin.TrackingMode.kTrackingOrientation;
        if (settings.trackPosition)
            trackingMode |= (int)SvrPlugin.TrackingMode.kTrackingPosition;
       
        plugin.SetTrackingMode(trackingMode);
        plugin.SetPerformanceLevels((int)settings.cpuPerfLevel, (int)settings.gpuPerfLevel);
        plugin.SetVSyncCount((int)settings.vSyncCount);
        QualitySettings.vSyncCount = (int)settings.vSyncCount;
    }

}