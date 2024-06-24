using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using UMol;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MolecularDocking2 : MonoBehaviour
{
    //public Slider radiusSlider; // Slider ���������
    //public GameObject Sphere; // Sphere ��Ϸ���������
    public void Garb()
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
        GameObject modelObject = GameObject.Find(selName);
        SphereCollider sphereCollider = modelObject.GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
           

            // ��������
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            // ����������ΪmodelObject��������
            sphere.transform.parent = modelObject.transform;
            float radius = 6f;
            sphere.transform.localScale = new Vector3(radius, radius, radius);
            Renderer sphereRenderer = sphere.GetComponent<Renderer>();

            // ����͸������
            Material transparentMaterial = new Material(Shader.Find("Transparent/Diffuse"));

            // ����͸����
            float alpha = 0.5f; // ��Χ��0����ȫ͸������1����ȫ��͸����
            Color color = sphereRenderer.material.color;
            color.a = alpha;
            transparentMaterial.color = color;

            // ��͸������Ӧ��������
            sphereRenderer.material = transparentMaterial;
            sphereCollider = modelObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 3f;
            sphereCollider.center = sphere.transform.localPosition;

            Rigidbody rigidbody = modelObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            GameObject rayGrabInteraction = new GameObject("RayGrabInteraction");
            // ��rayGrabInteraction����ΪmodelObject��������
            rayGrabInteraction.transform.parent = modelObject.transform;
            MoveFromTargetProvider moveFromTargetProvider = rayGrabInteraction.AddComponent<MoveFromTargetProvider>();
            GrabFreeTransformer grabFreeTransformer = rayGrabInteraction.AddComponent<GrabFreeTransformer>();
            Grabbable grabbable = rayGrabInteraction.AddComponent<Grabbable>();
            RayInteractable rayInteractable = rayGrabInteraction.AddComponent<RayInteractable>();
            ColliderSurface surface = rayGrabInteraction.AddComponent<ColliderSurface>();

            grabbable.InjectOptionalTargetTransform(modelObject.transform);
            grabbable.InjectOptionalRigidbody(rigidbody);
            surface.InjectCollider(sphereCollider);
            rayInteractable.InjectOptionalPointableElement(grabbable);
            rayInteractable.InjectSurface(surface);
            rayInteractable.InjectOptionalMovementProvider(moveFromTargetProvider);
        }
    }
    
}
