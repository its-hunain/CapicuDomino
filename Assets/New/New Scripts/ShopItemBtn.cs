using Dominos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


    public List<Material> domMats = new List<Material>();


    // Start is called before the first frame update
    void Start()
    {
        if (isBought)
        {
            priceTxt.text = "SELECT";
        }
        btn.onClick.AddListener(() => OnPurchase());
    }
    void OnEnable()
    {
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
        UI_Manager.instance.purchaseSuccessPanel.SetActive(!isBought);
        UI_Manager.instance.shopScreen.ChangeSelectable();


        switch (itemType)
        {
            case ItemTypeEnum.coin:
                break;

            case ItemTypeEnum.domino:
                if (!isBought)
                {
                    BuyProduct();
                }
               // UpdateTileTheme(Color.blue);
                selectionFrame.gameObject.SetActive(true);
                PlayerPrefs.SetString("TileTheme", productId);
                break;

            case ItemTypeEnum.table:
                if (!isBought)
                {
                    BuyProduct();
                }
                selectionFrame.gameObject.SetActive(true);
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
        Debug.Log("OnSuccessfullyGetProducts: " + keyValuePairs.ToString());
        UI_ScreenManager.instance.ownedProducts.Add(BuyShopItem.FromJson(keyValuePairs.ToString()));

        isBought = true;
        priceTxt.text = "SELECT";
        WaitingLoader.instance.ShowHide();
    }


    void OnFail(string msg)
    {
        Debug.LogError("Error: " + msg);
        WaitingLoader.instance.ShowHide();

    }


    public void UpdateTileTheme(Color color)
    {
        foreach (var item in domMats)
        {
            item.color = color;
        }
    }
}
public enum ItemTypeEnum
{
    coin,
    domino,
    table
}


