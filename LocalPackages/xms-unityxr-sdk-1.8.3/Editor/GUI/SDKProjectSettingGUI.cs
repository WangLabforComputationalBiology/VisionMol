using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ximmerse.XR
{
    /// <summary>
    /// Ximmerse XR SDK Project setting GUI.
    /// </summary>
    public class SDKProjectSettingGUI
    {
        [SettingsProvider]
        public static SettingsProvider CreateRxSDKProjectSetting()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/RhinoX Setting", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "RhinoX SDK Settings",

                guiHandler = OnGUI,

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "RhinoX", "Rx", "Ximmerse", "XR" }),

                activateHandler = (searchContext, rootElement) =>
                {
                   
                },
            };

            return provider;
        }

        static void OnGUI(string SearchContext)
        {
            GUIInternal();
        }

        private static void GUIInternal()
        {

        }
    }
}