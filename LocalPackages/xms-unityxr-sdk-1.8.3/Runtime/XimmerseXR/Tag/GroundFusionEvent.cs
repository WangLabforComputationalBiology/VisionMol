using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.XR.Tag
{
    [AddComponentMenu("Ximmerse XR/Ground Fusion Event")]
    public class GroundFusionEvent : MonoBehaviour
    {
        [System.Serializable]
        public class OnFirstFusionEvent : UnityEngine.Events.UnityEvent
        { }
        [System.Serializable]
        public class OnFusionEnter : UnityEngine.Events.UnityEvent
        { }
        [System.Serializable]
        public class OnFusionStay : UnityEngine.Events.UnityEvent
        { }
        [System.Serializable]
        public class OnFusionExit : UnityEngine.Events.UnityEvent
        { }
        [Header("--- Fusion Event ---")]
        public OnFirstFusionEvent onFirstFusionEvent = new OnFirstFusionEvent();
        public OnFusionEnter onFusionEnter = new OnFusionEnter();
        public OnFusionStay onFusionStay = new OnFusionStay();
        public OnFusionExit onFusionExit = new OnFusionExit();


        private bool first = false;
        private bool exit = false;
        private bool isFirstFusionEnter = false;

        private bool isFusionEnter = false;

        private bool isFusionStay = false;

        private bool isFusionExit = false;

        private bool fusionState = false;
        private TagGroundPlane tagGroundPlane;

        private Vector3 fusionPos;
        private Quaternion fusionRot;


        private void Start()
        {
            tagGroundPlane = GetComponent<TagGroundPlane>();
        }

        private void Update()
        {
            GetFusionState();
            if (FirstFusionEnter())
            {
                Debug.Log("QmQ:FirstFusionEnter");
                onFirstFusionEvent?.Invoke();
                onFirstFusionEvent.RemoveAllListeners();
            }
            if (FusionEnter())
            {
                Debug.Log("QmQ:FusionEnter");
                onFusionEnter?.Invoke();
            }
            if (FusionStay())
            {
                Debug.Log("QmQ:FusionStay");
                onFusionStay?.Invoke();
            }
            if (FusionExit())
            {
                Debug.Log("QmQ:FusionExit");
                onFusionExit?.Invoke();
            }
        }

        /// <summary>
        /// First identification
        /// </summary>
        /// <returns></returns>
        private bool FirstFusionEnter()
        {
            if (fusionState && !first)
            {
                first = true;
                isFirstFusionEnter = true;
            }
            else
            {
                isFirstFusionEnter = false;
            }
            return isFirstFusionEnter;
        }
        /// <summary>
        /// Identify
        /// </summary>
        /// <returns></returns>
        private bool FusionEnter()
        {
            if (fusionState && isFusionEnter == false && isFusionStay == false)
            {
                isFusionEnter = true;
                isFusionStay = true;
            }
            else
            {
                isFusionEnter = false;
            }
            return isFusionEnter;
        }
        /// <summary>
        /// Identifying
        /// </summary>
        /// <returns></returns>
        private bool FusionStay()
        {
            if (fusionState)
            {
                isFusionStay = true;
                exit = true;
            }
            else
            {
                isFusionStay = false;
            }
            return isFusionStay;
        }
        /// <summary>
        /// lose
        /// </summary>
        /// <returns></returns>
        private bool FusionExit()
        {
            if (fusionState == false && exit)
            {
                exit = false;
                isFusionExit = true;
            }
            else
            {
                isFusionExit = false;
            }
            return isFusionExit;
        }


        private void GetFusionState()
        {
            if (tagGroundPlane != null)
            {
                if (tagGroundPlane.isValid)
                {
                    if ((fusionPos != TagProfileLoading.Instance.BeaconFusionPos)||(fusionRot.eulerAngles!= TagProfileLoading.Instance.BeaconFusionRot.eulerAngles))
                    {
                        fusionState = true;
                        fusionPos = TagProfileLoading.Instance.BeaconFusionPos;
                        fusionRot.eulerAngles = TagProfileLoading.Instance.BeaconFusionRot.eulerAngles;
                    }
                    else
                    {
                        fusionState = false;
                    }
                }
                else
                {
                    fusionState = false;
                }
            }
        }
    }
}

