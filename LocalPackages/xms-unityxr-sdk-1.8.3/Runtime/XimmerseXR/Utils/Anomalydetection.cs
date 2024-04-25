using UnityEngine;
using UnityEngine.UI;

namespace Ximmerse.XR.Utils
{
    /// <summary>
    /// Anomaly detection
    /// </summary>
    public class Anomalydetection : SvrEventMonitor
    {
        [SerializeField]
        private Text warningText;
        [SerializeField]
        private float keepTime = 2.0f;
        private void OnEnable()
        {
            if (warningText != null)
            {
                //warningText.enabled = false;
                warningText.color = Color.green;
                warningText.text = "";
            }
        }

        private void Update()
        {
            Inspect(warningText, keepTime);
        }
    }
}


