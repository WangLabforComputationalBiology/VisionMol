using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMol;
using System.Security;
using UMol.API;
using System;
using UnityEngine.UI.Extensions.ColorPicker;
using UnityEngine.Events;


public class ManagerScript : MonoBehaviour
{

    private Button CartoonssButton;



    private bool isHydrogenVisible = true; // 初始状态为可见


    //获得编写的ProteinManager脚本引用
    public static GameObject getProteinController()
    {
        GameObject ProteinController = null;

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform proteinController = canvas.transform.Find("ProteinController");

            if (proteinController != null)
            {
                ProteinController = proteinController.gameObject;
            }
        }

        return ProteinController;
    }
    public static ProteinManager getProteinManager(GameObject pc)
    {
        return pc.GetComponent<ProteinManager>();
    }


    public void Showbondorder()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName = "all(" + s.name + ")";
        string selName = s.name;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            APIPython.showSelection(selName, "bondorder");
        }
        else
        {
            selName = "all(" + s.name + ")";
            APIPython.showSelection(selName, "bondorder");

        }
    }
    public void ShowHideHydrogen()
    {
        GameObject find = GameObject.Find("AtomOptiHBRepresentation/AtomMesh0");

        if (find != null)
        {
            Renderer renderer = find.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = !isHydrogenVisible; // 切换模型的可见性
                isHydrogenVisible = !isHydrogenVisible; // 更新布尔值
            }
        }
    }
    public void cealignLasts()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        int N = sm.loadedStructures.Count;
        Debug.Log(N);
        if (N < 2)
        {
            Debug.LogWarning("Not enough structures to align");
            return;
        }
        string s1 = sm.loadedStructures[N - 2].ToSelectionName();
        string s2 = sm.loadedStructures[N - 1].ToSelectionName();
        Debug.Log(s1 + s2);
        APIPython.cealign(s1, s2);
    }
    public void HelixCol()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName="all("+s.name+")";
        string selName = s.name ;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            CartoonssButton = GameObject.Find("RepOptions/RowC/Options/ColorSSButtons/ColorHelix").GetComponent<Button>();
            ColorPickerControl cartoonHelixColorPicker = GameObject.Find("RepOptions/RowC/Options/HelixPicker").gameObject.GetComponent<ColorPickerControl>();
            cartoonHelixColorPicker.onValueChanged.AddListener(
            delegate {
                APIPython.setCartoonColorSS(selName, "helix", cartoonHelixColorPicker.CurrentColor);
            });
        }
        else
        {
            selName = "all(" + s.name + ")";
            CartoonssButton = GameObject.Find("RepOptions/RowC/Options/ColorSSButtons/ColorHelix").GetComponent<Button>();
            ColorPickerControl cartoonHelixColorPicker = GameObject.Find("RepOptions/RowC/Options/HelixPicker").gameObject.GetComponent<ColorPickerControl>();
            cartoonHelixColorPicker.onValueChanged.AddListener(
            delegate {
                APIPython.setCartoonColorSS(selName, "helix", cartoonHelixColorPicker.CurrentColor);
            });
        }
        Debug.Log("Coil" + s.name);
    }
    public void SheetCol()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName = "all(" + s.name + ")";
        string selName = s.name ;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            Debug.Log(selName);
            CartoonssButton = GameObject.Find("RepOptions/RowC/Options/ColorSSButtons/ColorSheet").GetComponent<Button>();
            ColorPickerControl cartoonHelixColorPicker = GameObject.Find("RepOptions/RowC/Options/SheetPicker").gameObject.GetComponent<ColorPickerControl>();
            cartoonHelixColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.setCartoonColorSS(selName, "sheet", cartoonHelixColorPicker.CurrentColor);
            });
        }
        else
        {
            selName = "all(" + s.name + ")";
            CartoonssButton = GameObject.Find("RepOptions/RowC/Options/ColorSSButtons/ColorSheet").GetComponent<Button>();
            ColorPickerControl cartoonHelixColorPicker = GameObject.Find("RepOptions/RowC/Options/SheetPicker").gameObject.GetComponent<ColorPickerControl>();
            cartoonHelixColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.setCartoonColorSS(selName, "sheet", cartoonHelixColorPicker.CurrentColor);
            });
        }
        Debug.Log("Sheet" + s.name);
    }
    public void ShowseleC()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName = "all(" + s.name + ")";
        string selName = s.name;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {

        }else {
            selName = "all(" + s.name + ")";
        }

        APIPython.deleteRepresentationInSelection(selName, "hb");
    }
    public void ColiCol()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName = "all(" + s.name + ")";
        string selName = s.name;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            CartoonssButton = GameObject.Find("RepOptions/RowC/Options/ColorSSButtons/ColorCoil").GetComponent<Button>();
            ColorPickerControl cartoonHelixColorPicker = GameObject.Find("RepOptions/RowC/Options/CoilPicker").gameObject.GetComponent<ColorPickerControl>();
            cartoonHelixColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.setCartoonColorSS(selName, "coil", cartoonHelixColorPicker.CurrentColor);
            });
        }
        else
        {
            selName = "all(" + s.name + ")";
            CartoonssButton = GameObject.Find("RepOptions/RowC/Options/ColorSSButtons/ColorCoil").GetComponent<Button>();
            ColorPickerControl cartoonHelixColorPicker = GameObject.Find("RepOptions/RowC/Options/CoilPicker").gameObject.GetComponent<ColorPickerControl>();
            cartoonHelixColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.setCartoonColorSS(selName, "coil", cartoonHelixColorPicker.CurrentColor);
            });
        }
    }  
   
    public void ResetColor(Button newButtonGo)
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName = "all(" + s.name + ")";
        string selName = s.name;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            newButtonGo.onClick.AddListener(
            delegate {
                switch (newButtonGo.name)
                {
                    case "ResetColorL": APIPython.resetColorSelection(selName, "l");
                        break;
                    case "ResetColorC": APIPython.resetColorSelection(selName, "c");
                        break;
                    case "ResetColorHB": APIPython.resetColorSelection(selName, "hb");
                        break;
                    case "ResetColorS": APIPython.resetColorSelection(selName, "s");
                        break;
                }
            });
        }
        else
        {
            selName = "all(" + s.name + ")";
            newButtonGo.onClick.AddListener(
            delegate {
                switch (newButtonGo.name)
                {
                    case "ResetColorL":
                        APIPython.resetColorSelection(selName, "l");
                        break;
                    case "ResetColorC":
                        APIPython.resetColorSelection(selName, "c");
                        break;
                    case "ResetColorHB":
                        APIPython.resetColorSelection(selName, "hb");
                        break;
                    case "ResetColorS":
                        APIPython.resetColorSelection(selName, "s");
                        break;
                }
            });
        }

    }

    public void Linecolor()
    {
        //以下为增加的引用代码
        GameObject pc = getProteinController();
        ProteinManager pm = getProteinManager(pc);
        //以上为增加的引用代码

        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        ColorPickerControl lineColorPicker = GameObject.Find("RepOptions/RowLine/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

        foreach (UnityMolStructure s in sm.loadedStructures)
        {
            string selName = pm.Set_selName(s.name);
            GameObject protein = pm.FindChildObject(selName);

            if (protein.activeInHierarchy)
            {
                lineColorPicker.onValueChanged.AddListener(
                delegate
                {
                    // 在回调函数中增加对 protein 对象当前状态的检查
                    if (protein.activeInHierarchy)
                    {
                        APIPython.colorSelection(selName, "l", lineColorPicker.CurrentColor);
                    }
                });
            }
        }
    }

    /*
    public void Linecolor()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName="all("+s.name+")";
        string selName = s.name;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            ColorPickerControl lineColorPicker = GameObject.Find("RepOptions/RowLine/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

            lineColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.colorSelection(selName, "l", lineColorPicker.CurrentColor);
            });
        }
        else
        {
            selName = "all(" + s.name + ")";
            ColorPickerControl lineColorPicker = GameObject.Find("RepOptions/RowLine/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

            lineColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.colorSelection(selName, "l", lineColorPicker.CurrentColor);
            });
        }
    }
     */
    public void Surfacecolor()
    {
        //以下为增加的引用代码
        GameObject pc = getProteinController();
        ProteinManager pm = getProteinManager(pc);
        //以上为增加的引用代码

        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        ColorPickerControl surColorPicker = GameObject.Find("RepOptions/RowS/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

        foreach (UnityMolStructure s in sm.loadedStructures)
        {
            string selName = pm.Set_selName(s.name);
            GameObject protein = pm.FindChildObject(selName);

            if (protein.activeInHierarchy)
            {
                surColorPicker.onValueChanged.AddListener(
                delegate
                {
                    // 在回调函数中增加对 protein 对象当前状态的检查
                    if (protein.activeInHierarchy)
                    {
                        APIPython.colorSelection(selName, "s", surColorPicker.CurrentColor);
                    }
                });
            }
        }
    }

    /*
    public void Surfacecolor()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName="all("+s.name+")";
        string selName = s.name;
        Debug.Log(selName);
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            ColorPickerControl surColorPicker = GameObject.Find("RepOptions/RowS/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

            surColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.colorSelection(selName, "s", surColorPicker.CurrentColor);
            });
        }
        else
        {
            selName = "all(" + s.name + ")";
            ColorPickerControl surColorPicker = GameObject.Find("RepOptions/RowS/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

            surColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.colorSelection(selName, "s", surColorPicker.CurrentColor);
            });
        }
    }
     */

    public void Cartooncolor()
    {
        //以下为增加的引用代码
        GameObject pc = getProteinController();
        ProteinManager pm = getProteinManager(pc);
        //以上为增加的引用代码

        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        ColorPickerControl CColorPicker = GameObject.Find("RepOptions/RowC/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

        foreach (UnityMolStructure s in sm.loadedStructures)
        {
            string selName = pm.Set_selName(s.name);
            GameObject protein = pm.FindChildObject(selName);

            if (protein.activeInHierarchy)
            {
                CColorPicker.onValueChanged.AddListener(
                delegate
                {
                    // 在回调函数中增加对 protein 对象当前状态的检查
                    if (protein.activeInHierarchy)
                    {
                        APIPython.colorSelection(selName, "c", CColorPicker.CurrentColor);
                    }
                });
            }
        }
    }


    /*
    public void Cartooncolor()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName="all("+s.name+")";
        string selName = s.name;
        Debug.Log(selName);
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            ColorPickerControl CColorPicker = GameObject.Find("RepOptions/RowC/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

            CColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.colorSelection(selName, "c", CColorPicker.CurrentColor);
            });
        }
        else
        {
            selName = "all(" + s.name + ")";
            ColorPickerControl CColorPicker = GameObject.Find("RepOptions/RowC/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

            CColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.colorSelection(selName, "c", CColorPicker.CurrentColor);
            });
        }
    }
     */


    public void HBcolor()
    {
        //以下为增加的引用代码
        GameObject pc = getProteinController();
        ProteinManager pm = getProteinManager(pc);
        //以上为增加的引用代码

        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        ColorPickerControl HBColorPicker = GameObject.Find("RepOptions/RowHB/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

        foreach (UnityMolStructure s in sm.loadedStructures)
        {
            string selName = pm.Set_selName(s.name);
            GameObject protein = pm.FindChildObject(selName);

            if (protein.activeInHierarchy)
            {
                HBColorPicker.onValueChanged.AddListener(
                delegate
                {
                    // 在回调函数中增加对 protein 对象当前状态的检查
                    if (protein.activeInHierarchy)
                    {
                        APIPython.colorSelection(selName, "hb", HBColorPicker.CurrentColor);
                    }
                });
            }
        }
    }


    /*
    public void HBcolor()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        //string selName="all("+s.name+")";
        string selName = s.name;
        Debug.Log(selName);
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            ColorPickerControl HBColorPicker = GameObject.Find("RepOptions/RowHB/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

            HBColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.colorSelection(selName, "hb", HBColorPicker.CurrentColor);
            });
        }
        else
        {
            selName = "all(" + s.name + ")";
            ColorPickerControl HBColorPicker = GameObject.Find("RepOptions/RowHB/ColorPicker").gameObject.GetComponent<ColorPickerControl>();

            HBColorPicker.onValueChanged.AddListener(
            delegate
            {
                APIPython.colorSelection(selName, "hb", HBColorPicker.CurrentColor);
            });
        }
    }
     */
}



