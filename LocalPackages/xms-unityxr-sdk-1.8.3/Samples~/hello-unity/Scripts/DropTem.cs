using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTem : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.GetComponent<Canvas>().sortingOrder = -1;

        
    }
}
