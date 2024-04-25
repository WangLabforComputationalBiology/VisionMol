using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;
using Ximmerse.XR;

namespace Ximmerse.XR
{
    public class MRCAPI
    {

        [HideInInspector]
        public static GameObject thirdpersonCamObj = null;

        static RenderTexture thirdpersonCamRT = null;
        static RenderTexture[] layerTexture = null;
        static int imageIndex = 0;

        static  int imageWidth = 1024, imageHeight = 512, samples = 1;
        static float fovX = 64.0f, fovY = 36.0f;


        public static void BeginRendering(ScriptableRenderContext arg1, Camera[] arg2)
        {
            foreach (Camera cam in arg2)
            {
                if (cam != null && Camera.main == cam)
                {
                    OnPreRenderCallBack(cam);
                }
            }
        }

        public static void EndRendering(ScriptableRenderContext arg1, Camera[] arg2)
        {
            foreach (Camera cam in arg2)
            {
                if (cam != null && Camera.main == cam)
                {
                    OnPostRenderCallBack(cam);
                }
            }
        }

        public static void OnPreRenderCallBack(Camera cam)
        {
            if (thirdpersonCamObj != null)
            {
                //thirdpersonCamObj.transform.position = trackingObject.position;
                //thirdpersonCamObj.transform.rotation = trackingObject.rotation;
            }
#if !UNITY_EDITOR
        NativePluginApi.Unity_getImageIndex(ref imageIndex);

        if(layerTexture[imageIndex] == null)
        { 
            layerTexture[imageIndex] = RenderTexture.GetTemporary(imageWidth, imageHeight, 24, RenderTextureFormat.ARGB32);
        }

        IntPtr Ptr1 = layerTexture[imageIndex].GetNativeTexturePtr();
        UInt64 handle1 = (UInt64)Ptr1;
        NativePluginApi.Unity_setMRCTextureHandle((int)handle1);
#endif
        }

        public static void OnPostRenderCallBack(Camera cam)
        {
#if !UNITY_EDITOR
        Texture dstT = layerTexture[imageIndex];

        RenderTexture rt = thirdpersonCamRT;

        Graphics.CopyTexture(rt, 0, 0, dstT, 0, 0);
#endif
        }


        public static void CreateMRCCam(Transform trackingObject)
        {
            layerTexture = new RenderTexture[4];

            if (thirdpersonCamObj == null)
            {
                Debug.Log("CreateMRCCam");
                thirdpersonCamObj = new GameObject("ThirdPersonCam");
                thirdpersonCamObj.transform.parent = trackingObject;
                thirdpersonCamObj.AddComponent<Camera>();

            }
            InitThirdPersonCam(thirdpersonCamObj.GetComponent<Camera>());
            thirdpersonCamObj.SetActive(true);

#if !UNITY_EDITOR
            NativePluginApi.Unity_startMRC(ref imageWidth, ref imageHeight, ref fovX, ref fovY, ref samples);
#endif

            //if (foregroundCamObj == null)
            //{
            //    foregroundCamObj = new GameObject("myForegroundCamera");
            //    foregroundCamObj.transform.parent = Camera.main.transform.parent;
            //    foregroundCamObj.AddComponent<Camera>();
            //    PLog.i(TAG_MRC, "create foreground camera object.");
            //}
            //InitForegroundCam(foregroundCamObj.GetComponent<Camera>());
            //foregroundCamObj.SetActive(true);

            //mrcCamObjActived = true;
        }

        private static void InitThirdPersonCam(Camera camera)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            camera.stereoTargetEye = StereoTargetEyeMask.None;
            camera.transform.localScale = Vector3.one;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localEulerAngles = Vector3.zero;
            camera.depth = 9999;
            camera.gameObject.layer = 0;
            camera.orthographic = false;
            camera.fieldOfView = fovY;
            camera.aspect = fovX / fovY;
            if (samples > 1)
                camera.allowMSAA = true;
            else
                camera.allowMSAA = false;
            camera.cullingMask = -1;
            if (thirdpersonCamRT == null)
            {
                thirdpersonCamRT = new RenderTexture(imageWidth, imageHeight, 24, RenderTextureFormat.ARGB32);
            }
            thirdpersonCamRT.name = "ThirdPersonMrcRenderTexture";
            camera.targetTexture = thirdpersonCamRT;
        }

        public static void CalibrationMRCCam(float px, float py, float pz, float rx, float ry,float rz, float rw)
        {
            if (thirdpersonCamObj == null) return;

            thirdpersonCamObj.transform.position = new Vector3(px, py, pz);
            thirdpersonCamObj.transform.rotation = new Quaternion(rx, ry, rz, rw);
        }
    }
}
