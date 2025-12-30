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
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // 🔹 Register ALL products here
        builder.AddProduct("120coins", ProductType.Consumable);
        builder.AddProduct("650coins", ProductType.Consumable);
        builder.AddProduct("1500coins", ProductType.Consumable);
        builder.AddProduct("3500coins", ProductType.Consumable);
        builder.AddProduct("9500coins", ProductType.Consumable);
        builder.AddProduct("20000coins", ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
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
        Debug.Log("IAP Initialized Successfully");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP Init Failed: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP Init Failed: {error} - {message}");
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
