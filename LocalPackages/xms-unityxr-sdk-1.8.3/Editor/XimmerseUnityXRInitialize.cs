#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ximmerse.XR.LegacySDKs
{
    /// <summary>
    /// Ximmerse unity XR initializer.
    /// </summary>
    public static class XimmerseUnityXRInitialize
    {

        [MenuItem("Ximmerse XR SDK/Initialize XR SDK",false,0)]
        public static void InitializeXRSDK ()
        {
            SetupAndroidSetting();
        }


        static void SetupAndroidSetting()
        {
            //Set multi-thread = false
            PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, false);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.graphicsJobMode = GraphicsJobMode.Legacy;
            //Disable GPU skinning to save GPU power
            PlayerSettings.gpuSkinning = false;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.openGLRequireES31 = true;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });

            PlayerSettings.Android.forceSDCardPermission = true;

            QualitySettings.vSyncCount = 0;//No vsync

            //IL2CPP and 64bit:
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            //Android API level :
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif