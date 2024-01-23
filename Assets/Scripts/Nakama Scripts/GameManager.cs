using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvatarBuilder;
using Dominos;
using Nakama;
using Nakama.TinyJson;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// A singleton class that handles all game level logic for multiplayer.
/// </summary>
public class GameManager : MonoBehaviour
{
    public UnityEvent OnRequestQuitMatch;


    public bool isTurnInProgress = false;

    [Header("GamePlay Assets")]
    public NakamaConnection NakamaConnection;
    public GridManager gridManager;

    [Header("Chat Data")]
    public string chatChannelName;
    public string chatChannelID;
    [SerializeField] public ChatManager chatManager;
    [SerializeField] public List<IUserPresence> chatUsers = new List<IUserPresence>(4);


    [Header("Important Multiplayer Data")]
    public GameObject localPlayerGameObj;
    public GameObject NetworkRemotePlayerPrefab;
    public string localUserDisplayName;
    [SerializeField] public IUserPresence localUserPresence;
    [SerializeField] public IMatch currentMatch;
    [SerializeField] public string hostUserID;// Declare a variable to store which presence is the host //Session ID

    [Header("Multiplayers Tiles Order")]
    [SerializeField] public List<NewJsonPlayer> PlayersDataInJsonOrder = new List<NewJsonPlayer>();


    //Static Members
    public static GameManager instace;
    public static SortedDictionary<string, GameObject> players;

    //[Header("Static Data For Multiplayer")]
    public static int mySeatIndex = 1;

    //[Header("Multiplayers Tiles Order")]
    public static List<int> multiplayerTilesOrder = new List<int>();
    public static List<int> tieTilesOrder = new List<int>();

    //[Header("Multiplayers Tiles Order")]
    public static List<object> playersData = new List<object>();

    public string HashKey { get; private set; }
    public string MatchId { get; private set; }

    public static double serverTime = (DateTime.UtcNow - Epoch.instance.epochStart).TotalMilliseconds;



