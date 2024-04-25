using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMol;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

public class Label : MonoBehaviour
{
    public TMP_Dropdown loadedLabelTMP_Dropdown;
    public Slider sizeSlider;
    private List<Transform> parentTs;
    private List<TextMeshPro> textList;
    TextMeshPro text;

    private void Start()
    {
        parentTs = new List<Transform>();
        textList = new List<TextMeshPro>();
    }


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
    //以上为增加的代码



    public void LabelClear()
    {
        for (int i = textList.Count - 1; i >= 0; i--)
        {
            Destroy(textList[i].gameObject);
            textList.RemoveAt(i);
        }
        for (int i = parentTs.Count - 1; i >= 0; i--)
        {
            Destroy(parentTs[i].gameObject);
            parentTs.RemoveAt(i);
        }
        for (int i = loadedLabelTMP_Dropdown.options.Count - 1; i >= 0; i--)
        {
            if (loadedLabelTMP_Dropdown.options[i].text != "All")
                loadedLabelTMP_Dropdown.options.RemoveAt(i);
        }
    }

    public void LabelResidues()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        

        //添加循环使标签修改每个蛋白质模型
        foreach(UnityMolStructure s in sm.loadedStructures)
        {
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
                List<UnityMolModel> ms = s.models;

                Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
                Transform sT = loadedMolT.Find(s.ToSelectionName());

                Transform residueTextT = new GameObject("ResidueText").transform;
                parentTs.Add(residueTextT);
                residueTextT.SetParent(sT, false);
                foreach (UnityMolModel m in ms)
                {
                    foreach (UnityMolChain chain in m.chains.Values)
                    {
                        foreach (UnityMolResidue residue in chain.residues.Values)
                        {
                            Vector3 textPos = residue.allAtoms[0].position;
                            string textStr = residue.getResidueName3() + "-" + residue.id;
                            GameObject textGo = new GameObject(residue.getResidueName3() + "-" + residue.id);

                            textGo.transform.SetParent(residueTextT.transform, false);
                            text = textGo.AddComponent<TextMeshPro>();
                            SetText(text, textStr, textPos);

                            AddLoadedLabel(residue.getResidueName3());
                        }

                    }
                }
                //原本代码插入


            }
            //增加的判定代码
        }
    }

    
    /*  代码备份，只能对最新的对象进行标签的修改
    public void LabelResidues()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        List<UnityMolModel> ms = s.models;

        Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
        Transform sT = loadedMolT.Find(s.ToSelectionName());

        Transform residueTextT = new GameObject("ResidueText").transform;
        parentTs.Add(residueTextT);
        residueTextT.SetParent(sT, false);
        foreach (UnityMolModel m in ms)
        {
            foreach (UnityMolChain chain in m.chains.Values)
            {
                foreach (UnityMolResidue residue in chain.residues.Values)
                {
                    Vector3 textPos = residue.allAtoms[0].position;
                    string textStr = residue.getResidueName3() + "-" + residue.id;
                    GameObject textGo = new GameObject(residue.getResidueName3() + "-" + residue.id);

                    textGo.transform.SetParent(residueTextT.transform, false);
                    text = textGo.AddComponent<TextMeshPro>();
                    SetText(text, textStr, textPos);

                    AddLoadedLabel(residue.getResidueName3());
                }

            }
        }
    }
     */


    public void LabelResiduesOneLetter()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();

        //遍历加载的所有模型
        foreach (UnityMolStructure s in sm.loadedStructures)
        {
            //以下为增加的引用代码
            GameObject pc = getProteinController();
            ProteinManager pm = getProteinManager(pc);
            //以上为增加的引用代码


            //增加的判定语句代码
            string SelName = pm.Set_selName(s.name);
            GameObject protein = pm.FindChildObject(SelName);

            if (protein.activeInHierarchy)
            {
                
                //插入原本代码
                List<UnityMolModel> ms = s.models;

                Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
                Transform sT = loadedMolT.Find(s.ToSelectionName());

                Transform residueLabelT = new GameObject("ResidueLabel").transform;
                parentTs.Add(residueLabelT);
                residueLabelT.SetParent(sT, false);
                foreach (UnityMolModel m in ms)
                {
                    foreach (UnityMolChain chain in m.chains.Values)
                    {
                        foreach (UnityMolResidue residue in chain.residues.Values)
                        {
                            Vector3 textPos = residue.allAtoms[0].position;
                            string textStr = residue.getResidueName1() + residue.id;
                            GameObject textGo = new GameObject(residue.getResidueName1() + residue.id);

                            textGo.transform.SetParent(residueLabelT.transform, false);
                            text = textGo.AddComponent<TextMeshPro>();

                            SetText(text, textStr, textPos);

                            AddLoadedLabel(residue.getResidueName1());
                        }

                    }
                }
                //插入原本代码

            }
        }
    }


    /* 代码备份，只能对最新的对象进行标签的修改
    public void LabelResiduesOneLetter()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();
        List<UnityMolModel> ms = s.models;

        Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
        Transform sT = loadedMolT.Find(s.ToSelectionName());

        Transform residueLabelT = new GameObject("ResidueLabel").transform;
        parentTs.Add(residueLabelT);
        residueLabelT.SetParent(sT, false);
        foreach (UnityMolModel m in ms)
        {
            foreach (UnityMolChain chain in m.chains.Values)
            {
                foreach (UnityMolResidue residue in chain.residues.Values)
                {
                    Vector3 textPos = residue.allAtoms[0].position;
                    string textStr = residue.getResidueName1() + residue.id;
                    GameObject textGo = new GameObject(residue.getResidueName1() + residue.id);

                    textGo.transform.SetParent(residueLabelT.transform, false);
                    text = textGo.AddComponent<TextMeshPro>();

                    SetText(text, textStr, textPos);

                    AddLoadedLabel(residue.getResidueName1());
                }

            }
        }
    }
     */

    public void LabelChains()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();

        //遍历加载的所有模型
        foreach (UnityMolStructure s in sm.loadedStructures)
        {
            //以下为增加的引用代码
            GameObject pc = getProteinController();
            ProteinManager pm = getProteinManager(pc);
            //以上为增加的引用代码


            //增加的判定语句代码
            string SelName = pm.Set_selName(s.name);
            GameObject protein = pm.FindChildObject(SelName);

            if (protein.activeInHierarchy)
            {
                //插入原本代码
                List<UnityMolModel> ms = s.models;

                Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
                Transform sT = loadedMolT.Find(s.ToSelectionName());

                Transform chainLabelT = new GameObject("ChainLabel").transform;
                parentTs.Add(chainLabelT);
                chainLabelT.SetParent(sT, false);

                foreach (UnityMolModel m in ms)
                {
                    foreach (UnityMolChain chain in m.chains.Values)
                    {
                        List<UnityMolAtom> atomsList = chain.allAtoms;
                        Vector3 textPos1 = atomsList.First().position;
                        Vector3 textPos2 = atomsList.Last().position;
                        string textStr = "chain " + chain.name;
                        GameObject textGo1 = new GameObject("chain " + chain.name);
                        GameObject textGo2 = new GameObject("chain " + chain.name);

                        textGo1.transform.SetParent(chainLabelT.transform, false);
                        textGo2.transform.SetParent(chainLabelT.transform, false);
                        text = textGo1.AddComponent<TextMeshPro>();
                        SetText(text, textStr, textPos1);

                        text = textGo2.AddComponent<TextMeshPro>();
                        SetText(text, textStr, textPos2);

                        AddLoadedLabel("chain " + chain.name);
                    }
                }
            }
        }
    }


    /*代码备份，只能对最新的对象进行标签的修改
            public void LabelChains()
        {
            LabelClear();
            UnityMolStructureManager sm = UnityMolMain.getStructureManager();
            UnityMolStructure s = sm.GetCurrentStructure();
            List<UnityMolModel> ms = s.models;

            Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
            Transform sT = loadedMolT.Find(s.ToSelectionName());

            Transform chainLabelT = new GameObject("ChainLabel").transform;
            parentTs.Add(chainLabelT);
            chainLabelT.SetParent(sT, false);

            foreach (UnityMolModel m in ms)
            {
                foreach (UnityMolChain chain in m.chains.Values)
                {
                    List<UnityMolAtom> atomsList = chain.allAtoms;
                    Vector3 textPos1 = atomsList.First().position;
                    Vector3 textPos2 = atomsList.Last().position;
                    string textStr = "chain " + chain.name;
                    GameObject textGo1 = new GameObject("chain " + chain.name);
                    GameObject textGo2 = new GameObject("chain " + chain.name);

                    textGo1.transform.SetParent(chainLabelT.transform, false);
                    textGo2.transform.SetParent(chainLabelT.transform, false);
                    text = textGo1.AddComponent<TextMeshPro>();
                    SetText(text, textStr, textPos1);

                    text = textGo2.AddComponent<TextMeshPro>();
                    SetText(text, textStr, textPos2);

                    AddLoadedLabel("chain " + chain.name);
                }
            }
        }
     */


    public void LabelAtomName()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager(); //结构管理器

        foreach (UnityMolStructure s in sm.loadedStructures)
        {
            //以下为增加的引用代码
            GameObject pc = getProteinController();
            ProteinManager pm = getProteinManager(pc);
            //以上为增加的引用代码


            //增加的判定语句代码
            string SelName = pm.Set_selName(s.name);
            GameObject protein = pm.FindChildObject(SelName);

            if (protein.activeInHierarchy)
            {
                //插入原本代码
                Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
                Transform sT = loadedMolT.Find(s.ToSelectionName());
                Transform atomNameLabelT = new GameObject("AtomNameLabel").transform;
                parentTs.Add(atomNameLabelT);
                atomNameLabelT.SetParent(sT, false);

                List<UnityMolModel> ms = s.models;
                foreach (UnityMolModel m in ms)
                {
                    foreach (UnityMolChain chain in m.chains.Values)
                    {
                        foreach (UnityMolResidue residue in chain.residues.Values)
                        {
                            foreach (UnityMolAtom atom in residue.allAtoms)
                            {
                                Vector3 textPos = atom.position;
                                string textStr = atom.name;
                                GameObject textGo = new GameObject(atom.name);

                                textGo.transform.SetParent(atomNameLabelT.transform, false);
                                text = textGo.AddComponent<TextMeshPro>();

                                SetText(text, textStr, textPos);

                                AddLoadedLabel(atom.type);
                            }
                        }
                    }
                }
            }
        }
    }


    /*  
     *  
        public void LabelAtomName()
        {
            LabelClear();
            UnityMolStructureManager sm = UnityMolMain.getStructureManager(); //结构管理器
            UnityMolStructure s = sm.GetCurrentStructure(); //结构
            Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
            Transform sT = loadedMolT.Find(s.ToSelectionName());
            Transform atomNameLabelT = new GameObject("AtomNameLabel").transform;
            parentTs.Add(atomNameLabelT);
            atomNameLabelT.SetParent(sT, false);

            List<UnityMolModel> ms = s.models;
            foreach (UnityMolModel m in ms)
            {
                foreach (UnityMolChain chain in m.chains.Values)
                {
                    foreach (UnityMolResidue residue in chain.residues.Values)
                    {
                        foreach (UnityMolAtom atom in residue.allAtoms)
                        {
                            Vector3 textPos = atom.position;
                            string textStr = atom.name;
                            GameObject textGo = new GameObject(atom.name);

                            textGo.transform.SetParent(atomNameLabelT.transform, false);
                            text = textGo.AddComponent<TextMeshPro>();

                            SetText(text, textStr, textPos);

                            AddLoadedLabel(atom.type);
                        }
                    }
                }
            }
        }
     */

    public void LabelElementSymbol()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager(); //结构管理器

        foreach (UnityMolStructure s in sm.loadedStructures)
        {
            //以下为增加的引用代码
            GameObject pc = getProteinController();
            ProteinManager pm = getProteinManager(pc);
            //以上为增加的引用代码


            //增加的判定语句代码
            string SelName = pm.Set_selName(s.name);
            GameObject protein = pm.FindChildObject(SelName);

            if (protein.activeInHierarchy)
            {
                //插入原本代码
                Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
                Transform sT = loadedMolT.Find(s.ToSelectionName());
                Transform atomNameLabelT = new GameObject("lElementSymbolLabel").transform;
                parentTs.Add(atomNameLabelT);
                atomNameLabelT.SetParent(sT, false);

                List<UnityMolModel> ms = s.models;
                foreach (UnityMolModel m in ms)
                {
                    foreach (UnityMolChain chain in m.chains.Values)
                    {
                        foreach (UnityMolResidue residue in chain.residues.Values)
                        {
                            foreach (UnityMolAtom atom in residue.allAtoms)
                            {
                                Vector3 textPos = atom.position;
                                string textStr = atom.type;
                                GameObject textGo = new GameObject(atom.type);

                                textGo.transform.SetParent(atomNameLabelT.transform, false);
                                text = textGo.AddComponent<TextMeshPro>();

                                SetText(text, textStr, textPos);

                                AddLoadedLabel(atom.type);
                            }
                        }
                    }
                }
            }
        }
    }


    /*
        public void LabelElementSymbol()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager(); //结构管理器
        UnityMolStructure s = sm.GetCurrentStructure(); //结构
        Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
        Transform sT = loadedMolT.Find(s.ToSelectionName());
        Transform atomNameLabelT = new GameObject("lElementSymbolLabel").transform;
        parentTs.Add(atomNameLabelT);
        atomNameLabelT.SetParent(sT, false);

        List<UnityMolModel> ms = s.models;
        foreach (UnityMolModel m in ms)
        {
            foreach (UnityMolChain chain in m.chains.Values)
            {
                foreach (UnityMolResidue residue in chain.residues.Values)
                {
                    foreach (UnityMolAtom atom in residue.allAtoms)
                    {
                        Vector3 textPos = atom.position;
                        string textStr = atom.type;
                        GameObject textGo = new GameObject(atom.type);

                        textGo.transform.SetParent(atomNameLabelT.transform, false);
                        text = textGo.AddComponent<TextMeshPro>();

                        SetText(text, textStr, textPos);

                        AddLoadedLabel(atom.type);
                    }
                }
            }
        }
    }
     */


    public void LabelResidueName()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager(); //结构管理器

        foreach (UnityMolStructure s in sm.loadedStructures)
        {
            //以下为增加的引用代码
            GameObject pc = getProteinController();
            ProteinManager pm = getProteinManager(pc);
            //以上为增加的引用代码


            //增加的判定语句代码
            string SelName = pm.Set_selName(s.name);
            GameObject protein = pm.FindChildObject(SelName);

            if (protein.activeInHierarchy)
            {
                //插入原本代码
                Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
                Transform sT = loadedMolT.Find(s.ToSelectionName());
                Transform atomNameLabelT = new GameObject("AtomNameLabel").transform;
                parentTs.Add(atomNameLabelT);
                atomNameLabelT.SetParent(sT, false);

                List<UnityMolModel> ms = s.models;
                foreach (UnityMolModel m in ms)
                {
                    foreach (UnityMolChain chain in m.chains.Values)
                    {
                        foreach (UnityMolResidue residue in chain.residues.Values)
                        {
                            foreach (UnityMolAtom atom in residue.allAtoms)
                            {
                                Vector3 textPos = atom.position;
                                string textStr = atom.residue.name;
                                GameObject textGo = new GameObject(atom.residue.name);

                                textGo.transform.SetParent(atomNameLabelT.transform, false);
                                text = textGo.AddComponent<TextMeshPro>();
                                SetText(text, textStr, textPos);

                                AddLoadedLabel(atom.residue.name);
                            }
                        }
                    }
                }
            }

        }
    }


    /*
        public void LabelResidueName()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager(); //结构管理器
        UnityMolStructure s = sm.GetCurrentStructure(); //结构
        Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
        Transform sT = loadedMolT.Find(s.ToSelectionName());
        Transform atomNameLabelT = new GameObject("AtomNameLabel").transform;
        parentTs.Add(atomNameLabelT);
        atomNameLabelT.SetParent(sT, false);

        List<UnityMolModel> ms = s.models;
        foreach (UnityMolModel m in ms)
        {
            foreach (UnityMolChain chain in m.chains.Values)
            {
                foreach (UnityMolResidue residue in chain.residues.Values)
                {
                    foreach (UnityMolAtom atom in residue.allAtoms)
                    {
                        Vector3 textPos = atom.position;
                        string textStr = atom.residue.name;
                        GameObject textGo = new GameObject(atom.residue.name);

                        textGo.transform.SetParent(atomNameLabelT.transform, false);
                        text = textGo.AddComponent<TextMeshPro>();
                        SetText(text, textStr, textPos);

                        AddLoadedLabel(atom.residue.name);
                    }
                }
            }
        }
    }
     */


    public void LabelBFactor()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();

        foreach (UnityMolStructure s in sm.loadedStructures)
        {
            //以下为增加的引用代码
            GameObject pc = getProteinController();
            ProteinManager pm = getProteinManager(pc);
            //以上为增加的引用代码


            //增加的判定语句代码
            string SelName = pm.Set_selName(s.name);
            GameObject protein = pm.FindChildObject(SelName);

            if (protein.activeInHierarchy)
            {
                //插入原本代码
                Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
                Transform sT = loadedMolT.Find(s.ToSelectionName());

                Transform bfactorTextT = new GameObject("BfactorLabel").transform;
                parentTs.Add(bfactorTextT);
                bfactorTextT.SetParent(sT, false);
                foreach (UnityMolAtom atom in s.atomToGo.Keys)
                {
                    Vector3 textPos = atom.position;
                    string textStr = atom.bfactor.ToString();
                    GameObject textGo = new GameObject("Text");

                    textGo.transform.SetParent(bfactorTextT.transform, false);
                    text = textGo.AddComponent<TextMeshPro>();

                    SetText(text, textStr, textPos);
                }
            }
        }
    }


    /*
        public void LabelBFactor()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();

        Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
        Transform sT = loadedMolT.Find(s.ToSelectionName());

        Transform bfactorTextT = new GameObject("BfactorLabel").transform;
        parentTs.Add(bfactorTextT);
        bfactorTextT.SetParent(sT, false);
        foreach (UnityMolAtom atom in s.atomToGo.Keys)
        {
            Vector3 textPos = atom.position;
            string textStr = atom.bfactor.ToString();
            GameObject textGo = new GameObject("Text");

            textGo.transform.SetParent(bfactorTextT.transform, false);
            text = textGo.AddComponent<TextMeshPro>();

            SetText(text, textStr, textPos);
        }
    }
     */


    public void LabelVdwRadius()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager(); //结构管理器

        foreach (UnityMolStructure s in sm.loadedStructures)
        {
            //以下为增加的引用代码
            GameObject pc = getProteinController();
            ProteinManager pm = getProteinManager(pc);
            //以上为增加的引用代码


            //增加的判定语句代码
            string SelName = pm.Set_selName(s.name);
            GameObject protein = pm.FindChildObject(SelName);

            if (protein.activeInHierarchy)
            {
                //插入原本代码
                Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
                Transform sT = loadedMolT.Find(s.ToSelectionName());
                Transform atomNameLabelT = new GameObject("VdwRadiusLabel").transform;
                parentTs.Add(atomNameLabelT);
                atomNameLabelT.SetParent(sT, false);

                List<UnityMolModel> ms = s.models;
                foreach (UnityMolModel m in ms)
                {
                    foreach (UnityMolChain chain in m.chains.Values)
                    {
                        foreach (UnityMolResidue residue in chain.residues.Values)
                        {
                            foreach (UnityMolAtom atom in residue.allAtoms)
                            {
                                Vector3 textPos = atom.position;
                                string textStr = atom.radius.ToString();
                                GameObject textGo = new GameObject("Text");

                                textGo.transform.SetParent(atomNameLabelT.transform, false);
                                text = textGo.AddComponent<TextMeshPro>();

                                SetText(text, textStr, textPos);
                            }
                        }
                    }
                }
            }
        }
    }

    /*
        public void LabelVdwRadius()
    {
        LabelClear();
        UnityMolStructureManager sm = UnityMolMain.getStructureManager(); //结构管理器
        UnityMolStructure s = sm.GetCurrentStructure(); //结构
        Transform loadedMolT = GameObject.Find("LoadedMolecules").transform;
        Transform sT = loadedMolT.Find(s.ToSelectionName());
        Transform atomNameLabelT = new GameObject("VdwRadiusLabel").transform;
        parentTs.Add(atomNameLabelT);
        atomNameLabelT.SetParent(sT, false);

        List<UnityMolModel> ms = s.models;
        foreach (UnityMolModel m in ms)
        {
            foreach (UnityMolChain chain in m.chains.Values)
            {
                foreach (UnityMolResidue residue in chain.residues.Values)
                {
                    foreach (UnityMolAtom atom in residue.allAtoms)
                    {
                        Vector3 textPos = atom.position;
                        string textStr = atom.radius.ToString();
                        GameObject textGo = new GameObject("Text");

                        textGo.transform.SetParent(atomNameLabelT.transform, false);
                        text = textGo.AddComponent<TextMeshPro>();

                        SetText(text, textStr, textPos);
                    }
                }
            }
        }
    }
     */

    //设置text相关参数
    private TextMeshPro SetText(TextMeshPro text, string textStr, Vector3 textPos)
    {
        text.alignment = TextAlignmentOptions.Center; //字体对齐方式
        text.text = textStr; //字体内容
        text.fontSize = sizeSlider.value; //字体大小
        SetColor(Color.white); //字体颜色
        textList.Add(text);

        text.gameObject.AddComponent<LookAtCamera>();
        text.transform.localPosition = textPos; ///字体位置
        return text;
    }

    //修改字体颜色
     public void ChooseColor(string color)
    {
        switch (color)
        {
            case "White":
                SetColor(Color.white);
                break;
            case "Red":
                SetColor(Color.red);
                break;
            case "Yellow":
                SetColor(Color.yellow);
                break;
            case "Green":
                SetColor(Color.green);
                break;
            case "Cyan":
                SetColor(Color.cyan);
                break;
            case "Blue":
                SetColor(Color.blue);
                break;
            case "Magenta":
                SetColor(Color.magenta);
                break;
            case "Gray":
                SetColor(Color.gray);
                break;
        }
    }
    private void SetColor(Color color)
    {
        foreach (TextMeshPro text in textList)
        {
            if(text.gameObject.activeInHierarchy)
                text.color = color;
        }
    }

    //修改字体大小
    public void LabelSize()
    {
        foreach (TextMeshPro text in textList)
        {
            text.fontSize = sizeSlider.value;
        }
    }

    public void SetLoadedLabel()
    {
        foreach(TextMeshPro text in textList)
        {
            if (text.text.Contains(loadedLabelTMP_Dropdown.options[loadedLabelTMP_Dropdown.value].text) || loadedLabelTMP_Dropdown.options[loadedLabelTMP_Dropdown.value].text == "All")
            {
                text.gameObject.SetActive(true);
            }
            else
            {
                text.gameObject.SetActive(false);
            }
        }
    }

    public void AddLoadedLabel(string str)
    {
        if (!loadedLabelTMP_Dropdown.options.Any(option => option.text == str))
        {
            loadedLabelTMP_Dropdown.options.Add(new TMP_Dropdown.OptionData() { text = str });
        }
    }
}

