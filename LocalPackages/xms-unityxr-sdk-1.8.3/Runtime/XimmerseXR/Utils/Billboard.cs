using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.XR
{
    public class Billboard : MonoBehaviour
    {
        Transform mainCamera;

        public bool revert;

        public bool keepHorizontal = true;

        // Update is called once per frame
        void LateUpdate()
        {
            if (!mainCamera)
            {
                mainCamera = Camera.main.transform;
            }

            if (!mainCamera)
            {
                return;
            }
            Vector3 direction = (transform.position - mainCamera.position).normalized;
            if(keepHorizontal)
            {
                direction.y = 0;
                direction = direction.normalized;
            }

            if(revert)
            {
                direction *= -1;
            }
            transform.forward = direction;

        }
    }
}