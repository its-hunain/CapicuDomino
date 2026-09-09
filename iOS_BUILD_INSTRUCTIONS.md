# iOS Build Instructions - Facebook SDK & AdMob Setup

## Issue: "No such module FBSDKCoreKit" Error

This happens because the iOS Resolver was disabled to prevent build hangs. You need to manually install CocoaPods dependencies.

## Step-by-Step Fix:

### 1. Locate Your iOS Build Folder
After building from Unity, find where Unity created the Xcode project. It's usually:
- `/Users/sarim/Documents/GithubRepo/capicu build/` (based on logs)
- Or wherever you selected when building iOS from Unity

### 2. Create Podfile in iOS Build Folder

Navigate to your iOS build folder and create a `Podfile`:

```bash
cd "/path/to/your/ios-build-folder"

cat > Podfile << 'EOF'
platform :ios, '13.0'

target 'Unity-iPhone' do
  use_frameworks!

  # Facebook SDK v18.0.0
  pod 'FBSDKCoreKit', '~> 18.0.0'
  pod 'FBSDKCoreKit_Basics', '~> 18.0.0'
  pod 'FBSDKLoginKit', '~> 18.0.0'
  pod 'FBSDKShareKit', '~> 18.0.0'
  pod 'FBSDKGamingServicesKit', '~> 18.0.0'

  # Google Mobile Ads v12.11.0
  pod 'Google-Mobile-Ads-SDK', '~> 12.11.0'
end

post_install do |installer|
  installer.pods_project.targets.each do |target|
    target.build_configurations.each do |config|
      # Disable for pod targets to avoid duplicate symbols
      config.build_settings['ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES'] = 'NO'
      config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = '13.0'
    end
  end

  # Enable Swift library embedding for main target
  installer.aggregate_targets.each do |aggregate_target|
    aggregate_target.user_project.targets.each do |target|
      if target.name == 'Unity-iPhone'
        target.build_configurations.each do |config|
          config.build_settings['ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES'] = 'YES'
        end
      end
    end
  end
end
EOF
```

### 3. Install CocoaPods Dependencies

```bash
# Make sure you're in the iOS build folder
pod install

# This will create:
# - Pods/ folder
# - Unity-iPhone.xcworkspace
# - Podfile.lock
```

### 4. Open Workspace (NOT Project!)

**IMPORTANT:** You MUST open the `.xcworkspace` file, NOT the `.xcodeproj` file:

```bash
open Unity-iPhone.xcworkspace
```

### 5. Verify in Xcode

In Xcode, check:
1. Left sidebar should show "Unity-iPhone" AND "Pods" projects
2. Go to Unity-iPhone target → Build Phases → Link Binary With Libraries
3. You should see pod frameworks listed

### 6. Build Settings to Verify

Select "Unity-iPhone" target → Build Settings → Search for:

1. **Framework Search Paths** - Should include:
   - `$(inherited)`
   - `${PODS_CONFIGURATION_BUILD_DIR}/FBSDKCoreKit`
   - Other pod paths...

2. **Header Search Paths** - Should include:
   - `$(inherited)`
   - `${PODS_CONFIGURATION_BUILD_DIR}/FBSDKCoreKit/FBSDKCoreKit.framework/Headers`

3. **ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES** - Should be `YES` for Unity-iPhone target

### 7. Clean and Archive

In Xcode:
1. Product → Clean Build Folder (Cmd+Shift+K)
2. Product → Archive

## Troubleshooting:

### If "No such module" error persists:

**Option A: Check if you opened workspace**
- Close Xcode
- Make sure you're opening `Unity-iPhone.xcworkspace` not `.xcodeproj`

**Option B: Clean derived data**
```bash
rm -rf ~/Library/Developer/Xcode/DerivedData/*
```

**Option C: Verify pods installed**
```bash
cd /path/to/ios-build-folder
ls -la Pods/FBSDKCoreKit
```

**Option D: Re-run pod install**
```bash
pod deintegrate
pod install
```

### If Swift version mismatch:

Add to Podfile (before `end`):
```ruby
  # Set Swift version for all pods
  config.build_settings['SWIFT_VERSION'] = '5.0'
```

## After Successful Archive:

You'll have an IPA that works with both Facebook SDK and AdMob without crashes!

## Re-enabling iOS Resolver for Future Builds:

Once you confirm manual setup works, you can re-enable iOS Resolver:

```bash
cd /Users/sarim/Documents/GithubRepo/CapicuDomino

mv Assets/ExternalDependencyManager/Editor/1.2.186/Google.IOSResolver.dll.DISABLED \
   Assets/ExternalDependencyManager/Editor/1.2.186/Google.IOSResolver.dll

mv Assets/ExternalDependencyManager/Editor/1.2.186/Google.IOSResolver.dll.DISABLED.meta \
   Assets/ExternalDependencyManager/Editor/1.2.186/Google.IOSResolver.dll.meta
```

Then configure it properly in Unity:
- Assets → External Dependency Manager → iOS Resolver → Settings
- Integration: "Workspace"
- Uncheck "Auto Install Cocoapods Tools"
- Check "Add use_frameworks!"
