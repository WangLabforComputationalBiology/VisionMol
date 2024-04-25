using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformReset : MonoBehaviour
{
    List<Rigidbody> gameobjects = new List<Rigidbody>();
    Dictionary<Rigidbody, Vector3> vector3 = new Dictionary<Rigidbody, Vector3>();
    Dictionary<Rigidbody, Quaternion> quaternions = new Dictionary<Rigidbody, Quaternion>();
    void Awake()
    {
        Transform[] G = GetComponentsInChildren<Transform>();
        foreach (var child in G)
        {
            Rigidbody rigidbody = child.GetComponent<Rigidbody>();
            if (rigidbody!=null)
            {
                gameobjects.Add(rigidbody);
                Vector3 pos = new Vector3();
                Quaternion rot = new Quaternion();
                pos = child.position;
                rot = child.rotation;
                vector3.Add(rigidbody, pos);
                quaternions.Add(rigidbody, rot);
            }
        }
    }
    void Update()
    {
        if (Input.GetAxis("XRI_Right_MenuButton") != 0)
        {
            ResetTransform();
        }
    }
    public void ResetTransform()
    {
        foreach (var item in gameobjects)
        {
            item.MovePosition(vector3[item]);
            item.MoveRotation(quaternions[item]);
            item.velocity = Vector3.zero;
        }
    }
}
