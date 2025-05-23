using System;
using System.Collections;
using System.Collections.Generic;
using Dominos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerPersonalData : MonoBehaviour
{
    
    public static string Player_OS;
    public static string GameId;
    public static string playerUserID;//= UI_Manager.instance.userName;
    public static int playerSeatID;
    public static int playerScore;
    public static int playerDomiCoins;
    public static string playerName;// = UI_Manager.instance.userName;
    public static string playerEmail;
    public static string playerPassword;
    public static string authProvider;
    public static bool playerWhiteListed;


    public static string country;
    public static int age;
    public static string gender;
    public static float discount;
    public static string location;
    public static string profilePicURL;
    public static PlayerStates playerStates;
    public static Texture2D playerTexture;
    [Header("Nakama Details")]
    public static string sessionID;
    internal static List<FriendDetail> facebookFriends;

    internal static void OnSuccessfullyProfileDownload(string keyValuePairs, long successCode)
    {
        Debug.Log("OnSuccessfullyProfileDownload: "  + keyValuePairs.ToString());
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

        PlayerPersonalDataJSON playerPersonalDataJson = JsonConvert.DeserializeObject<PlayerPersonalDataJSON>(keyValuePairs);

        Debug.Log("playerPersonalDataJson: "+JsonConvert.SerializeObject(keyValuePairs.ToString()));
        if (!String.IsNullOrEmpty(playerPersonalDataJson.Data.User.accessToken))
        {
            Global.GetBearerToken = Global.GetAuthToken = playerPersonalDataJson.Data.User.accessToken;
        }
        WebServiceManager.instance.playerPersonalData = playerPersonalDataJson;
        playerUserID = playerPersonalDataJson.Data.User.userID;
        playerName = playerPersonalDataJson.Data.User.UserName;//.FirstName + " " + playerPersonalDataJson.Data.User.LastName;
        playerEmail = playerPersonalDataJson.Data.User.Email;
        country = playerPersonalDataJson.Data.User.Country;
        gender = playerPersonalDataJson.Data.User.Gender;
        age = int.Parse(playerPersonalDataJson.Data.User.Age);
        playerDomiCoins = playerPersonalDataJson.Data.User.Domicoins;
        //Debug.LogError("playerDomiCoins: " + playerDomiCoins);
        playerWhiteListed = playerPersonalDataJson.Data.User.WhiteListed;
        //discount = playerPersonalDataJson.Data.User.discount;
        //Debug.Log("discount: " + discount.ToString());
        playerPassword = playerPersonalDataJson.Data.User.Email;
        location = playerPersonalDataJson.Data.User.Country;
        profilePicURL = playerPersonalDataJson.Data.User.ProfilePicUrl;

        PlayerStates playerStatesJson = new PlayerStates();
        playerStatesJson.gamesPlayed = playerPersonalDataJson.Data.Games.GamesPlayed;
        playerStatesJson.gamesWon = playerPersonalDataJson.Data.Games.GamesWon;
        playerStatesJson.gamesWonPercentage = playerPersonalDataJson.Data.Games.GamesWonPercentage;

        //playerStatesJson.tournnamentPlayed = playerPersonalDataJson.Data.TournamentStats.TournamentPlayed;
        //playerStatesJson.tournnamentWon = playerPersonalDataJson.Data.TournamentStats.TournamentWon;
        //playerStatesJson.tournnamentLost = playerPersonalDataJson.Data.TournamentStats.TournamentLost;
        //playerStatesJson.championshipsWon = playerPersonalDataJson.Data.TournamentStats.ChampionshipWon;

        playerStatesJson.playerFlagShortCode = playerPersonalDataJson.Data.User.Country;



        playerStates = playerStatesJson;

        ImageCacheManager.instance.CheckOrDownloadImage(profilePicURL, null, UpdatePic);

        //WebServiceManager.instance.StartCoroutine(_GetTexture(profilePicURL));
        WebServiceManager.instance.StartCoroutine(_GetFlag(playerStatesJson.playerFlagShortCode));

        //if (SceneManager.GetActiveScene().name.Equals(Global.SplashScene))
        //{
        //    Debug.Log("Global.gameType: " + Global.gameType);
        //    SplashScreen.instance.OnLoadingCompleted();
        //}
    }
    internal static void OnSuccessfullyProfileUpdated(string keyValuePairs, long successCode)
    {
        Debug.Log("OnSuccessfullyProfileUpdated: " + keyValuePairs.ToString());
        if (!ResponseStatus.Check(successCode))
        {
            Debug.LogError("successCode Error: " + successCode.ToString());
            Debug.LogError("Some Error: " + keyValuePairs.ToString());
            
            if (successCode == 403)
            {
                WebglUserSession.userLoggedIn = false;
                UI_ScreenManager.OpenClosePopUp(WebglUserSession.instance.userSessionFailedPopUp, true, true);
                return;

            }
            MesgBar.instance.show(ErrorData.FromJson(keyValuePairs.ToString()).message[0]);
            return;
        }
        WebglUserSession.userLoggedIn = true;

        User user = User.FromJson(keyValuePairs.ToString());

            Global.GetBearerToken = Global.GetAuthToken = user.accessToken;
        WebServiceManager.instance.playerPersonalData.Data.User = user;
        playerUserID = user.userID;
        playerName =   user.UserName;//.FirstName + " " + playerPersonalDataJson.Data.User.LastName;
        playerEmail =  user.Email;
        country =      user.Country;
        gender = user.Gender;
        age = int.Parse(user.Age);
        playerDomiCoins = user.Domicoins;
        //Debug.LogError("playerDomiCoins: " + playerDomiCoins);
        playerWhiteListed = user.WhiteListed;
        //discount = playerPersonalDataJson.Data.User.discount;
        //Debug.Log("discount: " + discount.ToString());
        playerPassword = user.Email;
        location = user.Country;
        profilePicURL = user.ProfilePicUrl;
        country = user.Country;

        ImageCacheManager.instance.CheckOrDownloadImage(profilePicURL , null, UpdatePic);
        //WebServiceManager.instance.StartCoroutine(_GetTexture(profilePicURL));
         WebServiceManager.instance.StartCoroutine(_GetFlag(country));

        //if (SceneManager.GetActiveScene().name.Equals(Global.SplashScene))
        //{
        //    Debug.Log("Global.gameType: " + Global.gameType);
        //    SplashScreen.instance.OnLoadingCompleted();
        //}
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
            print("PlayerPersonalData.playerTexture : "  + PlayerPersonalData.playerTexture == null);
        }

    }

    private static void UpdatePic(Texture2D texture)
    {
        playerTexture = texture;
        UI_Manager.instance.UpdateUI();

        UI_Manager.instance.settingScreen.GetSoundSettings();

    }

    /// <summary>
    /// Download My Flag
    /// </summary>
    /// <returns></returns>
    static IEnumerator _GetFlag(string shortCode)
    {
        //UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://countryflagsapi.com/png/" + shortCode);
        Texture2D tempTexture = WebServiceManager.instance.tempFlagTexture;
        Debug.LogError("shortCode: " + shortCode);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://flagcdn.com/h20/" + shortCode.ToLower() + ".jpg");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            tempTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
        playerStates.flagSprite = Sprite.Create((Texture2D)tempTexture, new Rect(0.0f, 0.0f, tempTexture.width, tempTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
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

    public string playerFlagShortCode;

    public Sprite flagSprite;

    public void GenerateDummyData()
    {   
        gamesPlayed = UnityEngine.Random.Range(0, 10).ToString();
        gamesWon = UnityEngine.Random.Range(0, 10).ToString();
        gamesWonPercentage = UnityEngine.Random.Range(0f, 10.0f).ToString();


        playerFlagShortCode = WebServiceManager.instance.flagSprites[UnityEngine.Random.Range(0, WebServiceManager.instance.flagSprites.Count)].name;

        flagSprite = WebServiceManager.instance.FindFlagSprite(playerFlagShortCode);
    }
}