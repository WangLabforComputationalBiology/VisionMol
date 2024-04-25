using UnityEngine;
using TMPro; // 引入TextMeshPro命名空间
using UnityEngine.UI;

public class SceneSwitch : MonoBehaviour
{
    public GameObject[] scenes; // 存放场景预制件的数组
    private GameObject currentScene; // 当前显示的场景预制件
    public TextMeshProUGUI sceneNameText; // 用于显示场景名称的TextMeshProUGUI组件
    private int currentIndex = 0; // 当前场景的索引

    void Start()
    {
        if (scenes.Length > 0)
        {
            LoadScene(currentIndex);
        }
    }

    public void LoadScene(int index)
    {
        if (currentScene != null)
        {
            Destroy(currentScene); // 销毁当前场景预制件
            Debug.Log("Destroyed current scene.");
        }

        if (index >= 0 && index < scenes.Length)
        {
            currentScene = Instantiate(scenes[index], scenes[index].transform.position, scenes[index].transform.rotation);
            Debug.Log("Loaded new scene: " + currentScene.name);

            if (sceneNameText != null)
            {
                sceneNameText.text = "Current Scene: " + currentScene.name;
            }
        }
        else
        {
            Debug.LogWarning("Index out of range: " + index);
            if (sceneNameText != null)
            {
                sceneNameText.text = "Error: Scene index out of range!";
            }
        }
    }

    public void LoadNextScene()
    {
        currentIndex = (currentIndex + 1) % scenes.Length; // 循环到下一个索引
        LoadScene(currentIndex);
    }

    public void LoadPreviousScene()
    {
        currentIndex = (currentIndex - 1 + scenes.Length) % scenes.Length; // 循环到上一个索引
        LoadScene(currentIndex);
    }
}
