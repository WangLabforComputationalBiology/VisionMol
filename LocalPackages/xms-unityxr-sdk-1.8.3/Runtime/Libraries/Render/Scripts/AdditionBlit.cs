using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using Ximmerse.XR;

public class AdditionBlit : MonoBehaviour
{
    RenderTexture rt1, rt2;
    RenderTargetIdentifier RTI1, RTI2;

    public void AddCommandBuffer(CommandBuffer commandBuffer)
    {
        //m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, commandBuffer); // “‘∑¿ÕÚ“ª
        Camera.main.AddCommandBuffer(CameraEvent.AfterDepthTexture, commandBuffer);
    }
    private Material material;

    bool sethandle=false;
    bool left = true;
    bool isRenderTextureCreat;
    IEnumerator Start()
    {
        while (!SvrPlugin.Instance.IsRunning())
        {
            yield return null;
        }
        Debug.Log("AdditionBlit");
        //SvrPlugin.DeviceInfo
        material = Resources.Load("Display/Material/depthBlitMat") as Material;
        var desc = XRSettings.eyeTextureDesc;
        int samp = XRSettings.eyeTextureDesc.msaaSamples;
        rt1 = new RenderTexture(desc.width, desc.height, 24, RenderTextureFormat.Depth);
        rt2 = new RenderTexture(desc.width, desc.height, 24, RenderTextureFormat.Depth);
        //rt1 = RenderTexture.GetTemporary(desc.width, desc.height, 24, RenderTextureFormat.Depth);
        //rt2 = RenderTexture.GetTemporary(desc.width, desc.height, 24, RenderTextureFormat.Depth);
#if !UNITY_EDITOR
        rt1.Create();
        rt2.Create();
        //IntPtr Ptr1 = rt1.GetNativeTexturePtr();
        //UInt64 handle1 = (UInt64)Ptr1;
        //IntPtr Ptr2 = rt2.GetNativeTexturePtr();
        //UInt64 handle2 = (UInt64)Ptr2;

        //NativePluginApi.Unity_setDepthHandle((int)handle1, (int)handle2);
        NativePluginApi.Unity_setMSAALevel(samp);

        //NativePluginApi.Unity_setPTW(true);
        //Debug.Log(samp);
        RTI1 = new RenderTargetIdentifier(rt1);
        RTI2 = new RenderTargetIdentifier(rt2);

        AttachmentDescriptor AttachDesc = new AttachmentDescriptor(RenderTextureFormat.Depth);
        AttachDesc.clearDepth = 1.0f;
        AttachDesc.loadAction = RenderBufferLoadAction.Clear;
        AttachDesc.storeAction = RenderBufferStoreAction.Store;
        AttachDesc.ConfigureTarget(RTI1, false, true);
        AttachDesc.ConfigureTarget(RTI2, false, true);

        NativePluginApi.Unity_setClipPlane(Camera.main.nearClipPlane, Camera.main.farClipPlane);
        Debug.Log("mainCamera:" + Camera.main.nearClipPlane + "     " + Camera.main.farClipPlane);
#endif
        //CommandBuffer buffer = new CommandBuffer();

        //buffer.Blit(RTI1, RTI2, material);
        //AddCommandBuffer(buffer);
        isRenderTextureCreat = true;
        DontDestroyOnLoad(gameObject);
    }

    private void OnRenderObject()
    {
        if (isRenderTextureCreat)
        {
            if (left)
            {
                Graphics.Blit(rt2, rt1, material);
                left = false;
            }
            else
            {
                Graphics.Blit(rt1, rt2, material);
                left = true;
            }
        }
    }

    private void Update()
    {
#if !UNITY_EDITOR
        if (isRenderTextureCreat)
        {
            //rt1.filterMode= FilterMode.Point;
            rt1.DiscardContents();
            rt2.DiscardContents();
            left = true;
            if (!sethandle)
            {
                IntPtr Ptr1 = rt1.GetNativeTexturePtr();
                UInt64 handle1 = (UInt64)Ptr1;
                IntPtr Ptr2 = rt2.GetNativeTexturePtr();
                UInt64 handle2 = (UInt64)Ptr2;
                if (handle1 != 0 && handle2 != 0)
                {
                    NativePluginApi.Unity_setDepthHandle((int)handle1, (int)handle2);
                    sethandle = true;
                }
            }
        }
#endif

    }
}
