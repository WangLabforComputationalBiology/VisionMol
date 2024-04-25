using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Ximmerse.XR
{
    public class BuildPlayerProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {

        /// <summary>
        /// The subpath of tracking profile files to streaming assets.
        /// </summary>
        const string kTrackingProfiles = "/TrackingProfiles";

        public int callbackOrder => 1;


        /// <summary>
        /// Copy tracking profiles from plugin to streaming assets.
        /// </summary>
        static void CopyTrackingProfiles()
        {
            string streamingAssets = Application.streamingAssetsPath;

            if (!Directory.Exists(streamingAssets))
            {
                Directory.CreateDirectory(streamingAssets);
            }

            string destPath = streamingAssets + kTrackingProfiles;
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            if (Directory.Exists(SDKEditorUtility.kPluginTrackingProfilePath))
            {
                var jsons = Directory.GetFiles(SDKEditorUtility.kPluginTrackingProfilePath, "*.json", SearchOption.TopDirectoryOnly);
                var datas = Directory.GetFiles(SDKEditorUtility.kPluginTrackingProfilePath, "*.dat", SearchOption.TopDirectoryOnly);
                foreach (var json in jsons)
                {
                    string newFileName = Path.Combine(destPath, Path.GetFileName(json));
                    File.Copy(json, newFileName, true);
                    Debug.LogFormat("Copy: {0}=>{1}", json, newFileName);
                }
                foreach (var data in datas)
                {
                    string newFileName = Path.Combine(destPath, Path.GetFileName(data));
                    File.Copy(data, newFileName, true);
                    Debug.LogFormat("Copy: {0}=>{1}", data, newFileName);
                }
                Debug.LogFormat("All tracking profiles has been copied from {0} => {1}", SDKEditorUtility.kPluginTrackingProfilePath, destPath);
            }
        }

        /// <summary>
        /// Cleanup tracking profiles after building player.
        /// </summary>
        static void CleanupTrackingProfiles()
        {
            string streamingAssets = Application.streamingAssetsPath;

            if (!Directory.Exists(streamingAssets))
            {
                return;
            }

            string destPath = streamingAssets + kTrackingProfiles;
            if (!Directory.Exists(destPath))
            {
                return;
            }

            Directory.Delete(destPath, true); //delete the tracking profiles directory
            string metaDestPath = destPath + ".meta";
            if (File.Exists(metaDestPath))
            {
                File.Delete(metaDestPath); //delete dest path meta
            }

            var f = Directory.GetFiles(streamingAssets);
            if (f.Length == 0)
            {
                Directory.Delete(streamingAssets, true); //delete the sa dir if empty
                string metaSA = streamingAssets + ".meta";
                if (File.Exists(metaSA))
                {
                    File.Delete(metaSA); //delete SA meta
                }
            }
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.Android)
            {
                CleanupTrackingProfiles();
            }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.Android)
            {
                CopyTrackingProfiles();
            }
        }
    }
}
