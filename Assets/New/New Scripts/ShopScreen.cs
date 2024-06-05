using Dominos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MonoBehaviour
{
    public Button coinsBtn;
    public Button tilesBtn;
    public Button tablesBtn;
    public Button backBtn;
    public Button settingsBtn;

    public Text coinsText;

    public GameObject coinsScroll;
    public GameObject tilesScroll;
    public GameObject tablesScroll;

    public Image selectionImage;

    public List<ShopItemBtn> shopBtns;


    [SerializeField]
    public FetchOwnShopItem ownedProducts;// = new List<BuyShopItem>();

    // Start is called before the first frame update
    void Start()
    {
        coinsText.text= PlayerPersonalData.playerDomiCoins.ToString();

        GetOwnedProds();
        coinsBtn.onClick.AddListener(() => SwitchPanels(coinsBtn,coinsScroll));
        tilesBtn.onClick.AddListener(() => SwitchPanels(tilesBtn,tilesScroll));
        tablesBtn.onClick.AddListener(() => SwitchPanels(tablesBtn,tablesScroll));

        backBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.shopScreen.gameObject, false));
        settingsBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.settingScreen.gameObject, true));

        foreach (var item in shopBtns)
        {
            if (ownedProducts.myProducts.Exists(x => x.productId == item.productId))
                item.isBought = true;
        }
    }

    void SwitchPanels(Button btn, GameObject panel)
    {
        print("karwa");
        coinsScroll.SetActive(false);
        tilesScroll.SetActive(false);
        tablesScroll.SetActive(false);

        panel.SetActive(true);
        selectionImage.transform.position = btn.transform.position;
    }

    public void ChangeSelectable()
    {
        foreach (var item in shopBtns)
        {
            if (item.selectionFrame)
            {
                item.selectionFrame.gameObject.SetActive(false);
            }
        }
    }
    [ContextMenu("Try")]
    void GetOwnedProds()
    {
        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getProducts, Method.GET, null, null, OnSuccess, OnFail);
    }
    void OnSuccess(string keyValuePairs, long successCode)
    {
        Debug.Log("OnSuccessfullyGetProducts: " + keyValuePairs.ToString());
        ownedProducts = FetchOwnShopItem.FromJson(keyValuePairs.ToString());


        foreach (var item in shopBtns)
        {
            if (ownedProducts.myProducts.Exists(x => x.productId == item.productId))
                item.isBought = true;
        }


    }
    void OnFail(string msg)
    {
        Debug.Log("OnFailGetProducts: " + msg);

    }


}
