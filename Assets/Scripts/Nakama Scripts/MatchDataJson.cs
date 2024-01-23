using System.Collections.Generic;
using Nakama.TinyJson;
using UnityEngine;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Dominos;

/// <summary>
/// A static class that creates JSON string network messages.
/// </summary>
public static class MatchDataJson
{
    //JSON Keys
    public const string tileKey = "TileValue";
    public const string turnIndexKey = "turnIndex";
    public const string shortCode = "shortCode";
    public const string playerIdKey = "playerID";
    public const string skipTurnReason = "skipTurnReason";
    public const string tilePossibilityKey = "tilePossibility";
    public const string tilePossibilityDirectionNameKey = "tilePossibilityDirectionName";
    public const string tilePossibilityParentValueKey = "tilePossibilityParentValue";
    public const string tilePossibility_IsSameFace = "isSameFace";
    public const string roundInProgress = "roundInProgress";


    /// <summary>
    /// Creates a network message containing velocity and position.
    /// </summary>
    /// <param name="velocity">The velocity to send.</param>
    /// <param name="position">The position to send.</param>
    /// <returns>A JSONified string containing velocity and position data.</returns>
    public static string VelocityAndPosition(Vector2 velocity, Vector3 position)
    {
        var values = new Dictionary<string, string>
        {
            { "velocity.x", velocity.x.ToString() },
            { "velocity.y", velocity.y.ToString() },
            { "position.x", position.x.ToString() },
            { "position.y", position.y.ToString() }
        };
        return values.ToJson();
    }



    /// <summary>
    /// Creates a network message containing player input.
    /// </summary>
    /// <returns>A JSONified string containing player input.</returns>
    ///
    public static string SendMoveValues(string tileValue, string tilePossibilityValue, string tilePossibilitydirectionName,string tilePossibilityParentValue, bool isSameFace, string currentPlayerID)
    {
        var values = new Dictionary<string, string>
        {
            { tileKey, tileValue },
            { tilePossibilityKey, tilePossibilityValue },
            { tilePossibilityDirectionNameKey, tilePossibilitydirectionName },
            { tilePossibilityParentValueKey, tilePossibilityParentValue },
            { tilePossibility_IsSameFace, isSameFace.ToString() },
            { playerIdKey, currentPlayerID }
            //{ roundInProgress, RoundInProgress.ToString() }
        };
        return values.ToJson();
    }

    /// <summary>
    /// Creates a network message containing player input.
    /// </summary>
    /// <returns>A JSONified string containing player input.</returns>
    public static string PickUpNewTileFromBoneyard( string currentPlayerID)
    {
        var values = new Dictionary<string, string>
        {
            { playerIdKey, currentPlayerID }
        };

        return values.ToJson();
    }

    /// <summary>
    /// Creates a network message containing player input.
    /// </summary>
    /// <returns>A JSONified string containing player input.</returns>
    ///
    public static string SkipMyTurn( string currentPlayerID, string reason)
    {
        var values = new Dictionary<string, string>
        {
            { playerIdKey, currentPlayerID },
            { skipTurnReason,reason  }
        };

        return values.ToJson();
    }

