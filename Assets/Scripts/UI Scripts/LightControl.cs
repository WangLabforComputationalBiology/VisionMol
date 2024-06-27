using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间以使用UI组件

public class LightControl : MonoBehaviour
{
    public Light sceneLight; // 可以在Unity编辑器中拖拽一个灯光到这个变量
    public Button toggleButton; // 可以在Unity编辑器中拖拽一个按钮到这个变量
    private bool isLightOn = true; // 跟踪灯光状态

    void Start()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleLight); // 为按钮添加点击事件监听器
        }
    }

    void ToggleLight()
    {
        isLightOn = !isLightOn; // 切换灯光状态
        sceneLight.intensity = isLightOn ? 1.0f : 0.1f; // 如果灯光是开的，亮度为1.0；如果关的，亮度为0.1
    }
}
