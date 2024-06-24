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
    //public Slider radiusSlider; // Slider 组件的引用
    //public GameObject Sphere; // Sphere 游戏对象的引用
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
           

            // 创建球体
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            // 将球体设置为modelObject的子物体
            sphere.transform.parent = modelObject.transform;
            float radius = 6f;
            sphere.transform.localScale = new Vector3(radius, radius, radius);
            Renderer sphereRenderer = sphere.GetComponent<Renderer>();

            // 创建透明材质
            Material transparentMaterial = new Material(Shader.Find("Transparent/Diffuse"));

            // 设置透明度
            float alpha = 0.5f; // 范围从0（完全透明）到1（完全不透明）
            Color color = sphereRenderer.material.color;
            color.a = alpha;
            transparentMaterial.color = color;

            // 将透明材质应用于球体
            sphereRenderer.material = transparentMaterial;
            sphereCollider = modelObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 3f;
            sphereCollider.center = sphere.transform.localPosition;

            Rigidbody rigidbody = modelObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            GameObject rayGrabInteraction = new GameObject("RayGrabInteraction");
            // 将rayGrabInteraction设置为modelObject的子物体
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
