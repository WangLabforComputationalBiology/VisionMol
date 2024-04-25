using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float rotationSpeed = 60f; // 旋转速度,单位为度/秒
    public Vector3 rotationAxis = Vector3.up; // 旋转轴,默认为 Y 轴

    private void Update()
    {
        // 计算旋转角度
        float rotationAngle = rotationSpeed * Time.deltaTime;

        // 创建一个四元数,表示绕指定轴旋转的角度
        Quaternion rotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);

        // 将当前游戏对象的旋转应用于该四元数
        transform.rotation = rotation * transform.rotation;
    }
}