    /// <summary>
    /// Creates a network message containing player JSON_DataProfileStats.
    /// </summary>
    /// <returns>A JSONified string containing player data.</returns>
    ///
    public static string JSON_DataProfileStats()
    {

        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("blockChainData", Serialize.ToJson(PlayerPersonalData.playerStates.blockChainData));
        values.Add("userGameModeInfo", Serialize.ToJson(PlayerPersonalData.playerStates.userGameModeInfo));
        values.Add("gamesPlayed", PlayerPersonalData.playerStates.gamesPlayed.ToString());
        values.Add("gamesWon", PlayerPersonalData.playerStates.gamesWon.ToString());
        values.Add("gamesWonPercentage", PlayerPersonalData.playerStates.gamesWonPercentage.ToString());
        values.Add("tournnamentPlayed", PlayerPersonalData.playerStates.tournnamentPlayed.ToString());
        values.Add("tournnamentWon", PlayerPersonalData.playerStates.tournnamentWon.ToString());
        values.Add("tournnamentLost", PlayerPersonalData.playerStates.tournnamentLost.ToString());
        values.Add("championshipsWon", PlayerPersonalData.playerStates.championshipsWon.ToString());
        values.Add("playerClass", PlayerPersonalData.playerStates.playerClass.ToString());
        values.Add("playerFlagShortCode", PlayerPersonalData.playerStates.playerFlagShortCode.ToString());
        //{
        //    { "blockChainData", PlayerPersonalData.playerStates.blockChainData},
        //    { "gamesPlayed", PlayerPersonalData.playerStates.gamesPlayed.ToString() },
        //    { "gamesWon", PlayerPersonalData.playerStates.gamesWon.ToString() },
        //    { "gamesWonPercentage", PlayerPersonalData.playerStates.gamesWonPercentage.ToString() },
        //    { "tournnamentPlayed", PlayerPersonalData.playerStates.tournnamentPlayed.ToString() },
        //    { "tournnamentWon", PlayerPersonalData.playerStates.tournnamentWon.ToString() },
        //    { "tournnamentLost", PlayerPersonalData.playerStates.tournnamentLost.ToString() },
        //    { "championshipsWon", PlayerPersonalData.playerStates.championshipsWon.ToString() },
        //    { "playerClass", PlayerPersonalData.playerStates.playerClass.ToString() },
        //    { "playerFlagShortCode", PlayerPersonalData.playerStates.playerFlagShortCode.ToString() }
        //};
        return values.ToJson();
    }

    /// <summary>
    /// Creates a network message specifying that the player died and the position when they died.
    /// </summary>
    /// <param name="position">The position on death.</param>
    /// <returns>A JSONified string containing the player's position on death.</returns>
    public static string Died(Vector3 position)
    {
        var values = new Dictionary<string, string>
        {
            { "position.x", position.x.ToString() },
            { "position.y", position.y.ToString() }
        };

        return values.ToJson();
    }

    /// <summary>
    /// Creates a network message specifying that the player respawned and at what spawn point.
    /// </summary>
    /// <param name="spawnIndex">The spawn point.</param>
    /// <returns>A JSONified string containing the player's respawn point.</returns>
    public static string Respawned(int spawnIndex)
    {
        var values = new Dictionary<string, string>
        {
            { "spawnIndex", spawnIndex.ToString() },
        };

        return values.ToJson();
    }

    /// <summary>
    /// Creates a network message indicating a new round should begin and who won the previous round.
    /// </summary>
    /// <param name="winnerPlayerName">The winning player's name.</param>
    /// <returns>A JSONified string containing the winning players name.</returns>
    public static string StartNewRound(string winnerPlayerName)
    {
        var values = new Dictionary<string, string>
        {
            { "winningPlayerName", winnerPlayerName }
        };
        
        return values.ToJson();
    }
}


