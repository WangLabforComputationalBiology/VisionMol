using UnityEngine;
using UnityEngine.UI; // ����UI�����ռ�
public class LabelManager : MonoBehaviour
{
    // ����������Ҫ��ʾ�İ�ť����
    public GameObject[] buttonsToToggle;

    // �����������LoadPDB��ť���ʱ������
    public void ToggleButtons()
    {
        foreach (GameObject button in buttonsToToggle)
        {
            button.SetActive(!button.activeSelf); // �����ť�����صģ�����ʾ�����������ʾ�ģ���������
        }
    }

    void Start()
    {
        // ��ʼ��ʱ�������а�ť
        foreach (GameObject button in buttonsToToggle)
        {
            button.SetActive(false);
        }
    }

}
