using System.Collections;
using System.Collections.Generic;
using UMol;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class MolecularDocking : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}
    public void GetCartoon()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName = "all(" + s.name + ")";
        string selName = s.name;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {

        }
        else
        {
            selName = "all(" + s.name + ")";
        }
        Debug.Log(selName) ;
        Transform selTransform = GameObject.Find(selName)?.transform;
        Transform parentTransform = selTransform?.Find("AtomCartoonRepresentation");
        if (parentTransform != null)
        {
            foreach (Transform childTransform in parentTransform)
            {
                string childName = childTransform.name;
                GameObject modelObject = childTransform.gameObject;
                SphereCollider sphereCollider = modelObject.AddComponent<SphereCollider>();
                sphereCollider.radius = 10f;
                XRGrabInteractable grabInteractable=modelObject.AddComponent<XRGrabInteractable>();
                grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
                Rigidbody rigidbody= modelObject.GetComponent<Rigidbody>();
                rigidbody.useGravity=false;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rigidbody.drag = 10f;
                rigidbody.angularDrag = 2;
            }
        }
        else
        {
            Debug.LogWarning("Cannot find parent game object: AtomCartoonRepresentation");
        }
    }
    public void GetLine()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName = "all(" + s.name + ")";
        string selName = s.name;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {

        }
        else
        {
            selName = "all(" + s.name + ")";
        }
        Debug.Log(selName);
        Transform selTransform = GameObject.Find(selName)?.transform;
        Transform parentTransform = selTransform?.Find("BondLineRepresentation");
        if (parentTransform != null)
        {
            foreach (Transform childTransform in parentTransform)
            {
                string childName = childTransform.name;
                GameObject modelObject = childTransform.gameObject;
                SphereCollider sphereCollider = modelObject.AddComponent<SphereCollider>();
                sphereCollider.radius = 10f;
                XRGrabInteractable grabInteractable = modelObject.AddComponent<XRGrabInteractable>();
                grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
                Rigidbody rigidbody = modelObject.GetComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rigidbody.drag = 10f;
                rigidbody.angularDrag = 2;
            }
        }
        else
        {
            Debug.LogWarning("Cannot find parent game object: AtomCartoonRepresentation");
        }
    }
    // Update is called once per frame
    //void Update()
    //{

    //}
}