namespace Dominos
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using AssetBuilder;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class MatchStart
    {
        [JsonProperty("newPlayers")]
        public List<NewJsonPlayer> newPlayers = new List<NewJsonPlayer>();
    }

    [Serializable]
    public partial class NewJsonPlayer
    {
        [JsonProperty("userId")]
        public string UserId;

        [JsonProperty("username")]
        public string username;
    }

    public partial class MatchStart
    {
        public static MatchStart FromJson(string json) => JsonConvert.DeserializeObject<MatchStart>(json, Dominos.Converter.Settings);
    }


    public class UpdatePlayerScore
    {
        public int score;
        public string playerID;
    }

    public class PlayerIdParse
    {
        public string playerID;
    }



    /// <summary>
    /// 
    /// </summary>
    #region PlayerMetaData

    public partial class MetaData
    {
        [JsonProperty("gamesWon")]
        public string GamesWon = "";

        [JsonProperty("gamesPlayed")]
        public string GamesPlayed = "";

        [JsonProperty("playerClass")]
        public string PlayerClass = "";

        [JsonProperty("tournnamentWon")]
        public string TournnamentWon = "";

        [JsonProperty("tournnamentLost")]
        public string TournnamentLost = "";

        [JsonProperty("championshipsWon")]
        public string ChampionshipsWon = "";

        [JsonProperty("tournnamentPlayed")]
        public string TournnamentPlayed = "";

        [JsonProperty("gamesWonPercentage")]
        public string GamesWonPercentage = "";

        [JsonProperty("playerFlagShortCode")]
        public string PlayerFlagShortCode = "";

        [JsonProperty("blockChainData")]
        public string blockChainData;

        [JsonProperty("userGameModeInfo")]
        public string userGameModeInfo;
    }


    [Serializable]
    public partial class UserGameModeInfo
    {
        [JsonProperty("UserGameModeScoreInfo")]
        public UserGameModeScoreInfo[] userGameModeScoreInfos;
    }

    public partial class UserGameModeInfo
    {
        public static UserGameModeInfo FromJson(string json) => JsonConvert.DeserializeObject<UserGameModeInfo>(json, Dominos.Converter.Settings);
    }

    public partial class BlockChainData
    {
        public static BlockChainData FromJson(string json) => JsonConvert.DeserializeObject<BlockChainData>(json, Dominos.Converter.Settings);
    }


    public partial class MetaData
    {
        public static MetaData FromJson(string json) => JsonConvert.DeserializeObject<MetaData>(json, Dominos.Converter.Settings);
    }

    #endregion


    #region Turn Move Data
    public partial class TurnMove
    {
        [JsonProperty("nextTurnIndex")]
        public string nextTurnIndex { get; set; }

        [JsonProperty("turnData")]
        public TurnData turnData { get; set; }
    }
    public partial class TurnData
    {

        //For Turns
        [JsonProperty("TileValue")]
        public string TileValue { get; set; }

        [JsonProperty("tilePossibility")]
        public string TilePossibility { get; set; }

        [JsonProperty("tilePossibilityDirectionName")]
        public string TilePossibilityDirectionName { get; set; }

        [JsonProperty("tilePossibilityParentValue")]
        public string TilePossibilityParentValue { get; set; }

        [JsonProperty("isSameFace")]
        public string IsSameFace { get; set; }

        //For Skip and time end
        [JsonProperty("shortCode")]
        public string ShortCode { get; set; }

        [JsonProperty("playerID")]
        public string PlayerId { get; set; }

        [JsonProperty("skipTurnReason")]
        public string SkipTurnReason { get; set; }
    }

    public partial class TurnMove
    {
        public static TurnMove FromJson(string json) => JsonConvert.DeserializeObject<TurnMove>(json, Dominos.Converter.Settings);
    }
    #endregion

    #region Skip Turn Data
    public partial class SkipTurnUpdate
    {
        [JsonProperty("playerID")]
        public string PlayerId { get; set; }

        [JsonProperty("skipReason")]
        public string SkipReason { get; set; }
    }
    public partial class SkipTurnUpdate
    {
        public static SkipTurnUpdate FromJson(string json) => JsonConvert.DeserializeObject<SkipTurnUpdate>(json, Dominos.Converter.Settings);
    }
    #endregion
    public static class Serialize
    {
        public static string ToJson(this BlockChainData self) => JsonConvert.SerializeObject(self, Dominos.Converter.Settings);
        public static string ToJson(this UserGameModeInfo self) => JsonConvert.SerializeObject(self, Dominos.Converter.Settings);
        public static string ToJson(this MatchStart self) => JsonConvert.SerializeObject(self, Dominos.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }


}