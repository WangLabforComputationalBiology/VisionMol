using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenController : MonoBehaviour
{
    void Start()
    {
        // ��parent�����ֻ�ȡparent���������
        GameObject parent = GameObject.Find("LoadedMolecules");

        // ����ҵ���parent����
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

    // ����������ķ�������Ҫ�ڸ��������ɺ����
    public void ControlChildren(GameObject parent)
    {
        // ȷ���ṩ��parent��������Ч��
        if (parent != null)
        {
            // �����������µ�����������
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                // ��ȡ�������Transform����
                Transform child = parent.transform.GetChild(i);
            }
        }
    }
}
