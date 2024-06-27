using UnityEngine;
using UnityEngine.UI;
public class PrefabLoader : MonoBehaviour
{
    public GameObject prefabToLoad;
    public Transform canvasTransform;
    private GameObject currentUI; // �洢��ǰʵ������UIԤ�Ƽ�

    public void LoadPrefab()
    {
        if (prefabToLoad != null && canvasTransform != null)
        {
            if (currentUI != null)
            {
                Destroy(currentUI); // ȷ��ÿ��ֻ��һ��ʵ��
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
            currentUI.SetActive(false); // ���ص�ǰUI
        }
    }
}
