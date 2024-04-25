using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.XR
{
    /// <summary>
    /// Marker data descriptor.
    /// </summary>
    public struct MarkerDescriptor
    {
        public int id;
        public float size;
        /// <summary>
        /// true for controller, cube, false for single card.
        /// </summary>
        public bool isGroup;
    }

}