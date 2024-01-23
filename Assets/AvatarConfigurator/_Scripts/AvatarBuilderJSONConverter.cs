//Anas
using UnityEngine;

public class AvatarBuilderJSONConverter: MonoBehaviour { }
namespace AvatarBuilder
{

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using AssetBuilder;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;


    #region Categories
    public partial class GetCatagories
    {
        [JsonProperty("message")]
        public string Message;

        [JsonProperty("data")]
        public List<JsonCatagory> Data;
    }

    /*
        "_id": "63481cfa8c5d57181eb38d9d",
        "name": "Nose",
        "shortCode": "nose",
        "id": 6
    */
    public partial class JsonCatagory
    {
        [JsonProperty("_id")]
        public string mongoDBCategory_Id;

        [JsonProperty("id")]
        public string mapCategoryId;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("shortCode")]
        public string shortCode;

        [JsonProperty("gender")]
        public string gender;
    }

    public partial class GetCatagories
    {
        public static GetCatagories FromJson(string json) => JsonConvert.DeserializeObject<GetCatagories>(json, AvatarBuilder.Converter.Settings);
    }
    #endregion

    #region Items Json
    [Serializable]
    public partial class ItemThumbnailJson
    {
        [JsonProperty("message")]
        public string Message;

        [JsonProperty("data")]
        public ThumbnailJsonData Data;
    }

    [Serializable]
    public partial class CachedItemThumbnailJson
    {
        [JsonProperty("message")]
        public string Message;

        [JsonProperty("avatarCategories")]
        public List<ThumbnailJsonData> DataList = new List<ThumbnailJsonData>();

        [JsonProperty("maleDefaultAvatar")]
        public List<ThumbnailJsonData> maleDefaultDataList = new List<ThumbnailJsonData>();

        [JsonProperty("femaleDafaultAvatar")]
        public List<ThumbnailJsonData> femaleDefaultDataList = new List<ThumbnailJsonData>();

        [JsonProperty("avatarGender")]
        public string gender = "male";

        [JsonProperty("name")]
        public string avatarName = "male";

        [JsonProperty("maleAvatarPrice")]
        public string maleAvatarPrice;

        [JsonProperty("femaleAvatarPrice")]
        public string femaleAvatarPrice;

        [JsonProperty("maleCode")]
        public string maleAvatarCode;

        [JsonProperty("femaleCode")]
        public string femaleAvatarCode;

        [JsonProperty("isNewUser")]
        public bool IsNewUser;

        //Default Json
        public CachedItemThumbnailJson()
        {
            Message = "";
            DataList = new List<ThumbnailJsonData>();
            //maleDefaultDataList = new List<ThumbnailJsonData>();
            //femaleDefaultDataList = new List<ThumbnailJsonData>();
            gender = "male";
            IsNewUser = true;
        }
    }

    [Serializable]
    public partial class ThumbnailJsonData
    {
        [JsonProperty("_id")]
        public string mongodbID;

        [JsonProperty("shortCode")]
        public string CategoryType;

        [JsonProperty("data")]
        public ItemDataJson Data;
    }

    [Serializable]
    public partial class ItemDataJson
    {
        [JsonProperty("texture")]
        public List<ItemProperties> Texture = new List<ItemProperties>();

        [JsonProperty("mesh")]
        public List<ItemProperties> Mesh = new List<ItemProperties>();

        [JsonProperty("color")]
        public List<ItemProperties> Color = new List<ItemProperties>();

        [JsonProperty("blend")]
        public List<ItemProperties> Blend = new List<ItemProperties>();
    }

    [Serializable]
    public partial class ItemProperties
    {

        [JsonProperty("itemID")]
        public string itemID;

        [JsonProperty("url")]
        public string Url;

        [JsonProperty("value")]
        public string Value;

        [JsonProperty("thumbnailURL")]
        public string ThumbnailUrl;
    }

    public partial class ItemThumbnailJson
    {
        public static ItemThumbnailJson FromJson(string json) => JsonConvert.DeserializeObject<ItemThumbnailJson>(json, Dominos.Converter.Settings);
    }

    public partial class CachedItemThumbnailJson
    {
        public static CachedItemThumbnailJson FromJson(string json) => JsonConvert.DeserializeObject<CachedItemThumbnailJson>(json, Dominos.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ItemThumbnailJson self) => JsonConvert.SerializeObject(self, Dominos.Converter.Settings);
        public static string ToJson(this TournamentDetails self) => JsonConvert.SerializeObject(self, Dominos.Converter.Settings);
        public static string ToJson(this CachedItemThumbnailJson self) => JsonConvert.SerializeObject(self, Dominos.Converter.Settings);
        public static string ToJson(this Mint self) => JsonConvert.SerializeObject(self, Dominos.Converter.Settings);
        public static string ToJson(this DispatchEvent self) => JsonConvert.SerializeObject(self, Dominos.Converter.Settings);
        public static string ToJson(this DispatchEventPayload self) => JsonConvert.SerializeObject(self, Dominos.Converter.Settings);
        public static string ToJson(this AttemptToJoinTournamentMatch self) => JsonConvert.SerializeObject(self, Dominos.Converter.Settings);
        public static string ToJson(this TimeEndToFindTournamentMatchOponents self) => JsonConvert.SerializeObject(self, Dominos.Converter.Settings);
    }
    #endregion

    public partial class UploadImageToS3
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public UploadDataToS3 Data { get; set; }
    }

    #region Upload Picture to S3
    public partial class UploadObjectToS3
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public List<string> Data = new List<string>();
    }

    public partial class UploadDataToS3
    {
        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("file_url")]
        public string FileUrl { get; set; }

        [JsonProperty("assetType")]
        public string Assettype { get; set; }
    }

    public partial class UploadImageToS3
    {
        public static UploadImageToS3 FromJson(string json) => JsonConvert.DeserializeObject<UploadImageToS3>(json, AvatarBuilder.Converter.Settings);
    }


    public partial class UploadObjectToS3
    {
        public static UploadObjectToS3 FromJson(string json) => JsonConvert.DeserializeObject<UploadObjectToS3>(json, AvatarBuilder.Converter.Settings);
    }

    #endregion


    #region Mint
    public partial class Mint
    {
        [JsonProperty("data")]
        public List<MintData> tokens = new List<MintData>();
    }

    [Serializable]
    public partial class MintData
    {
        [JsonProperty("description")]
        public string description = "";

        [JsonProperty("external_url")]
        public string ExternalUrl = "";

        [JsonProperty("image")]
        public string imageS3URL = "";

        [JsonProperty("name")]
        public string name = "";

        [JsonProperty("price")]
        public string price = "";

        [JsonProperty("type")]
        public string type;

        [JsonProperty("itemCategory")]
        public string itemCategory;

        [JsonProperty("itemclass")]
        public string itemclass;

        [JsonProperty("shortCode")]
        public string shortCode = "";
    }

    public partial class MintData
    {
        public static MintData FromJson(string json) => JsonConvert.DeserializeObject<MintData>(json, Dominos.Converter.Settings);
    }
    #endregion


    #region DispatchEvent
    public partial class DispatchEvent
    {
        [JsonProperty("eventType")]
        public string eventType { get; set; }

        [JsonProperty("payload")]
        public string payload { get; set; }

    }

    public partial class DispatchEventObject
    {
        [JsonProperty("fromUnity")]
        public bool FromUnity { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("payload")]
        public string Payload { get; set; }
    }

    public partial class GetCharactersPayLoad
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("tokenURI")]
        public List<Uri> TokenUri { get; set; }
    }

    [System.Serializable]
    public class CharactersOwnNftPayload
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public List<NFTUriAndToken> data = new List<NFTUriAndToken>();
    }

    [System.Serializable]
    public class NFTUriAndToken
    {
        [JsonProperty("uri")]
        public string Uri;

        [JsonProperty("tokenId")]
        public string TokenID;
    }

    [System.Serializable]
    public class NFT
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("tokenId")]
        public string TokenID;

        [JsonProperty("mintData")]
        public MintData MintData;
    }
 
    public partial class GetCharactersPayLoad
    {
        public static GetCharactersPayLoad FromJson(string json) => JsonConvert.DeserializeObject<GetCharactersPayLoad>(json, Dominos.Converter.Settings);
    }

    public partial class DispatchEventPayload
    {
        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("tokenURI")]
        public List<string> tokenUri = new List<string>();

        [JsonProperty("price")]
        public List<string> price = new List<string>();

        [JsonProperty("categoryType")]
        public List<string> categoryType = new List<string>();

        [JsonProperty("shortCode")]
        public List<string> shortCode = new List<string>();

        [JsonProperty("type")]
        public List<string> type = new List<string>();

        [JsonProperty("itemClass")]
        public List<string> itemClass = new List<string>();

    }

    [Serializable]
    public partial class TournamentDetails
    {
        [JsonProperty("gameRule")]
        public string gameRule = "";        //HighFive, GameMode2, GameMode3, GameMode4, GameMode5, GameMode6.

        [JsonProperty("coins")]
        public int coinsToPlay = 0;         //max 3 options.

        [JsonProperty("gameType")]
        public string GameType = "";        //Tournament, SingleMatch

        [JsonProperty("noOfPlayers")]
        public int maxPlayers = 0;          //2, 3, 4 only.

        [JsonProperty("_id")]
        public string matchID = "";         //database tournament Bracket match id.

        [JsonProperty("status")]
        public string matchStatus = "";     //open, started, ended

        [JsonProperty("tournamentId")]
        public string tournamentID = "";    //database tournament id.

        [JsonProperty("timeDifference")]
        public string timeDifference = "";  //time Difference.

        [JsonProperty("matchDate")]
        public string matchDate = "";  //Time when match start.

        [JsonProperty("serverTime")]
        public string serverTime = "";  //Time on server right now.

        [JsonProperty("gameCenter")]
        public string gameCenterID = "asassa";  //gameCenter ID.
    }

    public partial class AttemptToJoinTournamentMatch
    {
        [JsonProperty("matchId")]
        public string matchId { get; set; }

        [JsonProperty("userId")]
        public string userId { get; set; }
    }


    public partial class TimeEndToFindTournamentMatchOponents
    {
        [JsonProperty("isWinner")]
        public bool IsWinner { get; set; }
    }

    public partial class TimeEndToFindTournamentMatchOponents
    {
        public static TimeEndToFindTournamentMatchOponents FromJson(string json) => JsonConvert.DeserializeObject<TimeEndToFindTournamentMatchOponents>(json, Dominos.Converter.Settings);
    }

    public partial class TournamentDetails
    {
        public static TournamentDetails FromJson(string json) => JsonConvert.DeserializeObject<TournamentDetails>(json, Dominos.Converter.Settings);
    }

    public partial class DispatchEventObject
    {
        public static DispatchEventObject FromJson(string json) => JsonConvert.DeserializeObject<DispatchEventObject>(json, Dominos.Converter.Settings);
    }

    public partial class DispatchEvent
    {
        public static DispatchEvent FromJson(string json) => JsonConvert.DeserializeObject<DispatchEvent>(json, Dominos.Converter.Settings);
    }
    #endregion DispatchEvent


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