using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMol;
using System.Security;
using UMol.API;
using System;
using UnityEngine.UI.Extensions.ColorPicker;

public class ColorByResidueButtonHandler : MonoBehaviour
{
    private UnityMolStructureManager sm;

    public void Cresidue()
    {
        sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        string selName =s.name ;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            APIPython.colorByResidue(selName, "c");
        }
        else
        {
            selName = "all(" + s.name + ")";
            APIPython.colorByResidue(selName, "c");
        }


    }
    public void Sresidue()
    {
        sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        string selName = s.name;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            APIPython.colorByResidue(selName, "s");
        }
        else
        {
            selName = "all(" + s.name + ")";
            APIPython.colorByResidue(selName, "s");
        }
    }
    public void HBresidue()
    {
        sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        string selName = s.name;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            APIPython.colorByResidue(selName, "hb");
        }
        else
        {
            selName = "all(" + s.name + ")";
            APIPython.colorByResidue(selName, "hb");
        }
    }
    public void Lineresidue() 
    {
        sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        string selName = s.name;
        if (selName.StartsWith("all(") && selName.EndsWith(")"))
        {
            APIPython.colorByResidue(selName, "l");
        }
        else
        {
            selName = "all(" + s.name + ")";
            APIPython.colorByResidue(selName, "l");
        }
    }
}