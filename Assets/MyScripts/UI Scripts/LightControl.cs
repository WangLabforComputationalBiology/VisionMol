using UnityEngine;
using UnityEngine.UI; // ����UI�����ռ���ʹ��UI���

public class LightControl : MonoBehaviour
{
    public Light sceneLight; // ������Unity�༭������קһ���ƹ⵽�������
    public Button toggleButton; // ������Unity�༭������קһ����ť���������
    private bool isLightOn = true; // ���ٵƹ�״̬

    void Start()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleLight); // Ϊ��ť��ӵ���¼�������
        }
    }

    void ToggleLight()
    {
        isLightOn = !isLightOn; // �л��ƹ�״̬
        sceneLight.intensity = isLightOn ? 1.0f : 0.1f; // ����ƹ��ǿ��ģ�����Ϊ1.0������صģ�����Ϊ0.1
    }
}
