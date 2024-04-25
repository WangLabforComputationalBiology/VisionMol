using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI.Extensions.ColorPicker;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class LFsetting : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas canvas;
    public RectTransform canvasRect;

    public void ChangSizeAndPlace()
    {
        GameObject canvasGameObject = GameObject.Find("SimpleFileBrowserCanvas(Clone)");

        if (canvasGameObject != null)
        {
            // 获取Canvas和RectTransform组件
            canvas = canvasGameObject.GetComponent<Canvas>();
            canvasRect = canvasGameObject.GetComponent<RectTransform>();

            // 设置Canvas的RenderMode为WorldSpace
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 1;
            // 设置Canvas的坐标为原点
            canvasRect.anchoredPosition3D = new Vector3(2, 6, 8);

            // 设置Canvas的大小为0.01
            canvasRect.localScale = Vector3.one * 0.01f;
            canvasRect.AddComponent<TrackedDeviceGraphicRaycaster>();  
        }
        else
        {
            Debug.LogError("Could not find SimpleFileBrowserCanvas(Clone) GameObject.");
        }
    }
}
