# Google AdMob Integration Guide - Capicu Domino

## Overview
This guide outlines the complete implementation of Google AdMob ads in the Capicu mobile app, including all required placements and configurations.

## Ad Placements Summary

### 1. VS AI Mode
- **Interstitial Ads**: Every 3 rounds (mandatory)
- **Reward Ads**: At the end of each Final game (optional, grants 100 bonus coins)

### 2. Online Mode
- **Reward Ads**: After a Final game is completed (optional, grants 100 bonus coins)

### 3. In-App Store
- **Reward Ads**: Available in the coin purchase section (optional, grants 50 bonus coins with 5-minute cooldown)

## Installation Steps

### Step 1: Install Google Mobile Ads SDK

1. **Using Unity Package Manager:**
   - Open Unity Editor
   - Go to Window > Package Manager
   - Click the "+" button in the top-left corner
   - Select "Add package from git URL"
   - Enter: `https://github.com/googleads/googleads-mobile-unity.git`
   - Click "Add"

2. **Alternative - Using Asset Package:**
   - Download the latest Google Mobile Ads Unity Plugin from: https://github.com/googleads/googleads-mobile-unity/releases
   - Import the `.unitypackage` into your project

### Step 2: Configure AdMob in Your Project

1. **Create AdMob Account:**
   - Go to https://admob.google.com/
   - Sign in with your Google account
   - Create a new app or select your existing Capicu app

2. **Create Ad Units:**

   **For Android:**
   - Create an Interstitial Ad Unit (for VS AI mode every 3 rounds)
   - Create a Rewarded Ad Unit (for reward ads)
   - Note down the Ad Unit IDs

   **For iOS:**
   - Create an Interstitial Ad Unit (for VS AI mode every 3 rounds)
   - Create a Rewarded Ad Unit (for reward ads)
   - Note down the Ad Unit IDs

3. **Update AdMobManager.cs with Your Ad Unit IDs:**

   Open `Assets/Scripts/Managers/AdMobManager.cs` and replace the test IDs with your actual Ad Unit IDs:

   ```csharp
   [Header("Ad Unit IDs - Android")]
   [SerializeField] private string androidInterstitialAdUnitId = "YOUR_ANDROID_INTERSTITIAL_AD_UNIT_ID";
   [SerializeField] private string androidRewardedAdUnitId = "YOUR_ANDROID_REWARDED_AD_UNIT_ID";

   [Header("Ad Unit IDs - iOS")]
   [SerializeField] private string iosInterstitialAdUnitId = "YOUR_IOS_INTERSTITIAL_AD_UNIT_ID";
   [SerializeField] private string iosRewardedAdUnitId = "YOUR_IOS_REWARDED_AD_UNIT_ID";
   ```

### Step 3: Setup AdMob Manager in Scene

1. **Create AdMobManager GameObject:**
   - In your main menu or persistent scene, create a new empty GameObject
   - Name it "AdMobManager"
   - Add the `AdMobManager.cs` component to it
   - The AdMobManager will persist across scenes (DontDestroyOnLoad)

2. **Configure AdMobManager in Inspector:**
   - Select the AdMobManager GameObject
   - In the Inspector, enter your Ad Unit IDs in the appropriate fields
   - Save the scene

### Step 4: Setup UI Elements

#### For WinnerScreen (Game End Screen):

1. Open the WinnerScreen prefab/scene
2. Add a new UI Button called "WatchAdBtn"
3. Position it appropriately on the screen
4. Add a Text component to the button with the text "Watch Ad (+100 Coins)"
5. Assign the button to the `watchAdBtn` field in the WinnerScreen component

#### For ShopScreen (In-App Store):

1. Open the ShopScreen prefab/scene
2. Add a new UI Button called "WatchAdForCoinsBtn" in the coins section
3. Position it with other coin purchase options
4. Add a Text component to the button with the text "Watch Ad (+50 Coins)"
5. Assign the button to the `watchAdForCoinsBtn` field in the ShopScreen component

### Step 5: Android Configuration

