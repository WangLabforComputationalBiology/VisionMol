using UnityEngine;
using UnityEngine.UI;
public class PrefabLoader : MonoBehaviour
{
    public GameObject prefabToLoad;
    public Transform canvasTransform;
    private GameObject currentUI; // 存储当前实例化的UI预制件

    public void LoadPrefab()
    {
        if (prefabToLoad != null && canvasTransform != null)
        {
            if (currentUI != null)
            {
                Destroy(currentUI); // 确保每次只有一个实例
            }

            currentUI = Instantiate(prefabToLoad, canvasTransform);

            RectTransform rtInstance = currentUI.GetComponent<RectTransform>();
            RectTransform rtPrefab = prefabToLoad.GetComponent<RectTransform>();

            rtInstance.anchoredPosition = rtPrefab.anchoredPosition;
            rtInstance.sizeDelta = rtPrefab.sizeDelta;
            rtInstance.localScale = rtPrefab.localScale;
            rtInstance.offsetMin = rtPrefab.offsetMin;
            rtInstance.offsetMax = rtPrefab.offsetMax;

            rtInstance.anchorMin = rtPrefab.anchorMin;
            rtInstance.anchorMax = rtPrefab.anchorMax;

            currentUI.SetActive(true);
        }
    }

    public void HideCurrentUI()
    {
        if (currentUI != null)
        {
            currentUI.SetActive(false); // 隐藏当前UI
        }
    }
}
