using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ximmerse.XR.Utils
{
    public class KeyCodeConstant : KeyCodeInput
    {
        [SerializeField] public string defaultKey;
        private void Start()
        {
            keycodeBtn.onClick.AddListener(KeyCodeBtnAddListener);
        }
    }

}
