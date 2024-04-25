using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BinaryCharm.UI.TextColorButtons.Extensions {
    public static class SerializedObjectEx {

        public static SerializedProperty FindPropertyByPath(this SerializedObject rSO, string sPath) {
            int iDotPos = sPath.IndexOf(".");
            if (iDotPos == -1) return rSO.FindProperty(sPath);

            string sPropId = sPath.Substring(0, iDotPos);
            SerializedProperty rSP = rSO.FindProperty(sPropId);
            while (true) {
                sPath = sPath.Substring(iDotPos + 1);
                iDotPos = sPath.IndexOf(".");
                if (iDotPos == -1) break;
                rSP = rSP.FindPropertyRelative(sPath.Substring(0, iDotPos));
            };
            return rSP.FindPropertyRelative(sPath);
        }
    }
}