    private void Awake()
    {
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
        {
            gridManager.gameObject.SetActive(true);
            gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
        instace = this;

        this.HashKey = Utilities.Base64Encode(Utilities.calculateHashKey());
    }

    private async void Start()
    {
        Debug.Log("GameManager Start...");
        // Initialise the OnRequestQuitMatch event if required.
        if (OnRequestQuitMatch == null) OnRequestQuitMatch = new UnityEvent();

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
        {
            chatManager = new ChatManager(this);

            // Create an empty dictionary to hold references to the currently connected players.
            players = new SortedDictionary<string, GameObject>();

            // Get a reference to the UnityMainThreadDispatcher.
            // We use this to queue event handler callbacks on the main thread.
            // If we did not do this, we would not be able to instantiate objects or manipulate things like UI.
            var mainThread = UnityMainThreadDispatcher.Instance();

            // Connect to the Nakama server.
            await NakamaConnection.Connect();

            // Setup network event handlers.
            NakamaConnection.Socket.ReceivedMatchmakerMatched += m => mainThread.Enqueue(async () => await OnReceivedMatchmakerMatched(m));
            NakamaConnection.Socket.ReceivedMatchPresence += m => mainThread.Enqueue( ()=> OnReceivedMatchPresence(m));
            NakamaConnection.Socket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));

            // Setup Chat event handlers.
            NakamaConnection.Socket.ReceivedChannelPresence += presenceEvent => mainThread.Enqueue(() => chatManager.OnReceivedChatChannelPresence(presenceEvent));
            NakamaConnection.Socket.ReceivedChannelMessage += message => mainThread.Enqueue(()=> chatManager.ShowRecievedMessage(message));

            // Socket Handlers
            NakamaConnection.Socket.Closed += async () => await QuitMatch();
            // Setup unityevent handlers.
            OnRequestQuitMatch.AddListener(async () => await QuitMatch());

            if (GameRulesManager.currentSelectedGame_GameType == GameRulesManager.GameType.SingleMatch)
                FindMatch();
        }
    }

    internal void FindTournamentGame()
    {
        StartCoroutine(FindingaMchAsync());
    }

    IEnumerator FindingaMchAsync()
    {
        Debug.Log("FindingaMchAsync....");
        yield return new WaitUntil(()=> NakamaConnection.Session != null && NakamaConnection.Socket != null && NakamaConnection.Socket.IsConnected);//To Resolved the error for tournament embedded.
        Debug.Log("Socket Connected....");
        //When the time ends and Nakama is connected. Hit Nakama Find Match Querry
        FindMatch();
    }
    /// <summary>
    /// Destroy 
    /// </summary>
    private void OnDestroy()
    {
        if (GameRulesManager.forceLeave)
        {
            //if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

            if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer && NakamaConnection.Socket.IsConnected)
            {
                Debug.Log(gameObject.name + " detect error: ln: 146");
                OnRequestQuitMatch.Invoke();
            }
        }
    }

    /// <summary>
    /// Called when a MatchmakerMatched event is received from the Nakama server.
    /// </summary>
    /// <param name="matched">The MatchmakerMatched data.</param>
    private async Task OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        Debug.Log("OnReceivedMatchmakerMatched, matched.MatchId: " + matched.MatchId);
        // Cache a reference to the local user.
        localUserPresence = matched.Self.Presence;
        //PlayerPersonalData.playerUserID = matched.Self.Presence.UserId;
        PlayerPersonalData.sessionID = matched.Self.Presence.SessionId;

        // Join the match.
        var match = await NakamaConnection.Socket.JoinMatchAsync(matched.MatchId);

        Debug.Log("OnReceivedMatchmakerMatched 2");

        // Cache a reference to the current match.
        currentMatch = match;
        chatChannelName = currentMatch.Id;

        this.MatchId = currentMatch.Id;

        OnCountdownTimerEnded();

        if (currentMatch != null)
        {
            Debug.Log("SendMatchStateAsync: OpCodes.READY, matched.Self.Presence.Username: " + matched.Self.Presence.Username);

            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (PlayerPersonalData.playerStates == null)
            {
                PlayerStates playerStates = new PlayerStates();
                playerStates.GenerateDummyData();
                PlayerPersonalData.playerStates = playerStates;
            }

            await SavePlayerStatsOnNakama();

            SendMatchStateAsync(OpCodes.READY, dict.ToJson());
        }
        GamePlayWaitingPopUp.instance.EnableDisable(false);

        //Joining Chat Channel after joining     match.
        await chatManager.JoinChatChannel(chatChannelName);
    }

    private void _FillPlayerProperties(string matchId, IApiUser apiUser, NewJsonPlayer jsonPlayerData , GameObject player , int index)
    {
        RemotePlayerNetworkData remotePlayerNetworkData = player.GetComponent<PlayerNetworkRemoteSync>().NetworkData;
        remotePlayerNetworkData.MatchId = matchId;
        remotePlayerNetworkData.User = jsonPlayerData;
        remotePlayerNetworkData.UserDetails = apiUser;
        remotePlayerNetworkData.AvatarURL = apiUser.AvatarUrl;
        remotePlayerNetworkData.DisplayName = apiUser.DisplayName;
        remotePlayerNetworkData.UserName = apiUser.Username;
        remotePlayerNetworkData.UserID = apiUser.Id;

        Player playerProfileData = GamePlayUIPanel.instance.players[index];
        remotePlayerNetworkData.player = playerProfileData;
        remotePlayerNetworkData.seatIndex = playerProfileData.playerPersonalData.playerSeatID;



        //This is Me, don't download texture and all stuff
        if (remotePlayerNetworkData.isLocalUser)
        {
            Debug.Log("This is Me, don't download texture and all stuff. . . ");
            remotePlayerNetworkData.UserTexture = PlayerPersonalData.playerTexture;
            remotePlayerNetworkData.player.playerPersonalData.playerStates = PlayerPersonalData.playerStates;
            remotePlayerNetworkData.player.SetDataToProfile(PlayerPersonalData.playerStates, PlayerPersonalData.playerUserID, PlayerPersonalData.playerName , PlayerPersonalData.playerTexture);
        }
        else
        {
            Debug.Log("Player Meta Data: " + apiUser.Metadata.ToString());
            var metaData = MetaData.FromJson(apiUser.Metadata);
            Debug.Log("playerProfileData : " , playerProfileData);
            PlayerStates playerStates = playerProfileData.playerPersonalData.UpdatePlayerStats(metaData);
            
            remotePlayerNetworkData.player.playerPersonalData.playerStates = playerStates;
            remotePlayerNetworkData.player.SetDataToProfile(playerStates, remotePlayerNetworkData.UserName, remotePlayerNetworkData.DisplayName, remotePlayerNetworkData.AvatarURL);
        }
    }



    /// <summary>
    /// 0 = Female
    /// 1 = Male
    /// </summary>
    /// <param name="gender"></param>
    public void SelectAvatar(Gender gender)
    {
        AvatarParent_FbxHolder.instance.cachedSelecteditem.gender = gender.ToString();

        Debug.Log("gender: " + gender.ToString());
        foreach (var item in AvatarParent_FbxHolder.instance.avatars)
        {
            if (item.tag.ToLower() == gender.ToString().ToLower())
            {
                AvatarParent_FbxHolder.instance.currentSelectedAvatar = item;
                break;
            }
        }
    }


    /// <summary>
    /// Called when a player/s joins or leaves the match.
    /// </summary>
    /// <param name="matchPresenceEvent">The MatchPresenceEvent data.</param>
    private async void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        Debug.Log("OnReceivedMatchPresence");

        //bool someOneJoins = false;
        //// For each new user that joins, spawn a player for them.
        //foreach (var user in matchPresenceEvent.Joins)
        //{
        //    Debug.Log("Someone Joins: "+user.Username);
        //    SpawnPlayer(matchPresenceEvent.MatchId, user);
        //    someOneJoins = true;
        //    await Task.Delay(500);
        //}

        // For each player that leaves, despawn their player.

        foreach (var user in matchPresenceEvent.Leaves)
        {
            if (players.ContainsKey(user.SessionId))
            {
                Destroy(players[user.SessionId]);
                players.Remove(user.SessionId);

                //Debug.Log("Someone Left: " + user.Username);
                //Destroy(players[user.SessionId]);
                //players.Remove(user.SessionId);
                //Player leftPlayer = GamePlayUIPanel.instance.players.Find(x => x.playerPersonalData.playerUserID.Equals(user.Username));
                //leftPlayer.DestroyPlayer();
                //if (players.Count == 1)
                //{
                //    GridManager.FinishTheGame(GamePlayUIPanel.instance.players.Find(x => x.playerPersonalData.playerUserID.Equals(PlayerPersonalData.playerUserID)));
                //}
            }
        }
    }

    /// <summary>
    /// Begins the matchmaking process.
    /// </summary>
    public async void FindMatch()
    {
        Debug.Log("Finding Match. . .");
        GamePlayWaitingPopUp.instance.SetData("Finding Opponents...", true);
        GamePlayWaitingPopUp.instance.EnableDisable(true);

        SetDisplayName(PlayerPersonalData.playerName);
        await NakamaConnection.UpdatePlayerDataOnNakama();

        await NakamaConnection.FindMatch();

        //StartWaitingTimer();

        StartCountdown();
    }

    double targetTimeInSeconds;
    bool isTimerActive = false;

    public void StartCountdown()
    {
        targetTimeInSeconds = Epoch.Now + 240000; //4 minutes
        isTimerActive = true;

        StartCoroutine(CountdownTimer());
    }

    private IEnumerator CountdownTimer()
    {
        while (isTimerActive)
        {
            double timeLeft = Epoch.SecondsLeft(targetTimeInSeconds);

            if (timeLeft > 0)
                GamePlayWaitingPopUp.instance.TimerText.text = timeLeft.ToString("N0") + " sec left.";
            else
            {
                OnCountdownTimerEnded();
            }
            yield return new WaitForSeconds(1f);
        }
    }
    //public void StartWaitingTimer()
    //{
    //    Invoke("OnTimeEnd", 120f);
    //}

    void StopTime()
    {
        isTimerActive = false;
    }

    public void OnCountdownTimerEnded()
    {
        StopTime();
        Debug.Log("OnCountdown Timer Ended");

        if (currentMatch == null)
        {
            GamePlayWaitingPopUp.instance.SetData("No Opponent Found.", false);
            NoPlayerFoundReload();
        }
    }

    public void NoPlayerFoundReload(bool isTournamentWinnerDeclared = false)
    {
            StartCoroutine(_ReloadSplashScene(isTournamentWinnerDeclared));
    }
    public IEnumerator _ReloadSplashScene(bool isTournamentWinnerDeclared)
    {
        CancelMatchmaking();
        GamePlayWaitingPopUp.instance.EnableDisable(true);
        yield return new WaitForSeconds(3f);
        GamePlayWaitingPopUp.instance.EnableDisable(false);

        if (GameRulesManager.currentSelectedGame_GameType == GameRulesManager.GameType.Tournament)
        {
            if (isTournamentWinnerDeclared)
            {
                GamePlayUIPanel.instance.SetData(true, PlayerPersonalData.playerTexture, GameRulesManager.currentSelectedGame_CoinsToPlay);
                yield return new WaitForSeconds(2f);
            }
        }
        else
        {
            Debug.Log("ReloadSplashScene");
            SceneManager.LoadScene(Global.SplashScene);
        }

    }

    public void TournamentMatchError()
    {
    }
    /// <summary>
    /// Cancels the matchmaking process.
    /// </summary>
    public async void CancelMatchmaking()
    {
        await NakamaConnection.CancelMatchmaking();
    }

    /// <summary>
    /// Called when new match state is received.
    /// </summary>
    /// <param name="matchState">The MatchState data.</param>
    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        Debug.Log("OnReceivedMatchState matchState : " + matchState);
        Debug.Log("OnReceivedMatchState opCode : " + matchState.OpCode.ToString());
        Debug.Log("OnReceivedMatchState gameobject.name : " + gameObject.name);

        HandleState(matchState);
    }

    private async void HandleState(IMatchState matchState)
    {
        var resp = Utilities.DecodeState(matchState.State);

        HashedData hashedData = resp.FromJson<HashedData>();

        //print("HashedData: " + hashedData.ToJson());
        string decodedData = hashedData.decodeData();
        print("DecodeData: " + decodedData);

        double timeStamp = double.Parse(hashedData.timeStamp);

        // Decide what to do based on the Operation Code of the incoming state data as defined in OpCodes.
        switch (matchState.OpCode)
        {
            case OpCodes.UNKNOWN:
                Debug.Log("OpCodes.UNKNOWN");
                break;

            case OpCodes.READY:
                Debug.Log("OpCodes.READY");

                break;

            case OpCodes.START_ROUND:
                Debug.Log("OpCodes.START_ROUND");
                gridManager.StartRound();
                break;

            case OpCodes.TURN_MOVE:
                Debug.Log("OpCodes.TURN_MOVE");
                isTurnInProgress = true;
                TurnTimerController.instance.UpdateActionTime(timeStamp);
                PerformMove(decodedData);
                break;

            case OpCodes.UPDATE:
                Debug.Log("OpCodes.UPDATE");
                Debug.Log("OpCodes.GAME_UPDATES: resp: " + decodedData);
                await UpdateGameState(decodedData, timeStamp);
                break;

            case OpCodes.ResetTurnIndex:
                Debug.Log("OpCodes.ResetTurnIndex");
                Debug.Log("Get Reset Turn Index");
                Debug.Log("data:" + decodedData.ToString());
                isTurnInProgress = true;
                //I have to get the index in this opcode to remove the first index error.
                GridManager.instance.hiddenLayerMask.SetActive(false);

                TurnTimerController.instance.UpdateActionTime(timeStamp);
                ServerData serverData = ServerData.FromJson(decodedData.ToString());
                TurnTimerController.nextTurnNumber = serverData.nextTurnIndex;
                break;

            case OpCodes.GAME_ENDED:
                Debug.Log("OpCodes.GAME_ENDED");
                Debug.Log("GAME_ENDED:  resp: " + decodedData);

                WinnerData winnerData = JsonUtility.FromJson<WinnerData>(decodedData.ToString());
                Player winner = GamePlayUIPanel.instance.players.Find(x => x.playerPersonalData.playerUserID.Equals(winnerData.playerUserID));
                gridManager.GameFinishCallBack(winner);
                break;

            default:
                Debug.Log(" Opcode not found. default case run. . .");
                break;
        }
    }
    private async Task UpdateGameState(string response, double timeStamp)
    {
        //response = "{" + response + "}";
        UpdateMessage updateMessage = response.FromJson<UpdateMessage>();

        Debug.Log("Updated Message Code: " + updateMessage.code);
        Debug.Log("Updated Message Data: " + updateMessage.data);

        
        switch(updateMessage.code)
        {
            case GameUpdates.UpdateTime:
                Debug.Log("GameUpdates.UpdateTime");
                Debug.Log("Update Time: " + updateMessage.data);
                Epoch.UpdateDiff(double.Parse(updateMessage.data));
                break;

            case GameUpdates.SpawnPlayers:
                Debug.Log("GameUpdates.SpawnPlayers");
                await OnSpawnPlayersUpdate(updateMessage.data);
                break;

            case GameUpdates.TILES_Order:
                Debug.Log("GameUpdates.TILES_Order");
                TurnTimerController.instance.UpdateActionTime(timeStamp);
                ShuffleMessage shuffleMessage = updateMessage.data.FromJson<ShuffleMessage>();
                multiplayerTilesOrder = shuffleMessage.shuffleArray.ToList();
                tieTilesOrder = shuffleMessage.tieShuffleArray.ToList();
                if (!gridManager.gameObject.activeInHierarchy) OnMatchStarted();
                gridManager.StartGame();
                break;

            case GameUpdates.PickUpNewTile:
                Debug.Log("GameUpdates.PickUpNewTile");

                TurnTimerController.instance.UpdateActionTime(timeStamp);

                //string playerId = updateMessage.data.Replace("\"", "");

                updateMessage.data = updateMessage.data.Replace(@"\", "");
                updateMessage.data = updateMessage.data.Substring(1, updateMessage.data.Length - 2);

                Debug.Log("updateMessage.data: "  + updateMessage.data.ToString());

                string playerId = updateMessage.data.ToString().FromJson<PlayerIdParse>().playerID; //. updateMessage.data.FromJson<PlayerIdParse>().playerID;

                //if (gridManager.currentPlayer.playerPersonalData.playerUserID == playerId)
                //{
                //    gridManager.currentPlayer.PickUpNewTileMultiplayer();
                //}
                //else
                //{
                    //Debug.Log("Current Player is not same with the person who sent the gameUpdate... ");
                    Player pickUpTilePlayer = GamePlayUIPanel.instance.players.ToList().Find(x => x.playerPersonalData.playerUserID == playerId);
                    pickUpTilePlayer.PickUpNewTileMultiplayer();
                //}
                break;


            case GameUpdates.SkipTurn:

                Debug.Log("GameUpdates.SkipTurn");
                TurnTimerController.instance.UpdateActionTime(timeStamp);
                //TurnMove skipTurn = TurnMove.FromJson(updateMessage.data);
                ServerData serverData = ServerData.FromJson(updateMessage.data);

                print("ServerData: " + serverData.ToJson());

                SkipTurnUpdate skipTurn = SkipTurnUpdate.FromJson(serverData.dataInJson);

                print("SkipTurnUpdate: " + skipTurn.SkipReason);


                TurnTimerController.nextTurnNumber = serverData.nextTurnIndex;
                //if (gridManager.currentPlayer.playerPersonalData.playerUserID == skipTurn.PlayerId)
                //{
                //    TurnTimerController.instance.ResetTimer(gridManager.currentPlayer);
                //    Debug.Log("1");
                //    gridManager.currentPlayer.SkipMyTurn(skipTurn.SkipReason);
                //}
                //else
                //{
                //    Debug.Log("Current Player is not same with the person who sent the gameUpdate... ");
                Player skipTurnPlayer = GamePlayUIPanel.instance.players.ToList().Find(x => x.playerPersonalData.playerUserID == skipTurn.PlayerId);
                TurnTimerController.instance.ResetTimer(skipTurnPlayer);
                Debug.Log("2");
                skipTurnPlayer.SkipMyTurn(skipTurn.SkipReason);
                //}
                break;


            case GameUpdates.TurnTimeout:

                Debug.Log("GameUpdates.TurnTimeout");
                //TurnTimerController.instance.UpdateActionTime(timeStamp);

                //TurnMove endTurnData = TurnMove.FromJson(updateMessage.data);

                //TurnTimerController.nextTurnNumber = int.Parse(endTurnData.nextTurnIndex);

                //Debug.Log("TURN TIMEOUT FOR : " + TurnTimerController.nextTurnNumber);
                //Debug.Log("TurnData : " + endTurnData.turnData.PlayerId);

                //Player turnTimeoutPlayer = GamePlayUIPanel.instance.players.ToList().Find(x => x.playerPersonalData.playerUserID == endTurnData.turnData.PlayerId);
                //TurnTimerController.instance.ResetTimer(turnTimeoutPlayer);
                //if (GridManager.instance.currentPlayer.isMe)
                //{
                //    GridManager.instance.currentPlayer.AutoPlayTurnMultiplayer();
                //}
                break;

            case GameUpdates.PlayerLeft:

                Debug.Log("GameUpdates.PlayerLeft");
                serverData = ServerData.FromJson(updateMessage.data);

                print("GameUpdates.PlayerLeft: ServerData: " + updateMessage.data);

                skipTurn = SkipTurnUpdate.FromJson(serverData.dataInJson);

                Debug.Log("Old Player Count Before Removed: " + GamePlayUIPanel.instance.players.Count);

                if (skipTurn.PlayerId == PlayerPersonalData.playerUserID)
                {
                    Debug.Log("Server Kicked me..");
                    Debug.Log(gameObject.name + " detect error: ln: 660");
                    OnRequestQuitMatch.Invoke();
                }
                else
                {
                    print("Reason: " + skipTurn.SkipReason);

                    Player leftPlayer = GamePlayUIPanel.instance.players.ToList().Find(x => x.playerPersonalData.playerUserID == skipTurn.PlayerId);

                    if (isTurnInProgress)
                    {
                        if (skipTurn.PlayerId == gridManager.currentPlayer.playerPersonalData.playerUserID)
                        {
                            if (gridManager.isFirstTurn)
                            {
                                Debug.LogError("First Turn Player Left without playing his turn, Restart Round...");

                                if (GamePlayUIPanel.instance.players.Count > 2)//if more than 2 players exist
                                {
                                    TurnTimerController.instance.ResetTimer(gridManager.currentPlayer);
                                    Debug.LogError("Execute....");
                                    GridManager.roundNum--;
                                    StartCoroutine(gridManager._RestartRound());
                                }
                            }
                            else
                            {
                                TurnTimerController.instance.UpdateActionTime(timeStamp);

                                TurnTimerController.nextTurnNumber = serverData.nextTurnIndex;
                                TurnTimerController.instance.ResetTimer(leftPlayer);
                                Debug.Log("5");
                                leftPlayer.SkipMyTurn(skipTurn.SkipReason);
                            }
                        }
                        else
                        {
                            GamePlayUIPanel.instance.PopUpController(skipTurn.SkipReason, leftPlayer.playerPersonalData.playerTexture);
                        }
                    }
                    else
                    {
                        Debug.LogError("Player Left, But the game is not in progress.");
                    }

                    GamePlayUIPanel.instance.PopUpController(skipTurn.SkipReason, leftPlayer.playerPersonalData.playerTexture);
                    StartCoroutine(PlayerLeft(leftPlayer));
                }
                break;


            case GameUpdates.RoundEnd:
                Debug.Log("GameUpdates.RoundEnd");
                Debug.LogError("Game State: GameUpdates.RoundEnd: " + GameUpdates.RoundEnd);
                isTurnInProgress = false;
                break;


            case GameUpdates.RestartRound:
                Debug.Log("GameUpdates.RestartRound");
                GamePlayWaitingPopUp.instance.EnableDisable(false);
                GetShuffledArray();
                break;


            case GameUpdates.Capicua:
                Debug.Log("GameUpdates.Capicua");
                Debug.Log("It's a Capicua ");
                Rule4.canLastTilePlayedOnBothSides = Rule4.CheckCapicua(2);
                break;


            case GameUpdates.MultipleOfFive:

                Debug.Log("GameUpdates.MultipleOfFive");
                updateMessage.data = updateMessage.data.Replace(@"\", "");
                updateMessage.data = updateMessage.data.Substring(1, updateMessage.data.Length-2);
                Debug.Log("It's HighFiveScore" + updateMessage.data);

                UpdatePlayerScore playerScoreObj = updateMessage.data.ToString().FromJson<UpdatePlayerScore>();

                //Debug.Log("highFiveScore" + highFiveScore.ToString());

                Player player = GamePlayUIPanel.instance.players.Find(x => x.playerPersonalData.playerUserID.Equals(playerScoreObj.playerID));
                StartCoroutine(GridManager.instance.GiveMultipleOfFiveScore(playerScoreObj.score, player));
                break;


            case GameUpdates.KickPlayers:
                Debug.Log("GameUpdates.KickPlayers");
                print("kickedPlayers.");
                string[] kickedPlayersIds = JsonConvert.DeserializeObject<string[]>(updateMessage.data);
                if (kickedPlayersIds.Length > 0)
                {
                    if (kickedPlayersIds.ToList().Find(x=>x.Equals(PlayerPersonalData.playerUserID)) != null) //It's me
                    {
                        Debug.Log("Server Kicked me..");
                        Debug.Log(gameObject.name + " detect error: ln: 751");
                        OnRequestQuitMatch.Invoke();
                    }
                    else
                    {
                        Debug.Log("KickedPlayers ..");
                        KickedPlayers(kickedPlayersIds);
                        GamePlayWaitingPopUp.instance.EnableDisable(false);
                        GetShuffledArray();
                    }
                }
                else
                {
                        Debug.Log("KickedPlayers ..");
                        GamePlayWaitingPopUp.instance.EnableDisable(false);
                        GetShuffledArray();
                }
                break;


            default:
                Debug.Log("GameUpdates.default");
                Debug.LogError("Error: Default Witch Case Executed...");
                break;
        }
    }


    IEnumerator PlayerLeft(Player player)
    {
        yield return new WaitForSeconds(2f);
        player.DestroyPlayer();

        //Update the Host player if the Host left.
        if (player.playerPersonalData.playerUserID == hostUserID)
        {
            if (currentMatch == null)
                yield break;
            hostUserID = GamePlayUIPanel.instance.players.OrderBy(x => x.playerPersonalData.playerUserID).First().playerPersonalData.playerUserID;
        }

    }

    void KickedPlayers(string[] playerIds)
    {
        foreach (var item in playerIds)
        {
            Player leftPlayer = GamePlayUIPanel.instance.players.Find(x => x.playerPersonalData.playerUserID.Equals(item));
            leftPlayer.DestroyPlayer();
        }
    }

    private async Task OnSpawnPlayersUpdate(string decodedData)
    {
        MatchStart matchStart = JsonConvert.DeserializeObject<MatchStart>(decodedData); //decodedData.FromJson<MatchStart>();

        print("OnPlayersSpawnUpdate: " + matchStart.newPlayers.Count);
        if (GameRulesManager.currentSelectedGame_GameType == GameRulesManager.GameType.Tournament)
        {
            GameRulesManager.noOfPlayers = matchStart.newPlayers.Count;
        } 
        await SpawnAllPlayers(matchStart);

        try
        {
            hostUserID   = GamePlayUIPanel.instance.players.OrderBy(x => x.playerPersonalData.playerUserID).First().playerPersonalData.playerUserID;
        }
        catch (Exception ex)
        {
            Debug.Log("ex: " + ex);
        }
    }
    private void OnMatchStarted()
    {
        Debug.Log("Grid Manager ON.. . . ");
        gridManager.gameObject.SetActive(true);
    }
    private void OnDataCompromised()
    {
        Debug.Log("DATA IS COMPROMISED!!!");
    }


    private void PerformMove(string resp)
    {
        var turnMove = TurnMove.FromJson(resp);
        //print("1---------------- ");
        _SetInputFromState(turnMove);
    }

    /// <summary>
    /// Sets the appropriate input values on the PlayerMovementController and PlayerWeaponController based on incoming state data.
    /// </summary>
    /// <param name="state">The incoming state Dictionary.</param>
    private void _SetInputFromState(TurnMove turnMove)
    {
        //print("2---------------- ");
        //var stateDictionary = GetStateAsDictionary(state);

        TurnTimerController.nextTurnNumber = int.Parse(turnMove.nextTurnIndex);

        string inputTileValue = turnMove.turnData.TileValue;//stateDictionary[MatchDataJson.tileKey].ToString();
        int possibilityTileValue = int.Parse(turnMove.turnData.TilePossibility);//int.Parse(stateDictionary[MatchDataJson.tilePossibilityKey]);
        string possibilityTileDirectionName = turnMove.turnData.TilePossibilityDirectionName;//stateDictionary[MatchDataJson.tilePossibilityDirectionNameKey].ToString();
        string tilePossibilityParentValue = turnMove.turnData.TilePossibilityParentValue;//stateDictionary[MatchDataJson.tilePossibilityParentValueKey].ToString();
        bool tilePossibility_IsSameFace = bool.Parse(turnMove.turnData.IsSameFace);//bool.Parse(stateDictionary[MatchDataJson.tilePossibility_IsSameFace]);
        int first = Convert.ToInt32(Char.GetNumericValue(inputTileValue[0]));
        int second = Convert.ToInt32(Char.GetNumericValue(inputTileValue[1]));

        int tilePossibilityParentValue_First = Convert.ToInt32(Char.GetNumericValue(tilePossibilityParentValue[0]));
        int tilePossibilityParentValue_Second = Convert.ToInt32(Char.GetNumericValue(tilePossibilityParentValue[1]));

        Debug.Log("inputTileValue Value = " + first +","+second);
        Debug.Log("possibilityTileValue Value = " + possibilityTileValue);
        //Tile tile = gridManager.currentPlayer.dominosCurrentList.Find(x => x.First == first && x.Second == second);
        Tile tile = null;//gridManager.currentPlayer.dominosCurrentList.Find(x => x.First == first && x.Second == second);
        TilePossibilities tilePossibility = null;

        if (possibilityTileValue != -1)
        {
            tilePossibility = FindObjectsOfType<TilePossibilities>(true).ToList().Find
            (   x => x.value == possibilityTileValue &&
                x.GetComponentInParent<Tile>().First == tilePossibilityParentValue_First &&
                x.GetComponentInParent<Tile>().Second == tilePossibilityParentValue_Second &&
                x.isSamePhase == tilePossibility_IsSameFace &&
                x.directionOfTile == possibilityTileDirectionName   );
        }
        
        Debug.Log(tilePossibility == null ? "tilePossibility Not Found" : "TilePossibility Found");
        Debug.Log(possibilityTileDirectionName == null ? "possibilityTileDirectionName Not Found" : "possibilityTileDirectionName Found");


        
        if (tile == null)
        {
            //Placing first tile
            tile = FindObjectsOfType<Tile>(true).ToList().Find(x => x.First == first && x.Second == second);
        }

        Debug.Log(tile == null ? "Tile Not Found" : "Tile Found");

        Player player = GamePlayUIPanel.instance.players.Find(x => x.playerPersonalData.playerUserID.Equals(turnMove.turnData.PlayerId));
        if (player == null)
            Debug.Log("Some Error here, player not found...");
        else
            GridManager.instance.currentPlayer = player;

        //GridManager.instance.hiddenLayerMask.SetActive(false);

        if (tile != null && possibilityTileValue == -1)
        {
            //Placing first tile
            tile.placeFirstTile(); // Multiplayer Game, Playing first move
        }
        if (tile != null && tilePossibility != null && !string.IsNullOrEmpty(possibilityTileDirectionName))
        {
            tile.PlaceTileOnParticularPosition(tilePossibility);
        }
    }

    /// <summary>
    /// Spawns a player.
    /// </summary>
    /// <param name="matchId">The match the player is connected to.</param>
    /// <param name="jsonPlayer">The player's network presence data.</param>
    /// <param name="spawnIndex">The spawn location index they should be spawned at.</param>
    private async Task SpawnPlayer(string matchId, NewJsonPlayer jsonPlayer, int sortIndex, int mySeatIndex)
    {
        Debug.Log("SpawnPlayer");
        // If the player has already been spawned, return early.
        if (players.ContainsKey(jsonPlayer.UserId))
        {
            return;
        }


        // Set a variable to check if the player is the local player or not based on session ID.
        bool isLocal = jsonPlayer.UserId == localUserPresence.SessionId;

        Debug.Log("SpawnPlayer isLocal: " + isLocal);
        // Choose the appropriate player prefab based on if it's the local player or not.
        var playerPrefab = NetworkRemotePlayerPrefab;

        // Spawn the new player.
        var player = Instantiate(playerPrefab, GamePlayUIPanel.instance.players[sortIndex].playerPhysicalPosition.currentplayerPosition.position, Quaternion.identity);

        //Destroy(spawnPoint.gameObject);
        //// Setup the appropriate network data values for player.
        RemotePlayerNetworkData remotePlayerNetworkData = player.GetComponent<PlayerNetworkRemoteSync>().NetworkData;
        remotePlayerNetworkData.isLocalUser = isLocal;

        List<string> userID = new List<string>();
        userID.Add(jsonPlayer.UserId);

        var iApiUsersData = await NakamaConnection.Client.GetUsersAsync(NakamaConnection.Session, userID);
        IApiUser apiUser = iApiUsersData.Users.ToList().Find(x => x.Id == jsonPlayer.UserId);

        Debug.Log("apiUser Display Name ====== " + apiUser.DisplayName);
        _FillPlayerProperties(matchId, apiUser, jsonPlayer, player , sortIndex);


        // Add the player to the players array.
        players.Add(jsonPlayer.UserId, player);

        // If this is our local player, add a listener for the PlayerDied event.
        if (isLocal)
        {
            localPlayerGameObj = player;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="matchStart"></param>
    /// <returns></returns>
    public async Task SpawnAllPlayers(MatchStart matchStart)
    {
        //var matchStart = MatchStart.FromJson(response);
        foreach (var item in matchStart.newPlayers)
        {
            Debug.Log("item.username: " + item.username);
        }
        Debug.Log("localUserPresence.Username: " + localUserPresence.Username);

        var keyValuePair = matchStart.newPlayers.Single(x => x.username == localUserPresence.Username);

        PlayersDataInJsonOrder.AddRange(matchStart.newPlayers);
        if (keyValuePair != null)
        {
            Debug.Log("this is me, and my username is: " + keyValuePair.username);
            mySeatIndex = getIndexOfKey(matchStart.newPlayers, localUserPresence.Username);
            Debug.Log("My Seat Index is : " + mySeatIndex);
        }

        GamePlayUIPanel.arrangeProfilesOrderBasedOnMySeatNumber(mySeatIndex);

        //GridManager.RemoveExtraPlaces(GameRulesManager.noOfPlayers);

        //await Task.Delay(1000);
        int currentUserIndex = 0;
        foreach (var item in matchStart.newPlayers)
        {
           await SpawnPlayer(currentMatch.Id, item, currentUserIndex , mySeatIndex);
           currentUserIndex++;
        }

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="tempDict"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private int getIndexOfKey(List<NewJsonPlayer> tempDict, string key)
    {
        int index = -1;
        foreach (var value in tempDict)
        {
            index++;
            if (key == value.username)
                return index;
        }
        return -1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task SavePlayerStatsOnNakama()
    {
        try
        {
            var response = await NakamaConnection.Socket.RpcAsync("rpcUpdateMetadataKey", MatchDataJson.JSON_DataProfileStats());

            print("Sending States....... "+response);
        }
        catch (Exception exception)
        {
            Debug.Log("exception: " + exception.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void GetShuffledArray()
    {
        try
        {
            Dictionary<string, string> hostValueDict = new Dictionary<string, string> {
                { "HostID", hostUserID},
            };
            UpdateMessage updateMessage = new UpdateMessage
            {
                code = GameUpdates.TILES_Order,
                data = hostValueDict.ToJson()
            };
            SendMatchStateAsync(OpCodes.UPDATE, updateMessage.ToJson());
        }
        catch (Exception exception)
        {
            Debug.Log("exception: " + exception.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void NextRoundCall()
    {
        try
        {
            UpdateMessage updateMessage = new UpdateMessage
            {
                code = GameUpdates.RestartRound,
                data = "Restart Round"
            };
            SendMatchStateAsync(OpCodes.UPDATE, updateMessage.ToJson());
        }
        catch (Exception exception)
        {
            Debug.Log("exception: " + exception.Message);
        }
    }

    /// <summary>
    /// Quits the current match.
    /// </summary>
    public async Task QuitMatch()
    {
        Debug.Log("QuitMatch");
        // Ask Nakama to leave the match.
        if(currentMatch != null && NakamaConnection.Socket.IsConnected)  await NakamaConnection.Socket.LeaveMatchAsync(currentMatch);
        //await NakamaConnection.Socket.CloseAsync();
        // Reset the currentMatch and localUser variables.
        currentMatch = null;
        localUserPresence = null;

        // Destroy all existing player GameObjects.
        foreach (var player in players.Values)
        {
            Destroy(player);
        }

        // Clear the players array.
        players.Clear();

        GridManager.ResetStaticFields();
        if (GameRulesManager.currentSelectedGame_GameType == GameRulesManager.GameType.SingleMatch)
        {
            SceneManager.LoadScene(Global.UIScene);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="msgType"></param>
    /// <param name="msgIndex"></param>
    public void SendChatMessage(ChatData.MsgType msgType, int msgIndex)
    {
        ChatData chatData = new ChatData
        {
            _msgType = msgType,
            MsgIndex = msgIndex
        };

        Debug.Log("chatChannel ID : " + chatChannelID);
        chatManager.SendChatMessage(chatChannelID, chatData);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="opCode"></param>
    /// <param name="state"></param>
    public async void SendMatchStateAsync(long opCode, string state)
    {
        //print("Before Hash: " + state);

        HashedData hashedData = new HashedData(state, Epoch.Now, HashKey, this.MatchId);

        Debug.Log("SendMatchStateAsync: Opcode = " + opCode  + " state = " + state);

        await NakamaConnection.Socket.SendMatchStateAsync(currentMatch.Id, opCode, hashedData.ToJson());
    }

    /// <summary>
    /// Sets the local user's display name.
    /// </summary>
    /// <param name="displayName">The new display name for the local user</param>
    public void SetDisplayName(string displayName)
    {
        // We could set this on our Nakama Client using the below code:
        // await NakamaConnection.Client.UpdateAccountAsync(NakamaConnection.Session, null, displayName);
        // However, since we're using Device Id authentication, when running 2 or more clients locally they would both display the same name when testing/debugging.
        // So in this instance we will just set a local variable instead.
        localUserDisplayName = displayName;
    }


    internal static double GetServerTime()
    {
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
            return DateTime.Now.Millisecond;
        else
            return serverTime;
    }
}

[Serializable]
public class ChatManager 
{
    GameManager gameManager = null;
    public ChatManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
    public async Task JoinChatChannel(string channelName)
    {
        Debug.Log("JoinChatChannel Joining . . . " );
        var roomName = channelName;
        var persistence = true;
        var hidden = false;
        var channel = await gameManager.NakamaConnection.Socket.JoinChatAsync(roomName, ChannelType.Room, persistence, hidden);
        gameManager.chatUsers.AddRange(channel.Presences);
        Debug.Log("JoinChatChannel Joined. channel ID" + channel.Id);
        gameManager.chatChannelID = channel.Id;
    }

    public async void SendChatMessage(string channelID, ChatData chatData)
    {
        var channelId = channelID;

        Dictionary<string, ChatData> keyValuePairs = new Dictionary<string, ChatData>();
        keyValuePairs.Add("msg",chatData);
        var content = keyValuePairs.ToJson();
        var sendAck = await gameManager.NakamaConnection.Socket.WriteChatMessageAsync(channelId, content);

    }

    internal void OnReceivedChatChannelPresence(IChannelPresenceEvent presenceEvent)
    {
            foreach (var presence in presenceEvent.Leaves)
            {
                GameManager.instace.chatUsers.Remove(presence);
            }
            GameManager.instace.chatUsers.AddRange(presenceEvent.Joins);
            Console.WriteLine("Room users: [{0}]", string.Join(",\n  ", GameManager.instace.chatUsers));
    }

    internal void ShowRecievedMessage(IApiChannelMessage message)
    {
        Debug.Log("Message Recieved: ");
        //Debug.Log("message.Content: " + message.Content);
        //Debug.Log("message.SenderId: " + message.SenderId);

        if (message.SenderId == gameManager.localUserPresence.UserId)
        {
            Debug.Log("i recieved my own msg. . .");
        }

        IUserPresence userPresence = gameManager.chatUsers.Find(x => x.UserId.Equals(message.SenderId));

        if (userPresence!=null)
        {
            Dictionary<string, ChatData> keyValuePairs = message.Content.FromJson<Dictionary<string, ChatData>>();

            ChatData chatData = null;
            keyValuePairs.TryGetValue("msg" , out chatData);

            Debug.Log("chatData: " + chatData);
            ChatScreenHandler.instance.SpawnChatMessage(chatData._msgType, chatData.MsgIndex,message.Username);
        }
    }
}

public class ChatData
{
    public enum MsgType
    {
        TextMsg,
        Emojis
    }
    public MsgType _msgType = MsgType.Emojis;
    public int MsgIndex = -1;
}

[Serializable]
public class ServerData
{
    public int nextTurnIndex;
    public string dataInJson;

    public static ServerData FromJson(string json) => JsonConvert.DeserializeObject<ServerData>(json, Dominos.Converter.Settings);
}

///Comments
#region Download Picture through Async Method

//public async Task<Texture2D> DownLoadPicture(string url)
//{
//    using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
//    {
//        await uwr.SendWebRequest();

//        if (uwr.result != UnityWebRequest.Result.Success)
//        {
//            Debug.Log(uwr.error);
//            return null;
//        }
//        else
//        {
//            // Get downloaded asset bundle
//            return DownloadHandlerTexture.GetContent(uwr); 
//        }
//    }
//}

//public struct UnityWebRequestAwaiter : INotifyCompletion
//{
//    private UnityWebRequestAsyncOperation asyncOp;
//    private Action continuation;

//    public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
//    {
//        this.asyncOp = asyncOp;
//        continuation = null;
//    }

//    public bool IsCompleted { get { return asyncOp.isDone; } }

//    public void GetResult() { }

//    public void OnCompleted(Action continuation)
//    {
//        this.continuation = continuation;
//        asyncOp.completed += OnRequestCompleted;
//    }

//    private void OnRequestCompleted(AsyncOperation obj)
//    {
//        continuation?.Invoke();
//    }
//}

//public static class ExtensionMethods
//{
//    public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
//    {
//        return new UnityWebRequestAwaiter(asyncOp);
//    }
//}

#endregion