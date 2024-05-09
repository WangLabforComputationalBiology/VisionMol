using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UMol;
using UMol.API;
using UnityEngine;
using UnityEngine.UI;

public class ProteinManager : MonoBehaviour
{
    //创建MyloadedStructures数组和需要的参数
    public int MaxCurrent = 3;
    public int MyCurrent = 0;
    public int _myCurrent = 1;
    public string[] MyloadedStructures = new string[3] { "Protein_1","Protein_2","Protein_3"};

    //创建数组指向蛋白质切换按钮
    public Toggle[] SwitchProtein = new Toggle[3];


    //监听toggle按钮
    void Start()
    {
        foreach (Toggle toggle in SwitchProtein)
        {
            toggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(toggle);
            });
        }
    }


    //把蛋白质名称同步存入自建的MyloadedStructures，并把文本切换为蛋白质名称
    private void Update()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();

        //监听是否有添加蛋白质
        MyCurrent = sm.loadedStructures.Count - 1;

        if (MyCurrent != _myCurrent)
        {
            _myCurrent = MyCurrent;
            //保存蛋白质的名称到数组中
            MyloadedStructures[MyCurrent] = s.name;

            //切换文本名称到toggle按钮
            if (MyCurrent < MaxCurrent)
            {
                if (SwitchProtein[MyCurrent] != null)
                {
                    // 获取与 Toggle 组件相关联的 Label 组件
                    Text label = SwitchProtein[MyCurrent].GetComponentInChildren<Text>();

                    if (label != null)
                    {
                        // 设置 Label 组件的文本
                        label.text = MyloadedStructures[MyCurrent];
                    }
                }
            }
        }
    }

    /*
    //把蛋白质名称同步存入自建的MyloadedStructures，并把文本切换为蛋白质名称
    public void SaveProtein()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();

        //保存蛋白质的名称到数组中
        MyCurrent = sm.loadedStructures.Count-1;
        MyloadedStructures[MyCurrent] = s.name;

        //切换文本名称到toggle按钮
        if (MyCurrent < MaxCurrent)
        {
            if (SwitchProtein[MyCurrent] != null)
            {
                // 获取与 Toggle 组件相关联的 Label 组件
                Text label = SwitchProtein[MyCurrent].GetComponentInChildren<Text>();

                if (label != null)
                {
                    // 设置 Label 组件的文本
                    label.text = MyloadedStructures[MyCurrent];
                }
            }
        }
    }
    */


    // Toggle 值改变事件处理方法,通过toggle的Text文本名称修改激活状态
    public void ToggleValueChanged(Toggle changedToggle)
    {
        // 从 Toggle 的子项中获取 Label 组件
        Text label = changedToggle.GetComponentInChildren<Text>();

        if (label != null)
        {
            //提取label的Text文本
            string selName = Set_selName(label.text);

            GameObject childObject = FindChildObject(selName);

            if (childObject != null)
            {
                childObject.SetActive(changedToggle.isOn);
            }
        }
    }


    //格式化名称
    public string Set_selName(string name)
    {
        string selName = null;

        if (name.StartsWith("all(") && name.EndsWith(")"))
        {
            selName = name;
        }
        else
        {
            selName = "all(" + name + ")";
        }

        return selName;
    }

    //获取对应名称蛋白质子物体
    public GameObject FindChildObject(string selName)
    {
        GameObject childObject = null;

        //确保名称格式化
        selName = Set_selName(selName);

        // 通过名称获取父物体LoadedMolecules的引用
        GameObject loadedMolecules = GameObject.Find("LoadedMolecules");

        // 检查是否成功找到父物体
        if (loadedMolecules != null)
        {
            // 成功找到父物体，接下来通过名称找到父物体下对应toggle的子物体
            Transform childTransform = loadedMolecules.transform.Find(selName);

            // 检查是否成功找到子物体
            if (childTransform != null)
            {
                // 成功找到子物体
                childObject = childTransform.gameObject;
            }
        }

        return childObject;
    }
}

//需要添加的判定代码
/*
                 //以下为增加的引用代码
        GameObject pc = getProteinController();
        ProteinManager pm = getProteinManager(pc);
                //以上为增加的引用代码


                //增加的判定语句代码
        string SelName = pm.Set_selName(s.name);
        GameObject protein = pm.FindChildObject(SelName);

                if (protein.activeInHierarchy)
                {


                 //插入的原本的代码

                 //原本代码插入


                 }
                 //增加的判定代码
    
    
*/

//以下代码为在APIPython中的ShowAS方法的被判定代码
/*
                foreach (UnityMolRepresentation r in s.representations) {
                    r.Hide();
                }
 */
/*
                    List<UnityMolRepresentation> existingReps = repManager.representationExists(s.ToSelectionName(), repType);

                if (existingReps == null) {
                    repManager.AddRepresentation(s, repType.atomType, repType.bondType);
                }
                else {
                    foreach (UnityMolRepresentation existingRep in existingReps) {
                        existingRep.Show();
                    }
                }
 */

//APIPython脚本中的setHyperBallMetaphore方法
/*
                UnityMolSelection sel = selM.selections[selName];

            RepType repType = getRepType("hb");

            List<UnityMolRepresentation> existingReps = repManager.representationExists(selName, repType);
            if (existingReps != null) {
                foreach (UnityMolRepresentation existingRep in existingReps) {
                    foreach (SubRepresentation sr in existingRep.subReps) {
                        UnityMolHStickMeshManager hsManager = (UnityMolHStickMeshManager) sr.bondRepManager;
                        UnityMolHBallMeshManager hbManager = (UnityMolHBallMeshManager) sr.atomRepManager;
                        if (hsManager != null) {
                            hsManager.SetShrink(shrink);
                            hsManager.SetSizes(sel.atoms, scaleBond);
                        }

                        hbManager.SetSizes(sel.atoms, scaleAtom);
                        if (doAO) {
                            hbManager.computeAO();
                        }
                    }
                }
            }
 */