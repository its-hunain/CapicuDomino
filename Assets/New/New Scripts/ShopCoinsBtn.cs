using Dominos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class ShopCoinsBtn : MonoBehaviour, IDetailedStoreListener
{
    public Button buyBtn;
    public string productId; // Set this in the Unity Inspector (e.g., "com.yourcompany.100coins")
    public float price;
    public int value;

    private IStoreController storeController;
    private IExtensionProvider extensionProvider;
    private bool isPurchasing = false;

    void Start()
    {
        buyBtn.onClick.AddListener(() => InitiatePurchase());
        InitializeIAP();
    }

    void InitializeIAP()
    {
        if (storeController == null)
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(productId, ProductType.Consumable);
            UnityPurchasing.Initialize(this, builder);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;
        Debug.Log("IAP Initialization successful for: " + productId);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP Initialization Failed: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("IAP Initialization Failed: " + error + " - " + message);
    }

    void InitiatePurchase()
    {
        if (isPurchasing)
        {
            Debug.Log("Purchase already in progress");
            return;
        }

#if UNITY_EDITOR
        // For testing in Unity Editor - bypass IAP and directly call backend
        Debug.Log("EDITOR MODE: Simulating successful IAP purchase");
        isPurchasing = true;
        UpdateCoins();
        return;
#endif

        if (storeController == null)
        {
            Debug.LogError("Store Controller not initialized");
            UI_Manager.instance.ChangeScreen(UI_Manager.instance.purchaseFailPanel, true);
            return;
        }

        isPurchasing = true;
        storeController.InitiatePurchase(productId);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (string.Equals(args.purchasedProduct.definition.id, productId, System.StringComparison.Ordinal))
        {
            Debug.Log("Purchase successful: " + productId);

            // IAP successful, now call backend API
            UpdateCoins();

            return PurchaseProcessingResult.Complete;
        }

        isPurchasing = false;
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("Purchase Failed: " + failureReason);
        isPurchasing = false;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.purchaseFailPanel, true);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError("Purchase Failed: " + failureDescription);
        isPurchasing = false;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.purchaseFailPanel, true);
    }

    void UpdateCoins()
    {
        Dictionary<string, object> postData = new Dictionary<string, object>();
        postData.Add("coins", value);

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getPlayerProfile, Method.POST, null, postData, OnSuccess, OnFail);
    }

    void OnSuccess(string data, long code)
    {
        isPurchasing = false;
        User user = User.FromJson(data.ToString());

        WebServiceManager.instance.playerPersonalData.Data.User.Domicoins=PlayerPersonalData.playerDomiCoins = user.Domicoins;

        UI_Manager.instance.menuScreen.coinTxt.text = user.Domicoins.ToString();
        UI_Manager.instance.shopScreen.coinsText.text = user.Domicoins.ToString();

        UI_Manager.instance.ChangeScreen(UI_Manager.instance.purchaseSuccessPanel,true);
    }

    void OnFail(string data)
    {
        isPurchasing = false;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.purchaseFailPanel, true);
    }
}
