using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ximmerse.XR.Utils;


namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// Virtual hand renderer, for hand tracking.
    /// </summary>
    public class VirtualHandRenderer : MonoBehaviour
    {

        public HandnessType handness = HandnessType.Left;

        /// <summary>
        /// If true, colliders are disabled when start the behaviour.
        /// </summary>
        public bool DisableCollidersAtStartup = true;

        /// <summary>
        /// The game object prefab of palm.
        /// </summary>
        public GameObject palm;

        /// <summary>
        /// Joints of thumb finger
        /// </summary>
        public GameObject[] thumbJoints;

        public GameObject[] thumbConnections;

        /// <summary>
        /// Joints of index finger
        /// </summary>
        public GameObject[] indexJoints;

        public GameObject[] indexConnections;

        /// <summary>
        /// Joints of middle finger
        /// </summary>
        public GameObject[] middleJoints;
        public GameObject[] middleConnections;

        /// <summary>
        /// Joints of ring finger
        /// </summary>
        public GameObject[] ringJoints;
        public GameObject[] ringConnections;
        /// <summary>
        /// Joints of little finger
        /// </summary>
        public GameObject[] littleJoints;
        public GameObject[] littleConnections;

        /// <summary>
        /// Deactivate model time valve.
        /// </summary>
        const float kDeactiveTimeValve = 0.10f;

        float deactiveTimeCounter = 0;

        private bool isHandModelActive = false;

        Collider[] subColliders = null;


        private void Awake()
        {
            subColliders = GetComponentsInChildren<Collider>(true);
        }

        private void Start()
        {
            EnableHandModel(false);
            if(DisableCollidersAtStartup)
            {
                this.EnableSubColliders(false);
            }
        }


        // Update is called once per frame
        void LateUpdate()
        {
            if (HandTracking.IsHandTrackingEnable)
            {
                if (isHandModelActive)
                {
                    UpdateHandTransforms();
                }
                var trackedInfo = this.handness == HandnessType.Left ? HandTracking.LeftHandTrackInfo : HandTracking.RightHandTrackInfo;
                if (!trackedInfo.IsValid)
                {
                    deactiveTimeCounter += Time.deltaTime;
                    if (deactiveTimeCounter >= kDeactiveTimeValve && isHandModelActive)
                    {
                        EnableHandModel(false);
                    }
                }
                else
                {
                    if (!isHandModelActive)
                    {
                        EnableHandModel(true);
                    }
                    deactiveTimeCounter = 0;//reset time counter on valid hand case.
                }
            }
            else
            {
                EnableHandModel(false);
            }
        }

        /// <summary>
        /// update transforms of each finger's joint and connection.
        /// </summary>
        private void UpdateHandTransforms()
        {
            var trackedInfo = this.handness == HandnessType.Left ? HandTracking.LeftHandTrackInfo : HandTracking.RightHandTrackInfo;
            this.palm.transform.SetPositionAndRotation(trackedInfo.PalmPosition, trackedInfo.PalmRotation);
            this.palm.transform.localScale = trackedInfo.PalmScale;
            UpdateFingerJoints(this.thumbJoints, this.thumbConnections, trackedInfo.ThumbFinger);
            UpdateFingerJoints(this.indexJoints, this.indexConnections, trackedInfo.IndexFinger);
            UpdateFingerJoints(this.middleJoints, this.middleConnections, trackedInfo.MiddleFinger);
            UpdateFingerJoints(this.ringJoints, this.ringConnections, trackedInfo.RingFinger);
            UpdateFingerJoints(this.littleJoints, this.littleConnections, trackedInfo.LittleFinger);
        }

        private void UpdateFingerJoints(GameObject[] joints, GameObject[] connections, RawFingerTrackingInfo fingerTrackInfo)
        {
            for (int i = 0, iMax = joints.Length; i < iMax; i++)
            {
                joints[i].transform.position = fingerTrackInfo.Positions[i];
            }
            for (int i = 0, iMax = connections.Length; i < iMax; i++)
            {
                connections[i].transform.position = (fingerTrackInfo.Positions[i + 1] + fingerTrackInfo.Positions[i]) / 2;
                connections[i].transform.up = (fingerTrackInfo.Positions[i + 1] - fingerTrackInfo.Positions[i]).normalized;
            }
        }

        /// <summary>
        /// Activate / Deactivate hand model
        /// </summary>
        /// <param name="enabled"></param>
        void EnableHandModel(bool enabled)
        {
            isHandModelActive = enabled;
            palm.gameObject.SetActive(enabled);
            foreach (var g in thumbJoints)
            {
                g.gameObject.SetActive(enabled);
            }
            foreach (var g in indexJoints)
            {
                g.gameObject.SetActive(enabled);
            }
            foreach (var g in middleJoints)
            {
                g.gameObject.SetActive(enabled);
            }
            foreach (var g in ringJoints)
            {
                g.gameObject.SetActive(enabled);
            }
            foreach (var g in littleJoints)
            {
                g.gameObject.SetActive(enabled);
            }

            foreach (var g in thumbConnections)
            {
                g.gameObject.SetActive(enabled);
            }
            foreach (var g in indexConnections)
            {
                g.gameObject.SetActive(enabled);
            }
            foreach (var g in middleConnections)
            {
                g.gameObject.SetActive(enabled);
            }
            foreach (var g in ringConnections)
            {
                g.gameObject.SetActive(enabled);
            }
            foreach (var g in littleConnections)
            {
                g.gameObject.SetActive(enabled);
            }
        }

        /// <summary>
        /// Enable/disable sub colliders
        /// </summary>
        public void EnableSubColliders(bool enable)
        {
            foreach(var c in subColliders)
            {
                c.enabled = enable;
            }
        }
    }
}