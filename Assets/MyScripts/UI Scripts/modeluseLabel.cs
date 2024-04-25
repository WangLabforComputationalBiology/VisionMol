using UnityEngine;
using UnityEngine.UI;
public class modeluselabel : MonoBehaviour
{
    // 在Inspector中分配这些UI组件
    public GameObject label;
    public GameObject labelColor;
    public GameObject labelSize;
    public GameObject all;

    // 用于在Inspector中分配返回按钮
    public GameObject backButton;

    // 在游戏开始时运行的方法
    void Start()
    {
        // 初始隐藏所有UI组件和返回按钮
        HideAllElements();
    }

    // 显示所有UI组件的方法
    public void ShowAllElements()
    {
        label.SetActive(true);
        labelColor.SetActive(true);
        labelSize.SetActive(true);
        all.SetActive(true);

        // 显示返回按钮
        backButton.SetActive(true);
    }

    // 隐藏所有UI组件的方法
    public void HideAllElements()
    {
        label.SetActive(false);
        labelColor.SetActive(false);
        labelSize.SetActive(false);
        all.SetActive(false);

        // 隐藏返回按钮
        backButton.SetActive(false);
    }
}
