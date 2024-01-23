using System;
using System.Collections;
using System.Collections.Generic;
using Dominos;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectCoinsToPlayScreen : MonoBehaviour
{
    [Header("PopUp Items")]
    public Button backBtn;
    public CoinButton coinsBtnPrefab;
    public List<CoinButton> coinsBtnsList;
    public Transform coinsParentTransform;
    //public Button coins150Btn;
    //public Button coins200Btn;

    Animation anim;
    AnimationClip animClip;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        animClip = anim.GetClip("SelectCoinsToPlayScreen");
    }


    public void OnEnable()
    {
        anim[animClip.name].speed = 1;
        //anim[animClip.name].time = anim[animClip.name].length;
        anim.Play(animClip.name);
        ChangeSelectable();

        //ResetAllButtonState();

        LoadData();
    }

    public void OnDisable()
    {
        anim[animClip.name].speed = -1;
        anim[animClip.name].time = anim[animClip.name].length;
        anim.Play(animClip.name);
        DestroyOldData();
    }

    void Start()
    {
        backBtn.onClick.AddListener(() => backButtonDelegate());
    }


    private void LoadData()
    {

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
        {
            GameCenterDetails gameCenterDetail = UI_ScreenManager.instance.gameCenterSelectionScreen.gameCenters.data.Find(x => x._id == GameCenterSelectionScreen.selectedGameCenterID);
            RuleDetails ruleDetail = gameCenterDetail.gameCenterGames.Find(ruleDetail => ruleDetail.Rules[0].gameRulesName == GameRulesManager.currentSelectedGame_Rule);
            SpawnCoinsButton(ruleDetail);
        }
        else
        {
            SpawnCoinsInBotGame();
        }
    }

    int selectedCoinsToPlay;

    public void MoveToNextScreen(Button selectedBtn, int coinsToPlay)
    {
        selectedCoinsToPlay = coinsToPlay;

        SoundManager.instance.SelectModePlayer(true);

        Debug.Log("coinsToPlay: " + coinsToPlay);
        Debug.Log("playerDomiCoins: " + PlayerPersonalData.playerDomiCoins);

        ChangeSelectable(selectedBtn);

        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getDomiCoins, Method.GET, null, null, OnSuccessfullyDataDownloaded, OnFail, CACHEABLE.NULL, true, null);
    }

    private void OnFail(string obj)
    {
        Debug.LogError(obj);
        UI_ScreenManager.instance.errorPopUpScreen.OpenCloseWarning(true, "Something went wrong");
    }

    private void OnSuccessfullyDataDownloaded(JObject data, long code)
    {
        Debug.Log(data.ToString());
        if (ResponseStatus.Check(code))
        {

            GetDomiCoinsData domiCoinsData = GetDomiCoinsData.FromJson(data.ToString());
            if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer && domiCoinsData.Domicoins < selectedCoinsToPlay)
            {
                UI_ScreenManager.instance.errorPopUpScreen.OpenCloseWarning(true, "Not enough DomiCoins, please buy more to proceed");
            }
            else
            {
                GameRulesManager.currentSelectedGame_CoinsToPlay = selectedCoinsToPlay;
                OnDisable();
                StartCoroutine(UI_ScreenManager._ChangeScreen(UI_ScreenManager.instance.confirmationPopUpGameSelection.gameObject, gameObject, true));
            }
        }
        else
        {
            Debug.LogError("Something went wrong");
            UI_ScreenManager.instance.errorPopUpScreen.OpenCloseWarning(true, "Something went wrong");
        }
    }

    private void ChangeSelectable(Button selectedBtn = null)
    {
        foreach (var item in coinsBtnsList)
        {
            item.transform.GetChild(0).gameObject.SetActive(false);
        }

        if(selectedBtn!=null) selectedBtn.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void backButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);
        OnDisable();
        ChangeSelectable();
        StartCoroutine(UI_ScreenManager._ChangeScreen(UI_ScreenManager.instance.no_Of_Player_SelectionScreen.gameObject, gameObject, false));
    }


    private void SpawnCoinsButton(RuleDetails ruleDetail)
    {
        for (int i = 0; i < ruleDetail.Rules[0].coins.Count; i++)
        {
            CoinButton coinButton = Instantiate(coinsBtnPrefab,coinsParentTransform);
            coinButton.SetValue(ruleDetail.Rules[0].coins[i]);
            coinsBtnsList.Add(coinButton);
        }
    }


    private void SpawnCoinsInBotGame()
    {
        for (int i = 1; i < 4; i++)
        {
            CoinButton coinButton = Instantiate(coinsBtnPrefab,coinsParentTransform);
            coinButton.SetValue(i*100);
            coinsBtnsList.Add(coinButton);
        }
    }



    public void DestroyOldData()
    {
        Debug.Log("DestroyOldData");

        foreach (var item in coinsBtnsList)
        {
            Destroy(item.gameObject);
        }

        coinsBtnsList.Clear();
    }


}