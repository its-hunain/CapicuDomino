using Dominos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCoinsBtn : MonoBehaviour
{
    public Button buyBtn;
    public float price;
    public int value;

    // Start is called before the first frame update
    void Start()
    {
        buyBtn.onClick.AddListener(() => UpdateCoins());
    }

    // Update is called once per frame
    void UpdateCoins()
    {
        Dictionary<string, object> postData = new Dictionary<string, object>();

        postData.Add("coins", value);

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getPlayerProfile, Method.POST, null, postData, OnSuccess, OnFail);
    }

    void OnSuccess(string data, long code)
    {
        User user = User.FromJson(data.ToString());

        WebServiceManager.instance.playerPersonalData.Data.User.Domicoins=PlayerPersonalData.playerDomiCoins = user.Domicoins;

        UI_Manager.instance.menuScreen.coinTxt.text = user.Domicoins.ToString();
        UI_Manager.instance.shopScreen.coinsText.text = user.Domicoins.ToString();

        UI_Manager.instance.ChangeScreen (UI_Manager.instance.purchaseSuccessPanel,true);
    }
    void OnFail(string data)
    {
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.purchaseFailPanel, true);

    }
}
