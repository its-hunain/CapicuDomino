using Dominos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class ShopItemBtn : MonoBehaviour
{
    public string productId;
    public string productName;

    public ItemTypeEnum itemType;
    public float price;
    public bool isBought;


    public Button btn;
    public Text priceTxt;
    public Image selectionFrame;



    // Start is called before the first frame update
    void Start()
    {
        if (isBought)
        {
            priceTxt.text = "SELECT";
        }
        else { priceTxt.text = price.ToString() + " coins"; }
        btn.onClick.AddListener(() => OnPurchase());
    }
    void OnEnable()
    {
        //Debug.Log("Table Theme: "+ PlayerPrefs.GetString("TableTheme"));
        if (itemType == ItemTypeEnum.domino && PlayerPrefs.GetString("TileTheme") == productId)
        {
            isBought = true;
            OnPurchase();
        }

        else if (itemType == ItemTypeEnum.table && PlayerPrefs.GetString("TableTheme") == productId)
        {
            isBought = true;
            OnPurchase();
        }


    }


    void OnPurchase()
    {
        UI_Manager.instance.shopScreen.ChangeSelectable();
        
        if (isBought)
        {
            selectionFrame.gameObject.SetActive(true);
            return;
        }

        if (PlayerPersonalData.playerDomiCoins < price)
        {
            UI_Manager.instance.errorPopUpScreen.OpenCloseWarning(true, "You don't have enough coins");
            return;
        }

        BuyProduct();
        selectionFrame.gameObject.SetActive(true);

        switch (itemType)
        {
            case ItemTypeEnum.coin:
                // No action needed for ItemTypeEnum.coin (do nothing)
                break;
            case ItemTypeEnum.domino:
                PlayerPrefs.SetString("TileTheme", productId);
                break;
            case ItemTypeEnum.table:
                PlayerPrefs.SetString("TableTheme", productId);
                break;
        }
    }

    void BuyProduct()
    {
        Dictionary<string, object> postData = new Dictionary<string, object>();
        postData.Add("productId", productId);
        postData.Add("productName", productName);
        postData.Add("productPrice", price.ToString());

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.buyProducts, Method.POST, null, postData, OnSuccess, OnFail, CACHEABLE.NULL, true, null);

    }
    void OnSuccess(string keyValuePairs, long successCode)
    {
        // UI_Manager.instance.purchaseSuccessPanel.SetActive(true);
        if (itemType == ItemTypeEnum.domino)
        {
            PlayerPrefs.SetString("TileTheme", productId);
        }
        else if (itemType == ItemTypeEnum.table)
        {
            PlayerPrefs.SetString("TableTheme", productId);
        }
        else 
        { 
            Debug.Log("Coins Purchased.");
        }
        
        Debug.Log("OnSuccessfullyGetProducts: " + keyValuePairs.ToString());
        UI_Manager.instance.shopScreen.ownedProducts = FetchOwnShopItem.FromJson(keyValuePairs.ToString());

        isBought = true;
        priceTxt.text = "SELECT";
        WaitingLoader.instance.ShowHide();

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getPlayerProfile, Method.GET, null, null, OnSuccessfullyProfileDownload, PlayerPersonalData.OnFailDownload, CACHEABLE.NULL, true, null);
    }

    internal void OnSuccessfullyProfileDownload(string keyValuePairs, long successCode)
    {
        Debug.Log("OnSuccessfullyProfileDownload: " + keyValuePairs.ToString());
        if (!ResponseStatus.Check(successCode))
        {
            Debug.LogError("successCode Error: " + successCode.ToString());
            Debug.LogError("Some Error: " + keyValuePairs.ToString());
            if (successCode == 403)
            {
                WebglUserSession.userLoggedIn = false;
                UI_ScreenManager.OpenClosePopUp(WebglUserSession.instance.userSessionFailedPopUp, true, true);

            }
            return;
        }
        WebglUserSession.userLoggedIn = true;

        PlayerPersonalDataJSON2 playerPersonalDataJson = JsonConvert.DeserializeObject<PlayerPersonalDataJSON2>(keyValuePairs);

        Debug.Log("playerPersonalDataJson: " + JsonConvert.SerializeObject(keyValuePairs.ToString()));
        int coins 
            = WebServiceManager.instance.playerPersonalData.Data.User.Domicoins 
            = PlayerPersonalData.playerDomiCoins 
            = playerPersonalDataJson.Data.User.Domicoins;

        UI_Manager.instance.menuScreen.coinTxt.text = coins.ToString();
        UI_Manager.instance.shopScreen.coinsText.text = coins.ToString();
    }

    void OnFail(string msg)
    {
        Debug.LogError("Error: " + msg);
        WaitingLoader.instance.ShowHide();

    }


    //public void UpdateTileTheme(Color color)
    //{
    //    foreach (var item in domMats)
    //    {
    //        item.color = color;
    //    }
    //}
}
public enum ItemTypeEnum
{
    coin,
    domino,
    table
}


