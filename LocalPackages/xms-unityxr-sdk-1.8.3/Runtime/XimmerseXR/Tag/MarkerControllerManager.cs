using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Ximmerse.XR;

namespace Ximmerse.XR.Tag
{
    public class MarkerControllerManager : MonoBehaviour
    {
        [SerializeField]
        private string m_leftCalibraFilePath;

        [SerializeField]
        private string m_rightCalibraFilePath;

        private void Start()
        {
            if (TagProfileLoading.Instance!=null)
            {
                StartCoroutine("WaitLoading");
            }
        }
        private IEnumerator WaitLoading()
        {
            while (TagProfileLoading.Instance.ThreadLoad == null || TagProfileLoading.Instance.ThreadLoad.ThreadState == ThreadState.Running)
            {
                yield return null;
            }
            ThreadTagLoading();
        }
        private void ThreadTagLoading()
        {
            Thread thread;
            thread = new Thread(SetCalibraFile); 
            thread.Start();
        }
        private void SetCalibraFile()
        {
#if !UNITY_EDITOR
            if (m_leftCalibraFilePath!=null)
            {
                int[] ids = new int[5];
                XDevicePlugin.LoadTrackingMarkerSettingsFile(m_leftCalibraFilePath, out ids, 5);
            }
            if (m_rightCalibraFilePath != null)
            {
                int[] ids = new int[5];
                XDevicePlugin.LoadTrackingMarkerSettingsFile(m_rightCalibraFilePath, out ids, 5);
            }
#endif
        }
    }
}

