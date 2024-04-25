using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// Realtime hand gesture recoginzation.
    /// </summary>
    public class HandGestureRecognization : MonoBehaviour
    {
        private class GestureRecognizationState
        {

            public float sumTime = 0;

            public bool isEventFired;

        }

        /// <summary>
        /// If true, the gesture requires both hands matches (AND logic).
        /// </summary>
        public bool BothHands = true;

        public HandnessType handness = HandnessType.Left;

        public List<HandGestureSnapshotData> snapshotDatas = new List<HandGestureSnapshotData>();

        [System.Serializable]
        public class RecognizeGestureEvent : UnityEvent<HandGestureSnapshotData>
        {

        }

        /// <summary>
        /// Unity event on gesture recognized.
        /// </summary>
        public RecognizeGestureEvent OnRecognizedGesture = null;

        Dictionary<HandGestureSnapshotData, GestureRecognizationState> m_GestureMatchCheckBook = new Dictionary<HandGestureSnapshotData, GestureRecognizationState>();

        // Update is called once per frame
        void Update()
        {
            if (snapshotDatas == null || snapshotDatas.Count == 0 || HandTracking.IsHandTrackingEnable == false)
            {
                return;
            }

            for (int i = 0; i < snapshotDatas.Count; i++)
            {
                HandGestureSnapshotData snapshot = snapshotDatas[i];
                if (BothHands)
                {
                    var leftHandInfo = HandTracking.GetHandTrackingInfo(HandnessType.Left);
                    var rightHandInfo = HandTracking.GetHandTrackingInfo(HandnessType.Right);
                    if (leftHandInfo.IsValid && HandTracking.TryRecognize(leftHandInfo, snapshot) && rightHandInfo.IsValid && HandTracking.TryRecognize(rightHandInfo, snapshot))
                    {
                        OnGestureMatch(snapshot);
                    }
                    else
                    {
                        OnGestureUnmatch(snapshot);
                    }
                }
                else
                {
                    var handInfo = HandTracking.GetHandTrackingInfo(this.handness);
                    if (handInfo.IsValid && HandTracking.TryRecognize(handInfo, snapshot))
                    {
                        OnGestureMatch(snapshot);
                    }
                    else
                    {
                        OnGestureUnmatch(snapshot);
                    }
                }
            }
        }

        /// <summary>
        /// Callback on gesture match
        /// </summary>
        /// <param name="snapshotData"></param>
        private void OnGestureMatch(HandGestureSnapshotData snapshotData)
        {
            GestureRecognizationState state;
            if (m_GestureMatchCheckBook.ContainsKey(snapshotData))
            {
                state = m_GestureMatchCheckBook[snapshotData];
                state.sumTime = Mathf.Clamp(state.sumTime + Time.deltaTime, 0, 0.5f);
            }
            else
            {
                state = new GestureRecognizationState();
                state.sumTime = Mathf.Clamp(state.sumTime + Time.deltaTime, 0, 0.5f);
                m_GestureMatchCheckBook.Add(snapshotData, state);
            }
            //if the gesture is recognized for half sec :
            if (state.sumTime >= 0.5f && state.isEventFired == false)
            {
                state.isEventFired = true;
                //Fires event:
                OnRecognizedGesture?.Invoke(snapshotData);
            }
        }

        /// <summary>
        /// Callback on gesture not match
        /// </summary>
        /// <param name="snapshotData"></param>
        private void OnGestureUnmatch(HandGestureSnapshotData snapshotData)
        {
            GestureRecognizationState state;
            if (m_GestureMatchCheckBook.ContainsKey(snapshotData))
            {
                state = m_GestureMatchCheckBook[snapshotData];
                state.sumTime = Mathf.Clamp(state.sumTime - 2 * Time.deltaTime, 0, 0.5f);//decay twice faster
            }
            else
            {
                state = new GestureRecognizationState();
                state.sumTime = Mathf.Clamp(state.sumTime - 2 * Time.deltaTime, 0, 0.5f);//decay twice faster
                m_GestureMatchCheckBook.Add(snapshotData, state);
            }
            //reset the isEventFired field:
            if (state.sumTime <= 0.1f && state.isEventFired == true)
            {
                //Fires event:
                state.isEventFired = false;
            }
        }

    }

}