1. **Add AdMob App ID to AndroidManifest.xml:**

   Create or edit `Assets/Plugins/Android/AndroidManifest.xml`:

   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <manifest xmlns:android="http://schemas.android.com/apk/res/android">
       <application>
           <!-- AdMob App ID -->
           <meta-data
               android:name="com.google.android.gms.ads.APPLICATION_ID"
               android:value="ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY"/>
       </application>
   </manifest>
   ```

   Replace `ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY` with your actual AdMob App ID.

2. **Update Gradle Settings (if needed):**

   If you encounter issues, you may need to update your `mainTemplate.gradle` to include:

   ```gradle
   dependencies {
       implementation 'com.google.android.gms:play-services-ads:22.0.0'
   }
   ```

### Step 6: iOS Configuration

1. **Add AdMob App ID to Info.plist:**

   After building for iOS, add the following to your `Info.plist`:

   ```xml
   <key>GADApplicationIdentifier</key>
   <string>ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY</string>
   ```

   Replace `ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY` with your actual AdMob App ID.

2. **Add App Tracking Transparency (ATT) Description:**

   Add to `Info.plist`:

   ```xml
   <key>NSUserTrackingUsageDescription</key>
   <string>This app would like to use your advertising identifier to show you personalized ads.</string>
   ```

### Step 7: Testing

1. **Test with Test Ads:**
   - The AdMobManager is currently configured with test ad unit IDs
   - Run the game and verify ads appear correctly
   - Test all three scenarios:
     - VS AI mode: Play 3 rounds and verify interstitial ad appears
     - Game end: Complete a game and verify reward ad button appears
     - Store: Open shop and verify watch ad button works

2. **Test with Real Ads:**
   - Replace test ad unit IDs with your production IDs
   - Add your test device ID to receive test ads
   - Verify all ad placements work correctly

## Implementation Details

### Files Modified/Created:

1. **Created:**
   - `Assets/Scripts/Managers/AdMobManager.cs` - Main AdMob manager singleton

2. **Modified:**
   - `Assets/Scripts/GamePlay/GridManager.cs` - Added interstitial ad display every 3 rounds in VS AI mode
   - `Assets/Scripts/GamePlay/Panels/WinnerScreen.cs` - Added reward ad support for 100 coins at game end
   - `Assets/New/New Scripts/ShopScreen.cs` - Added reward ad support for 50 coins in store

### Ad Integration Logic:

#### VS AI Mode (GridManager.cs:394-400)
```csharp
// VS AI Mode: Show interstitial ad every 3 rounds
if (roundNum % 3 == 0 && AdMobManager.instance != null)
{
    Debug.Log("Round " + roundNum + " - Showing interstitial ad");
    AdMobManager.instance.ShowInterstitialAd();
    yield return new WaitForSeconds(1.5f); // Wait for ad to potentially show
}
```

#### Game End Reward Ad (WinnerScreen.cs)
- Added `watchAdBtn` UI button
- Shows optional reward ad that grants 100 coins
- Button disabled after watching ad (one per game)
- Works for both VS AI and Online modes

#### Store Reward Ad (ShopScreen.cs)
- Added `watchAdForCoinsBtn` UI button
- Shows optional reward ad that grants 50 coins
- 5-minute cooldown between ad watches
- Syncs with server after reward

## Ad Behavior Details

### Interstitial Ads (VS AI Mode)
- **Trigger:** Every 3rd round completion (rounds 3, 6, 9, etc.)
- **Type:** Mandatory (automatically shown)
- **Duration:** 5-15 seconds (typical)
- **Frequency:** Every 3 rounds

### Reward Ads (Game End)
- **Trigger:** Manual (player clicks "Watch Ad" button)
- **Reward:** 100 bonus coins
- **Frequency:** Once per game (button disabled after watching)
- **Available in:** Both VS AI and Online modes

### Reward Ads (Store)
- **Trigger:** Manual (player clicks "Watch Ad" button in shop)
- **Reward:** 50 bonus coins
- **Cooldown:** 5 minutes between watches
- **Button States:**
  - "Watch Ad (+50 Coins)" - Ready to watch
  - "Wait M:SS" - Cooldown in progress
  - "Loading Ad..." - Ad is loading

## Troubleshooting

### Common Issues:

1. **Ads not showing:**
   - Verify AdMob account is active
   - Check ad unit IDs are correct
   - Ensure test device is added in AdMob console
   - Check internet connection

2. **Build errors:**
   - Verify Google Mobile Ads SDK is properly imported
   - Check AndroidManifest.xml and Info.plist are configured correctly
   - Ensure all dependencies are resolved

3. **Rewards not being granted:**
   - Check AdMobManager singleton is present in scene
   - Verify reward callback is being triggered (check logs)
   - Ensure PlayerPersonalData is properly initialized

### Debug Logs:

The implementation includes extensive logging. Check Unity console for:
- "Initializing Google Mobile Ads SDK..."
- "Interstitial ad loaded successfully."
- "Rewarded ad loaded successfully."
- "Showing interstitial ad."
- "Rewarding player with X coins"

## Revenue Optimization Tips

1. **Ad Frequency:**
   - Current setting: Every 3 rounds for interstitial ads
   - Consider A/B testing different frequencies (every 2 rounds vs every 4 rounds)
   - Monitor user retention vs. ad revenue

2. **Reward Ad Placement:**
   - Current placement: Game end screen and store
   - Consider adding to daily login rewards
   - Consider adding to achievement unlocks

3. **Ad Mediation:**
   - Consider implementing AdMob mediation to maximize fill rate and eCPM
   - Add additional ad networks through mediation

## Next Steps

1. **Get AdMob Account Approved:**
   - Submit your app to AdMob for review
   - Wait for approval (typically 24-48 hours)

2. **Replace Test IDs:**
   - Once approved, replace all test ad unit IDs with production IDs

3. **Monitor Performance:**
   - Track ad impressions, clicks, and revenue in AdMob console
   - Monitor user feedback regarding ad frequency

4. **Optimize:**
   - Adjust ad frequency based on user retention and revenue data
   - Consider implementing ad-free purchase option

## Support

For issues or questions:
- Google AdMob Support: https://support.google.com/admob
- Unity Ads Integration: https://developers.google.com/admob/unity/start

## Version History

- **v1.0** - Initial AdMob integration with interstitial and reward ads
  - VS AI mode: Interstitial every 3 rounds
  - Game end: Reward ad for 100 coins
  - Store: Reward ad for 50 coins with cooldown
