using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    // 存储锚定的对象
    private GameObject anchoredObject = null;

    void Update()
    {
        // 检查是否点击了鼠标左键
        if (Input.GetMouseButtonDown(0))
        {
            // 从摄像机发射一条射线到点击位置
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 检查射线是否击中了某个对象
                if (hit.transform != null)
                {
                    // 检查该对象是否是LoadedMolecules的子物体
                    if (hit.transform.parent != null && hit.transform.parent.name == "LoadedMolecules")
                    {
                        // 将当前点击的对象设置为锚定对象
                        anchoredObject = hit.transform.gameObject;

                        // 更新所有子物体的相对坐标显示
                        UpdateRelativePositions();
                    }
                }
            }
        }
    }

    // 更新未被锚定的子物体相对于锚定子物体的坐标显示
    private void UpdateRelativePositions()
    {
        if (anchoredObject == null) return;

        foreach (Transform child in anchoredObject.transform.parent)
        {
            // 忽略锚定的自身物体
            if (child.gameObject == anchoredObject) continue;

            // 计算相对位置
            Vector3 relativePosition = child.position - anchoredObject.transform.position;

            // 显示相对位置，此处你需要替换为你的显示逻辑
            Debug.Log(child.gameObject.name + " relative position: " + relativePosition);
        }
    }
}