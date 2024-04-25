using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class SvrOverrideSettings
{
    public enum eAntiAliasing
    {
        NoOverride = -1,
        k1 = 1,
        k2 = 2,
        k4 = 4,
    };

    public enum eDepth
    {
        NoOverride = -1,
        k16 = 16,
        k24 = 24
    };

    public enum eChromaticAberrationCorrection
    {
        NoOverride = -1,
        kDisable = 0,
        kEnable = 1
    };

    public enum eVSyncCount
    {
        NoOverride = -1,
        k1 = 1,
        k2 = 2,
    };

    public enum eMasterTextureLimit
    {
        NoOverride = -1,
        k0 = 0, // full size
        k1 = 1, // half size
        k2 = 2, // quarter size
        k3 = 3, // ...
        k4 = 4  // ...
    };

    public enum ePerfLevel
    {
        NoOverride = -1,
        System = 0,
        Minimum = 1,
        Medium = 2,
        Maximum = 3
    };

    public delegate void OnAntiAliasingChangedCallback(eAntiAliasing antiAliasing);
    public delegate void OnDepthChangedCallback(eDepth depth);
    public delegate void OnResolutionScaleFactorChangedCallback(float resolutionScaleFactor);
    public delegate void OnChromaticAberrationCorrectionChangedCallback(eChromaticAberrationCorrection chromaticAberrationCorrection);
    public delegate void OnVSyncCountChangedCallback(eVSyncCount vSyncCount);
    public delegate void OnMasterTextureLimitChangedCallback(eMasterTextureLimit masterTextureLimit);
    public delegate void OnPerfLevelChangedCallback(ePerfLevel cpuPerfLevel, ePerfLevel gpuPerfLevel);
    public delegate void OnFoveateChangedCallback(Vector2 gain, float area, float minimum);

    public static OnAntiAliasingChangedCallback OnEyeAntiAliasingChangedEvent;
    public static OnDepthChangedCallback OnEyeDepthChangedEvent;
    public static OnResolutionScaleFactorChangedCallback OnEyeResolutionScaleFactorChangedEvent;
    public static OnAntiAliasingChangedCallback OnOverlayAntiAliasingChangedEvent;
    public static OnDepthChangedCallback OnOverlayDepthChangedEvent;
    public static OnResolutionScaleFactorChangedCallback OnOverlayResolutionScaleFactorChangedEvent;
    public static OnChromaticAberrationCorrectionChangedCallback OnChromaticAberrationCorrectionChangedEvent;
    public static OnVSyncCountChangedCallback OnVSyncCountChangedEvent;
    public static OnMasterTextureLimitChangedCallback OnMasterTextureLimitChangedEvent;
    public static OnPerfLevelChangedCallback OnPerfLevelChangedEvent;
    public static OnFoveateChangedCallback OnFoveateChangedEvent;

    private static eAntiAliasing eyeAntiAliasing = eAntiAliasing.NoOverride;
    private static eDepth eyeDepth = eDepth.NoOverride;
    private static float eyeResolutionScaleFactor = 0.0f;   // NoOverride
    private static eAntiAliasing overlayAntiAliasing = eAntiAliasing.NoOverride;
    private static eDepth overlayDepth = eDepth.NoOverride;
    private static float overlayResolutionScaleFactor = 0.0f;   // NoOverride
    private static eChromaticAberrationCorrection chromaticAberrationCorrection = eChromaticAberrationCorrection.NoOverride;   
    private static eVSyncCount vSyncCount = SvrOverrideSettings.eVSyncCount.NoOverride;
    private static eMasterTextureLimit masterTextureLimit = eMasterTextureLimit.NoOverride;
    private static ePerfLevel cpuPerfLevel = ePerfLevel.NoOverride;
    private static ePerfLevel gpuPerfLevel = ePerfLevel.NoOverride;
    private static Vector2 foveateGain = Vector3.zero;
    private static float foveateArea = 0;
    private static float foveateMinimum = 0;

    public static eAntiAliasing EyeAntiAliasing
    {
        get { return eyeAntiAliasing; }
        set { eyeAntiAliasing = value; OnEyeAntiAliasingChangedEvent.Invoke(eyeAntiAliasing); }
    }

    public static eDepth EyeDepth
    {
        get { return eyeDepth; }
        set { eyeDepth = value; OnEyeDepthChangedEvent.Invoke(eyeDepth); }
    }

    public static float EyeResolutionScaleFactor
    {
        get { return eyeResolutionScaleFactor; }
        set {eyeResolutionScaleFactor = value; OnEyeResolutionScaleFactorChangedEvent.Invoke(eyeResolutionScaleFactor); }
    }

    public static eAntiAliasing OverlayAntiAliasing
    {
        get { return overlayAntiAliasing; }
        set { overlayAntiAliasing = value; OnOverlayAntiAliasingChangedEvent.Invoke(overlayAntiAliasing); }
    }

   public static eDepth OverlayDepth
    {
        get { return overlayDepth; }
        set { overlayDepth = value; OnOverlayDepthChangedEvent.Invoke(overlayDepth); }
    }

    public static float OverlayResolutionScaleFactor
    {
        get { return overlayResolutionScaleFactor; }
        set { overlayResolutionScaleFactor = value; OnOverlayResolutionScaleFactorChangedEvent.Invoke(overlayResolutionScaleFactor); }
    }

    public static eChromaticAberrationCorrection ChromaticAberrationCorrection
    {
        get { return chromaticAberrationCorrection; }
        set { chromaticAberrationCorrection = value; OnChromaticAberrationCorrectionChangedEvent.Invoke(chromaticAberrationCorrection); }
    }

    public static eVSyncCount VSyncCount
    {
        get { return vSyncCount; }
        set { vSyncCount = value; OnVSyncCountChangedEvent.Invoke(vSyncCount); }
    }

    public static eMasterTextureLimit MasterTextureLimit
    {
        get { return masterTextureLimit; }
        set { masterTextureLimit = value; OnMasterTextureLimitChangedEvent.Invoke(masterTextureLimit); }
    }

    public static ePerfLevel CpuPerfLevel
    {
        get { return cpuPerfLevel; }
        set { cpuPerfLevel = value; OnPerfLevelChangedEvent.Invoke(cpuPerfLevel, gpuPerfLevel); }
    }

    public static ePerfLevel GpuPerfLevel
    {
        get { return gpuPerfLevel; }
        set { gpuPerfLevel = value; OnPerfLevelChangedEvent.Invoke(cpuPerfLevel, gpuPerfLevel); }
    }

    public static Vector2 FoveateGain
    {
        get { return foveateGain; }
        set { foveateGain = value; OnFoveateChangedEvent.Invoke(foveateGain, foveateArea, foveateMinimum); }
    }

    public static float FoveateArea
    {
        get { return foveateArea; }
        set { foveateArea = value; OnFoveateChangedEvent.Invoke(foveateGain, foveateArea, foveateMinimum); }
    }

    public static float FoveateMinimum
    {
        get { return foveateMinimum; }
        set { foveateMinimum = value; OnFoveateChangedEvent.Invoke(foveateGain, foveateArea, foveateMinimum); }
    }

}
