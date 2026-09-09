using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing;
using System.IO;

public class ConfigureGooglePlayStore
{
    [MenuItem("Tools/Configure IAP for Both Stores")]
    public static void ConfigureStore()
    {
        // Configure Unity IAP to use Google Play Store for Android
        UnityEditor.Purchasing.UnityPurchasingEditor.TargetAndroidStore(AppStore.GooglePlay);

        // Manually update BillingMode.json to include iOS configuration
        string billingModePath = "Assets/Resources/BillingMode.json";
        string billingModeContent = "{\"androidStore\":\"GooglePlay\",\"iOSStore\":\"AppleAppStore\"}";

        File.WriteAllText(billingModePath, billingModeContent);
        AssetDatabase.ImportAsset(billingModePath);

        Debug.Log("✅ Google Play Store configured for Android!");
        Debug.Log("✅ Apple App Store configured for iOS!");
        Debug.Log("✅ BillingMode.json has been created with both stores");
        Debug.Log("✅ Now build your AAB/IPA and test");

        AssetDatabase.Refresh();
    }
}
