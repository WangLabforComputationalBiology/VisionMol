using UnityEngine;
using UnityEngine.UI;
public class modeluselabel : MonoBehaviour
{
    // ��Inspector�з�����ЩUI���
    public GameObject label;
    public GameObject labelColor;
    public GameObject labelSize;
    public GameObject all;

    // ������Inspector�з��䷵�ذ�ť
    public GameObject backButton;

    // ����Ϸ��ʼʱ���еķ���
    void Start()
    {
        // ��ʼ��������UI����ͷ��ذ�ť
        HideAllElements();
    }

    // ��ʾ����UI����ķ���
    public void ShowAllElements()
    {
        label.SetActive(true);
        labelColor.SetActive(true);
        labelSize.SetActive(true);
        all.SetActive(true);

        // ��ʾ���ذ�ť
        backButton.SetActive(true);
    }

    // ��������UI����ķ���
    public void HideAllElements()
    {
        label.SetActive(false);
        labelColor.SetActive(false);
        labelSize.SetActive(false);
        all.SetActive(false);

        // ���ط��ذ�ť
        backButton.SetActive(false);
    }
}
