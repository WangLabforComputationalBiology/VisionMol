/*
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             Copyright (C) 2022 Binary Charm - All Rights Reserved
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@                  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@                        @@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@                           @@@@@@@@@@@@@@@@@@
             @@@@@@@@@   @@@@@@@@@@@  @@@@@        @@@@@@@@@@@@@@@
             @@@@@@@@@@@  @@@@@@@@@  @@@@@@@@@@       (@@@@@@@@@@@
             @@@@@@@@@@@@  @@@@@@@@& @@@@@@@@@@ @@@@     @@@@@@@@@
             @@@@@@@@@@@@@ @@@@@@@@@@ *@@@@@@@ @@@@@@@@@*   @@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@      @@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
*/

using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if TEXTMESHPRO_PRESENT
using TMPro;
#endif

using BinaryCharm.UI.TextColorButtons.Extensions;

#if SEMANTICCOLORPALETTE_PRESENT
using BinaryCharm.UI.TextColorButtons.Integrations.SemanticColorPalette;
using BinaryCharm.SemanticColorPalette.Colorers.UI;
    #if TEXTMESHPRO_PRESENT
using BinaryCharm.SemanticColorPalette.Colorers.TMPro;
    #endif
#endif

namespace BinaryCharm.UI.TextColorButtons.EditorIntegration {

    public static class TCB_MenuItems {

        [MenuItem("GameObject/Text Color Buttons/Transform into TextColorButton", false)]
        private static void TransformIntoTCB(MenuCommand menuCommand) {
            GameObject rSelectedGO = Selection.activeGameObject;

            Undo.SetCurrentGroupName("Conversion to TextColorButton");
            Undo.RegisterCompleteObjectUndo(rSelectedGO, "Conversion to TextColorButton");

            Type rType = lookUpSuitableTCBType(rSelectedGO);
            Button rBtn = rSelectedGO.GetComponent<Button>();

            EditorUtils.changeScript(rBtn, EditorUtils.getMonoScript(rType));

#if SEMANTICCOLORPALETTE_PRESENT
            TransformColorer(rSelectedGO);
#endif
        }

        [MenuItem("GameObject/Text Color Buttons/Transform into TextColorButton", true)]
        private static bool TransformIntoTCBValidation(MenuCommand menuCommand) {
            GameObject rSelectedGO = Selection.activeGameObject;
            if (rSelectedGO == null) return false;
            Type rType = lookUpSuitableTCBType(rSelectedGO);
            return rType != null;
        }

        private static Type lookUpSuitableTCBType(GameObject go) {
            Type ret = null;
            Button rBtn = go.GetComponent<Button>();
            if (rBtn != null) {
                if (go.transform.childCount > 0) {
                    Transform rLabelTr = go.transform.GetChild(0);

                    if (rLabelTr.GetComponent<Text>() != null) {
                        ret = typeof(TextColorButton);
                    }
#if TEXTMESHPRO_PRESENT
                    else if (rLabelTr.GetComponent<TextMeshProUGUI>() != null) {
                        ret = typeof(TMP_TextColorButton);
                    }
#endif
                }
            }

            if (ret == null || go.GetComponent(ret) != null) return null;
            return ret;
        }

        #region Semantic Color Palette integration ----------------------------
#if SEMANTICCOLORPALETTE_PRESENT
        private static readonly Dictionary<Type, Type> s_colorerMappings = new Dictionary<Type, Type>() {
            { typeof(TextColorButton), typeof(TextColorButtonColorer) },
    #if TEXTMESHPRO_PRESENT
            { typeof(TMP_TextColorButton), typeof(TMP_TextColorButtonColorer) },
    #endif
        };

        private static readonly HashSet<Type> s_upgradeableColorersSet = new HashSet<Type>() {
            typeof(SCP_ButtonColorer),
#if TEXTMESHPRO_PRESENT
            typeof(SCP_TMP_ButtonColorer)
#endif
        };

