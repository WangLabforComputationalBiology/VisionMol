using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间
public class LabelManager : MonoBehaviour
{
    // 假设这是你要显示的按钮数组
    public GameObject[] buttonsToToggle;

    // 这个方法将在LoadPDB按钮点击时被调用
    public void ToggleButtons()
    {
        foreach (GameObject button in buttonsToToggle)
        {
            button.SetActive(!button.activeSelf); // 如果按钮是隐藏的，则显示它；如果是显示的，则隐藏它
        }
    }

    void Start()
    {
        // 初始化时隐藏所有按钮
        foreach (GameObject button in buttonsToToggle)
        {
            button.SetActive(false);
        }
    }

}
