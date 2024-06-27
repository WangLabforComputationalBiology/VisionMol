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
            // ��ȡCanvas��RectTransform���
            canvas = canvasGameObject.GetComponent<Canvas>();
            canvasRect = canvasGameObject.GetComponent<RectTransform>();

            // ����Canvas��RenderModeΪWorldSpace
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 1;
            // ����Canvas������Ϊԭ��
            canvasRect.anchoredPosition3D = new Vector3(0.02f, 0.76f, 2.93f);

            // ����Canvas�Ĵ�СΪ0.01
            canvasRect.localScale = Vector3.one * 0.006f;
            canvasRect.AddComponent<TrackedDeviceGraphicRaycaster>();  
        }
        else
        {
            Debug.LogError("Could not find SimpleFileBrowserCanvas(Clone) GameObject.");
        }
    }
}