        private static Type lookUpUpgradeableColorer(GameObject go) {
            Component[] rComponents = go.GetComponents<Component>();
            foreach (Component rC in rComponents) {
                Type rType = rC.GetType();
                //if (s_colorerUpdateMappings.ContainsKey(rType)) return rType;
                if (s_upgradeableColorersSet.Contains(rType)) return rType;
            }
            return null;
        }

        private static Type lookUpSuitableColorerType(GameObject go) {
            Type ret = null;
            Component[] rComponents = go.GetComponents<Component>();
            foreach (Component rC in rComponents) {
                Type rType = rC.GetType();
                if (s_colorerMappings.TryGetValue(rType, out ret)) {
                    break;
                }
            }

            if (ret == null || go.GetComponent(ret) != null) return null;
            return ret;
        }

        [MenuItem("GameObject/Semantic Color Palette (Text Color Buttons)/Add appropriate colorer", false, 10)]
        static void AddColorer(MenuCommand menuCommand) {
            GameObject rSelectedGO = Selection.activeGameObject;
            Type rType = lookUpSuitableColorerType(rSelectedGO);
            Undo.AddComponent(rSelectedGO, rType);
        }

        [MenuItem("GameObject/Semantic Color Palette (Text Color Buttons)/Add appropriate colorer", true, 10)]
        static bool AddColorerValidation(MenuCommand menuCommand) {
            GameObject rSelectedGO = Selection.activeGameObject;
            if (rSelectedGO == null) return false;
            Type rUpgradeableType = lookUpUpgradeableColorer(rSelectedGO);
            if (rUpgradeableType != null) return false;
            Type rType = lookUpSuitableColorerType(rSelectedGO);
            return rType != null;
        }

        [MenuItem("GameObject/Semantic Color Palette (Text Color Buttons)/Upgrade colorer", false, 10)]
        static void UpgradeColorer(MenuCommand menuCommand) {
            GameObject rSelectedGO = Selection.activeGameObject;
            Undo.SetCurrentGroupName("Colorer upgrade");
            Undo.RegisterCompleteObjectUndo(rSelectedGO, "Colorer upgrade");
            TransformColorer(rSelectedGO);
        }

        [MenuItem("GameObject/Semantic Color Palette (Text Color Buttons)/Upgrade colorer", true, 10)]
        static bool UpgradeColorerValidation(MenuCommand menuCommand) {
            GameObject rSelectedGO = Selection.activeGameObject;
            if (rSelectedGO == null) return false;
            return lookUpUpgradeableColorer(rSelectedGO) != null
                && lookUpSuitableColorerType(rSelectedGO) != null;
        }

        private static void TransformColorer(GameObject rGO) {
            Type rType = lookUpUpgradeableColorer(rGO);
            if (rType == null) return;

            Type rNewType = lookUpSuitableColorerType(rGO);
            if (rNewType == null) return;

            // save label color id from "old" colorer
            var rColorer = rGO.GetComponent(rType);
            SerializedObject rColorerSO = new SerializedObject(rColorer);
            string sLabelColorPropertyPath = rType == typeof(SCP_ButtonColorer) ?
                "m_colors.labelTextColor.m_id" : // SCP_ButtonColorer
                "m_colors.labelText.vertexColor.m_id" //SCP_TMP_ButtonColorer
            ;
            SerializedProperty rLabelColorIdProp = rColorerSO.FindPropertyByPath(sLabelColorPropertyPath);
            rColorerSO.Update();
            int iColorId = rLabelColorIdProp.intValue;
            //---

            // change from *ButtonColorer to *TextColorButtonColorer
            EditorUtils.changeScript(rColorer, EditorUtils.getMonoScript(rNewType));

            // bring saved label color id to "new" colorer
            var rNewColorer = rGO.GetComponent(rNewType);
            SerializedObject rNewColorerSO = new SerializedObject(rNewColorer);
            SerializedProperty rNewLabelColorIdProp = rNewColorerSO.FindPropertyByPath("m_colors.label.normal.m_id");
            rNewColorerSO.Update();
            rNewLabelColorIdProp.intValue = iColorId;
            rNewColorerSO.ApplyModifiedProperties();
            //---
        }
#endif
        #endregion

    }

}
