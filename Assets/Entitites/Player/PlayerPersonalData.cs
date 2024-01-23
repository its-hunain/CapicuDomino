using System;
using System.Collections;
using System.Collections.Generic;
using Dominos;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerPersonalData : MonoBehaviour
{
    
    public static string Player_OS;
    public static string GameId;
    public static string playerUserID;
    public static int playerSeatID;
    public static int playerScore;
    public static float playerDomiCoins;
    public static string playerName;
    public static string playerEmail;
    public static string playerPassword;
    public static string authProvider;
    public static bool playerWhiteListed;

    public static float discount;
    public static string location;
    public static string profilePicURL;
    public static PlayerStates playerStates;
    public static UserGameModeScoreInfo[] userGameModeScoreInfos;
    public static Texture2D playerTexture;
    [Header("Nakama Details")]
    public static string sessionID;
    internal static List<FriendDetail> facebookFriends;

    internal static void OnSuccessfullyProfileDownload(JObject keyValuePairs, long successCode)
    {
        //Debug.Log("OnSuccessfullyProfileDownload: "  + keyValuePairs.ToString());
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

        PlayerPersonalDataJSON playerPersonalDataJson = PlayerPersonalDataJSON.FromJson(keyValuePairs.ToString());
        playerUserID = playerPersonalDataJson.Data.User.userID;
        playerName = playerPersonalDataJson.Data.User.FirstName + " " + playerPersonalDataJson.Data.User.LastName;
        playerEmail = playerPersonalDataJson.Data.User.Email;
        playerDomiCoins = float.Parse(playerPersonalDataJson.Data.User.Domicoins);
        //Debug.LogError("playerDomiCoins: " + playerDomiCoins);
        playerWhiteListed = playerPersonalDataJson.Data.User.WhiteListed;
        //discount = playerPersonalDataJson.Data.User.discount;
        //Debug.Log("discount: " + discount.ToString());
        playerPassword = playerPersonalDataJson.Data.User.Email;
        location = playerPersonalDataJson.Data.User.Country;
        profilePicURL = playerPersonalDataJson.Data.User.ProfilePicUrl;

        userGameModeScoreInfos = playerPersonalDataJson.Data.userGameModeScoreInfo;

        PlayerStates playerStatesJson = new PlayerStates();
        playerStatesJson.gamesPlayed = playerPersonalDataJson.Data.Games.GamesPlayed;
        playerStatesJson.gamesWon = playerPersonalDataJson.Data.Games.GamesWon;
        playerStatesJson.gamesWonPercentage = playerPersonalDataJson.Data.Games.GamesWonPercentage;

        playerStatesJson.tournnamentPlayed = playerPersonalDataJson.Data.TournamentStats.TournamentPlayed;
        playerStatesJson.tournnamentWon = playerPersonalDataJson.Data.TournamentStats.TournamentWon;
        playerStatesJson.tournnamentLost = playerPersonalDataJson.Data.TournamentStats.TournamentLost;
        playerStatesJson.championshipsWon = playerPersonalDataJson.Data.TournamentStats.ChampionshipWon;

        playerStatesJson.playerClass = playerPersonalDataJson.Data.User.playerClass;
        playerStatesJson.playerFlagShortCode = playerPersonalDataJson.Data.User.FlagShortCode;

        //WebServiceManager.instance.FindFlagSprite(playerStatesJson.playerFlagShortCode, playerStatesJson.flagSprite);
        playerStatesJson.classSprite = WebServiceManager.instance.FindClassSprite(playerStatesJson.playerClass);

        UserGameModeInfo userGameModeInfo = new UserGameModeInfo();
        userGameModeInfo.userGameModeScoreInfos = userGameModeScoreInfos;

        playerStatesJson.userGameModeInfo = userGameModeInfo;

        playerStates = playerStatesJson;

        WebServiceManager.instance.playerPersonalData = playerPersonalDataJson;
        WebServiceManager.instance.StartCoroutine(_GetTexture(profilePicURL));
        WebServiceManager.instance.StartCoroutine(_GetFlag(playerStatesJson.playerFlagShortCode));

        if (SceneManager.GetActiveScene().name.Equals(Global.SplashScene))
        {
            Debug.Log("Global.gameType: " + Global.gameType);
            SplashScreen.instance.OnLoadingCompleted();
        }
    }

    private static void OnFailfullyWhiteListedDiscoountDownload(string msg)
    {
        Debug.LogError("Error: " + msg);
    }

    private static void OnSuccessfullyWhiteListedDiscoountDownload(JObject keyValuePairs, long code)
    {
        string resp = keyValuePairs.ToString();
        if (ResponseStatus.Check(code))
        {
            CheckWhiteList checkWhiteList = CheckWhiteList.FromJson(resp);
            playerWhiteListed = checkWhiteList.isWhitelisted;
            discount = checkWhiteList.discount;
            Debug.Log("OnSuccessfullyWhiteListedDiscoountDownload: " + resp);
        }
        else
        {
            Debug.LogError("Error: " + resp);
        }
    }

    /// <summary>
    /// Download My Picture
    /// </summary>
    /// <returns></returns>
    static IEnumerator _GetTexture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            PlayerPersonalData.playerTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }


    /// <summary>
    /// Download My Flag
    /// </summary>
    /// <returns></returns>
    static IEnumerator _GetFlag(string shortCode)
    {
        //UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://countryflagsapi.com/png/" + shortCode);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://flagcdn.com/h20/" + shortCode.ToLower() + ".jpg");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            
            var tempTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            playerStates.flagSprite = Sprite.Create((Texture2D)tempTexture, new Rect(0.0f, 0.0f, tempTexture.width, tempTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    internal static void OnFailDownload(string msg)
    {
        Debug.LogError("Error: " + msg);
    }
}


[Serializable]
public class PlayerStates
{
    public string gamesPlayed;
    public string gamesWon;
    public string gamesWonPercentage;

    public int tournnamentPlayed;
    public int tournnamentWon;
    public int tournnamentLost;

    public int championshipsWon;

    public string playerClass;
    public string playerFlagShortCode;

    public Sprite flagSprite;
    public Sprite classSprite;

    [SerializeField]
    public BlockChainData blockChainData;

    [SerializeField]
    public UserGameModeInfo userGameModeInfo;

    public void GenerateDummyData()
    {   
        gamesPlayed = UnityEngine.Random.Range(0, 10).ToString();
        gamesWon = UnityEngine.Random.Range(0, 10).ToString();
        gamesWonPercentage = UnityEngine.Random.Range(0f, 10.0f).ToString();

        tournnamentPlayed = UnityEngine.Random.Range(0, 10);
        tournnamentWon = UnityEngine.Random.Range(0, 10);
        tournnamentLost = UnityEngine.Random.Range(0, 10);

        championshipsWon = UnityEngine.Random.Range(0, 10);

        playerClass = WebServiceManager.instance.classSprites[UnityEngine.Random.Range(0, WebServiceManager.instance.classSprites.Count)].name;
        playerFlagShortCode = WebServiceManager.instance.flagSprites[UnityEngine.Random.Range(0, WebServiceManager.instance.flagSprites.Count)].name;

        //WebServiceManager.instance.FindFlagSprite(playerFlagShortCode , flagSprite);
        flagSprite = WebServiceManager.instance.FindFlagSprite(playerFlagShortCode);
        classSprite = WebServiceManager.instance.FindClassSprite(playerClass);

        userGameModeInfo = new UserGameModeInfo();
        userGameModeInfo.userGameModeScoreInfos = GenerateRandomGameScoreInfoForBot();
    }


    UserGameModeScoreInfo[] GenerateRandomGameScoreInfoForBot()
    {
        List<UserGameModeScoreInfo> userGameModeScoreInfos = new List<UserGameModeScoreInfo>();
        UserGameModeScoreInfo userGameModeScoreInfo = new UserGameModeScoreInfo();
        userGameModeScoreInfo.GameMode = GameRulesManager.currentSelectedGame_Rule;
        userGameModeScoreInfo.medalRank = UnityEngine.Random.Range(1, 6);
        userGameModeScoreInfos.Add(userGameModeScoreInfo);
        return userGameModeScoreInfos.ToArray();
    }
}

//[Serializable]
//public class BlockChainData
//{
//    public string avatarJsonURL;
//    public string avatarScreenShotURL;
//}