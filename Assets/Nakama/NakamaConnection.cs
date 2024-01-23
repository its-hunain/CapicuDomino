using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AvatarBuilder;
using Nakama;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// A singleton class that handles all connectivity with the Nakama server.
/// </summary>
[Serializable]
[CreateAssetMenu]
public class NakamaConnection : ScriptableObject
{
    public static string Scheme;
    public static string Host;
    public static int Port;
    public static string ServerKey;

    private const string AuthTokenSessionPrefName = "authToken";
    private const string DeviceIdentifierPrefName = "deviceUniqueIdentifier";

    public IClient Client;
    public ISession Session;
    public ISocket Socket;

    private string currentMatchmakingTicket;
    private string currentMatchId;

    /// <summary>
    /// Connects to the Nakama server using device authentication and opens socket for realtime communication.
    /// </summary>
    public async Task Connect()
    {
        try
        {
            // Connect to the Nakama server.
            Client = new Client(Scheme, Host, Port, ServerKey, UnityWebRequestAdapter.Instance);

            // Attempt to restore an existing user session.
            var authToken = "";//PlayerPrefs.GetString(AuthTokenSessionPrefName);
            Session = null;

            if (!string.IsNullOrEmpty(authToken))
            {
                var session = Nakama.Session.Restore(authToken);
                if (!session.IsExpired)
                {
                    Session = session;
                }
                else
                {
                    Session = null;
                }

            }

            //Debug.Log("authToken: "  + authToken);

            // If we weren't able to restore an existing session, authenticate to create a new user session.
            if (Session == null)
            {
                Session = await Client.AuthenticateCustomAsync(PlayerPersonalData.playerUserID, PlayerPersonalData.playerUserID, true);
            }

            // Open a new Socket for realtime communication.
            Socket = Client.NewSocket();
            await Socket.ConnectAsync(Session, true);
            Debug.Log("UpdatePlayerDataOnNakama: Client = " + Client);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex);
            var gamePlayWaitingPopUp = GamePlayWaitingPopUp.instance == null ? FindObjectOfType<GamePlayWaitingPopUp>(false) : GamePlayWaitingPopUp.instance;
            gamePlayWaitingPopUp.SetData("Fail To Connect!", false);
        }
    }

    public async Task UpdatePlayerDataOnNakama()
    {
        string displayName = PlayerPersonalData.playerName;
        string avatarUrl = PlayerPersonalData.profilePicURL;
        string location = PlayerPersonalData.location;
        string userName = PlayerPersonalData.playerUserID; // User Name will be treat as userID for Nakama

        Debug.Log("UpdatePlayerDataOnNakama: displayName = " + displayName);
        if (Client == null)
        { await Connect(); }
        await Client.UpdateAccountAsync(Session, userName, displayName, avatarUrl, null/*PlayerPersonalData.playerStates.playerFlagShortCode*/, location);
    }

    /// <summary>
    /// Starts looking for a match with a given number of minimum players.
    /// </summary>
    public async Task FindMatch()
    {
        string GameRule = GameRulesManager.currentSelectedGame_Rule.ToString();
        double coinsToPlay = double.Parse(GameRulesManager.currentSelectedGame_CoinsToPlay.ToString());
        int maxPlayers = GameRulesManager.noOfPlayers;

        string query = "";
        const string engine = "unity";

        var stringProperties = new Dictionary<string, string>
        {
            { "engine", engine },
            {"gameRule", GameRule},
            {"gameType", GameRulesManager.currentSelectedGame_GameType.ToString()},
            {"gameCenter", GameCenterSelectionScreen.selectedGameCenterID.ToString()}
        };

        var numericProperties = new Dictionary<string, double>() {
            {"coins", coinsToPlay},
            {"noOfPlayers", GameRulesManager.noOfPlayers},
        };

        Debug.Log("GameCenterSelectionScreen.selectedGameCenterID : " + GameCenterSelectionScreen.selectedGameCenterID.ToString());


        query =
            "+properties.engine:" + engine +
            " +properties.gameRule:" + GameRule +
            " +properties.gameType:" + GameRulesManager.currentSelectedGame_GameType.ToString() +
            " +properties.gameCenter:" + GameCenterSelectionScreen.selectedGameCenterID.ToString() +
            " +properties.coins:" + coinsToPlay +
            " +properties.noOfPlayers:" + GameRulesManager.noOfPlayers;

        int minPlayer = GameRulesManager.currentSelectedGame_GameType == GameRulesManager.GameType.Tournament ? 2 : maxPlayers;

        // Add this client to the matchmaking pool and get a ticket.
        var matchmakerTicket = await Socket.AddMatchmakerAsync(query, minPlayer /*min count*/, maxPlayers /*max count*/, stringProperties, numericProperties);
        currentMatchmakingTicket = matchmakerTicket.Ticket;
    }

    public void TryToAttemptToJoinTournamentMatch_Api(string apiFunction)
    {
        AttemptToJoinTournamentMatch attemptToJoinTournamentMatch = new AttemptToJoinTournamentMatch
        {
            matchId = GameRulesManager.matchDetails.matchID,
            userId = PlayerPersonalData.playerUserID
        };


        Debug.Log("TryToAttemptToJoinTournamentMatch_Api apiFunction: " + apiFunction);
        Debug.Log("matchId: " + GameRulesManager.matchDetails.matchID);
        Debug.Log("userId: " + PlayerPersonalData.playerUserID);
        string jsonRawData = Serialize.ToJson(attemptToJoinTournamentMatch);

        WebServiceManager.instance.APIRequest(apiFunction, Method.PUT, jsonRawData, null, OnSuccessJoinTournamentMatch, OnFailJoinTournamentMatch);
    }

    public void NoPlayerEnterInTournamentMatch_Api(string apiFunction)
    {
        Dictionary<string, object> paramsData = new Dictionary<string, object>(); //Add After Api will Created
        paramsData.Add("matchId", GameRulesManager.matchDetails.matchID);

        WebServiceManager.instance.APIRequest(apiFunction, Method.GET, null, paramsData, OnSuccessWaitingOver, OnFailJoinTournamentMatch);
    }

    public void OnSuccessWaitingOver(JObject keyValuePairs, long code)
    {
        if (ResponseStatus.Check(code))
        {
            var timeEndToFindTournament = TimeEndToFindTournamentMatchOponents.FromJson(keyValuePairs.ToString());
            if (timeEndToFindTournament.IsWinner)
            {
                GamePlayWaitingPopUp.instance.SetData("Your Opponent fails to join on time!", false);
                GameManager.instace.NoPlayerFoundReload(timeEndToFindTournament.IsWinner);
            }
            else
            {
                GameManager.instace.StartCountdown();
            }
        }
        else
        {
            Debug.LogError("OnFailJoinTournamentMatch Error Code: " + code);

            GamePlayWaitingPopUp.instance.SetData("No Match Found!", false);
            GameManager.instace.NoPlayerFoundReload();
        }
    }

    public void OnSuccessJoinTournamentMatch(JObject keyValuePairs, long code)
    {
        Debug.Log("OnSuccessJoinTournamentMatch");
    }

    public void OnFailJoinTournamentMatch(string msg)
    {
        Debug.LogError("OnFailJoinTournamentMatch: " + msg);

        GamePlayWaitingPopUp.instance.SetData("No Match Found!", false);
        GameManager.instace.NoPlayerFoundReload();
    }

    //Comments
    ///// <summary>
    ///// Starts looking for a Tournament match with a given number of minimum players.
    ///// </summary>
    //public async Task FindMatch(string GameRule, double coinsToPlay = 0, int maxPlayers = 2 , string matchID = "")
    //{
    //    // Set some matchmaking properties to ensure we only look for games that are using the Unity client.
    //    // This is not a required when using the Unity Nakama SDK, 
    //    // however in this instance we are using it to differentiate different matchmaking requests across multiple platforms using the same Nakama server.
    //    var stringProperties = new Dictionary<string, string>
    //    {
    //        { "engine", "unity" },
    //        {"gameRule", GameRule},
    //        {"matchID", matchID}
    //    };

    //    var numericProperties = new Dictionary<string, double>() {
    //        {"coinsToPlay", coinsToPlay}
    //    };

    //    //var query = "+properties.region:europe +properties.rank:>=5 +properties.rank:<=10";
    //    var query = "+properties.engine:unity +properties.gameRule:" + GameRule + " +properties.coinsToPlay:" + coinsToPlay+ " +properties.matchID:" + matchID;

    //    const int minPlayers = 2; //Always 2

    //    // Add this client to the matchmaking pool and get a ticket.
    //    var matchmakerTicket = await Socket.AddMatchmakerAsync(query, minPlayers, maxPlayers, stringProperties , numericProperties);
    //    currentMatchmakingTicket = matchmakerTicket.Ticket;
    //}

    // Perhaps just call this directly from the Socket since it's public already?

    /// <summary>
    /// Cancels the current matchmaking request.
    /// </summary>
    public async Task CancelMatchmaking()
    {
        await Socket.RemoveMatchmakerAsync(currentMatchmakingTicket);
    }
}
