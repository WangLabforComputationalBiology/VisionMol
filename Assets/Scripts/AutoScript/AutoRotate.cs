using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float rotationSpeed = 60f; // ��ת�ٶ�,��λΪ��/��
    public Vector3 rotationAxis = Vector3.up; // ��ת��,Ĭ��Ϊ Y ��

    private void Update()
    {
        // ������ת�Ƕ�
        float rotationAngle = rotationSpeed * Time.deltaTime;

        // ����һ����Ԫ��,��ʾ��ָ������ת�ĽǶ�
        Quaternion rotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);

        // ����ǰ��Ϸ�������תӦ���ڸ���Ԫ��
        transform.rotation = rotation * transform.rotation;
    }
}