using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject[] buttons; // 存储所有按钮的数组

    // 调用此方法来隐藏其他所有按钮
    public void HideOtherButtons(GameObject selectedButton)
    {
        foreach (var button in buttons)
        {
            if (button != selectedButton)
            {
                button.SetActive(false); // 隐藏未被点击的按钮
            }
        }
    }

    // 调用此方法来显示所有按钮
    public void ShowAllButtons()
    {
        foreach (var button in buttons)
        {
            button.SetActive(true); // 显示所有按钮
        }
    }
}
