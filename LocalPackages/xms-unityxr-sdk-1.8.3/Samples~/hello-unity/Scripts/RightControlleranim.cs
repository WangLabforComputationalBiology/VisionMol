using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RightControlleranim : MonoBehaviour
{
    private Animator animator;
    private Transform R_Axis;
    private float n = 20f;
    private float x, z;
    private Vector3 rota;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Transform[] G = GetComponentsInChildren<Transform>();
        foreach (var child in G)
        {
            if (child.name == "Axis")
            {
                R_Axis = child;
            }
        }
    }
    void LateUpdate()
    {
        #region Left Hand
        if (Input.GetAxis("XRI_Right_PrimaryButton") != 0)
        {
            animator.Play("R_A", 3, 0.5f);
        }
        else animator.Play("R_A", 3, 0f);

        if (Input.GetAxis("XRI_Right_SecondaryButton") != 0)
        {
            animator.Play("R_B", 4, 0.5f);
        }
        else animator.Play("R_B", 4, 0f);
        if (Input.GetAxis("XRI_Right_Grip")!=0)
        {
            animator.Play("R_Grip", 1, Input.GetAxis("XRI_Right_Grip") / 2);
        }
        else animator.Play("R_Grip", 1, 0f);

        if (Input.GetAxis("XRI_Right_MenuButton") != 0)
        {
            animator.Play("R_Menu", 5, 0.5f);
        }
        else animator.Play("R_Menu", 5, 0f);

        if (Input.GetAxis("XRI_Right_Trigger") != 0)
        {
            animator.SetFloat("Trigger", Input.GetAxis("XRI_Right_Trigger") / 2);
            animator.Play("R_Trigger", 2, Input.GetAxis("XRI_Right_Trigger") / 2);
        }
        else animator.Play("R_Trigger", 2, 0f);

        if (Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal") != 0 || (Input.GetAxis("XRI_Right_Primary2DAxis_Vertical") != 0))
        {
            if (Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal") != 0)
            {
                Debug.Log("XRI_Right_Primary2DAxis_Horizontal" + Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal"));
                x = -(Input.GetAxis("XRI_Right_Primary2DAxis_Horizontal") * n);
            }
            else x = 0;
            if (Input.GetAxis("XRI_Right_Primary2DAxis_Vertical") != 0)
            {
                Debug.Log("XRI_Right_Primary2DAxis_Vertical" + Input.GetAxis("XRI_Right_Primary2DAxis_Vertical"));
                z = -(Input.GetAxis("XRI_Right_Primary2DAxis_Vertical") * n);
            }
            else z = 0;
        }
        else x = z = 0;
        rota = new Vector3(x, 0, z);
        R_Axis.localEulerAngles = rota;
        #endregion
    }
}