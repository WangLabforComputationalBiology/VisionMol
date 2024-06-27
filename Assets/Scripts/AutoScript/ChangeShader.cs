using UnityEngine;

public class ChangeShader : MonoBehaviour
{
    private const string SHADER_NAME = "Shaders/Custom/UnlitSurfaceVertexColorNotCullNoZTest";

    private void Start()
    {
        // 找到 "BondLineRepresentation" 游戏对象
        GameObject bondLineRepresentation = GameObject.Find("BondLineRepresentation");

        if (bondLineRepresentation != null)
        {
            // 获取 "BondLineRepresentation" 的所有子对象
            Transform[] childTransforms = bondLineRepresentation.GetComponentsInChildren<Transform>();

            // 加载自定义着色器
            Shader newShader = Shader.Find(SHADER_NAME);

            if (newShader != null)
            {
                foreach (Transform childTransform in childTransforms)
                {
                    // 检查子对象的名称是否包含 "BondLineMesh"
                    if (childTransform.name.Contains("BondLineMesh"))
                    {
                        // 获取子对象的 MeshRenderer 组件
                        MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();

                        if (meshRenderer != null)
                        {
                            // 创建一个新的材质实例
                            Material newMaterial = new Material(newShader);

                            // 将新材质应用到 MeshRenderer 组件
                            meshRenderer.material = newMaterial;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError($"Failed to load shader: {SHADER_NAME}");
            }
        }
    }
}