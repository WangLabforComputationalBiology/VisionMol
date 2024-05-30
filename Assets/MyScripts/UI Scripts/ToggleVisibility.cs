using UnityEngine;
using UnityEngine.UI;

public class ToggleVisibility : MonoBehaviour
{
    // ���õ���ť
    public Button toggleButton;
    // ���õ�Ҫ��ʾ/���ص�Ŀ�� GameObject
    public GameObject targetObject;

    // ��ʼ��
    void Start()
    {
        if (toggleButton != null && targetObject != null)
        {
            // ��Ӱ�ť����¼�������
            toggleButton.onClick.AddListener(ToggleTargetVisibility);
            // ����Ϸ��ʼʱ����Ŀ�����
            targetObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Button or Target Object is not assigned.");
        }
    }

    // �л�Ŀ�����Ŀɼ���
    void ToggleTargetVisibility()
    {
        if (targetObject != null)
        {
            // �л�Ŀ�����Ļ״̬
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}
