using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

/// <summary>
/// Prevents Unity Test Framework from generating performance test files that can hang iOS builds
/// Removes auto-generated test files before build starts
/// </summary>
public class ExcludeTestFilesFromBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        // List of auto-generated test files that can cause build hangs
        string[] testFiles = new string[]
        {
            "Assets/Resources/PerformanceTestRunInfo.json",
            "Assets/Resources/PerformanceTestRunSettings.json",
            "Assets/Resources/BillingMode.json"
        };

        foreach (string filePath in testFiles)
        {
            if (File.Exists(filePath))
            {
                Debug.LogWarning($"[Build] Removing auto-generated test file: {filePath}");
                File.Delete(filePath);

                // Also delete .meta file
                string metaPath = filePath + ".meta";
                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }
            }
        }

        // Refresh asset database to reflect deletions
        AssetDatabase.Refresh();

        Debug.Log("[Build] Test files cleanup completed - build should proceed normally");
    }
}
