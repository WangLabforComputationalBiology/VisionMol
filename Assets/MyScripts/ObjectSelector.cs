using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    // �洢ê���Ķ���
    private GameObject anchoredObject = null;

    void Update()
    {
        // ����Ƿ�����������
        if (Input.GetMouseButtonDown(0))
        {
            // �����������һ�����ߵ����λ��
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ��������Ƿ������ĳ������
                if (hit.transform != null)
                {
                    // ���ö����Ƿ���LoadedMolecules��������
                    if (hit.transform.parent != null && hit.transform.parent.name == "LoadedMolecules")
                    {
                        // ����ǰ����Ķ�������Ϊê������
                        anchoredObject = hit.transform.gameObject;

                        // ������������������������ʾ
                        UpdateRelativePositions();
                    }
                }
            }
        }
    }

    // ����δ��ê���������������ê���������������ʾ
    private void UpdateRelativePositions()
    {
        if (anchoredObject == null) return;

        foreach (Transform child in anchoredObject.transform.parent)
        {
            // ����ê������������
            if (child.gameObject == anchoredObject) continue;

            // �������λ��
            Vector3 relativePosition = child.position - anchoredObject.transform.position;

            // ��ʾ���λ�ã��˴�����Ҫ�滻Ϊ�����ʾ�߼�
            Debug.Log(child.gameObject.name + " relative position: " + relativePosition);
        }
    }
}