namespace Dominos
{
    using System;
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
        [JsonProperty("user")]
        public User User;

        [JsonProperty("games")]
        public Games Games;

        [JsonProperty("tournamentStats")]
        public TournamentStats TournamentStats;

        [JsonProperty("userGameModeScoreInfo")]
        public UserGameModeScoreInfo[] userGameModeScoreInfo;
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

    [Serializable]
    public partial class TournamentStats
    {
        [JsonProperty("tournamentPlayed")]
        public int TournamentPlayed;

        [JsonProperty("tournamentWon")]
        public int TournamentWon ;

        [JsonProperty("tournamentLost")]
        public int TournamentLost ;

        [JsonProperty("championshipWon")]
        public int ChampionshipWon ;
    }

    [Serializable]
    public partial class User
    {
        [JsonProperty("_id")]
        public string userID ;

        [JsonProperty("firstName")]
        public string FirstName ;

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

        [JsonProperty("characterId")]
        public string CharacterId = "";

        [JsonProperty("assetGender")]
        public string AssetGender = "";

        [JsonProperty("domicoins")]
        public string Domicoins = "";

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

        [JsonProperty("className", NullValueHandling = NullValueHandling.Ignore)]
        public string playerClass = "";
    }

    public partial class PlayerPersonalDataJSON
    {
        public static PlayerPersonalDataJSON FromJson(string json) => JsonConvert.DeserializeObject<PlayerPersonalDataJSON>(json, Dominos.Converter.Settings);
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