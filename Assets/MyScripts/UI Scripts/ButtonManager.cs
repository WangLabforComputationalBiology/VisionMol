using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject[] buttons; // �洢���а�ť������

    // ���ô˷����������������а�ť
    public void HideOtherButtons(GameObject selectedButton)
    {
        foreach (var button in buttons)
        {
            if (button != selectedButton)
            {
                button.SetActive(false); // ����δ������İ�ť
            }
        }
    }

    // ���ô˷�������ʾ���а�ť
    public void ShowAllButtons()
    {
        foreach (var button in buttons)
        {
            button.SetActive(true); // ��ʾ���а�ť
        }
    }
}
