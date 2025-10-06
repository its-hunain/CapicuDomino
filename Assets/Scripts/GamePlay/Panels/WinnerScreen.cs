using System.Collections;
using System.Collections.Generic;
using Dominos;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinnerScreen : MonoBehaviour
{
    public Image Status;
    public Image Avatar;
    public Text DomiCoinStatus;
    public Text DomiCoins;

    public Image WinBorder;
    public Image LostBorder;

    public List<GameObject> modeSprites = new List<GameObject>();

    public Button continueBtn;
    public Button watchAdBtn; // New button for watching reward ad

    private bool rewardAdWatched = false;

    void Start()
    {
        continueBtn.onClick.AddListener(() => ExitTheGame());

        // Setup reward ad button
        if (watchAdBtn != null)
        {
            watchAdBtn.onClick.AddListener(() => ShowRewardAd());
            UpdateRewardAdButton();
        }

        GameTypePopup();

        // Automatically show reward ad option at the end of game
        ShowRewardAdOption();
    }

    public void ExitTheGame()
    {
        if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

        if (GameRulesManager.currentSelectedGame_GameType == GameRulesManager.GameType.Tournament)
        {//dispach event on tournament match ending
            //JS_Hook.instance.OnTournamentMatchEnded();
        }

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer && GameManager.instace.NakamaConnection.Socket.IsConnected)
        {

            Debug.Log(gameObject.name + " detect error: ln: 36");
            GameManager.instace.OnRequestQuitMatch.Invoke();
        }
        else
        {
            GridManager.ResetStaticFields();
            SceneManager.LoadScene(Global.UIScene);
        }
    }
    public void GameTypePopup()
    {

        //    GameMode4, Block
        //  GameMode5, Capicu and Nines
        int index = 0;
        foreach (var item in modeSprites)
        {
            item.gameObject.SetActive(false);
        }


        if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode4)
            index = 1;

        else if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode5)
        {
            if (Rule5.isNines)
                index = 2;

            else
                index = 0;
        }
        modeSprites[index].gameObject.SetActive(true);

    }

    private void ShowRewardAdOption()
    {
        // Show reward ad option based on game mode
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
        {
            // VS AI Mode: Always show reward ad option (100 coins)
            Debug.Log("VS AI Mode - Reward ad available for 100 coins");
        }
        else if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
        {
            // Online Mode: Show reward ad option after Final game (100 coins)
            Debug.Log("Online Mode - Reward ad available for 100 coins");
        }
    }

    private void ShowRewardAd()
    {
        if (rewardAdWatched)
        {
            Debug.Log("Reward ad already watched for this game");
            return;
        }

        if (AdMobManager.instance != null && AdMobManager.instance.IsRewardedAdReady())
        {
            int rewardAmount = 100; // 100 coins for completing Final game

            AdMobManager.instance.ShowRewardedAd(
                onCompleted: () =>
                {
                    Debug.Log($"Rewarding player with {rewardAmount} coins");
                    RewardPlayer(rewardAmount);
                    rewardAdWatched = true;
                    UpdateRewardAdButton();
                },
                onFailed: () =>
                {
                    Debug.LogWarning("Reward ad failed to show");
                    if (MesgBar.instance != null)
                    {
                        MesgBar.instance.show("Ad is not available right now. Please try again later.");
                    }
                }
            );
        }
        else
        {
            Debug.LogWarning("Reward ad is not ready");
            if (MesgBar.instance != null)
            {
                MesgBar.instance.show("Ad is not available right now. Please try again later.");
            }
        }
    }

    private void RewardPlayer(int amount)
    {
        // Show success message first
        if (MesgBar.instance != null)
        {
            MesgBar.instance.show($"You earned {amount} bonus coins!" , false);
        }

        // Sync with server (server will add the amount and return new total)
        SyncCoinsWithServer(amount);
    }

    private void SyncCoinsWithServer(int amount)
    {
        // Send the amount to ADD (not the total)
        Dictionary<string, object> postData = new Dictionary<string, object>();
        postData.Add("coins", amount);

        WebServiceManager.instance.APIRequest(
            WebServiceManager.instance.getPlayerProfile,
            Method.POST,
            null,
            postData,
            (data, code) =>
            {
                // Update local coins with server response
                User user = User.FromJson(data.ToString());
                WebServiceManager.instance.playerPersonalData.Data.User.Domicoins = PlayerPersonalData.playerDomiCoins = user.Domicoins;
                
            },
            (msg) => { Debug.LogWarning($"WinnerScreen: Failed to sync coins with server: {msg}"); }
        );
    }

    private void UpdateRewardAdButton()
    {
        if (watchAdBtn != null)
        {
            // Disable button if ad already watched or if AdMob is not ready
            bool isAdReady = AdMobManager.instance != null && AdMobManager.instance.IsRewardedAdReady();
            watchAdBtn.interactable = !rewardAdWatched && isAdReady;

            // Update button text if it has a Text component
            Text btnText = watchAdBtn.GetComponentInChildren<Text>();
            if (btnText != null)
            {
                if (rewardAdWatched)
                {
                    btnText.text = "Ad Watched";
                }
                else
                {
                    btnText.text = "Watch Ad (+100 Coins)";
                }
            }
        }
    }

}
    