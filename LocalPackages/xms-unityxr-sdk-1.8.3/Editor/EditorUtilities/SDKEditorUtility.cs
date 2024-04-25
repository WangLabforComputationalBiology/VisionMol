using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using System;
using System.IO;


namespace Ximmerse.XR
{
    /// <summary>
    /// SDK editor utility.
    /// </summary>
    public static class SDKEditorUtility
    {

        /// <summary>
        /// The asset path of the ximmerse XR plugin.
        /// </summary>
        public static string kPluginFolderAssetPath
        {
            get; private set;
        }

        /// <summary>
        /// The absolute path of the ximmerse XR plugin.
        /// </summary>
        public static string kPluginFolderResolvePath
        {
            get; private set;
        }

        /// <summary>
        /// The asset path of the ximmerse XR plugin's tracking profile
        /// </summary>
        public static string kPluginTrackingProfilePath
        {
            get; private set;
        }

        public const string kXimmerseXRPackageName = "com.ximmerse.xr";

        /// <summary>
        /// The tracking profile's relative path to plugin root
        /// </summary>
        public const string kTrackingProfileRelativePath = "/TrackingProfiles";

        /// <summary>
        /// The tracking profile's absolute path.
        /// </summary>
        public static string kTrackingProfileResolvePath
        {
            get => kPluginFolderResolvePath + kTrackingProfileRelativePath;
        }

        [InitializeOnLoadMethod]
        static void InitializeEditor()
        {
            var pkg = GetXimmerseXRSDKPackageInfo();
            if (pkg != null)
            {
                kPluginFolderAssetPath = pkg.assetPath;
                kPluginFolderResolvePath = pkg.resolvedPath;
                kPluginTrackingProfilePath = pkg.assetPath + kTrackingProfileRelativePath;
                //Debug.LogFormat("Ximmerse XR plugin path : {0}, resolve path : {1}, tracking profile path: {2}", pkg.assetPath, pkg.resolvedPath, kPluginTrackingProfilePath);
            }
        }

        /// <summary>
        /// Gets the Ximmerse XR package info.
        /// </summary>
        /// <returns></returns>
        public static PackageInfo GetXimmerseXRSDKPackageInfo()
        {
            ListRequest list = Client.List(offlineMode: true);
            while (!list.IsCompleted)
            {

            }
            foreach (PackageInfo packageInfo in list.Result)
            {
                if (packageInfo.name.Equals(kXimmerseXRPackageName, StringComparison.OrdinalIgnoreCase))
                {
                    return packageInfo;
                }
            }
            return default(PackageInfo);
        }
    }
}