using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using Unity.Collections;
using Unity.Jobs;
using System.IO;
using System;
using Ximmerse.XR.Utils;
using Ximmerse.XR.Internal;
using SXR;
using Ximmerse.XR.Tag;

namespace Ximmerse.XR
{
    /// <summary>
    /// Ximmerse XR public interface
    /// </summary>
    public static class XimmerseXR
    {

        /// <summary>
        /// Gets the front end RGB camera's texture.
        /// If it's null, calls RequestWebCamTexture() to request one.
        /// </summary>
        public static WebCamTexture RGBCameraTexture
        {
            get; private set;
        }

        /// <summary>
        /// Starts internal web camera rgb camera texture.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fps"></param>
        internal static void RequestOpenRGBCamera(int width, int height, int fps = 60)
        {
            if(RGBCameraTexture)
            {
                if(!RGBCameraTexture.isPlaying)
                {
                    RGBCameraTexture.Play();
                }
            }
            else
            {
                WebCamDevice[] devices = WebCamTexture.devices;
                if (devices.Length == 0)
                {
                    Debug.LogError("RequestOpenRGBCamera() error : Fail to obtain any web camera texture !");
                    return;
                }
                if (RGBCameraTexture && RGBCameraTexture.isPlaying)
                {
                    Debug.Log("RequestOpenRGBCamera() : web camera texture is already playing !");
                    return;
                }
                for (int i = 0; i < devices.Length; i++)
                {
                    var _device = devices[i];
                    if (_device.isFrontFacing == true)
                    {
                        //RhinoX using front facing camera
                        RGBCameraTexture = new WebCamTexture(_device.name, width, height, fps);
                        RGBCameraTexture.Play();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Stops the internal WebCam rgb camera texture and dispose the webCamTexture object.
        /// </summary>
        /// <param name="disposeRGBCameraTexture">If true, the rgb camera texture will be disposed.</param>
        internal static void RequestStopRGBCamera(bool disposeRGBCameraTexture = false)
        {
            if (RGBCameraTexture && RGBCameraTexture.isPlaying)
            {
                RGBCameraTexture.Stop();
                if(disposeRGBCameraTexture)
                {
                    UnityEngine.Object.Destroy(RGBCameraTexture);
                    RGBCameraTexture = null;
                }
            }
        }

        /// <summary>
        /// Display eye reticle app.
        /// </summary>
        public static bool DisplayReticle
        {
            get => SvrPluginAndroid.Unity_getReticleRendering();
            set
            {
                SvrPluginAndroid.Unity_setReticleRendering(value);
            }
        }

        /// <summary>
        /// Sets reticle texture id.
        /// </summary>
        public static void SetReticleTexture(int texturePtr, int width, int height)
        {
            SvrPluginAndroid.Unity_setReticleTextureId(texturePtr, width, height);
        }

        /// <summary>
        /// Toggle overlay rendering on/off.
        /// </summary>
        public static bool OverlayRendering
        {
            get => SvrPluginAndroid.Unity_getOverlayRendering();
            set
            {
                SvrPluginAndroid.Unity_setOverlayRendering(value);
            }
        }

        /// <summary>
        /// Sets overlay renderer texture id.
        /// </summary>
        public static void SetOverlayRendererTextureID(int texturePtrLeft, int texturePtrRight)
        {
            SvrPluginAndroid.Unity_setOverlayRenderingTextureId(texturePtrLeft, texturePtrRight);
        }


        /// <summary>
        /// Loads a track profile.
        /// </summary>
        /// <param name="profile"></param>
        public static void LoadTrackingProfile(TrackingItem[] trackingItems)
        {
            XDevicePlugin.ResetTrackingMarkerSettings();
            foreach (var item in trackingItems)
            {
                NativeArray<int> ids = new NativeArray<int>(item.MarkerIDs.Length, Allocator.TempJob);
                //XDevicePlugin.LoadTrackingMarkerSettingsFile("/sdcard/vpusdk/marker_calib/BEACON/BEACON-500.json", ref ids, 100);
                XDevicePlugin.LoadTrackingMarkerSettingsFile(item.jsonName, ref ids, ids.Length);
                ids.Dispose();
            }
        }

        /// <summary>
        /// Loads ground plane layout.
        /// </summary>
        /// <param name="layout"></param>
        public static void LoadGroundPlaneLayout(GroundPlaneLayout layout)
        {
            if (SDKVariants.IsSupported)
                PluginVioFusion.plugin_vio_fusion_reset(0);
            foreach (var _group in layout.groundPlaneGroups)
            {
                foreach (var groundPlane in _group.groundPlanes)
                {
                    PluginVioFusion.XAttrBeaconInWorldInfo beacon_in_world_info = new PluginVioFusion.XAttrBeaconInWorldInfo(groundPlane.track_id);

                    //Base setting:
                    beacon_in_world_info.group_id = _group.groupIndex;
                    Quaternion q = Quaternion.Euler(groundPlane.euler);
                    beacon_in_world_info.beacon_id = groundPlane.track_id;
                    beacon_in_world_info.rotation[0] = q.x;
                    beacon_in_world_info.rotation[1] = q.y;
                    beacon_in_world_info.rotation[2] = q.z;
                    beacon_in_world_info.rotation[3] = q.w;
                    beacon_in_world_info.position[0] = groundPlane.position.x;
                    beacon_in_world_info.position[1] = groundPlane.position.y;
                    beacon_in_world_info.position[2] = groundPlane.position.z;

                    //Advance setting:
                    beacon_in_world_info.drift_recenter_angle_threshold = groundPlane.technicalParameter.drift_recenter_angle_threshold;
                    beacon_in_world_info.drift_recenter_distance_threshold = groundPlane.technicalParameter.drift_recenter_distance_threshold;
                    beacon_in_world_info.coord_system_flag = 1; //0=right hand coord system, 1 = left hand coord system,  constantly left hand for unity 
                    beacon_in_world_info.confidence_thresh = groundPlane.technicalParameter.confidence_thresh;
                    beacon_in_world_info.max_distance_thresh = groundPlane.technicalParameter.max_distance_thresh;
                    beacon_in_world_info.min_distance_thresh = groundPlane.technicalParameter.min_distance_thresh;

                    PluginVioFusion.plugin_vio_fusion_set_param(ref beacon_in_world_info);
                    Debug.LogFormat("Inject fusion ground plane id : {0}", groundPlane.track_id);
                }
            }
            SDKVariants.groundPlaneLayout = layout;//update current active layout

            if (SDKVariants.IsSupported)
                PluginVioFusion.plugin_vio_fusion_run(0);
        }

    }


}