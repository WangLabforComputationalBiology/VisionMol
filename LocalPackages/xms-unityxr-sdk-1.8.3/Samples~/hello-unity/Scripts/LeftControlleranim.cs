using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class LeftControlleranim : MonoBehaviour
{
    private Animator animator;
    private Transform L_Axis;
    private float n = 20f;
    private int i = 0;
    private float x, y;
    private Vector3 rota;
    private void Start()
    {
        animator = GetComponent<Animator>();
        Transform[] G = GetComponentsInChildren<Transform>();
        foreach (var child in G)
        {
            if (child.name == "Axis")
            {
                L_Axis = child;
            }
        }
    }

    void LateUpdate()
    {
        #region Left Hand
        if (Input.GetAxis("XRI_Left_PrimaryButton") != 0)
        {
            animator.Play("L_X", 3, 0.5f);
        }
        else animator.Play("L_X", 3, 0f);

        if (Input.GetAxis("XRI_Left_SecondaryButton") != 0)
        {
            animator.Play("L_Y", 4, 0.5f);
        }
        else animator.Play("L_Y", 4, 0f);

        if (Input.GetAxis("XRI_Left_GripButton") != 0)
        {
            animator.Play("L_Grip", 1, 0.5f);
        }
        else animator.Play("L_Default", 0, 0f);

        if (Input.GetAxis("XRI_Left_MenuButton") != 0)
        {
            animator.Play("L_Menu", 5, 0.5f);
        }
        else animator.Play("L_Menu", 5, 0f);

        if (Input.GetAxis("XRI_Left_Grip") != 0)
        {
            animator.Play("L_Grip", 1, Input.GetAxis("XRI_Left_Grip") / 2);
        }
        else animator.Play("L_Grip", 1, 0f);

        if (Input.GetAxis("XRI_Left_Trigger") != 0)
        {
            animator.SetFloat("Trigger", Input.GetAxis("XRI_Left_Trigger") / 2);
            animator.Play("L_Trigger", 2, Input.GetAxis("XRI_Left_Trigger") / 2);
        }
        else animator.Play("L_Trigger", 2, 0f);

        if (Input.GetAxis("XRI_Left_Primary2DAxis_Horizontal") != 0 || (Input.GetAxis("XRI_Left_Primary2DAxis_Vertical") != 0))
        {
            if (Input.GetAxis("XRI_Left_Primary2DAxis_Horizontal") != 0)
            {
                y = -(Input.GetAxis("XRI_Left_Primary2DAxis_Horizontal") * n);
            }
            else y = 0;
            if (Input.GetAxis("XRI_Left_Primary2DAxis_Vertical") != 0)
            {
                x = (Input.GetAxis("XRI_Left_Primary2DAxis_Vertical") * n);
            }
            else x = 0;
        }
        else x = y = 0;
        rota = new Vector3(x, y, 0);
        L_Axis.transform.localEulerAngles = rota;
        #endregion
    }
}