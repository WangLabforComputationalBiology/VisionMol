using UnityEngine;
using UnityEngine.UI;
public class ToggleButtons : MonoBehaviour
{
    // 使用数组定义按钮
    public GameObject[] residueButtons;

    // 用来跟踪按钮的显示状态
    private bool areButtonsVisible = false;

    // Start方法在游戏开始时调用一次
    private void Start()
    {
        // 将所有按钮初始化为不可见
        foreach (var button in residueButtons)
        {
            button.SetActive(false);
        }
    }

    // 这个方法将在"CBR"按钮被点击时被调用
    public void ToggleVisibility()
    {
        // 切换按钮的可见状态
        areButtonsVisible = !areButtonsVisible;

        // 遍历按钮数组并设置每个按钮的可见性
        foreach (var button in residueButtons)
        {
            button.SetActive(areButtonsVisible);
        }
    }
}
