using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject[] buttons; // �洢���а�ť������

    private GameObject activeButton; // ���ڴ洢��ǰ����İ�ť

    // ���ô˷����������������а�ť
    public void HideOtherButtons(GameObject selectedButton)
    {
        foreach (var button in buttons)
        {
            if (button != selectedButton)
            {
                button.SetActive(false); // ����δ������İ�ť
            }
            else
            {
                activeButton = button; // ���µ�ǰ����İ�ť
            }
        }
    }

    // ���ô˷�������ʾ���а�ť
    //public void ShowAllButtons()
    //{
    //    foreach (var button in buttons)
    //    {
    //        button.SetActive(true); // ��ʾ���а�ť
    //    }
    //    activeButton = null; // ���õ�ǰ����İ�ť
    //}

    // ���·�������������Ϸ����ʱͨ�����������¼������а�ť
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.R)) // �������R��
    //    {
    //        ShowAllButtons(); // ���ò���ʾ���а�ť
    //    }
    //}
}
