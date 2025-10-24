#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

/// <summary>
/// Comprehensive iOS post-build processor
/// Handles ALL iOS build configuration automatically:
/// - ATT permission for AdMob
/// - Swift library embedding for Facebook SDK & AdMob
/// - Framework embedding for CocoaPods
/// - Podfile post-install hooks
///
/// NO MANUAL STEPS REQUIRED - Just build from Unity and run pod install!
/// </summary>
public class IOSBuildPostProcessor
{
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS) return;

        UnityEngine.Debug.Log("========================================");
        UnityEngine.Debug.Log("iOS Build Post-Processor Started");
        UnityEngine.Debug.Log("========================================");

        // 1. Add ATT Permission to Info.plist
        AddATTPermission(pathToBuiltProject);

        // 2. Configure Swift Library Embedding
        ConfigureSwiftLibraries(pathToBuiltProject);

        // 3. Fix Podfile with proper post_install hooks
        FixPodfile(pathToBuiltProject);

        UnityEngine.Debug.Log("========================================");
        UnityEngine.Debug.Log("✅ iOS Build Post-Processing Complete!");
        UnityEngine.Debug.Log("========================================");
        UnityEngine.Debug.Log("Next Steps:");
        UnityEngine.Debug.Log("  1. cd \"" + pathToBuiltProject + "\"");
        UnityEngine.Debug.Log("  2. pod install");
        UnityEngine.Debug.Log("  3. open Unity-iPhone.xcworkspace");
        UnityEngine.Debug.Log("  4. Build & Run!");
        UnityEngine.Debug.Log("========================================");
    }

    private static void AddATTPermission(string pathToBuiltProject)
    {
        string plistPath = pathToBuiltProject + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict rootDict = plist.root;

        // Add App Tracking Transparency permission
        if (!rootDict.values.ContainsKey("NSUserTrackingUsageDescription"))
        {
            rootDict.SetString("NSUserTrackingUsageDescription",
                "This app uses your data to provide personalized ads and improve your experience.");
            UnityEngine.Debug.Log("✅ Added ATT permission to Info.plist");
        }

        plist.WriteToFile(plistPath);
    }

    private static void ConfigureSwiftLibraries(string pathToBuiltProject)
    {
        string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);

        // Main target: Enable Swift library embedding
        string mainTarget = project.GetUnityMainTargetGuid();
        project.SetBuildProperty(mainTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

        // Framework target: Disable Swift library embedding (avoid duplicates)
        string frameworkTarget = project.GetUnityFrameworkTargetGuid();
        project.SetBuildProperty(frameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

        // Disable bitcode (required for Facebook SDK)
        project.SetBuildProperty(mainTarget, "ENABLE_BITCODE", "NO");
        project.SetBuildProperty(frameworkTarget, "ENABLE_BITCODE", "NO");

        project.WriteToFile(projectPath);
        UnityEngine.Debug.Log("✅ Configured Swift library embedding");
    }

    private static void FixPodfile(string pathToBuiltProject)
    {
        string podfilePath = pathToBuiltProject + "/Podfile";

        if (!File.Exists(podfilePath))
        {
            UnityEngine.Debug.LogWarning("⚠️ Podfile not found - iOS Resolver may not have run");
            return;
        }

        string podfileContent = File.ReadAllText(podfilePath);

        // Check if post_install already exists
        if (podfileContent.Contains("post_install do |installer|"))
        {
            UnityEngine.Debug.Log("ℹ️ Podfile already has post_install hook");
            return;
        }

        // Add comprehensive post_install hook
        string postInstallHook = @"

post_install do |installer|
  installer.pods_project.targets.each do |target|
    target.build_configurations.each do |config|
      # Disable bitcode for all pods
      config.build_settings['ENABLE_BITCODE'] = 'NO'

      # Set minimum iOS version
      config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = '13.0'

      # Disable Swift library embedding for pod targets
      config.build_settings['ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES'] = 'NO'

      # Enable module support
      config.build_settings['BUILD_LIBRARY_FOR_DISTRIBUTION'] = 'YES'
    end
  end

  # Enable Swift library embedding ONLY for main app target
  installer.aggregate_targets.each do |aggregate_target|
    aggregate_target.xcconfigs.each do |config_name, config_file|
      if aggregate_target.name == 'Pods-Unity-iPhone'
        config_file.attributes['ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES'] = 'YES'
      end
    end
  end
end
";

        // Append post_install hook to Podfile
        File.AppendAllText(podfilePath, postInstallHook);
        UnityEngine.Debug.Log("✅ Added post_install hook to Podfile");
    }
}
#endif
