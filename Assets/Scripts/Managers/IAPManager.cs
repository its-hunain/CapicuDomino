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
            InitializeIAP();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeIAP()
    {
        Debug.Log("========================================");
        Debug.Log("IAP INITIALIZATION STARTED");
        Debug.Log("========================================");

        // Initialize with StandardPurchasingModule - will use real stores when billing library is present
        var module = StandardPurchasingModule.Instance();

        // IMPORTANT: Disable fake store for production builds
        // Fake store should only be used in Unity Editor for testing
        #if !UNITY_EDITOR
        module.useFakeStoreAlways = false;
        module.useFakeStoreUIMode = FakeStoreUIMode.DeveloperUser;
        Debug.Log("✅ Fake store disabled - using real store");
        #else
        Debug.Log("⚠️ Editor mode - using fake store for testing");
        #endif

        var builder = ConfigurationBuilder.Instance(module);

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
