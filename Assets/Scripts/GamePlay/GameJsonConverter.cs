namespace Dominos
{
    using System;
    using System.Collections.Generic;
    using AssetBuilder;
    using Newtonsoft.Json;
    using UnityEngine;

    public class GameJsonConverter : MonoBehaviour
    {    }


    /// <summary>
    /// Get Domi Coins
    /// </summary>
    #region
    public partial class GetDomiCoinsData
    {
        public float Domicoins;
    }

    public partial class GetDomiCoinsData
    {
        public static GetDomiCoinsData FromJson(string json) => JsonConvert.DeserializeObject<GetDomiCoinsData>(json, Dominos.Converter.Settings);
    }

    #endregion

    #region Player Data Json
    [Serializable]
    public partial class PlayerPersonalDataJSON
    {
        [JsonProperty("message")]
        public string Message;

        [JsonProperty("data")]
        public PlayerData Data;
    }

    [Serializable]
    public partial class PlayerData
    {
        [JsonProperty("result")]
        public User User;

        [JsonProperty("games")]
        public Games Games;
    }

    [Serializable]
    public partial class PlayerPersonalDataJSON2
    {
        [JsonProperty("message")]
        public string Message;

        [JsonProperty("data")]
        public PlayerData2 Data;
    }

    [Serializable]
    public partial class PlayerData2
    {
        [JsonProperty("user")]
        public User User;

        [JsonProperty("games")]
        public Games Games;
    }

    [Serializable]
    public partial class UserGameModeScoreInfo
    {
        [JsonProperty("id")]
        public string id;

        [JsonProperty("MMR")]
        public string MMR;

        [JsonProperty("medalName")]
        public string medalName;

        [JsonProperty("medalRank")]
        public int medalRank;

        [JsonProperty("GameMode")]
        public GameRulesManager.GameRules GameMode;
    }


    [Serializable]
    public partial class Games
    {
        [JsonProperty("gamesWon", NullValueHandling = NullValueHandling.Ignore)]
        public string GamesWon;

        [JsonProperty("gamesPlayed", NullValueHandling = NullValueHandling.Ignore)]
        public string GamesPlayed;

        [JsonProperty("gamesWonPercentage", NullValueHandling = NullValueHandling.Ignore)]
        public string GamesWonPercentage;
    }

    public partial class ErrorData
    {
        public int statusCode { get; set; }
        public List<string> message { get; set; }
        public string error { get; set; }
    }

    [Serializable]
    public partial class User
    {
        [JsonProperty("accessToken")]
        public string accessToken;

        [JsonProperty("_id")]
        public string userID ;

        [JsonProperty("firstName")]
        public string FirstName ;

        [JsonProperty("age")]
        public string Age ;

        [JsonProperty("whiteListed")]
        public bool WhiteListed;

        [JsonProperty("discount")]
        public float discount;

        [JsonProperty("lastName")]
        public string LastName ;

        [JsonProperty("email")]
        public string Email = "";

        [JsonProperty("password")]
        public string Password = "";

        [JsonProperty("userName")]
        public string UserName = "";

        [JsonProperty("authProivder")]
        public string AuthProvider = "";

        [JsonProperty("level")]
        public int Level = 0;


        [JsonProperty("assetGender")]
        public string AssetGender = "";

        [JsonProperty("coins")]
        public int Domicoins;

        [JsonProperty("is_admin")]
        public bool isAdmin;

        [JsonProperty("country")]
        public string Country;

        [JsonProperty("dateOfBirth", NullValueHandling = NullValueHandling.Ignore)]
        public string DateOfBirth = "";

        [JsonProperty("gender", NullValueHandling = NullValueHandling.Ignore)]
        public string Gender = "";

        [JsonProperty("flagShortCode", NullValueHandling = NullValueHandling.Ignore)]
        public string FlagShortCode = "";

        [JsonProperty("profilePicUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string ProfilePicUrl = "";

    }
    /// <summary>

    [Serializable]
    public partial class FetchOwnShopItem
    {
        [JsonProperty("_id")]
        public string _id;

        [JsonProperty("myProducts")]
        public List<BuyShopItem> myProducts;
    }

    public partial class FetchOwnShopItem
    {
        public static FetchOwnShopItem FromJson(string json) => JsonConvert.DeserializeObject<FetchOwnShopItem>(json, Dominos.Converter.Settings);

    }

    [Serializable]
    public partial class BuyShopItem
    {
        [JsonProperty("productId")]
        public string productId;
        [JsonProperty("productName")]
        public string productName;
        [JsonProperty("productPrice")]
        public string productPrice;
    }
    /// </summary>
    /// 




    public partial class PlayerPersonalDataJSON
    {
        public static PlayerPersonalDataJSON FromJson(string json) => JsonConvert.DeserializeObject<PlayerPersonalDataJSON>(json, Dominos.Converter.Settings);
    }
    public partial class User
    {
        public static User FromJson(string json) => JsonConvert.DeserializeObject<User>(json, Dominos.Converter.Settings);
    }

    public partial class ErrorData
    {
        public static ErrorData FromJson(string json) => JsonConvert.DeserializeObject<ErrorData>(json, Dominos.Converter.Settings);
    }

    [Serializable]
    public partial class CheckWhiteList
    {
        [JsonProperty("data")]
        public bool isWhitelisted;

        [JsonProperty("discount")]
        public float discount;
    }

    public partial class CheckWhiteList
    {
        public static CheckWhiteList FromJson(string json) => JsonConvert.DeserializeObject<CheckWhiteList>(json, Dominos.Converter.Settings);
    }



    public class ImageData
    {
        public string file_name { get; set; }
        public string file_url { get; set; }
    }

    public partial class ImageUpload
    {
        public string message { get; set; }
        public ImageData data { get; set; }
    }
    public partial class ImageUpload
    {
        public static ImageUpload FromJson(string json) => JsonConvert.DeserializeObject<ImageUpload>(json, Dominos.Converter.Settings);
    }


    #endregion

    #region Seasons

    public partial class SeasonalEnvironments
    {
        [JsonProperty("name")]
        public string seosonName { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }
    }

    public partial class SeasonalEnvironments
    {
        public static SeasonalEnvironments FromJson(string json) => JsonConvert.DeserializeObject<SeasonalEnvironments>(json, Dominos.Converter.Settings);
    }
    #endregion

    #region Config
    [Serializable]
    public partial class Config
    {
        [JsonProperty("baseUrl")]
        public string BaseUrl;

        [JsonProperty("websiteUrl")]
        public string WebsiteUrl;

        [JsonProperty("nakamaScheme")]
        public string NakamaScheme;

        [JsonProperty("nakamaHost")]
        public string NakamaHost;

        [JsonProperty("nakamaPort")]
        public int NakamaPort;

        [JsonProperty("nakamaServerKey")]
        public string NakamaServerKey;
    }
    public partial class Config
    {
        public static Config FromJson(string json) => JsonConvert.DeserializeObject<Config>(json, Dominos.Converter.Settings);
    }

    #endregion
}