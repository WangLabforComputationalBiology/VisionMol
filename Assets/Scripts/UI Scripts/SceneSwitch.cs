using UnityEngine;
using TMPro; // ����TextMeshPro�����ռ�
using UnityEngine.UI;

public class SceneSwitch : MonoBehaviour
{
    public GameObject[] scenes; // ��ų���Ԥ�Ƽ�������
    private GameObject currentScene; // ��ǰ��ʾ�ĳ���Ԥ�Ƽ�
    public TextMeshProUGUI sceneNameText; // ������ʾ�������Ƶ�TextMeshProUGUI���
    private int currentIndex = 0; // ��ǰ����������

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
            Destroy(currentScene); // ���ٵ�ǰ����Ԥ�Ƽ�
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
        currentIndex = (currentIndex + 1) % scenes.Length; // ѭ������һ������
        LoadScene(currentIndex);
    }

    public void LoadPreviousScene()
    {
        currentIndex = (currentIndex - 1 + scenes.Length) % scenes.Length; // ѭ������һ������
        LoadScene(currentIndex);
    }
}
