using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Ximmerse.XR.InputSystems;
using TouchlessA3D;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ximmerse.XR.Gesture
{
    public class HandReticleSimple : MonoBehaviour
    {
        private XRRayInteractor rayInteractor;
        private HandnessType handness;
        [SerializeField] private SpriteRenderer cursorSpriteRenderer;
        [SerializeField] private GameObject cursorGO;
        private Vector3 hitpos, hitnormal;
        [SerializeField] private Sprite clickedSprite;
        [SerializeField] private Sprite hoverSprite;
        HandTrackingInfo currentHandTrackInfo;


        public XRRayInteractor handXrRayInteractor
        {
            get => rayInteractor;
            set => rayInteractor = value;
        }
        public HandnessType CursorHandnessType
        {
            get => handness;
            set => handness = value;
        }

        private void EnableRenderer(bool start_re)
        {

            cursorGO.SetActive(start_re);
        }


        private float ControlShape(HandTrackingInfo currentHandTrackInfo)
        {
            Vector3 thumfingerpos = currentHandTrackInfo.ThumbFinger.Positions[currentHandTrackInfo.ThumbFinger.Positions.Length - 1];
            Vector3 indexfingerpos = currentHandTrackInfo.IndexFinger.Positions[currentHandTrackInfo.IndexFinger.Positions.Length - 1];
            //计算食指与拇指之间的距离映射至Mesh的动画
            float distance = Vector3.Distance(thumfingerpos, indexfingerpos);
            float percentage = distance / 0.08f;
            if (percentage > 1) percentage = 1;

            return percentage;
        }

        void Update()
        {
            if (HandTracking.IsHandTrackingEnable)
            {
                currentHandTrackInfo = HandTracking.GetHandTrackingInfo(handness);

                HandAnchorInputDevice input = HandAnchorInputDevice.GetInput(this.handness);

                if (currentHandTrackInfo.IsValid)
                {
                    EnableRenderer(true);
                }
                else EnableRenderer(false);

                rayInteractor.TryGetHitInfo(out hitpos, out hitnormal, out _, out bool isValid);
                if (isValid)
                {
                    transform.position = hitpos;
                    this.transform.rotation = Quaternion.LookRotation(hitnormal);
                }
                else
                {
                    transform.position = input.anchorPointRuntime;
                    this.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - this.transform.position);
                }

                float pinchScale = 0.5f + ControlShape(currentHandTrackInfo) * 0.5f;
                float dist2Camera = Vector3.Distance(Camera.main.transform.position, this.transform.position);
                float distScale = dist2Camera / 0.5f;
                cursorGO.transform.localScale = pinchScale * distScale * Vector3.one;

                if (currentHandTrackInfo.NativeGestureType == (int)GestureType.CLOSED_PINCH)
                {
                    cursorSpriteRenderer.sprite = clickedSprite;
                }
                else
                {
                    cursorSpriteRenderer.sprite = hoverSprite;
                }
            }
            else
            {
                EnableRenderer(false);
            }
        }
    }
}


