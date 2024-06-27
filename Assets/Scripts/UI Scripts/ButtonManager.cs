using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject[] buttons; // 存储所有按钮的数组

    private GameObject activeButton; // 用于存储当前激活的按钮

    // 调用此方法来隐藏其他所有按钮
    public void HideOtherButtons(GameObject selectedButton)
    {
        foreach (var button in buttons)
        {
            if (button != selectedButton)
            {
                button.SetActive(false); // 隐藏未被点击的按钮
            }
            else
            {
                activeButton = button; // 更新当前激活的按钮
            }
        }
    }

    // 调用此方法来显示所有按钮
    //public void ShowAllButtons()
    //{
    //    foreach (var button in buttons)
    //    {
    //        button.SetActive(true); // 显示所有按钮
    //    }
    //    activeButton = null; // 重置当前激活的按钮
    //}

    // 更新方法，允许在游戏运行时通过按键来重新激活所有按钮
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.R)) // 如果按下R键
    //    {
    //        ShowAllButtons(); // 重置并显示所有按钮
    //    }
    //}
}
