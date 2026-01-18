using Dominos;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    public static IAPManager Instance;

    private IStoreController storeController;
    private IExtensionProvider extensionProvider;
    private bool isPurchasing = false;
    private int pendingCoinValue;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeIAP();
    }

    void InitializeIAP()
    {
        Debug.Log("========================================");
        Debug.Log("IAP INITIALIZATION STARTED");
        Debug.Log("========================================");

        // 🔑 CRITICAL: Override the default store BEFORE initializing
        // This ensures Unity IAP uses the correct store regardless of BillingMode.json
        #if UNITY_ANDROID && !UNITY_EDITOR
        DefaultStoreHelper.OverrideDefaultStoreName(GooglePlay.Name);
        Debug.Log("✅ Forced Google Play Store for Android");
        #elif UNITY_IOS && !UNITY_EDITOR
        DefaultStoreHelper.OverrideDefaultStoreName(AppleAppStore.Name);
        Debug.Log("✅ Forced Apple App Store for iOS");
        #endif

        // Initialize with StandardPurchasingModule
        var module = StandardPurchasingModule.Instance();

        // IMPORTANT: Only configure fake store in Unity Editor
        // Production builds should NEVER set useFakeStoreUIMode or useFakeStoreAlways
        #if UNITY_EDITOR
        module.useFakeStoreUIMode = FakeStoreUIMode.DeveloperUser;
        Debug.Log("⚠️ Editor mode - using fake store for testing");
        #else
        // DO NOT set any fake store properties here
        // Let Unity IAP use the real Google Play/App Store automatically
        Debug.Log("✅ Production build - will use real store (Google Play/App Store)");
        #endif

        var builder = ConfigurationBuilder.Instance(module);

        // ℹ️ NOTE: In Unity IAP 5.x, the Google Play public key is configured automatically
        // through Unity Services and the generated GooglePlayTangle.cs file.
        // No manual SetPublicKey() call is needed - Unity IAP handles this internally.

        // 🔹 Register ALL products here
        // IMPORTANT: These product IDs must match exactly with your Google Play Console and App Store Connect
        Debug.Log("Registering IAP products...");
        builder.AddProduct("120coins", ProductType.Consumable);
        builder.AddProduct("650coins", ProductType.Consumable);
        builder.AddProduct("1500coins", ProductType.Consumable);
        builder.AddProduct("3500coins", ProductType.Consumable);
        builder.AddProduct("9500coins", ProductType.Consumable);
        builder.AddProduct("20000coins", ProductType.Consumable);
        Debug.Log("✅ 6 products registered");

        Debug.Log("Calling UnityPurchasing.Initialize...");
        UnityPurchasing.Initialize(this, builder);
        Debug.Log("IAP Initialization request sent - waiting for callback...");
    }

    #region Public Purchase API

    public void BuyCoins(string productId, int coinValue)
    {
        if (isPurchasing || storeController == null)
        {
            Debug.LogWarning("Purchase blocked or IAP not ready");
            return;
        }

        isPurchasing = true;
        pendingCoinValue = coinValue;

#if UNITY_EDITOR
        Debug.Log("EDITOR MODE: Simulating purchase");
        OnPurchaseSuccess();
        return;
#endif

        storeController.InitiatePurchase(productId);
    }

    #endregion

    #region Unity IAP Callbacks

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;
        Debug.Log("========================================");
        Debug.Log("✅✅✅ IAP INITIALIZED SUCCESSFULLY ✅✅✅");
        Debug.Log("========================================");

        // Check which store is being used
        #if UNITY_ANDROID
        Debug.Log($"Platform: Android");
        Debug.Log($"Store Name: {StandardPurchasingModule.Instance().appStore}");
        var googleConfig = extensions.GetExtension<IGooglePlayStoreExtensions>();
        if (googleConfig != null)
        {
            Debug.Log("✅ Google Play Store Extensions detected - using REAL store");
        }
        else
        {
            Debug.LogWarning("⚠️ Google Play Store Extensions NOT found - might be using fake store");
        }
        #elif UNITY_IOS
        Debug.Log($"Platform: iOS");
        Debug.Log($"Store Name: {StandardPurchasingModule.Instance().appStore}");
        #endif

        Debug.Log("Available products:");
        foreach (var product in controller.products.all)
        {
            Debug.Log($"  - {product.definition.id}: Available={product.availableToPurchase}, Price={product.metadata.localizedPriceString}");
        }
        Debug.Log("========================================");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("========================================");
        Debug.LogError("❌❌❌ IAP INITIALIZATION FAILED ❌❌❌");
        Debug.LogError("========================================");
        Debug.LogError("Reason: " + error);
        Debug.LogError("========================================");
        Debug.LogError("Possible causes:");
        Debug.LogError("1. In-App Purchase capability not enabled in Xcode");
        Debug.LogError("2. Products not created in App Store Connect");
        Debug.LogError("3. Bundle ID mismatch");
        Debug.LogError("4. StoreKit framework not linked");
        Debug.LogError("========================================");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("========================================");
        Debug.LogError("❌❌❌ IAP INITIALIZATION FAILED ❌❌❌");
        Debug.LogError("========================================");
        Debug.LogError($"Reason: {error}");
        Debug.LogError($"Message: {message}");
        Debug.LogError("========================================");
        Debug.LogError("Possible causes:");
        Debug.LogError("1. In-App Purchase capability not enabled in Xcode");
        Debug.LogError("2. Products not created in App Store Connect");
        Debug.LogError("3. Bundle ID mismatch");
        Debug.LogError("4. StoreKit framework not linked");
        Debug.LogError("========================================");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("Purchase Success: " + args.purchasedProduct.definition.id);

        // 🔐 IMPORTANT: Send receipt to backend in production
        // args.purchasedProduct.receipt

        OnPurchaseSuccess();
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError("Purchase Failed: " + reason);
        isPurchasing = false;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.purchaseFailPanel, true);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
    {
        Debug.LogError("Purchase Failed: " + description);
        isPurchasing = false;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.purchaseFailPanel, true);
    }

    #endregion

    #region Backend Integration

    void OnPurchaseSuccess()
    {
        Dictionary<string, object> postData = new Dictionary<string, object>();
        postData.Add("coins", pendingCoinValue);

        WebServiceManager.instance.APIRequest(
            WebServiceManager.instance.getPlayerProfile,
            Method.POST,
            null,
            postData,
            OnCoinsUpdated,
            OnBackendFail
        );
    }

    void OnCoinsUpdated(string data, long code)
    {
        isPurchasing = false;

        User user = User.FromJson(data);
        WebServiceManager.instance.playerPersonalData.Data.User.Domicoins =
            PlayerPersonalData.playerDomiCoins = user.Domicoins;

        UI_Manager.instance.menuScreen.coinTxt.text = user.Domicoins.ToString();
        UI_Manager.instance.shopScreen.coinsText.text = user.Domicoins.ToString();

        UI_Manager.instance.ChangeScreen(UI_Manager.instance.purchaseSuccessPanel, true);
    }

    void OnBackendFail(string error)
    {
        isPurchasing = false;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.purchaseFailPanel, true);
    }

    #endregion
}
