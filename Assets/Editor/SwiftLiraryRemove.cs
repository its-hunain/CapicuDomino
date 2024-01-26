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
    /// Automatically disables ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES on iOS builds
    /// Reference : https://www.cxyzjd.com/article/qq534575060/114381877 no.49
    /// </summary>
    public static class IOSAlwaysEmbedSwiftStandardLibrariesDisabler
    {
        [PostProcessBuildAttribute(999)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuildProject)
        {
            if (buildTarget != BuildTarget.iOS) return;
            string projectPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);
 
            //Disabling ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES Unity Framework target
            string target = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
 
            pbxProject.WriteToFile(projectPath);
         }
    }
}
#endif