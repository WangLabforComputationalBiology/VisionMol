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
    //����MyloadedStructures�������Ҫ�Ĳ���
    public int MaxCurrent = 3;
    public int MyCurrent = 0;
    public int _myCurrent = 1;
    public string[] MyloadedStructures = new string[3] { "Protein_1","Protein_2","Protein_3"};

    //��������ָ�򵰰����л���ť
    public Toggle[] SwitchProtein = new Toggle[3];


    //����toggle��ť
    void Start()
    {
        foreach (Toggle toggle in SwitchProtein)
        {
            toggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(toggle);
            });
        }
    }


    //�ѵ���������ͬ�������Խ���MyloadedStructures�������ı��л�Ϊ����������
    private void Update()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();

        //�����Ƿ�����ӵ�����
        MyCurrent = sm.loadedStructures.Count - 1;

        if (MyCurrent != _myCurrent)
        {
            _myCurrent = MyCurrent;
            //���浰���ʵ����Ƶ�������
            MyloadedStructures[MyCurrent] = s.name;

            //�л��ı����Ƶ�toggle��ť
            if (MyCurrent < MaxCurrent)
            {
                if (SwitchProtein[MyCurrent] != null)
                {
                    // ��ȡ�� Toggle ���������� Label ���
                    Text label = SwitchProtein[MyCurrent].GetComponentInChildren<Text>();

                    if (label != null)
                    {
                        // ���� Label ������ı�
                        label.text = MyloadedStructures[MyCurrent];
                    }
                }
            }
        }
    }

    /*
    //�ѵ���������ͬ�������Խ���MyloadedStructures�������ı��л�Ϊ����������
    public void SaveProtein()
    {
        UnityMolStructureManager sm = UnityMolMain.getStructureManager();
        UnityMolStructure s = sm.GetCurrentStructure();

        //���浰���ʵ����Ƶ�������
        MyCurrent = sm.loadedStructures.Count-1;
        MyloadedStructures[MyCurrent] = s.name;

        //�л��ı����Ƶ�toggle��ť
        if (MyCurrent < MaxCurrent)
        {
            if (SwitchProtein[MyCurrent] != null)
            {
                // ��ȡ�� Toggle ���������� Label ���
                Text label = SwitchProtein[MyCurrent].GetComponentInChildren<Text>();

                if (label != null)
                {
                    // ���� Label ������ı�
                    label.text = MyloadedStructures[MyCurrent];
                }
            }
        }
    }
    */


    // Toggle ֵ�ı��¼�������,ͨ��toggle��Text�ı������޸ļ���״̬
    public void ToggleValueChanged(Toggle changedToggle)
    {
        // �� Toggle �������л�ȡ Label ���
        Text label = changedToggle.GetComponentInChildren<Text>();

        if (label != null)
        {
            //��ȡlabel��Text�ı�
            string selName = Set_selName(label.text);

            GameObject childObject = FindChildObject(selName);

            if (childObject != null)
            {
                childObject.SetActive(changedToggle.isOn);
            }
        }
    }


    //��ʽ������
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

    //��ȡ��Ӧ���Ƶ�����������
    public GameObject FindChildObject(string selName)
    {
        GameObject childObject = null;

        //ȷ�����Ƹ�ʽ��
        selName = Set_selName(selName);

        // ͨ�����ƻ�ȡ������LoadedMolecules������
        GameObject loadedMolecules = GameObject.Find("LoadedMolecules");

        // ����Ƿ�ɹ��ҵ�������
        if (loadedMolecules != null)
        {
            // �ɹ��ҵ������壬������ͨ�������ҵ��������¶�Ӧtoggle��������
            Transform childTransform = loadedMolecules.transform.Find(selName);

            // ����Ƿ�ɹ��ҵ�������
            if (childTransform != null)
            {
                // �ɹ��ҵ�������
                childObject = childTransform.gameObject;
            }
        }

        return childObject;
    }
}

//��Ҫ��ӵ��ж�����
/*
                 //����Ϊ���ӵ����ô���
        GameObject pc = getProteinController();
        ProteinManager pm = getProteinManager(pc);
                //����Ϊ���ӵ����ô���


                //���ӵ��ж�������
        string SelName = pm.Set_selName(s.name);
        GameObject protein = pm.FindChildObject(SelName);

                if (protein.activeInHierarchy)
                {


                 //�����ԭ���Ĵ���

                 //ԭ���������


                 }
                 //���ӵ��ж�����
    
    
*/

//���´���Ϊ��APIPython�е�ShowAS�����ı��ж�����
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

//APIPython�ű��е�setHyperBallMetaphore����
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