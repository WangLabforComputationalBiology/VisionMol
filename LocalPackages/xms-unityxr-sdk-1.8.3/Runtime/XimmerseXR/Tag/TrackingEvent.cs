using UnityEngine;

namespace Ximmerse.XR.Tag
{
    [AddComponentMenu("Ximmerse XR/Tracking Event")]
    public class TrackingEvent : MonoBehaviour
    {
        [System.Serializable]
        public class OnFirstTrackingEvent : UnityEngine.Events.UnityEvent
        { }
        [System.Serializable]
        public class OnTrackingEnter : UnityEngine.Events.UnityEvent
        { }
        [System.Serializable]
        public class OnTrackingStay : UnityEngine.Events.UnityEvent
        { }
        [System.Serializable]
        public class OnTrackingExit : UnityEngine.Events.UnityEvent
        { }
        [Header("--- Tracking Event ---")]
        public OnFirstTrackingEvent onFirstTrackingEvent = new OnFirstTrackingEvent();
        public OnTrackingEnter onTrackingEnter = new OnTrackingEnter();
        public OnTrackingStay onTrackingStay = new OnTrackingStay();
        public OnTrackingExit onTrackingExit = new OnTrackingExit();


        private bool first = false;
        private bool exit = false;
        private bool isFirstTrackingEnter = false;

        private bool isTrackingEnter = false;

        private bool isTrackingStay = false;

        private bool isTrackingExit = false;

        private bool trackingstate = false;
        private TagTracking tagTracking;
        private TagGroundPlane tagGroundPlane;

        private void Start()
        {
            tagTracking = GetComponent<TagTracking>();
            tagGroundPlane = GetComponent<TagGroundPlane>();
        }

        private void Update()
        {
            GetTrackingState();
            if (FirstTrackingEnter())
            {
                onFirstTrackingEvent?.Invoke();
                onFirstTrackingEvent.RemoveAllListeners();
            }
            if (TrackingEnter())
            {
                onTrackingEnter?.Invoke();
            }
            if (TrackingStay())
            {
                onTrackingStay?.Invoke();
            }
            if (TrackingExit())
            {
                onTrackingExit?.Invoke();
            }
        }

        /// <summary>
        /// First identification
        /// </summary>
        /// <returns></returns>
        private bool FirstTrackingEnter()
        {
            if (trackingstate && first == false)
            {
                first = true;
                isFirstTrackingEnter = true;
            }
            else
            {
                isFirstTrackingEnter = false;
            }
            return isFirstTrackingEnter;
        }
        /// <summary>
        /// Identify
        /// </summary>
        /// <returns></returns>
        private bool TrackingEnter()
        {
            if (trackingstate && isTrackingEnter == false && isTrackingStay == false)
            {
                isTrackingEnter = true;
                isTrackingStay = true;
            }
            else
            {
                isTrackingEnter = false;
            }
            return isTrackingEnter;
        }
        /// <summary>
        /// Identifying
        /// </summary>
        /// <returns></returns>
        private bool TrackingStay()
        {
            if (trackingstate)
            {
                isTrackingStay = true;
                exit = true;
            }
            else
            {
                isTrackingStay = false;
            }
            return isTrackingStay;
        }
        /// <summary>
        /// lose
        /// </summary>
        /// <returns></returns>
        private bool TrackingExit()
        {
            if (trackingstate == false && exit)
            {
                exit = false;
                isTrackingExit = true;
            }
            else
            {
                isTrackingExit = false;
            }
            return isTrackingExit;
        }


        private void GetTrackingState()
        {
            if (tagGroundPlane!=null)
            {
                trackingstate = tagGroundPlane.isTracking;
            }
            if (tagTracking!=null)
            {
                trackingstate = tagTracking.isTracking;
            }
        }

    }
}

