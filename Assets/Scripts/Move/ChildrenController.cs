using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenController : MonoBehaviour
{
    void Start()
    {
        // 用parent的名字获取parent物体的引用
        GameObject parent = GameObject.Find("LoadedMolecules");

        // 如果找到了parent物体
        if (parent != null)
        {
            ControlChildren(parent);
            Debug.Log("Success");
        }
        else
        {
            Debug.Log("Parent object 'LoadedMolecules' not found!");
        }
    }

    // 控制子物体的方法，需要在父物体生成后调用
    public void ControlChildren(GameObject parent)
    {
        // 确保提供的parent对象是有效的
        if (parent != null)
        {
            // 遍历父物体下的所有子物体
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                // 获取子物体的Transform引用
                Transform child = parent.transform.GetChild(i);
            }
        }
    }
}
