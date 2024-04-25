using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// Hand anchor reticle renderer.
    /// </summary>
    public class HandAnchorReticle : MonoBehaviour
    {

        /// <summary>
        /// The handness side
        /// </summary>
        public HandnessType handness;

        /// <summary>
        /// XR interactor
        /// </summary>
        public XRRayInteractor interactor;

        /// <summary>
        /// The anchor reticle transform
        /// </summary>
        public Transform anchorReticleTransform;

        /// <summary>
        /// Un hovering state reticle scale
        /// </summary>
        public float unHoverStateScale = 0.5f;

        /// <summary>
        /// Hovering state reticle scale
        /// </summary>
        public float hoveringStateScale = 1;


        float scale = 0;

        public bool alignReticleToSurface = false;

        TrackedDeviceEventData m_PointerEvent;

        Camera m_mainCam;

        public Camera MainCam
        {
            get
            {
                if (m_mainCam == null)
                {
                    m_mainCam = Camera.main;
                }
                return m_mainCam;
            }
        }

        private void Awake()
        {
            interactor = GetComponent<XRRayInteractor>();
            scale = unHoverStateScale;
        }

        private void LateUpdate()
        {
            var trackingInfo = HandAnchorInputDevice.GetInput(this.handness);
            if (trackingInfo.InputState_XR_Space.trackingState == 0)
            {
                anchorReticleTransform.gameObject.SetActive(false);
            }
            else
            {
                anchorReticleTransform.gameObject.SetActive(true);

                anchorReticleTransform.position = trackingInfo.InputState_World_Space.anchorPosition;
                anchorReticleTransform.rotation = trackingInfo.InputState_World_Space.anchorRayRotation;

                bool isHovering3D = interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit3D);
                bool isHoveringUI = interactor.TryGetCurrentUIRaycastResult(out RaycastResult hitUI);

                bool isHovering = isHovering3D || isHoveringUI;
                scale = Internal.Math.Approach(scale, isHovering ? hoveringStateScale : unHoverStateScale, 1, Time.deltaTime);
                anchorReticleTransform.localScale = Vector3.one * scale;
                // Debug.LogFormat("Setting scale : {0}", scale);
                //Is selecting:
                if (interactor.interactablesSelected.Count > 0)
                {
                    //anchorReticleTransform.localScale *= 
                }

                if (alignReticleToSurface && isHovering)
                {
                    Vector3 hitPoint = default;
                    Vector3 hitNormal = default;
                    if (isHovering3D && !isHoveringUI)
                    {
                        hitPoint = hit3D.point;
                        hitNormal = hit3D.normal;
                    }
                    else if (!isHovering3D && isHoveringUI)
                    {
                        hitPoint = hitUI.worldPosition;
                        hitNormal = hitUI.worldNormal;
                    }
                    else
                    {
                        float d3D = Vector3.Distance(MainCam.transform.position, hit3D.point);
                        float dUI = Vector3.Distance(MainCam.transform.position, hitUI.worldPosition);
                        bool useHit3D = d3D < dUI;
                        hitPoint = useHit3D ? hit3D.point : hitUI.worldPosition;
                        hitNormal = useHit3D ? hit3D.normal : hitUI.worldNormal;
                    }

                    anchorReticleTransform.position = hitPoint + hitNormal * 0.1f;
                }
            }
        }

    }
}