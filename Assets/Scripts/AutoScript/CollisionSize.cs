using System.Collections;
using System.Collections.Generic;
using UMol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.Threading;
public class CollisionSize : MonoBehaviour
{   
    public Slider radiusSlider; // Slider 组件的引用
    public GameObject Sphere; // Sphere 游戏对象的引用
    private void Start()
    {
         
        radiusSlider.minValue = 1f; // 最小半径为 1
        radiusSlider.maxValue = 10.0f; // 最大半径为 10
        Sphere.SetActive(false);
        // 添加 Slider 值更改事件的监听
        radiusSlider.onValueChanged.AddListener(UpdateSphereRadius);
    }


    
    private void UpdateSphereRadius(float value)
    {
        Sphere.SetActive(true);
        Sphere.transform.localScale = new Vector3(0.1f*value, 0.1f * value, 0.1f * value);
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
        Transform parentTransform = selTransform?.Find("AtomCartoonRepresentation");
        if (parentTransform != null)
        {
            foreach (Transform childTransform in parentTransform)
            {
                string childName = childTransform.name;
                GameObject modelObject = childTransform.gameObject;
                SphereCollider sphereCollider = modelObject.GetComponent<SphereCollider>();
                sphereCollider.radius = value;
                Sphere.transform.position = sphereCollider.transform.TransformPoint(sphereCollider.center);

                //sphereCollider.radius = radiusSlider.value;
                //Sphere.transform.position = sphereCollider.center;
                //Sphere.transform.localScale = new Vector3(radiusSlider.value, radiusSlider.value, radiusSlider.value);              
            }
        }
        else
        {
            Debug.LogWarning("Cannot find parent game object: AtomCartoonRepresentation");
        }
        Transform parentTransformL = selTransform?.Find("BondLineRepresentation");
        if (parentTransform != null)
        {
            foreach (Transform childTransform in parentTransform)
            {
                string childName = childTransform.name;
                GameObject modelObject = childTransform.gameObject;
                SphereCollider sphereCollider = modelObject.GetComponent<SphereCollider>();
                sphereCollider.radius = value;
                Sphere.transform.position = sphereCollider.transform.TransformPoint(sphereCollider.center);             
            }
        }
        else
        {
            Debug.LogWarning("Cannot find parent game object: AtomCartoonRepresentation");
        }
        

    }
    private void Update()
    {
        GameObject sizesa = GameObject.Find("LoadedMolecules");
        sizesa.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

}
    