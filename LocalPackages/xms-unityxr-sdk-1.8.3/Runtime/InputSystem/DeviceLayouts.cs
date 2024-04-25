using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Scripting;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEditor;

namespace Ximmerse.XR.InputSystems
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class InputLayout
    {
        static InputLayout()
        {
            RegisterInputLayouts();
        }

        public static void RegisterInputLayouts()
        {
            InputSystem.RegisterLayout<XRHMD>(matches: new InputDeviceMatcher().WithInterface(XRUtilities.InterfaceMatchAnyVersion).WithProduct(@"^(Xim_HMD)|^(Xim)|^(HMD)"));
            InputSystem.RegisterLayout<XRControllerWithRumble>(matches: new InputDeviceMatcher().WithInterface(XRUtilities.InterfaceMatchAnyVersion).WithProduct(@"^(LeftHand)|^(RightHand)"));
        }
    }
}