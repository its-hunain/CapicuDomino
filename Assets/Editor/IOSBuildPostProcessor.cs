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
/// - Podfile restructuring (pods ONLY in UnityFramework, NOT in Unity-iPhone)
/// - Podfile post-install hooks
///
/// NO MANUAL STEPS REQUIRED - Just build from Unity and run pod install!
/// This prevents duplicate class warnings and ensures ads work correctly.
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

        // Extract all pod declarations from the Podfile
        System.Collections.Generic.List<string> podDeclarations = new System.Collections.Generic.List<string>();
        string[] lines = podfileContent.Split('\n');

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("pod '") && !podDeclarations.Contains(trimmedLine))
            {
                podDeclarations.Add(trimmedLine);
            }
        }

        UnityEngine.Debug.Log($"Found {podDeclarations.Count} unique pod declarations");

        // Generate a completely new Podfile with correct structure
        System.Text.StringBuilder podfileBuilder = new System.Text.StringBuilder();
        podfileBuilder.AppendLine("source 'https://cdn.cocoapods.org/'");
        podfileBuilder.AppendLine("source 'https://github.com/CocoaPods/Specs'");
        podfileBuilder.AppendLine();
        podfileBuilder.AppendLine("platform :ios, '13.0'");
        podfileBuilder.AppendLine();
        podfileBuilder.AppendLine("# Pods ONLY for UnityFramework (where the code actually runs)");
        podfileBuilder.AppendLine("target 'UnityFramework' do");
        podfileBuilder.AppendLine("  use_frameworks!");
        podfileBuilder.AppendLine();

        // Add all pod declarations to UnityFramework target
        foreach (string pod in podDeclarations)
        {
            podfileBuilder.AppendLine("  " + pod);
        }

        podfileBuilder.AppendLine("end");
        podfileBuilder.AppendLine();
        podfileBuilder.AppendLine("# Main app target - NO pods here (just use_frameworks for compatibility)");
        podfileBuilder.AppendLine("target 'Unity-iPhone' do");
        podfileBuilder.AppendLine("  use_frameworks!");
        podfileBuilder.AppendLine("end");
        podfileBuilder.AppendLine();
        podfileBuilder.AppendLine("post_install do |installer|");
        podfileBuilder.AppendLine("  installer.pods_project.targets.each do |target|");
        podfileBuilder.AppendLine("    target.build_configurations.each do |config|");
        podfileBuilder.AppendLine("      config.build_settings['ENABLE_BITCODE'] = 'NO'");
        podfileBuilder.AppendLine("      config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = '13.0'");
        podfileBuilder.AppendLine("      config.build_settings['BUILD_LIBRARY_FOR_DISTRIBUTION'] = 'YES'");
        podfileBuilder.AppendLine("      config.build_settings['ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES'] = 'NO'");
        podfileBuilder.AppendLine("    end");
        podfileBuilder.AppendLine("  end");
        podfileBuilder.AppendLine();
        podfileBuilder.AppendLine("  # Enable Swift embedding ONLY for Unity-iPhone");
        podfileBuilder.AppendLine("  installer.aggregate_targets.each do |aggregate_target|");
        podfileBuilder.AppendLine("    aggregate_target.xcconfigs.each do |config_name, config_file|");
        podfileBuilder.AppendLine("      if aggregate_target.name == 'Pods-Unity-iPhone'");
        podfileBuilder.AppendLine("        config_file.attributes['ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES'] = 'YES'");
        podfileBuilder.AppendLine("      end");
        podfileBuilder.AppendLine("    end");
        podfileBuilder.AppendLine("  end");
        podfileBuilder.AppendLine("end");

        string newPodfile = podfileBuilder.ToString();

        // Write the new Podfile
        File.WriteAllText(podfilePath, newPodfile);
        UnityEngine.Debug.Log("✅ Restructured Podfile - Pods only in UnityFramework target");
        UnityEngine.Debug.Log("   This prevents duplicate class warnings and ensures proper linking");
    }
}
#endif
