using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
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
    public int _myCurrent = -1;

    //用于保存名称
    public string[] MyloadedStructures = new string[3] { "Protein_1", "Protein_2", "Protein_3" };
    //用于切换判定
    public int[] SwitchNumber = new int[3] { 1, 1, 1 };

    //创建数组指向蛋白质切换按钮
    public Toggle[] SwitchProtein = new Toggle[3];
    //指向蛋白质显示按钮
    public Toggle[] DisplayProtein = new Toggle[3];


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

        MyCurrent = sm.loadedStructures.Count - 1;

        if (MyCurrent != _myCurrent)
        {
            //监听是否有添加蛋白质
            _myCurrent = MyCurrent;
            //保存蛋白质的名称到数组中
            UnityMolStructure s = sm.GetCurrentStructure();
            MyloadedStructures[MyCurrent] = s.name;

            //切换文本名称到toggle按钮
            if (MyCurrent < MaxCurrent)
            {
                if (SwitchProtein[MyCurrent] != null)
                {
                    // 获取与 Toggle 组件相关联的 Label 组件
                    //蛋白质切换toggle
                    Text label_1 = SwitchProtein[MyCurrent].GetComponentInChildren<Text>();
                    //蛋白质显示toggle
                    Text label_2 = DisplayProtein[MyCurrent].GetComponentInChildren<Text>();

                    if (label_1 != null)
                    {
                        // 设置 Label 组件的文本
                        label_1.text = MyloadedStructures[MyCurrent];
                    }

                    if (label_2 != null)
                    {
                        // 设置 Label 组件的文本
                        label_2.text = MyloadedStructures[MyCurrent];
                    }
                }
            }
        }
    }


    // Toggle 值改变事件处理方法,通过toggle的Text文本名称修改 *切换判定
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
                SwitchChange(label.text, changedToggle.isOn);
            }
        }
    }


    //通过toggle值改变事件，通过文本名称进行 *显示切换
    public void DisplayToggleValueChanged(Toggle displayToggle)
    {
        // 从 Toggle 的子项中获取 Label 组件
        Text label = displayToggle.GetComponentInChildren<Text>();

        if (label != null)
        {
            //提取label的Text文本
            string selName = Set_selName(label.text);

            GameObject childObject = FindChildObject(selName);

            if (childObject != null)
            {
                childObject.SetActive(displayToggle.isOn);
            }
        }
    }


    //修改切换判定的判定数字s
    public void SwitchChange(string name,bool toggle_isOn)
    {
        int index = GetIndex(MyloadedStructures, name);

        if (index != -1)
        {
            if (toggle_isOn == true)
            {
                SwitchNumber[index] = 1;
            }
            else
            {
                SwitchNumber[index] = 0;
            }
        }
    }

    //通过名称获取数组索引
    public int GetIndex(string[] array, string value)
    {
        return System.Array.IndexOf(array, value);
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


//相关脚本：APIPython，Label，ManagerScript
//需要添加的判定代码
/*
                 //以下为增加的引用代码
        GameObject pc = getProteinController();
        ProteinManager pm = getProteinManager(pc);
                //以上为增加的引用代码


                //增加的判定语句代码
        string SelName = pm.Set_selName(s.name);
        GameObject protein = pm.FindChildObject(SelName);

                if (pm.SwitchNumber[pm.GetIndex(pm.MyloadedStructures,s.name)] == 1)
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