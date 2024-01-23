using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBuilder;
using AvatarBuilder;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using static GameRulesManager;

public class SplashScreen : MonoBehaviour
{
    public bool isAnimating = true;
    public float waitTime;
    public Image loadingImage;
    public Text loadingTxt;
    public GameObject ErrorPopUp;
    public Text ErrorPopUpTxt;
    public static SplashScreen instance;

    private void OnEnable()
    {
        
        isAnimating = true;
    }

    private void Awake()
    {
        //Application.runInBackground = true; // :)
        //QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;


        if (instance == null)
        {
            instance = this;
        }


    }
    // Update is called once per frame
    void Update()
    {
        if (isAnimating && loadingImage != null)
        {
            //fill in waitTime seconds
            loadingImage.fillAmount += 1.0f / waitTime * Time.deltaTime;
            loadingTxt.text = ((int)(loadingImage.fillAmount * 100)).ToString()+"%";
            if (loadingImage.fillAmount >=1.0f)
            {
                isAnimating = false;
                SceneManager.LoadScene(Global.UIScene);

            }
        }
    }

    //private IEnumerator Start()
    //{
    //    yield return new WaitUntil(() => !string.IsNullOrEmpty(Global.GetBearerToken) && !string.IsNullOrEmpty(WebServiceManager.baseURL));

    //}

    public void OnLoadingCompleted()
    {
        StartCoroutine(_OnLoadingCompleted());
    }
    public IEnumerator _OnLoadingCompleted()
    {
        yield return new WaitUntil(()=> !isAnimating);

        string SceneName = currentSelectedGame_GameType.Equals(GameType.Tournament) ? Global.GameScene : Global.UIScene;
        Debug.Log("Scene Loading: " + SceneName);
        SceneManager.LoadScene(SceneName);
    }

    internal static void GetMatchTypeForGameCallBack(TournamentDetails details)
    {

        if (details.GameType.Equals(GameType.Tournament.ToString()))
        {
            Debug.Log("Tournament Match");
            Debug.Log("tournamentDetails.gameRule: " + details.gameRule);
            Debug.Log("tournamentDetails.coinsToPlay: " + details.coinsToPlay);
            Debug.Log("tournamentDetails.maxPlayers: " + details.maxPlayers);
            Debug.Log("tournamentDetails.timeDifference: " + details.timeDifference);
            Debug.Log("tournamentDetails.matchStatus: " + details.matchStatus);
            Debug.Log("tournamentDetails.matchID: " + details.matchID);
            Debug.Log("tournamentDetails.tournamentID: " + details.tournamentID);
            Debug.Log("tournamentDetails.gameCenterID: " + details.gameCenterID);


            matchDetails = details;
            currentSelectedGame_GameType    =  GameType.Tournament;
            currentSelectedGame_MatchType   =  MatchType.Multiplayer;
            currentSelectedGame_CoinsToPlay = details.coinsToPlay;
            GameCenterSelectionScreen.selectedGameCenterID = details.gameCenterID;
            GameRules gameRule = (GameRules)Enum.Parse(typeof(GameRules), details.gameRule);
            currentSelectedGame_Rule    = gameRule;
            noOfPlayers = details.maxPlayers;
        }
        else                                                    
        {
            Debug.Log("Single Match");
            currentSelectedGame_GameType    =  GameType.SingleMatch;
        }

        Debug.Log("GetMatchTypeForGameCallBack Successfully.");
    }

    public void OnSuccessfullyMatchDetailsDownload(JObject arg1, long code)
    {
        if (ResponseStatus.Check(code))
        {
            Debug.Log("arg1.ToString() => " + arg1.ToString());
            TournamentDetails matchDetails = TournamentDetails.FromJson(arg1.ToString());
            
            GetMatchTypeForGameCallBack(matchDetails);
            if (matchDetails.matchStatus == "open")
            {
                Debug.LogError("Match Details: " + matchDetails.ToJson());

                OnLoadingCompleted();
            }
            else
            {
                Debug.LogError("Match Status: " + matchDetails.matchStatus);
                ShowError("The Match has already been " + matchDetails.matchStatus + ".");
            }
        }
        else
        {
            Debug.LogError("Some Error in getTournamentMatchDetails...");
            ShowError("Match not Found.");
        }
    }

    private void ShowError(string msg)
    {
        ErrorPopUpTxt.text = msg;
        ErrorPopUp.SetActive(true);
    }

    public void OnFail(string response)
    {
        Debug.LogError("Error OnFail getTournamentMatchDetails: " + response);
        ShowError("Msg: " + response);
    }
}