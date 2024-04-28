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

using UnityEditor;
using UnityEngine;

namespace BinaryCharm.UI.TextColorButtons.EditorIntegration {
    public static class EditorUtils {
        public static MonoScript getMonoScript(Type rType) {
            foreach (var script in Resources.FindObjectsOfTypeAll<MonoScript>()) {
                if (script.GetClass() == rType) return script;
            }
            return null;
        }

        public static void changeScript(Component rComponent, MonoScript rScript) {
            SerializedObject rSO = new SerializedObject(rComponent);
            SerializedProperty rScriptProp = rSO.FindProperty("m_Script");
            rSO.Update();
            rScriptProp.objectReferenceValue = rScript;
            rSO.ApplyModifiedProperties();
        }
    }
}