using UnityEngine;
using UnityEngine.UI;
public class ToggleButtons : MonoBehaviour
{
    // ʹ�����鶨�尴ť
    public GameObject[] residueButtons;

    // �������ٰ�ť����ʾ״̬
    private bool areButtonsVisible = false;

    // Start��������Ϸ��ʼʱ����һ��
    private void Start()
    {
        // �����а�ť��ʼ��Ϊ���ɼ�
        foreach (var button in residueButtons)
        {
            button.SetActive(false);
        }
    }

    // �����������"CBR"��ť�����ʱ������
    public void ToggleVisibility()
    {
        // �л���ť�Ŀɼ�״̬
        areButtonsVisible = !areButtonsVisible;

        // ������ť���鲢����ÿ����ť�Ŀɼ���
        foreach (var button in residueButtons)
        {
            button.SetActive(areButtonsVisible);
        }
    }
}
