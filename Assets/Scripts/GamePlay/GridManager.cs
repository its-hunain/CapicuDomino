using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvatarBuilder;
using Dominos;
using Nakama.TinyJson;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [Header("High Five Special Collider")]
    public List<PlayerData> tempPlayers = new();

    [Space]
    [Header("High Five Special Collider")]
    public HighFiveColliders highFiveColliders;

    [Space]
    [Header("Hidden Layer For Multiplayer")]
    public GameObject hiddenLayerMask;

    [Space]
    [Header("Grid Manager Variables")]
    public Transform tableChildParent;
    public Transform centerPoint;

    [Space]
    [Header("Xtra Pos For PopUp Heading")]
    public RectTransform HeadingPos;

    //Static variables
    public static GridManager instance;

    //[Header("Players Count")]
    //public int noOfPlayers = 2;

    [Header("Public Dominos List")]
    public List<Domino> dominosList = new List<Domino>();
    public List<Tile> dominosCurrentList = new List<Tile>();

    [Header("Domino Tile Prefab")]
    public Tile sameFaceDomino;

    [Header("Domino Tile Prefab")]
    public Tile differentFaceDomino;

    [Header("UI Tile Prefab")]
    public ScrollRect playerHandScrollRect;
    public UITile uiTilePrefab;

    [Header("Tiles that placed in Table")]
    public List<Tile> addedInMap = new List<Tile>();
    [Header("Tiles that placed in Table On Left")]
    public List<Tile> addedInMapOnLeftSide = new List<Tile>();
    [Header("Tiles that placed in Table On Right")]
    public List<Tile> addedInMapOnRightSide = new List<Tile>();


    public Tile firstTile;
    public int firstTileValue = 0;
    public Player currentPlayer;
    public int TurnSwitch = 0;
    public bool isFirstTurn = false;


    [Header("Colliders Parent")]
    public Transform normalCollidersParent;
    public Transform highFiveCollidersParent;

    //[Header("Table Colliders")]
    //public List<Collider> colliders;

    //[Header("All Five Table Colliders")]
    //public List<Collider> TwoExtraColliders;

    [Header("Table Colliders Parent")]
    public Transform currentSelectedCollidersParentTransform;

    [Header("Shuffling Panel")]
    public GameObject FakeShufflingPanel;
    public Animation FakeShufflingAnimation;

    public Transform outsideTransform;


    [Header("Special Variables")]
    public Player lastTilePlayedPlayer;
    public Player firstTilePlayer;
    public bool wasLastRoundTie = false;
    public bool autoArrangementAllowed = true;

    public static int roundNum = 1;

    TableBordersController tableBordersController;

    private void Awake()
    {
        instance = this;
        tableBordersController = FindObjectOfType<TableBordersController>();
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot) RemoveExtraPlaces(GameRulesManager.noOfPlayers);
    }

    //Calling this start in restarting game as well.
    public void Start()
    {
        Debug.Log("Match Started...");

        //if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
        //{
        //    yield return new WaitUntil(() => GameManager.multiplayerTilesOrder.Count > 0 && GameManager.players.Count == GameRulesManager.noOfPlayers);
        //}
        //else
        //{
        //    StartGame();
        //}

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        currentSelectedCollidersParentTransform = (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode1) ? highFiveCollidersParent : normalCollidersParent;

        if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode1) currentSelectedCollidersParentTransform.gameObject.SetActive(true);

        autoArrangementAllowed = true;

        ResetColliders();

        FillingDominosList();

        ResetTableData();

        SpawningAndShufflingTilesInTwoRows();

        StartCoroutine(_DistributeTilesToPlayers());
    }

    public IEnumerator _DistributeTilesToPlayers()
    {
        Debug.Log("_DistributeTilesToPlayers");

        //tableBordersController.EnableDisableParent(false);

        TurnTimerController.nextTurnNumber = 0;
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(_DistributeCardsToPlayerHandsAndBoneYard());
        yield return new WaitForSeconds(2f);

        //tableBordersController.EnableDisableParent(true);

        KeyValuePair<bool, Player> reShufflePlayer = CheckDoubleFacesCount();
        if (reShufflePlayer.Key == true)
        {
            GamePlayUIPanel.instance.PopUpController("5 Doubles", reShufflePlayer.Value.playerPersonalData.playerTexture);
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(reShufflePlayer.Value._ShowDoubleFaceTiles());
            yield return new WaitForSeconds(1f);
            roundNum--;
            StartCoroutine(_RestartRound());
            yield break;
        }

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
        {
            Player myPlayer = GamePlayUIPanel.instance.players.Find(x => x.isMe);
            Debug.Log("Updating first player turn. my player: " + myPlayer.name);
            foreach (var item in myPlayer.dominosCurrentList)
            {
                myPlayer.MakeTileInteractableToMove(item, false);
            }
            Debug.Log("Sending first player turn index to server....");

            Dictionary<string, string> emptyData = new Dictionary<string, string> {
                        { "START_ROUND","" }
                    };

            GameManager.instace.SendMatchStateAsync(OpCodes.START_ROUND, emptyData.ToJson());
        }
        else
        {
            StartRound();
        }
    }

    public void StartRound()
    {
        StartCoroutine(_StartRound());
    }

    void UpdateTempPlayers()
    {
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
        {
            return;
        }

        if (roundNum == 1)
        {
            foreach (var item in GamePlayUIPanel.instance.players)
            {
                tempPlayers.Add(item.playerPersonalData);
            }
        }
        else
        {
            //Update Score
            foreach (var item in GamePlayUIPanel.instance.players)
            {
                tempPlayers.Find(x=> x.playerUserID.Equals(item.playerPersonalData.playerUserID)).playerScore = item.playerPersonalData.playerScore;
            }
        }
    }
    IEnumerator _StartRound()
    {
        UpdateTempPlayers(); // Maintaining temp players for Leaderboard.

        if (roundNum == 1)
        {
            if (firstTile != null)
            {
                isFirstTurn = true;

                foreach (var item in currentPlayer.dominosCurrentList)
                {
                    currentPlayer.MakeTileInteractableToMove(item, false);

                }

                if (currentPlayer.isMe)
                {
                    currentPlayer.MakeTileInteractableToMove(firstTile, true);
                }

                TurnTimerController.instance.StartTimer(currentPlayer);

                if (!currentPlayer.isMe)
                {
                    if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
                    {
                        firstTile.placeFirstTile(); //Bot Game, playing first move
                    }
                    else
                    {
                        //Server Update
                        Debug.Log("Multiplayer, Waiting for first player move. . .");
                    }
                }


            }
            else
            {
                StartGame();
            }
        }
        else
        {
            firstTilePlayer = currentPlayer = GameRulesManager.FindNextRoundPlayer(); //Finding first tile player of this round
            isFirstTurn = true;
            firstTile = GameRulesManager.FindFirstTileForSubSequentRounds(currentPlayer.dominosCurrentList); //Finding first tile for cache, if a player can't play then AutoPlay his turn when time ends
            firstTileValue = firstTile.First;

            foreach (var item in currentPlayer.dominosCurrentList)
            {
                currentPlayer.MakeTileInteractableToMove(item, false);
            }
            if (currentPlayer.isMe)
            {
                foreach (var item in currentPlayer.dominosCurrentList)
                {
                    currentPlayer.MakeTileInteractableToMove(item, true);
                }
            }

            TurnTimerController.instance.StartTimer(currentPlayer);

            if (!currentPlayer.isMe)
            {
                if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
                {
                    //firstTile = GameRulesManager.FindFirstTileForSubSequentRounds(currentPlayer.dominosCurrentList); //Finding first tile if it's a bot player of this round
                    //firstTileValue = firstTile.First; //saving (6,6)
                    Debug.Log("Next Round, first player: " + firstTilePlayer, firstTilePlayer);
                    firstTile.placeFirstTile(); //Bot Game, playing first move
                }
                else
                {
                    //Server Update
                    Debug.Log("Multiplayer, Waiting for first player move. . .");
                }
            }
        }

        //Sending First Player Turn Index.
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
        {
            if (currentPlayer == null)
            {
                Debug.LogError("Current Player is Null... can't Send first player turn index to server. ");
                yield return new WaitForSeconds(1f);
                StartCoroutine(_RestartRound());
                yield break;
            }
            else
            {
                if (PlayerPersonalData.playerUserID.Equals(currentPlayer.playerPersonalData.playerUserID))
                {
                    int index = currentPlayer != null ? GamePlayUIPanel.instance.players.IndexOf(currentPlayer) : 0;
                    //TurnTimerController.nextTurnNumber = index; // Remove this line from here to remove the first index error.

                    Dictionary<string, string> hostValueDict = new Dictionary<string, string> {
                        { "currentIndex",index.ToString() }
                    };

                    Debug.Log("Sending first player turn index to server....");

                    GameManager.instace.SendMatchStateAsync(OpCodes.ResetTurnIndex, hostValueDict.ToJson());
                }
            }
        }
    }

    private KeyValuePair<bool, Player> CheckDoubleFacesCount()
    {
        foreach (var player in GamePlayUIPanel.instance.players)
        {
            int doubleFaceCount = 0;
            foreach (var tile in player.dominosCurrentList)
            {
                if (tile.SameFace)
                {
                    doubleFaceCount++;
                    if (doubleFaceCount >= 5) return new KeyValuePair<bool, Player>(true, player);
                }
            }
        }
        return new KeyValuePair<bool, Player>(false, null);
    }

    public IEnumerator _RestartRound()
    {
        //GameRulesManager.GameStarted = false;

        //Destroying each player hand tiles
        foreach (var player in GamePlayUIPanel.instance.players)
        {
            player.DestroyOldDominoTiles();
        }

        tableChildParent.transform.position = Vector3.zero;
        GamePlayUIPanel.instance.UpdateRound();
        //ResetTableData();
        GamePlayUIPanel.instance.PopUpController(HeadingPos, "Round " + roundNum.ToString());

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
        {
            yield return new WaitForSeconds(2f);
            GameManager.instace.NextRoundCall();
            //Waiting For Someomne
            Debug.Log("Waiting for all player to be ready");
            GamePlayWaitingPopUp.instance.SetData("Waiting for all the players to be ready...", false);
            GamePlayWaitingPopUp.instance.EnableDisable(true);
        }
        else
        {
            yield return new WaitForSeconds(1);

            StartGame();
        }
    }

    private void ResetColliders()
    {
        GridManager.instance.hiddenLayerMask.SetActive(false);

        currentSelectedCollidersParentTransform.gameObject.SetActive(true);
        foreach (var item in currentSelectedCollidersParentTransform.GetComponentsInChildren<Collider>())
        {
            item.enabled = true;
        }
    }

    public IEnumerator _ChangeTurn(Tile tile = null)
    {
        //currentPlayer.UpdateTilesCount();
        TurnTimerController.instance.StopTimer(currentPlayer);

        //Set current player remaining tiles unmoveable
        for (int i = 0; i < currentPlayer.dominosCurrentList.Count; i++)
        {
            currentPlayer.MakeTileInteractableToMove(currentPlayer.dominosCurrentList[i], false);
        }

        //Turn Off Shines and selection
        currentPlayer.UpdateShineChild();
        currentPlayer.UpdateTilesCount();

        //Debug.Log("TurnSwitch Count: " + TurnSwitch);
        //if current players tiles finish || if all players skip
        if (currentPlayer.dominosCurrentList.Count == 0 || TurnSwitch == GamePlayUIPanel.instance.players.Count)
        {
            if (currentPlayer.dominosCurrentList.Count == 0) // Special Case for HighFive on last turn.
            {
                currentPlayer.Shuffle(); //Sort Hand Tile of Current Player
                yield return new WaitForSeconds(1);
                ChangeCollidersState(true);
                //StartCoroutine(_CheckMultiplayerOfFive());
                yield return new WaitForSeconds(1);
            }


            if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
            {
                //Let all player broadcast this call.. no worries
                //if (PlayerPersonalData.playerUserID.Equals(currentPlayer.playerPersonalData.playerUserID))
                //{
                GameManager.instace.isTurnInProgress = false;
                UpdateMessage message = new UpdateMessage
                {
                    code = GameUpdates.RoundEnd,
                    data = "Round End"
                };
                GameManager.instace.SendMatchStateAsync(OpCodes.UPDATE, message.ToJson());
                //}
            }

            //hiddenLayerMask.SetActive(false);

            ChangeCollidersState(false);
            yield return new WaitForSeconds(1f);

            GamePlayUIPanel.instance.PopUpController(HeadingPos, "Round Finish!");

            yield return new WaitForSeconds(1f);

            Debug.Log("addedInMap Count: " + addedInMap.Count);

            //Step 1: Side the open tiles.
            Debug.Log("Step 1: _SideTheExtraTilesFromTable");
            yield return StartCoroutine(_SideTheExtraTilesFromTable());

            //Step 2: Show Each Player hand Tiles
            Debug.Log("Step 2: _ShowTiles");

            Debug.Log("GamePlayUIPanel.instance.players count: " + GamePlayUIPanel.instance.players.Count);
            try
            {
                foreach (var item in GamePlayUIPanel.instance.players)
                {
                    if (item.dominosCurrentList.Count > 0)
                    {
                        yield return StartCoroutine(item._ShowTiles());
                    }
                }
            }
            finally
            {
                Debug.Log("GamePlayUIPanel.instance.players count: " + GamePlayUIPanel.instance.players.Count);
            }
            //Step 3: Calculating Sum of each Players hand tile
            Debug.Log("Step 3: CalculateSumOfEachTile");
            List<PlayerScore> playerScoresList = CalculateSumOfEachTile();

            //Step 4: _Show Tile Score To Badge
            Debug.Log("Step 4: _ShowTileScoreToBadge");
            foreach (var item in playerScoresList)
            {
                yield return StartCoroutine(item.Player._ShowTileScoreToBadge(item.Score));
            }
            yield return new WaitForSeconds(1.5f);

            //Step 5: Calculating Total of all Players hand tile
            int allPlayerPipsSum = CalculatingTotalOfAllPlayers(playerScoresList);
            Debug.Log("Step 5: CalculatingTotalOfAllPlayers allPlayerPipsSum = " + allPlayerPipsSum);

            //High five Scoring implementation
            if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode1)
            {
                Debug.Log("it's high five so changing the sum formula...");
                Debug.Log("old sum was = " + allPlayerPipsSum);
                allPlayerPipsSum = HighFiveRules.CalculateRoundEndScoring(allPlayerPipsSum);
                Debug.Log("New Sum is: " + allPlayerPipsSum);
            }

            //Rule4 Scoring implementation
            if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode4)
            {
                Debug.Log("it's Rule4 so changing the sum formula...");
                Debug.Log("old sum was = " + allPlayerPipsSum);
                allPlayerPipsSum = Rule4.CalculateRoundEndScoring(allPlayerPipsSum);
                Debug.Log("New Sum is: " + allPlayerPipsSum);
            }

            if (currentPlayer.dominosCurrentList.Count == 0)
            {
                //Check Sabanio
                if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode6)
                {
                    Debug.Log("********************* Check Sabanio *********************");
                    allPlayerPipsSum = (Rule6.CheckSabanio(currentPlayer.currentPlacedTile)) ? allPlayerPipsSum *= 2 : allPlayerPipsSum; //Double the score if Sabanio
                    yield return null;
                }

                wasLastRoundTie = false;
                Player badgePlayer = GameRulesManager.lastRoundWinnerPlayer = currentPlayer;

                if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode2)
                {

                    badgePlayer = Rule2.FindHighestScorer(playerScoresList);

                    PlayerScore looserPlayer = Rule2.CalculateScore_RoundBlocked(playerScoresList); //looser player

                    //if tie, then play tie breaker
                    if (looserPlayer.Player == null)
                    {
                        Debug.Log("Play Tie Breaker.");
                        List<PlayerScore> tiePlayers = GameRulesManager.FindTiePlayers(playerScoresList);

                        //Destroying each player hand tiles
                        foreach (var player in GamePlayUIPanel.instance.players)
                        {
                            player.DestroyOldDominoTiles();
                        }

                        yield return StartCoroutine(_PlayTieBreaker(tiePlayers));

                        try
                        {
                            looserPlayer.Player = badgePlayer = TieBreaker.instance.winnerPlayer.Player;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("ex: " + ex.ToString());
                        }
                    }
                    wasLastRoundTie = looserPlayer.Player == null ? true : false;
                    GameRulesManager.lastRoundWinnerPlayer = currentPlayer;
                }

                //Because it's a 1 round game so game finish. don't distribute badges and all. . .
                if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode5)
                {
                    foreach (var item in playerScoresList)
                    {
                        //Step 8: Set Score on profile
                        yield return StartCoroutine(item.Player._SetScore(item.Score));
                        yield return new WaitForSeconds(0.5f);
                    }
                    yield return new WaitForSeconds(0.5f);

                    FinishTheGame(currentPlayer);
                    yield break;
                }
                else
                {
                    //Step 6: Move Badges To Center
                    Debug.Log("Step 6: MoveBadgesToCenter");
                    yield return StartCoroutine(LTGUI.MoveBadgesToCenter(badgePlayer, playerScoresList));

                    //Step 7: Move Badges To Winner Pos
                    yield return StartCoroutine(LTGUI.MoveBadgeToWinner(badgePlayer, allPlayerPipsSum));

                    //Step 8: Set Score on profile
                    yield return StartCoroutine(badgePlayer._SetScore(allPlayerPipsSum));
                }
            }
            else //Round Block
            {
                //Locked Game Cases here
                PlayerScore winnerPlayer = null;
                switch (GameRulesManager.currentSelectedGame_Rule)
                {
                    case GameRulesManager.GameRules.GameMode1:

                        Debug.Log("********** HighFive Blocked **********");

                        KeyValuePair<PlayerScore /*Round Winner*/, List<PlayerScore>/*Sorted Scoring Players*/> keyValuePair = HighFiveRules.CalculateScore_RoundBlocked(playerScoresList);
                        Debug.Log(keyValuePair.Key == null ? "Error to identify a winner. . ." : "No Error in winner.");
                        wasLastRoundTie = keyValuePair.Key == null ? true : false;

                        if (keyValuePair.Key != null)
                        {
                            Debug.Log("New List********** Winner : " + keyValuePair.Key, keyValuePair.Key.Player);
                            foreach (var item in keyValuePair.Value)
                            {
                                Debug.Log("Item: " + item + "Item Score: " + item.Score, item.Player);
                            }

                            foreach (var item in keyValuePair.Value)
                            {
                                int thisPlayerComputedScore = item.Score;
                                Player computedPlayer = item.Player;

                                //Step 6: Move Badges To Center
                                Debug.Log("Step 6: MoveBadgesToCenter");
                                yield return StartCoroutine(LTGUI.MoveBadgesToCenter(computedPlayer, playerScoresList));

                                //PlayerScore playerScore = playerScoresList.Find(x => x.Player.playerPersonalData.playerSeatID == computedPlayer.playerPersonalData.playerSeatID);

                                //Step 7: Move Badges To Winner Pos
                                yield return StartCoroutine(LTGUI.MoveBadgeToWinner(computedPlayer, thisPlayerComputedScore));

                                //Step 8: Set Score on profile
                                yield return StartCoroutine(item.Player._SetScore(thisPlayerComputedScore));
                                yield return new WaitForSeconds(0.5f);
                            }
                        }
                        else
                        {
                            Debug.Log("Round Scoring Tie");
                        }
                        break;

                    case GameRulesManager.GameRules.GameMode2:

                        Debug.Log("********** Rule2 Blocked **********");
                        winnerPlayer = Rule2.FindLowestScorer(playerScoresList);
                        Debug.Log("winnerPlayer: " + winnerPlayer.Player.name, winnerPlayer.Player.gameObject);

                        PlayerScore looserPlayer = Rule2.CalculateScore_RoundBlocked(playerScoresList); //looser player

                        //if tie, then play tie breaker
                        if (looserPlayer.Player == null)
                        {
                            Debug.Log("Play Tie Breaker.");
                            List<PlayerScore> tiePlayers = GameRulesManager.FindTiePlayers(playerScoresList);

                            //Destroying each player hand tiles
                            foreach (var player in GamePlayUIPanel.instance.players)
                            {
                                player.DestroyOldDominoTiles();
                            }

                            yield return StartCoroutine(_PlayTieBreaker(tiePlayers));

                            try
                            {
                                looserPlayer.Player = TieBreaker.instance.winnerPlayer.Player;
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError("ex: " + ex.ToString());
                            }
                        }

                        Debug.Log(looserPlayer.Player == null ? "Error to identify a tie. . ." : "No Error in Tie.");
                        wasLastRoundTie = looserPlayer.Player == null ? true : false;
                        GameRulesManager.lastRoundWinnerPlayer = (looserPlayer.Player == null) ? null : winnerPlayer.Player;

                        if (looserPlayer.Player != null)
                        {
                            //Step 6: Move Badges To Center
                            Debug.Log("Step 6: MoveBadgesToCenter");
                            yield return StartCoroutine(LTGUI.MoveBadgesToCenter(looserPlayer.Player, playerScoresList));

                            //Step 7: Move Badges To Loser Pos
                            yield return StartCoroutine(LTGUI.MoveBadgeToWinner(looserPlayer.Player, looserPlayer.Score));

                            //Step 8: Set Score on profile
                            yield return StartCoroutine(looserPlayer.Player._SetScore(looserPlayer.Score));
                            yield return new WaitForSeconds(0.5f);
                        }
                        else
                        {
                            Debug.Log("Round Scoring Tie");
                        }
                        break;

                    case GameRulesManager.GameRules.GameMode3:
                        Debug.Log("********** Rule3 Blocked **********");
                        winnerPlayer = Rule3.CalculateScore_RoundBlocked(playerScoresList);

                        //This condition will not work anymore because in this rule if tie in scoring we give first turn to the last round winner.
                        if (winnerPlayer == null)
                        {
                            List<PlayerScore> tiePlayers = GameRulesManager.FindTiePlayers(playerScoresList);

                            //Destroying each player hand tiles
                            foreach (var player in GamePlayUIPanel.instance.players)
                            {
                                player.DestroyOldDominoTiles();
                            }

                            yield return StartCoroutine(_PlayTieBreaker(tiePlayers));
                            winnerPlayer = TieBreaker.instance.winnerPlayer;
                        }

                        Debug.Log(winnerPlayer == null ? "Error to identify a tie. . ." : "No Error in Tie.");
                        wasLastRoundTie = winnerPlayer == null ? true : false;
                        GameRulesManager.lastRoundWinnerPlayer = (winnerPlayer == null) ? null : winnerPlayer.Player;

                        if (winnerPlayer != null)
                        {
                            //Step 6: Move Badges To Center
                            Debug.Log("Step 6: MoveBadgesToCenter");
                            yield return StartCoroutine(LTGUI.MoveBadgesToCenter(winnerPlayer.Player, playerScoresList));

                            //Step 7: Move Badges To Winner Pos
                            yield return StartCoroutine(LTGUI.MoveBadgeToWinner(winnerPlayer.Player, winnerPlayer.Score));

                            //Step 8: Set Score on profile
                            yield return StartCoroutine(winnerPlayer.Player._SetScore(winnerPlayer.Score));
                            yield return new WaitForSeconds(0.5f);
                        }
                        else
                        {
                            Debug.Log("Round Scoring Tie");
                        }
                        break;


                    case GameRulesManager.GameRules.GameMode4:
                        Debug.Log("********** Rule4 Blocked **********");
                        winnerPlayer = Rule4.CalculateScore_RoundBlocked(playerScoresList);

                        //Tie, Play Tie Breaker
                        //This code will not work anymore because in this rule if tie in scoring we give first turn to the last round winner.
                        if (winnerPlayer == null)
                        {
                            List<PlayerScore> tiePlayers = GameRulesManager.FindTiePlayers(playerScoresList);

                            //Destroying each player hand tiles
                            foreach (var player in GamePlayUIPanel.instance.players)
                            {
                                player.DestroyOldDominoTiles();
                            }

                            yield return StartCoroutine(_PlayTieBreaker(tiePlayers));
                            winnerPlayer = TieBreaker.instance.winnerPlayer;
                        }

                        Debug.Log(winnerPlayer == null ? "Error to identify a tie. . ." : "No Error in Tie.");
                        wasLastRoundTie = winnerPlayer == null ? true : false;
                        GameRulesManager.lastRoundWinnerPlayer = (winnerPlayer == null) ? null : winnerPlayer.Player;

                        if (winnerPlayer != null)
                        {
                            //Step 6: Move Badges To Center
                            Debug.Log("Step 6: MoveBadgesToCenter");
                            yield return StartCoroutine(LTGUI.MoveBadgesToCenter(winnerPlayer.Player, playerScoresList));

                            //Step 7: Move Badges To Winner Pos
                            yield return StartCoroutine(LTGUI.MoveBadgeToWinner(winnerPlayer.Player, winnerPlayer.Score));

                            //Step 8: Set Score on profile
                            yield return StartCoroutine(winnerPlayer.Player._SetScore(winnerPlayer.Score));
                            yield return new WaitForSeconds(0.5f);
                        }
                        else
                        {
                            Debug.Log("Round Scoring Tie");
                        }
                        break;

                    case GameRulesManager.GameRules.GameMode5:
                        Debug.Log("********** Rule5 Blocked **********");
                        winnerPlayer = Rule5.CalculateScore_RoundBlocked(playerScoresList);

                        ////Tie, Play Tie Breaker
                        //if (winnerPlayer == null)
                        //{
                        //    List<PlayerScore> tiePlayers = GameRulesManager.FindTiePlayers(playerScoresList);

                        //    //Destroying each player hand tiles
                        //    foreach (var player in GamePlayUIPanel.instance.players)
                        //    {
                        //        player.DestroyOldDominoTiles();
                        //    }

                        //    yield return StartCoroutine(_PlayTieBreaker(tiePlayers));
                        //    winnerPlayer = TieBreaker.instance.winnerPlayer;
                        //}

                        Debug.Log(winnerPlayer == null ? "Error to identify a tie. . ." : "No Error in Tie.");
                        wasLastRoundTie = winnerPlayer == null ? true : false;
                        GameRulesManager.lastRoundWinnerPlayer = (winnerPlayer == null) ? null : winnerPlayer.Player;

                        if (winnerPlayer != null)
                        {
                            foreach (var item in playerScoresList)
                            {
                                //Step 8: Set Score on profile
                                yield return StartCoroutine(item.Player._SetScore(item.Score));
                                yield return new WaitForSeconds(0.5f);
                            }
                            yield return new WaitForSeconds(0.5f);
                        }
                        else
                        {
                            Debug.Log("Round Scoring Tie");
                        }
                        break;

                    //Rule 6
                    case GameRulesManager.GameRules.GameMode6:
                        Debug.Log("********** Rule6 Blocked **********");
                        winnerPlayer = Rule6.CalculateScore_RoundBlocked(playerScoresList);

                        //Tie, Play Tie Breaker
                        if (winnerPlayer == null)
                        {

                            Debug.Log("********** Playing Tie Breaker **********");
                            List<PlayerScore> tiePlayers = GameRulesManager.FindTiePlayers(playerScoresList);

                            //Destroying each player hand tiles
                            foreach (var player in GamePlayUIPanel.instance.players)
                            {
                                player.DestroyOldDominoTiles();
                            }

                            yield return StartCoroutine(_PlayTieBreaker(tiePlayers));
                            winnerPlayer = TieBreaker.instance.winnerPlayer;
                        }

                        Debug.Log(winnerPlayer == null ? "Error to identify a tie. . ." : "No Error in Tie.");
                        wasLastRoundTie = winnerPlayer == null ? true : false;
                        GameRulesManager.lastRoundWinnerPlayer = (winnerPlayer == null) ? null : winnerPlayer.Player;

                        if (winnerPlayer != null)
                        {
                            //Means it's a tie, no score distribution in this round but winner will play the first turn in the next round.
                            if (winnerPlayer.Score == 0) Debug.Log("Distributing 0 because it's a tie");

                            //Step 6: Move Badges To Center
                            Debug.Log("Step 6: MoveBadgesToCenter");
                            yield return StartCoroutine(LTGUI.MoveBadgesToCenter(winnerPlayer.Player, playerScoresList));

                            yield return new WaitForSeconds(.5f);
                            //Step 7: Move Badges To Winner Pos
                            yield return StartCoroutine(LTGUI.MoveBadgeToWinner(winnerPlayer.Player, winnerPlayer.Score));

                            yield return new WaitForSeconds(.5f);
                            //Step 8: Set Score on profile
                            yield return StartCoroutine(winnerPlayer.Player._SetScore(winnerPlayer.Score));
                            yield return new WaitForSeconds(0.5f);
                        }
                        else
                        {
                            Debug.Log("Round Scoring Tie");
                        }
                        break;
                }
            }


            //Destroying each player hand tiles
            foreach (var player in GamePlayUIPanel.instance.players)
            {
                player.DestroyOldDominoTiles();
            }


            yield return new WaitForSeconds(1);

            Player winner = DecideWinner(currentPlayer);

            //Only for rule 2 and 5
            if (GameRulesManager.wasTheWinnerTied == true && winner == null)
            {
                Debug.Log("Winner tied so play tie breaker. . .");

                Debug.Log("********** Playing Tie Breaker **********");
                yield return new WaitForSeconds(1f);
                List<PlayerScore> tiePlayers = new List<PlayerScore>();
                if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode2)
                {
                    tiePlayers = GameRulesManager.FindLowestScorerTiedPlayers(GamePlayUIPanel.instance.players);
                }
                else
                {
                    tiePlayers = GameRulesManager.FindTiePlayers(playerScoresList);
                }

                //Destroying each player hand tiles
                foreach (var player in GamePlayUIPanel.instance.players)
                {
                    player.DestroyOldDominoTiles();
                }

                yield return StartCoroutine(_PlayTieBreaker(tiePlayers));
                winner = TieBreaker.instance.winnerPlayer.Player;
                GameRulesManager.wasTheWinnerTied = false;
            }

            if (winner != null)
            {
                FinishTheGame(winner);
            }

            else
            {
                Debug.Log("No winner Declared. . .");
                StartCoroutine(_RestartRound());
            }
        }
        else
        {


            if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode3)
            {
                if (TurnSwitch == GamePlayUIPanel.instance.players.Count - 1)
                {
                    Debug.Log("waiting");
                    yield return StartCoroutine(Rule3.PassRunningStepPoints(true));// == true ? "Yes it's a pass running step, rewarding 25 points" : "No pass running step");
                    Debug.Log("wait end");
                }
            }


            Player winner = (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode5) ? null : DecideWinner(currentPlayer);

            if (winner != null)
            {
                FinishTheGame(winner);
            }
            else
            {
                currentPlayer.Shuffle(); //Sort Hand Tile of Current Player
                yield return new WaitForSeconds(1);
                ChangeCollidersState(true);
                if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
                {
                    currentPlayer = GameRulesManager.ReturnNextIndexPlayer(currentPlayer);
                }
                else
                {
                    Debug.Log("Next Index Player: " + TurnTimerController.nextTurnNumber);
                    currentPlayer = GameRulesManager.ReturnNextIndexPlayer(TurnTimerController.nextTurnNumber);
                }

                StartCoroutine(currentPlayer._StartMove());
                TurnTimerController.instance.StartTimer(currentPlayer);
            }
        }
    }

    public IEnumerator _CheckMultiplayerOfFive()
    {
        //Checking Multiple of five
        if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode1)
        {

            TilePossibilities[] tilePossibilities = FindObjectsOfType<TilePossibilities>();

            foreach (var item in tilePossibilities)
            {
                Debug.Log("Finded Possibility: " + item.name, item);
            }

            int score = HighFiveRules.CalculateMultipleOfFive(tilePossibilities);

            if (score > 0)
            {
                //Debug.Log("%%%%%  1  %%%%%%");
                Player player = lastTilePlayedPlayer;
                if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
                {
                    //Debug.Log("%%%%%  2  %%%%%%");
                    if (player.playerPersonalData.playerUserID.Equals(PlayerPersonalData.playerUserID))
                    {
                        //Debug.Log("%%%%%  3  %%%%%%");
                        UpdatePlayerScore highFiveScore = new UpdatePlayerScore
                        {
                            score = score,
                            playerID = player.playerPersonalData.playerUserID
                        };

                        UpdateMessage updateMessage = new UpdateMessage
                        {
                            code = GameUpdates.MultipleOfFive,
                            data = highFiveScore.ToJson()
                        };
                        GameManager.instace.SendMatchStateAsync(OpCodes.UPDATE, updateMessage.ToJson());
                    }
                    else
                    {
                        Debug.Log("It's High Five. ");
                    }
                }
                else
                {
                    yield return StartCoroutine(GiveMultipleOfFiveScore(score, player));
                }
            }
        }
    }

    public IEnumerator GiveMultipleOfFiveScore(int score, Player player)
    {
        Debug.Log("GiveMultipleOfFiveScore");
        yield return StartCoroutine(player.SpawnPlayerScoreBadge(score, GamePlayUIPanel.instance.FinalPopUpPos, player.playerPersonalData.playerRawImage.transform));
        yield return StartCoroutine(player._SetScore(score));
        Player winner = DecideWinner(player);

        //if (lastTilePlayedPlayer.playerPersonalData.playerScore >= GameRulesManager.instance.currentSelectedGameScore)
        if (winner != null)
        {
            FinishTheGame(winner);
        }
    }

    public static void FinishTheGame(Player winner)
    {
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
        {
            if (PlayerPersonalData.playerUserID == winner.playerPersonalData.playerUserID) //Winner will call the event
            {
                List<GameEndPlayers> gameEndPlayers = new List<GameEndPlayers>();

                foreach (var item in instance.tempPlayers)
                {
                    GameEndPlayers endPlayer = new GameEndPlayers
                    {
                        playerScore = item.playerScore,
                        playerUserID = item.playerUserID
                    };
                    gameEndPlayers.Add(endPlayer);
                }

                WinnerData winnerData = new WinnerData(winner.playerPersonalData.playerUserID, gameEndPlayers.ToArray());
                string jsonData = winnerData.ToJson();
                Debug.Log("Winner Data Broadcast: " + jsonData);
                GameManager.instace.SendMatchStateAsync(OpCodes.GAME_ENDED, jsonData);
            }
        }
        else
        {
            instance.GameFinishCallBack(winner);
        }

    }

    public void GameFinishCallBack(Player winner)
    {
        TurnTimerController.instance.ResetTimer(winner);
        Debug.Log("Game Finish Winner is : " + winner.playerPersonalData.playerName, winner);
        GamePlayUIPanel.instance.PopUpController(instance.HeadingPos, winner.playerPersonalData.playerName + " Win");
        GamePlayUIPanel.instance.ResultText.text = "Player " + winner.playerPersonalData.playerSeatID + " Win";

        bool iAmTheWinner = (winner.playerPersonalData.playerUserID == PlayerPersonalData.playerUserID) ? true : false;

        GamePlayUIPanel.instance.SetData(iAmTheWinner, PlayerPersonalData.playerTexture, GameRulesManager.currentSelectedGame_CoinsToPlay, winner);

    }

    List<TilePossibilities> tilePossibilities = new List<TilePossibilities>();

    public void ChangeCollidersState(bool turnCollidersOn)
    {
        StopCoroutine("_TileMovingOnOffColliders");

        StartCoroutine(_TileMovingOnOffColliders(turnCollidersOn));
    }

    public IEnumerator _TileMovingOnOffColliders(bool turnCollidersOn)
    {
        Debug.Log("Changing Collider State: " + turnCollidersOn);
        yield return null;

        if (!turnCollidersOn)
        {
            tilePossibilities = new List<TilePossibilities>(); //Cache
            foreach (var item in FindObjectsOfType<TilePossibilities>())
            {
                tilePossibilities.Add(item);
                item.gameObject.SetActive(turnCollidersOn);
            }
        }
        else
        {
            foreach (var item in tilePossibilities)
            {
                item.gameObject.SetActive(turnCollidersOn);
            }
        }
        currentSelectedCollidersParentTransform.gameObject.SetActive(turnCollidersOn);
    }

    private void ResetTableData()
    {
        if (GameRulesManager.currentSelectedGame_Rule != GameRulesManager.GameRules.GameMode1)
        {
            currentSelectedCollidersParentTransform.gameObject.SetActive(false);
        }

        firstTileValue = 0;

        Debug.Log("New Round Restart");
        foreach (var tile in dominosCurrentList)
        {
            try { Destroy(tile.gameObject); }
            catch (Exception ex) { Debug.LogError("Ex: " + ex); }
        }
        dominosCurrentList.Clear();
        foreach (var tile in addedInMap)
        {
            try { Destroy(tile.gameObject); }
            catch (Exception ex) { Debug.LogError("Ex: " + ex); }
        }
        addedInMap.Clear();
        foreach (var tile in addedInMapOnLeftSide)
        {
            try { Destroy(tile.gameObject); }
            catch (Exception ex) { Debug.LogError("Ex: " + ex); }
        }
        addedInMapOnLeftSide.Clear();
        foreach (var tile in addedInMapOnRightSide)
        {
            try { Destroy(tile.gameObject); }
            catch (Exception ex) { Debug.LogError("Ex: " + ex); }
        }
        addedInMapOnRightSide.Clear();
    }

    public static void ResetStaticFields()
    {
        GameRulesManager.lastRoundWinnerPlayer = null;
        GameRulesManager.forceLeave = false;
        GameRulesManager.wasTheWinnerTied = false;
        GameRulesManager.TurnStarted = false;
        roundNum = 1;
        if (Time.timeScale != 1) Time.timeScale = 1;
    }

    private void FillingDominosList()
    {
        dominosList.AddRange(new List<Domino>()
        {
            new Domino(0, 0), new Domino(0, 1), new Domino(0, 2), new Domino(0, 3), new Domino(0, 4), new Domino(0, 5), new Domino(0, 6),
            new Domino(1, 1), new Domino(1, 2), new Domino(1, 3), new Domino(1, 4), new Domino(1, 5), new Domino(1, 6),
            new Domino(2, 2), new Domino(2, 3), new Domino(2, 4), new Domino(2, 5), new Domino(2, 6),
            new Domino(3, 3), new Domino(3, 4), new Domino(3, 5), new Domino(3, 6),
            new Domino(4, 4), new Domino(4, 5), new Domino(4, 6),
            new Domino(5, 5), new Domino(5, 6),
            new Domino(6, 6)
        });
    }

    private void SpawningAndShufflingTilesInTwoRows()
    {
        ChangeCollidersState(false);
        //Bottom Row

        for (int i = 0; i < 28; i++)
        {
            int j = (i < 14) ? i : (i % 14);
            //Debug.Log("j: " + j);
            int index = 0;
            if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
                index = UnityEngine.Random.Range(0, dominosList.Count);
            else
            {
                index = (int)GameManager.multiplayerTilesOrder[i];
            }

            Vector3 pos = new Vector3(0.975f - (j * 0.15f), 0, (i < 14) ? 0.5f : -0.5f);

            //Tile tile = new Tile(dominosList[index].first, dominosList[index].second);
            Tile tile = Instantiate((dominosList[index].first == dominosList[index].second) ? sameFaceDomino : differentFaceDomino, pos, Quaternion.Euler(90, 0, 0)) as Tile;

            //Set bg image, first and last value of tile
            tile.Populate(dominosList[index].first, dominosList[index].second);

            //Rename Spawn Tile
            tile.gameObject.name = tile.First.ToString() + tile.Second.ToString();

            tile.transform.localScale = Vector3.one;
            dominosCurrentList.Add(tile);
            if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot) dominosList.RemoveAt(index);
        }

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer) dominosList.Clear();
    }

    private IEnumerator _DistributeCardsToPlayerHandsAndBoneYard()
    {
        firstTile = null;
        if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode1)
            HighFiveRules.ResetData();

        List<Player> players = (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot) ? GamePlayUIPanel.instance.players : GamePlayUIPanel.instance.players.OrderBy(n => n.playerPersonalData.playerSeatID).ToList();
        foreach (var player in players)
        {
            if (SoundManager.instance != null) SoundManager.instance.DominoDistributionPlayer(true);


            //7 tiles distribution
            for (int i = 0; i < 7; i++)
            {
                if (roundNum == 1)
                {
                    //detecting first player who have big tile of same face.
                    if (dominosCurrentList[0].SameFace && dominosCurrentList[0].First > firstTileValue) //detecting same face big tile
                    {
                        firstTileValue = dominosCurrentList[0].First; //saving (6,6)
                        firstTile = dominosCurrentList[0]; //saving first tile to start playing (6,6)
                        firstTilePlayer = currentPlayer = player; //first player to play the turn
                        //Server Update
                        //FirstPlayer Update on server
                        //FirstPlayerIndex = currentPlayerIndex
                    }
                }
                player.dominosCurrentList.Add(dominosCurrentList[0]); //adding current tile in player hand list.

                if (player.isMe)
                {
                    spawnUITile(player);
                }

                dominosCurrentList.RemoveAt(0); //remove tile from public domino list.
                GamePlayUIPanel.UpdateBoneYardText(dominosCurrentList.Count);
            }
            player.Shuffle(); // Arranging players hand tiles
            yield return new WaitForSeconds(0.5f);
        }

        //if some tiles left after distribution
        if (dominosCurrentList.Count > 0)
        {
            if (SoundManager.instance != null) SoundManager.instance.BoneyardAudioPlayer(true);

            for (int i = 0; i < dominosCurrentList.Count; i++)
            {
                LeanTween.move(dominosCurrentList[i].gameObject, new Vector3(GamePlayUIPanel.instance.boneYardPlaces[i].position.x, GamePlayUIPanel.instance.boneYardPlaces[i].position.y, GamePlayUIPanel.instance.boneYardPlaces[i].position.z), 0.5f).setDelay(i * 0.04f);
                LeanTween.rotate(dominosCurrentList[i].gameObject, new Vector3(GamePlayUIPanel.instance.boneYardPlaces[i].eulerAngles.x, GamePlayUIPanel.instance.boneYardPlaces[i].eulerAngles.y, GamePlayUIPanel.instance.boneYardPlaces[i].eulerAngles.z), 0.5f).setDelay(i * 0.04f);
                dominosCurrentList[i].isPlayerHandDomino = false;

                dominosCurrentList[i].transform.localScale = Vector3.one;
                //dominosCurrentList[i].transform.eulerAngles = new Vector3(180, 0, 0);
            }
        }
    }

    public void spawnUITile(Player player)
    {
        GameObject uiTileGameObj = Instantiate(uiTilePrefab.gameObject, GamePlayUIPanel.instance.boneyardTileImage, false);
        //uiTileGameObj.transform.position = new Vector3(-1500, 70, 0);
        //uiTileGameObj.transform.rotation = Quaternion.identity;
        UITile uITile = uiTileGameObj.GetComponent<UITile>();
        uITile.SetValues(dominosCurrentList[0].originalSprite, dominosCurrentList[0], player);
        player.dominosCurrentListUI.Add(uITile);

        playerHandScrollRect.horizontalNormalizedPosition = 0;


    }

    public static void RemoveExtraPlaces(int numberOfPlayers)
    {

        foreach (var item in GamePlayUIPanel.instance.players)
        {
            item.gameObject.SetActive(true);
        }

        foreach (var item in SpawnPointManger.instance.points)
        {
            item.gameObject.SetActive(true);
        }

        //if 2 player game selected
        if (numberOfPlayers == 2)
        {
            //Removing UI Points

            //Set 2 
            //Destroy(GamePlayUIPanel.instance.players[1].gameObject);
            GamePlayUIPanel.instance.players[1].gameObject.SetActive(false);
            GamePlayUIPanel.instance.players.RemoveAt(1);

            //Seat 4
            //Destroy(GamePlayUIPanel.instance.players[2].gameObject);
            GamePlayUIPanel.instance.players[2].gameObject.SetActive(false);
            GamePlayUIPanel.instance.players.RemoveAt(2);

            //Removing Spawn Points
            //Player 4 Position
            SpawnPointManger.instance.points[1].gameObject.SetActive(false);
            SpawnPointManger.instance.points.RemoveAt(1);

            //Player 3 Position
            SpawnPointManger.instance.points[2].gameObject.SetActive(false);
            SpawnPointManger.instance.points.RemoveAt(2);

        }

        //if 3 player game selected
        else if (numberOfPlayers == 3)
        {
            //Removing UI Points

            //Seat 4
            //Destroy(GamePlayUIPanel.instance.players[1].gameObject);
            GamePlayUIPanel.instance.players[3].gameObject.SetActive(false);
            GamePlayUIPanel.instance.players.RemoveAt(3);

            //Removing Spawn Points
            //Player 4 Position
            SpawnPointManger.instance.points[3].gameObject.SetActive(false);
            SpawnPointManger.instance.points.RemoveAt(3);
        }
        else
        {
            //Do nothing
            //if 4 players game selected
        }

        //SpawnPointManger.ArrangeSpawnPoints((GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot) ? 0 : GameManager.mySeatIndex);
        //SpawnPointManger.ArrangeProfilesOrder((GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot) ? 0 : GameManager.mySeatIndex);
    }


    //Scoring Functions

    #region scoring

    private IEnumerator _SideTheExtraTilesFromTable()
    {
        foreach (var item in addedInMap)
        {
            item.isFrontFace = false;
            LeanTween.rotateX(item.gameObject, 90, 0.5f);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        int i = 0;
        foreach (var item in addedInMap)
        {
            LeanTween.move(item.gameObject, outsideTransform, 0.3f).setDelay(i * 0.04f);
            i++;
            //yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);
    }

    private List<PlayerScore> CalculateSumOfEachTile()
    {
        List<PlayerScore> playerScores = new List<PlayerScore>();

        Debug.Log("CalculatingSumOfEachPlayer");
        //Sum Score of each player
        foreach (var player in GamePlayUIPanel.instance.players)
        {
            int sumOfPipsInSinglePlayerHandScore = 0;

            foreach (var tile in player.dominosCurrentList)
            {
                sumOfPipsInSinglePlayerHandScore += tile.First;
                sumOfPipsInSinglePlayerHandScore += tile.Second;
            }

            PlayerScore playerScore = new PlayerScore
            {
                Player = player,
                Score = sumOfPipsInSinglePlayerHandScore
            };
            playerScores.Add(playerScore);
        }

        return playerScores;
    }

    private int CalculatingTotalOfAllPlayers(List<PlayerScore> playerScores)
    {
        int total = 0;
        total = playerScores.Sum(x => Convert.ToInt32(x.Score));
        return total;
    }

    public Player DecideWinner(Player player = null)
    {
        //Error here why i am always seeing a current player is a winner or not
        switch (GameRulesManager.currentSelectedGame_Rule)
        {
            case GameRulesManager.GameRules.GameMode1:
                player = HighFiveRules.DecideWinner();
                break;
            case GameRulesManager.GameRules.GameMode2:
                player = Rule2.DecideWinner();
                break;
            case GameRulesManager.GameRules.GameMode3:
                player = Rule3.DecideWinner();
                break;
            case GameRulesManager.GameRules.GameMode4:
                player = Rule4.DecideWinner();
                break;
            case GameRulesManager.GameRules.GameMode5:
                player = Rule5.DecideWinner();
                break;
            case GameRulesManager.GameRules.GameMode6:
                player = Rule6.DecideWinner();
                break;
            default:
                player = null;
                break;
        }
        return player;
    } //Decide Winner End

    #endregion

    private IEnumerator _PlayTieBreaker(List<PlayerScore> tiePlayers)
    {
        //Destroy Old Tiles
        //Distribute one tile tiePlayers
        //Find the smallest amount player
        GamePlayUIPanel.instance.PopUpController(HeadingPos, "Tie in scoring.");
        yield return new WaitForSeconds(1f);
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
        {
            yield return StartCoroutine(TieBreaker.instance.DistributeTilesToTiePlayers(tiePlayers));
        }
        else
        {
            yield return StartCoroutine(TieBreaker.instance.DistributeTilesToTiePlayers_Multiplayer(tiePlayers, GameManager.tieTilesOrder));
        }
    }
}

[Serializable]
public class Domino
{
    public int first;
    public int second;

    public Domino(int _first, int _second)
    {
        first = _first;
        second = _second;

    }
}


public class PlayerScore
{
    public Player Player;
    public int Score;
}