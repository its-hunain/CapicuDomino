#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Test
{
public class SwiftLiraryRemove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


    /// <summary>
    /// Properly configures ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES for iOS builds
    /// Main target needs YES to embed Swift libraries for FB SDK and AdMob
    /// Framework target needs NO to avoid duplicate symbols
    /// </summary>
    public static class IOSAlwaysEmbedSwiftStandardLibrariesFixer
    {
        [PostProcessBuildAttribute(999)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuildProject)
        {
            if (buildTarget != BuildTarget.iOS) return;
            string projectPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            // Enable Swift library embedding for main target (required for FB SDK and AdMob)
            string mainTarget = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(mainTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

            // Disable for Unity Framework target to avoid duplicate symbols
            string frameworkTarget = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(frameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

            pbxProject.WriteToFile(projectPath);

            Debug.Log("iOS Build: Configured Swift standard libraries - Main target: YES, Framework target: NO");
         }
    }
}
#endif