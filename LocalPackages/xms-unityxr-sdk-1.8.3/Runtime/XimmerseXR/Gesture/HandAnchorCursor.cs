using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ximmerse.XR.InputSystems;

namespace Ximmerse.XR.Gesture
{
    public class HandAnchorCursor : MonoBehaviour
    {
        [SerializeField] private HandnessType handness;
        [SerializeField] bool createCursor = true;
        HandReticleSimple handReticleSimple;

        public HandReticleSimple HandReticleGO
        {
            get => handReticleSimple;
        }

        private void Start()
        {
            if (createCursor)
            {
                GameObject leftAnchorInteractor = GameObject.Instantiate(Resources.Load("Gesture/Prefabs/Hand Reticle")) as GameObject;
                leftAnchorInteractor.name = "Hand Reticle";
                handReticleSimple = leftAnchorInteractor.GetComponent<HandReticleSimple>();
                handReticleSimple.handXrRayInteractor = GetComponent<XRRayInteractor>();
                handReticleSimple.CursorHandnessType = handness;
            }
        }
    }
}

