using UnityEngine;
using UnityEngine.UI;

public class ToggleVisibility : MonoBehaviour
{
    // 引用到按钮
    public Button toggleButton;
    // 引用到要显示/隐藏的目标 GameObject
    public GameObject targetObject;

    // 初始化
    void Start()
    {
        if (toggleButton != null && targetObject != null)
        {
            // 添加按钮点击事件监听器
            toggleButton.onClick.AddListener(ToggleTargetVisibility);
            // 在游戏开始时隐藏目标对象
            targetObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Button or Target Object is not assigned.");
        }
    }

    // 切换目标对象的可见性
    void ToggleTargetVisibility()
    {
        if (targetObject != null)
        {
            // 切换目标对象的活动状态
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}
