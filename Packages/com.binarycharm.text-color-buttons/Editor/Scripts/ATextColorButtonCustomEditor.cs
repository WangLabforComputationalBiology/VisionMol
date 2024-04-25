/*
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             Copyright (C) 2021 Binary Charm - All Rights Reserved
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

using UnityEditor;
using UnityEditor.UI;

namespace BinaryCharm.UI.TextColorButtons.Editor {

    [CanEditMultipleObjects]
    public abstract class ATextColorButtonCustomEditor<T> : ButtonEditor {

        private SerializedProperty m_TextColorBlockProperty;

        protected override void OnEnable() {
            base.OnEnable();
            m_TextColorBlockProperty = serializedObject.FindProperty(nameof(ATextColorButton<T>.m_TextColors));
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Label Text Colors");
            ++EditorGUI.indentLevel;
            EditorGUILayout.PropertyField(m_TextColorBlockProperty);
            --EditorGUI.indentLevel;
            serializedObject.ApplyModifiedProperties();
        }

    }

}
