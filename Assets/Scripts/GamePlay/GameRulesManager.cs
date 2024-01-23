using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AvatarBuilder;
using Newtonsoft.Json;
using UnityEngine;

public class GameRulesManager : MonoBehaviour
{
    public GridManager gridManager;
    public static Player lastRoundWinnerPlayer = null;
    public static bool wasTheWinnerTied = false;
    public static bool forceLeave = true;

    public static TournamentDetails matchDetails;

    public enum GameRules
    {
        GameMode1,
        GameMode2,
        GameMode3,
        GameMode4,
        GameMode5,
        GameMode6
    }

    public enum MatchType
    {
        Bot,
        Multiplayer
    }

    public enum GameType
    {
        SingleMatch,
        Tournament
    }



    public static MatchType currentSelectedGame_MatchType = MatchType.Bot;
    public static GameType currentSelectedGame_GameType = GameType.SingleMatch;
    public static GameRules currentSelectedGame_Rule = GameRules.GameMode1;
    public static string currentSelectedGame_RuleDescription = "";
    public static int currentSelectedGame_ScoreToWin = 200; //Dynamic based on game
    public static int currentSelectedGame_CoinsToPlay = 100; //100,200
    public static int noOfPlayers = 3;//2,3,4

    public static GameRulesManager instance;

    public static bool TurnStarted;

    private void Awake()
    {
        forceLeave = true;
        if (instance == null) instance = this;
    }

    private void Start()
    {
        switch (currentSelectedGame_Rule)
        {
            case GameRules.GameMode1:
                currentSelectedGame_ScoreToWin = HighFiveRules.maxScoreToWin;
                break;
            case GameRules.GameMode2:
                currentSelectedGame_ScoreToWin = Rule2.maxScoreToWin;
                break;
            case GameRules.GameMode3:
                currentSelectedGame_ScoreToWin = Rule3.maxScoreToWin;
                break;
            case GameRules.GameMode4:
                currentSelectedGame_ScoreToWin = Rule4.maxScoreToWin;
                break;
            case GameRules.GameMode5:
                currentSelectedGame_ScoreToWin = Rule5.maxScoreToWin;
                break;
            case GameRules.GameMode6:
                currentSelectedGame_ScoreToWin = Rule6.maxScoreToWin;
                break;
        }

        GamePlayUIPanel.instance.UpdateHeadingTexts();

        Debug.Log("*********Current Game Details*********");
        Debug.Log("Game Name: " + currentSelectedGame_Rule);
        Debug.Log("Game Score To Win: " + currentSelectedGame_ScoreToWin);
        Debug.Log("Game noOfPlayers: " + noOfPlayers);
        Debug.Log("Game currentSelectedGameFees: " + currentSelectedGame_CoinsToPlay);
        Debug.Log("Game currentSelectedMatchType: " + currentSelectedGame_MatchType);
        Debug.Log("Game currentSelectedGame_GameType: " + currentSelectedGame_GameType);
        Debug.Log("Game selectedGameCenterID: " + GameCenterSelectionScreen.selectedGameCenterID);
        Debug.Log("*********Current Game Details*********");
    }

    public static Player FindNextRoundPlayer()
    {
        Player player = null;
        switch (currentSelectedGame_Rule)
        {
            case GameRules.GameMode1:
                player = HighFiveRules.FindNextRoundPlayer();
                break;
            case GameRules.GameMode2:
                player = Rule2.FindNextRoundPlayer();
                break;
            case GameRules.GameMode3:
                player = Rule3.FindNextRoundPlayer();
                break;
            case GameRules.GameMode4:
                player = Rule4.FindNextRoundPlayer();
                break;
            case GameRules.GameMode5:
                player = Rule5.FindNextRoundPlayer();
                break;
            case GameRules.GameMode6:
                player = Rule6.FindNextRoundPlayer();
                break;
        }
        
        
        if (!GamePlayUIPanel.instance.players.Contains(player) && GamePlayUIPanel.instance.players.Count > 0)
        {
            Debug.LogError("Error to identify first turn Player, so player 0 will play the first turn.");
            return GamePlayUIPanel.instance.players[0];
        }
        else
        {
            return player;
        }
    }

    public static Tile FindFirstTileForSubSequentRounds(List<Tile> dominosCurrentList)
    {
        int highestValue = 0;

        Tile tile = null;
        var result = dominosCurrentList.Find(x => x.First == x.Second);
        if (result != null) tile = result;
        else
        {
            foreach (var item in dominosCurrentList)
            {
                if ((item.First + item.Second) > highestValue)
                {
                    highestValue = (item.First + item.Second);
                    tile = item;
                }
            }
        }
        return tile;
    }

    /// <summary>
    /// Bot
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static Player ReturnNextIndexPlayer(Player player)
    {
        int index = GamePlayUIPanel.instance.players.IndexOf(player);       //  if 4 player game 0,1,2,3   //   if 3 player game 0,1,2   //   if 2 player game 0,1
                                                                            //  Reset Turn to player1
        if (index == GamePlayUIPanel.instance.players.Count - 1)
        {
            player = GamePlayUIPanel.instance.players[0];
        }
        else
        {
            player = GamePlayUIPanel.instance.players[index + 1];
        }
        return player;
    }

    /// <summary>
    /// Multiplayer
    /// </summary>
    /// <param name="nextTurnIndex"></param>
    /// <returns></returns>
    public static Player ReturnNextIndexPlayer(int nextTurnIndex)
    {
        return GamePlayUIPanel.instance.players[nextTurnIndex];
    }

    internal static List<PlayerScore> FindTiePlayers(List<PlayerScore> playerScores)
    {
        List<PlayerScore> playerScoresListCache = new List<PlayerScore>();

        if (currentSelectedGame_Rule == GameRules.GameMode2)
        {
            playerScoresListCache = playerScores.OrderByDescending(x => x.Score).ToList();
        }
        else
        {
            playerScoresListCache = playerScores.OrderBy(x => x.Score).ToList();
        }

        if (playerScoresListCache[0].Score == playerScoresListCache[1].Score)
        {
            List<PlayerScore> Cache = new List<PlayerScore>();
            foreach (var item in playerScoresListCache)
            {
                if (item.Score == playerScoresListCache[0].Score)
                {
                    Debug.Log("item.Score: "  + item.Score);

                    Cache.Add(item);
                }
            }
            playerScoresListCache = Cache;
        }
        return playerScoresListCache;
    }


    internal static List<PlayerScore> FindLowestScorerTiedPlayers(List<Player> players)
    {
        List<Player> playersCache = new List<Player>();
        List<PlayerScore> playersScoresCache = new List<PlayerScore>();

        playersCache = players.OrderBy(x => x.playerPersonalData.playerScore).ToList(); //Ascending Order

        if (playersCache[0].playerPersonalData.playerScore == playersCache[1].playerPersonalData.playerScore)
        {
            List<PlayerScore> Cache = new List<PlayerScore>();
            foreach (var item in playersCache)
            {
                if (item.playerPersonalData.playerScore == playersCache[0].playerPersonalData.playerScore)
                {
                    Debug.Log("$$$$$$$$$$$$$$$ item.Score: " + item.playerPersonalData.playerScore);
                    PlayerScore playerScore = new PlayerScore
                    {
                        Player = item,
                        Score = item.playerPersonalData.playerScore

                    };
                    Cache.Add(playerScore);
                }
            }
            playersScoresCache = Cache;
        }
        return playersScoresCache;
    }

}

/// <summary>
/// HighFiveRules
/// </summary>
public class HighFiveRules : GameRulesManager
{
    public const int maxScoreToWin = 200; // Worst Looser = 0
    public static Tile firstSpinner = null;

    public static void ResetData()
    {
        firstSpinner = null;
    }


    /// <summary>
    /// Calculate the Round Off Sum.
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static int CalculateRoundEndScoring(int score)
    {
        int obtainedScore = 0;

        if      (score <= 2)                    obtainedScore = 0;
        else if (score >=3 && score <= 7)       obtainedScore = 5;
        else if (score >=8 && score <= 12)      obtainedScore = 10;
        else if (score >=13 && score <= 17)     obtainedScore = 15;
        else if (score >=18 && score <= 22)     obtainedScore = 20;
        else if (score >=23 && score <= 27)     obtainedScore = 25;
        else if (score >=28 && score <= 32)     obtainedScore = 30;
        else if (score >=33 && score <= 37)     obtainedScore = 35;
        else if (score >=38 && score <= 42)     obtainedScore = 40;
        else if (score >=43 && score <= 47)     obtainedScore = 45;
        else if (score >=48 && score <= 52)     obtainedScore = 50;
        else if (score >=53 && score <= 57)     obtainedScore = 55;
        else if (score >=58 && score <= 62)     obtainedScore = 60;
        else if (score >=63 && score <= 67)     obtainedScore = 65;
        else if (score >=68 && score <= 72)     obtainedScore = 70;
        else if (score >=73 && score <= 77)     obtainedScore = 75;
        else if (score >=78 && score <= 82)     obtainedScore = 80;
        else if (score >=83 && score <= 87)     obtainedScore = 85;
        else if (score >=88 && score <= 92)     obtainedScore = 90;
        else if (score >=93 && score <= 97)     obtainedScore = 95;
        else if (score >=98 && score <= 102)    obtainedScore = 100;
        else if (score >=103 && score <= 107)   obtainedScore = 105;
        else if (score >=108 && score <= 112)   obtainedScore = 110;
        else if (score >=113 && score <= 117)   obtainedScore = 115;
        else                                    obtainedScore = 200;

        return obtainedScore;
    }

    /// <summary>
    /// Calculate Score if Round Blocked.
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static KeyValuePair<PlayerScore /*Winner*/, List<PlayerScore>/*Arranged Players Score*/> CalculateScore_RoundBlocked(List<PlayerScore> playerScores)
    {
        playerScores = playerScores.OrderBy(x => x.Score).ToList(); // Acesding order
        List<PlayerScore> cache = new List<PlayerScore>();
        PlayerScore winnerPlayer = null;

        if (playerScores[0].Score == playerScores[1].Score)
        {
            lastRoundWinnerPlayer = GridManager.instance.firstTilePlayer;
            winnerPlayer = playerScores.Find(x => x.Player.playerPersonalData.playerUserID.Equals(lastRoundWinnerPlayer.playerPersonalData.playerUserID));

            Debug.Log("Yes it's a tie in scoring. . . but ignore it because it's high five give the next round 1st turn to: " + lastRoundWinnerPlayer, lastRoundWinnerPlayer);
            //return new KeyValuePair<PlayerScore, List<PlayerScore>>(null,null);
        }
        else
        {
            winnerPlayer = playerScores[0];
            lastRoundWinnerPlayer = winnerPlayer.Player;
        }


        for (int i = 0; i < playerScores.Count; i++)
        {
            int total = playerScores.Where(item => item != playerScores[i]).Sum(item => item.Score) - playerScores[i].Score; //difference between all and my score
            Debug.Log("old total: " + total);
            total = CalculateRoundEndScoring(total);
            Debug.Log("new total: " + total);
            PlayerScore playerScore = new PlayerScore
            {
                Player = playerScores[i].Player,
                Score = total
            };
            cache.Add(playerScore);
        }
        return new KeyValuePair<PlayerScore, List<PlayerScore>>(winnerPlayer, cache);
    }

    /// <summary>
    /// Calculating each Move Sum
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static int CalculateMultipleOfFive(TilePossibilities[] tilePossibilities)
    {
        //Special Case
        int sum = 0;

        Debug.Log("tilePossibilities Length: " + tilePossibilities.Length);

        //Special Case for 1st tile
        // if first tile is 1,4 or 2,3 or 6,4
        if (GridManager.instance.addedInMap.Count == 1)
        {
            Tile tile = GridManager.instance.addedInMap[0];
            sum = (tile.SameFace) ? tile.First * 2 :tile.First + tile.Second;
            Debug.Log("First Tile Score is: " + sum);
        }
        else
        {
            Dictionary<Tile, int> parents = new Dictionary<Tile, int>();
            foreach (var item in tilePossibilities)
            {
                Tile parent = item.GetComponentInParent<Tile>();
                //if (!parents.ContainsKey(parent) && GridManager.instance.firstTile != parent)
                if (!parents.ContainsKey(parent))
                {


                    //if spinner is open then add it's score other wise not add

                    if (parent == firstSpinner && isFirstSpinnerOpen() == false)
                        continue;

                    //Debug.Log("item: " + item.value , item);
                    //Debug.Log("Parent Tile: " + parent.name , parent);

                    int score = 0;
                    if (parent.SameFace)
                    {
                        //add both values
                        score = item.value * 2;
                    }
                    else
                    {
                        score = item.value;
                    }

                    parents.Add(parent, score);
                    sum += score;
                }
            }
        }

        Debug.Log("sum: " + sum);

        if (sum < 5)
            return 0;
        else if (sum % 5 == 0)
            return sum;
        else
            return 0;
    }

    public static Player DecideWinner()
    {
        List<Player> playersCacheOrderDesc = GamePlayUIPanel.instance.players.OrderByDescending(x => x.playerPersonalData.playerScore).ToList(); // OrderByDescending

        if (playersCacheOrderDesc[0].playerPersonalData.playerScore >= currentSelectedGame_ScoreToWin)
        {
            //Find the lowest score
            if (playersCacheOrderDesc[0].playerPersonalData.playerScore == playersCacheOrderDesc[1].playerPersonalData.playerScore)
            {
                Debug.Log("It's a tie to decide a winner, let's play another round. . .");
                return null;
            }
            else
            {
                return playersCacheOrderDesc[0];
            }
        }
        else
        {
            return null;
        }
    }

    public static bool fillFirstSpinner(Tile tile)
    {
        //Debug.Log("fill First Spinner");
        if (firstSpinner == null && tile.SameFace)
        {
            firstSpinner = tile;
            Debug.Log("First Spinner = " + firstSpinner, firstSpinner);
            return true;
        }
        else
            return false;
    }

    public static bool isFirstSpinnerOpen()
    {

        //find open child count
        bool isFirstSpinnerOpen = false;
        foreach (var item in firstSpinner.GetComponentsInChildren<TilePossibilities>(false))
        {
            if (item.directionOfTile == "Left" || item.directionOfTile == "Right")
            {
                isFirstSpinnerOpen = true;
                break;
            }
        }
        return isFirstSpinnerOpen;       
    }

    /// <summary>
    /// AntiClockwise always
    /// </summary>
    /// <returns></returns>
    public new static Player FindNextRoundPlayer()
    {
        Player player = null;

        if (lastRoundWinnerPlayer != null)
        {
            player = lastRoundWinnerPlayer;
        }
        else
        {
            if (GridManager.instance.wasLastRoundTie)
            {
                player = GridManager.instance.firstTilePlayer;
            }
            else
            {
                Debug.Log("Some Error here on detecting first turn player. . .");
                player = null;
            }
        }
        return player;
    }

}


/// <summary>
/// Regla No 2
/// </summary>
public class Rule2 : GameRulesManager
{
    public const int maxScoreToWin = 121; // Worst Looser = 121

    /// <summary>
    /// Calculate Score if Round Blocked.
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static PlayerScore/*Winner*/ CalculateScore_RoundBlocked(List<PlayerScore> playerScores)
    {
        List<PlayerScore> playerScoresListCache = playerScores.OrderByDescending(x => x.Score).ToList(); // Descesding order
        PlayerScore loserPlayer = new PlayerScore { Player = null,Score = 0 };

        //if tie in scoring
        if (playerScoresListCache[0].Score == playerScoresListCache[1].Score)
        {

            int total = playerScoresListCache.Sum(item => item.Score); //sum of all players points
            loserPlayer.Score = total;
            Debug.Log("Yes it's a tie in scoring. . . ");
        }
        else
        {
            loserPlayer = playerScoresListCache[0];
            int total = playerScoresListCache.Sum(item => item.Score); //sum of all players points
            loserPlayer.Score = total;
        }
        return loserPlayer;
    }

    /// <summary>
    /// Find Highest scorer
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static Player/*Winner*/ FindHighestScorer(List<PlayerScore> playerScoresList)
    {
        Debug.Log("it's Rule2 so finding highest scorer..");
        List<PlayerScore> playerScoresListCache = playerScoresList.OrderByDescending(x => x.Score).ToList(); //Descending order
        return playerScoresListCache[0].Player;
    }

    /// <summary>
    /// Find Lowest scorer
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static PlayerScore/*Winner*/ FindLowestScorer(List<PlayerScore> playerScoresList)
    {
        Debug.Log("it's Rule2 so finding lowest scorer because he will be the winner..");
        List<PlayerScore> playerScoresListCache = playerScoresList.OrderBy(x => x.Score).ToList(); //Ascending order
        return playerScoresListCache[0];
    }


    public static Player DecideWinner()
    {
        List<Player> playersCacheOrderDesc = GamePlayUIPanel.instance.players.OrderByDescending(x => x.playerPersonalData.playerScore).ToList(); // Descesding order
        List<Player> playersCacheOrderAsc = GamePlayUIPanel.instance.players.OrderBy(x => x.playerPersonalData.playerScore).ToList(); // Descesding order

        if(playersCacheOrderDesc[0].playerPersonalData.playerScore >= currentSelectedGame_ScoreToWin)
        {
            //Find the lowest score
            if (playersCacheOrderAsc[0].playerPersonalData.playerScore == playersCacheOrderAsc[1].playerPersonalData.playerScore)
            {
                Debug.Log("It's a tie to decide a winner, let's play another round. . .");
                wasTheWinnerTied = true;
                return null;
            }
            else
            {
                return playersCacheOrderAsc[0];
            }
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// AntiClockwise always
    /// </summary>
    /// <returns></returns>
    public new static Player FindNextRoundPlayer()
    {
        Player player = ReturnNextIndexPlayer(GridManager.instance.firstTilePlayer);
        return player;
    }
}


/// <summary>
/// Regla No 3
/// Same as Rule 4
/// </summary>
public class Rule3 : GameRulesManager
{
    public const int maxScoreToWin = 100; // Worst Looser = 0
    public const int passRunningPoint = 25;
    public const int startPointScore = 25;

    // <summary>
    //  Locked: In case of blocking or closing, the player who has the least amount of points will be the winning player.
    //  In case they add the same number of points in the block, the player who started in that hand will be the winner and will be the one who starts in the next hand.
    //  The hand player is the one who starts and the player to his right would be the subsequent hand player. 
    // </summary>
    /// <param name="playerScoresList"></param>
    /// <returns></returns>
    public static PlayerScore/*Winner*/ CalculateScore_RoundBlocked(List<PlayerScore> playerScores)
    {
        List<PlayerScore> playerScoresListCache = playerScores.OrderBy(x => x.Score).ToList(); // Acesding order
        PlayerScore winnerPlayer = null;

        //if tie in scoring
        if (playerScoresListCache[0].Score == playerScoresListCache[1].Score)
        {
            lastRoundWinnerPlayer = GridManager.instance.firstTilePlayer;
            winnerPlayer = playerScoresListCache.Find(x => x.Player.playerPersonalData.playerUserID.Equals(lastRoundWinnerPlayer.playerPersonalData.playerUserID));
            Debug.Log("Yes it's a tie in scoring. . . but ignore it because it's Rule3 so give the next round 1st turn to: " + lastRoundWinnerPlayer.playerPersonalData.playerName, lastRoundWinnerPlayer);
        }
        else
        {
            winnerPlayer = playerScoresListCache[0];
            lastRoundWinnerPlayer = winnerPlayer.Player;
        }
        //int total = playerScoresList.Sum(item => item.Score); //sum of all players points
        int total = playerScoresListCache.Where(item => item != playerScoresListCache[0]).Sum(item => item.Score); //sum of opponent points
        winnerPlayer.Score = total;
        return winnerPlayer;
    }

    // <summary>
    //  Pass Running Step: 
    //  If at any one time a player takes a running step
    //  (Since none of the other players have the pieces to play), the player who gave that running pass is added 25 points according to the rules.
    // </summary>
    #region Pass Running Points
    public static IEnumerator PassRunningStepPoints(bool scoreAwarded = false)
    {
        //if (GridManager.instance.lastTilePlayedPlayer == GridManager.instance.currentPlayer)
        if (scoreAwarded)
        {
            yield return instance.StartCoroutine(_GivePassRunningPointsToUser());
            Debug.Log("Yes it's a pass running step, rewarding 25 points");
        }
        else
        {
            Debug.Log("No pass running step");
        }
    }

    static IEnumerator _GivePassRunningPointsToUser()
    {
        Player lastTilePlayedPlayer = GridManager.instance.lastTilePlayedPlayer;
        Debug.Log("Rule 3: Pass Runnning Points to: " + lastTilePlayedPlayer);
        GamePlayUIPanel.instance.PopUpController("Pass Running Points" , lastTilePlayedPlayer.playerPersonalData.playerTexture);

        yield return instance.StartCoroutine(lastTilePlayedPlayer.SpawnPlayerScoreBadge(passRunningPoint, GamePlayUIPanel.instance.FinalPopUpPos, lastTilePlayedPlayer.playerPersonalData.playerRawImage.transform));
        yield return instance.StartCoroutine(lastTilePlayedPlayer._SetScore(passRunningPoint));
        //Player winner = GridManager.instance.DecideWinner(currentPlayer);
        //if (winner != null)
        //{
        //    GridManager.FinishTheGame(winner);
        //    Debug.LogError("Winner Found.");
        //}

    }
    #endregion Pass Running Points

    // <summary>
    //  Start Step:
    //  At the moment of placing the first chip, the immediate opponent cannot play, the player who left is added 25 points.
    // </summary>
    #region Start Points
    public static bool FirstImmediatePassPoints()
    {
        if (GridManager.instance.addedInMap.Count == 1 && GridManager.instance.TurnSwitch == 1) // i skip the turn and i am the immediate opponent
        {
            instance.StartCoroutine(_FirstImmediatePassPointsToWinner());
            return true;
        }
        else return false;
    }

    static IEnumerator _FirstImmediatePassPointsToWinner()
    {
        Player currentPlayer = GridManager.instance.currentPlayer;
        Player lastTilePlayedPlayer = GridManager.instance.lastTilePlayedPlayer;
        Debug.Log("Rule 3: Immediate Pass Point to: " + lastTilePlayedPlayer);
        GamePlayUIPanel.instance.PopUpController("Start Points" , lastTilePlayedPlayer.playerPersonalData.playerTexture);

        yield return instance.StartCoroutine(lastTilePlayedPlayer.SpawnPlayerScoreBadge(startPointScore, currentPlayer.playerPersonalData.playerRawImage.GetComponent<RectTransform>(), lastTilePlayedPlayer.playerPersonalData.playerRawImage.transform));
        yield return instance.StartCoroutine(lastTilePlayedPlayer._SetScore(startPointScore));
    }
    #endregion Pass Running Points

    /// <summary>
    /// Check Win State
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static Player DecideWinner()
    {
        List<Player> playersCacheOrderDesc = GamePlayUIPanel.instance.players.OrderByDescending(x => x.playerPersonalData.playerScore).ToList(); // Descesding order

        if (playersCacheOrderDesc[0].playerPersonalData.playerScore >= currentSelectedGame_ScoreToWin)
        {
            //Find the lowest score
            if (playersCacheOrderDesc[0].playerPersonalData.playerScore == playersCacheOrderDesc[1].playerPersonalData.playerScore)
            {
                Debug.Log("It's a tie to decide a winner, let's play another round. . .");
                return null;
            }
            else
            {
                return playersCacheOrderDesc[0];
            }
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Winner Decided:             On subsequent hands, the winner of the previous hand starts
    /// Locked (Winner Decided):    On subsequent hands, the winner of the previous hand starts
    /// Locked (Winner Tied):       The player who started in that hand will be the winner and will be the one who starts in the next hand.
    /// </summary>
    /// <returns></returns>
    public new static Player FindNextRoundPlayer()
    {
        Player player = null;

        if (lastRoundWinnerPlayer != null)
        {
            player = lastRoundWinnerPlayer;
        }
        else
        {
            if (GridManager.instance.wasLastRoundTie)
            {
                player = GridManager.instance.firstTilePlayer;
            }
            else
            {
                Debug.Log("Some Error here. . .");
                player = null;
            }
        }
        return player;
    }
}


/// <summary>
// * Rule 4
// * How to play:  
// * the player who has the double six starts the first game (double 5, double 4, double 3, double 2, double 1, if there is no double 6).. The game turns to the right. 
// * On subsequent hands, the winner of the previous hand starts, regardless of whether they come up with a double or not.
// * In the event that at the start of the hand no double domino player, the player with the highest domino comes out, this only applies to 2 or 3 players
    
// * Shoe to Shoemaker: 
// * It is called "shoemaker" when the winning person reaches 20 points while the opposite does not reach any. 
// * (make a distinctive sound when this happens). The game will restart in case that a player draws 5 or more doubles. 
/// </summary>
public class Rule4 : GameRulesManager
{
    public const int passPoint = 2;
    public const int maxScoreToWin = 20; // Worst Looser = 0
    public static bool canLastTilePlayedOnBothSides = false;

    // <summary>
    //  Scoring: 
    //  The sum of all points is awarded to the player who ran out of chips in his hand.
    //  The points are given by the sum of all the chips scored as follows: from 0 to 2 pints 1 point, 3 to 20, 2 points; From 21 to 30, 3 points; from 31 to 40, 4 points and so on. 
    //  The person who reaches 20 points first wins.
    // </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static int CalculateRoundEndScoring(int winnerScore)
    {
        Debug.Log(winnerScore);
        if (winnerScore >= 0 && winnerScore < 3) winnerScore = 1;                   // from 0 to 2 pints 1 point
        else if (winnerScore >= 3 && winnerScore <= 20) winnerScore = 2;            // 3 to 20, 2 points
        else if (winnerScore >= 21 && winnerScore <= 30) winnerScore = 3;           // 21 to 30, 3 points
        else if (winnerScore >= 31 && winnerScore <= 40) winnerScore = 4;           // from 31 to 40, 4 points
        else if (winnerScore >= 41 && winnerScore <= 50) winnerScore = 5;           // from 41 to 50, 5 points
        else if (winnerScore >= 51 && winnerScore <= 60) winnerScore = 6;           // from 51 to 60, 6 points
        else if (winnerScore >= 61 && winnerScore <= 70) winnerScore = 7;           // from 61 to 70, 7 points
        else if (winnerScore >= 71 && winnerScore <= 80) winnerScore = 8;           // from 71 to 80, 8 points
        else if (winnerScore >= 81 && winnerScore <= 90) winnerScore = 9;           // from 81 to 90, 9 points
        else if (winnerScore >= 91 && winnerScore <= 100) winnerScore = 10;         // from 91 to 100, 10 points
        else if (winnerScore >= 101 && winnerScore <= 110) winnerScore = 11;        // from 101 to 110, 11 points
        else if (winnerScore >= 111 && winnerScore <= 120) winnerScore = 12;        // from 111 to 120, 12 points
        else if (winnerScore >= 121 && winnerScore <= 130) winnerScore = 13;        // from 121 to 130, 13 points
        else if (winnerScore >= 131 && winnerScore <= 140) winnerScore = 14;        // from 131 to 140, 14 points
        else if (winnerScore >= 141 && winnerScore <= 150) winnerScore = 15;        // from 141 to 150, 15 points
        else if (winnerScore >= 151 && winnerScore <= 160) winnerScore = 16;        // from 151 to 160, 16 points
        else if (winnerScore >= 161 && winnerScore <= 170) winnerScore = 17;        // from 161 to 170, 17 points
        else if (winnerScore >= 171 && winnerScore <= 180) winnerScore = 18;        // from 171 to 180, 18 points
        else winnerScore = 19;

        //Capicua
        //double the score if last tile can be played on either both sides 
        if (canLastTilePlayedOnBothSides)
        {
            Debug.Log("It was a capicua, so doubling the given score.");
            winnerScore *= 2;
            canLastTilePlayedOnBothSides = false;
        }
        return winnerScore; 
    }

    //  ---Screen Saver

    // <summary>
    //  Locked: in the event of a locked, the points are counted individually and the player with the least amount of points wins. 
    //  Tie: In case they add the same number of points in the block, the player who started in that hand will be the winner and will be the one who starts in the next hand.
    //  The player of the hand is the one who starts and the player to his right.would be the next player in the hand.
    // </summary>
    /// <param name="playerScoresList"></param>
    /// <returns></returns>
    public static PlayerScore/*Winner*/ CalculateScore_RoundBlocked(List<PlayerScore> playerScoresList)
    {
        playerScoresList = playerScoresList.OrderBy(x => x.Score).ToList(); // Ascending order
        List<PlayerScore> cache = new List<PlayerScore>();
        PlayerScore winnerPlayer = null;

        //if tie in scoring
        if (playerScoresList[0].Score == playerScoresList[1].Score)
        {
            lastRoundWinnerPlayer = GridManager.instance.firstTilePlayer;
            winnerPlayer = playerScoresList.Find(x => x.Player.playerPersonalData.playerUserID.Equals(lastRoundWinnerPlayer.playerPersonalData.playerUserID));

            Debug.Log("Yes it's a tie in scoring. . . but ignore it because it's Rule4 give the next round 1st turn to: " + lastRoundWinnerPlayer, lastRoundWinnerPlayer);
        }
        else
        {
            winnerPlayer = playerScoresList[0];
            lastRoundWinnerPlayer = winnerPlayer.Player;
        }
        //int total = playerScoresList.Sum(item => item.Score); //sum of all players points
        int total = playerScoresList.Where(item => item != playerScoresList[0]).Sum(item => item.Score); //sum of opponent points
        total = CalculateRoundEndScoring(total);
        winnerPlayer.Score = total;
        return winnerPlayer;
    }

    /// <summary>
    ///  Capicúa: 
    ///     it is used in the Capicúa modality. If a player places his last piece, and it can be placed from both ends, he is said to be Capicúa,
    ///     and double the points earned on that hand are added to The person who made the Capicúa.If the token is double, then there is no capicúa.
    /// </summary>
    public static bool CheckCapicua(int numOfPossibilities)
    {
        if (numOfPossibilities >= 2)
        {

            Debug.Log("it's a Capicua");
            GamePlayUIPanel.instance.PopUpController("Capicua" , GridManager.instance.currentPlayer.playerPersonalData.playerTexture);
            return true;
        }
        else
        {
            Debug.Log("it's not a Capicua");
            return false;
        }
    }

    // <summary>
    //  Pass Points: 
    //  Each pass is 2 points, but when playing with less than 4 players, the player who has no chips must grab the ones that are free.At the end of the chips that are free, 
    //  if a player automatically passes, 2 points will be added to the player who made his opponent pass.
    // </summary>
    #region Pass Points
    public static bool CheckPassPoints()
    {
        if (GridManager.instance.lastTilePlayedPlayer != GridManager.instance.currentPlayer)
        {
            instance.StartCoroutine(_GivePassPointsToUser());
            return true;
        }
        else return false;
    }

    static IEnumerator _GivePassPointsToUser()
    {
        Player currentPlayer = GridManager.instance.currentPlayer;
        Player lastTilePlayedPlayer = GridManager.instance.lastTilePlayedPlayer;
        Debug.Log("Rule 4: Pass Points to: " + lastTilePlayedPlayer);
        GamePlayUIPanel.instance.PopUpController("Pass Points" , lastTilePlayedPlayer.playerPersonalData.playerTexture);

        yield return instance.StartCoroutine(lastTilePlayedPlayer.SpawnPlayerScoreBadge(passPoint, currentPlayer.playerPersonalData.playerRawImage.GetComponent<RectTransform>(), lastTilePlayedPlayer.playerPersonalData.playerRawImage.transform));
        yield return instance.StartCoroutine(lastTilePlayedPlayer._SetScore(passPoint));
    }
    #endregion Pass Points

    /// <summary>
    /// Check Win State
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static Player DecideWinner()
    {
        List<Player> playersCacheOrderDesc = GamePlayUIPanel.instance.players.OrderByDescending(x => x.playerPersonalData.playerScore).ToList(); // Descending order

        if (playersCacheOrderDesc[0].playerPersonalData.playerScore >= currentSelectedGame_ScoreToWin)
        {
            //Find the lowest score
            if (playersCacheOrderDesc[0].playerPersonalData.playerScore == playersCacheOrderDesc[1].playerPersonalData.playerScore)
            {
                Debug.Log("It's a tie to decide a winner, let's play another round. . .");
                return null;
            }
            else
            {
                CheckShoeMaker(playersCacheOrderDesc);
                return playersCacheOrderDesc[0];
            }
        }
        else
        {
            return null;
        }
    }


    public static void CheckShoeMaker(List<Player> playersCacheOrderDesc)
    {
        //ignoring the first top scorer playersCacheOrderDesc[0]
        for (int i = 1; i < playersCacheOrderDesc.Count; i++)
        {
            if (playersCacheOrderDesc[i].playerPersonalData.playerScore <= 0)
            {
                SoundManager.instance.ShoeMakerSound(true);
                GamePlayUIPanel.instance.PopUpController(GridManager.instance.HeadingPos, "Shoe Maker");
                break;
            }           
        }
    }
    /// <summary>
    /// Winner Decided:             On subsequent hands, the winner of the previous hand starts
    /// Locked (Winner Decided):    On subsequent hands, the winner of the previous hand starts
    /// Locked (Winner Tied):       The player who started in that hand will be the winner and will be the one who starts in the next hand.
    /// </summary>
    /// <returns></returns>
    public new static Player FindNextRoundPlayer()
    {
        Player player = null;

        if (lastRoundWinnerPlayer != null)
        {
            player = lastRoundWinnerPlayer;
        }
        else
        {
            if (GridManager.instance.wasLastRoundTie)
            {
                player = GridManager.instance.firstTilePlayer;
            }
            else
            {
                Debug.Log("Some Error here. . .");
                player = null;
            }
        }
        return player;
    }

}


/// <summary>
/// Regla No 5
/// </summary>
public class Rule5 : GameRulesManager
{
    public const int maxScoreToWin = 0; // Worst Looser = 10


    // <summary>
    //   Locked: in the event of a locked, the points are counted individually and the player with the least amount of points wins.
    //  In the event of a tie, the players who are tied must draw 1 chip each and the player with the fewest pints wins the game.
    // </summary>
    /// <param name="playerScoresList"></param>
    /// <returns></returns>
    public static PlayerScore/*Winner*/ CalculateScore_RoundBlocked(List<PlayerScore> playerScores)
    {
        List<PlayerScore> playerScoresListCache = playerScores.OrderBy(x => x.Score).ToList(); // Acesding order
        PlayerScore winnerPlayer = null;

        //if tie in scoring, Play the tie breaker
        if (playerScoresListCache[0].Score == playerScoresListCache[1].Score)
        {
            Debug.Log("it's a tie");
            winnerPlayer = null;
            lastRoundWinnerPlayer = null;
        }
        else
        {
            winnerPlayer = playerScoresListCache[0];
            lastRoundWinnerPlayer = winnerPlayer.Player;
        }
        return winnerPlayer;
    }

    /// <summary>
    /// Check Win State
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static Player DecideWinner()
    {
        //Asc Order
        Player[] playersCache = GamePlayUIPanel.instance.players.OrderBy(x => x.playerPersonalData.playerScore).ToArray();

        //foreach (var item in GamePlayUIPanel.instance.players)
        //{
        //    if (item.dominosCurrentList.Count == 0) { return item; }
        //}

        if (playersCache[0].playerPersonalData.playerScore >= currentSelectedGame_ScoreToWin)
        {
            //Tie, Play Tie Breaker
            if (playersCache[0].playerPersonalData.playerScore == playersCache[1].playerPersonalData.playerScore)
            {
                Debug.Log("It's a tie to decide a winner, let's play another round. . .");
                wasTheWinnerTied = true;
                return null;
            }
            else
            {
                return playersCache[0];
            }
        }
        else
        {
            return null; 
        }
    }

    /// <summary>
    /// Winner Decided:             On subsequent hands, the winner of the previous hand starts
    /// Locked (Winner Decided):    On subsequent hands, the winner of the previous hand starts
    /// Locked (Winner Tied):       The player who started in that hand will be the winner and will be the one who starts in the next hand.
    /// </summary>
    /// <returns></returns>
    public new static Player FindNextRoundPlayer()
    {
        Player player = null;

        if (lastRoundWinnerPlayer != null)
        {
            player = lastRoundWinnerPlayer;
        }
        else
        {
            if (GridManager.instance.wasLastRoundTie)
            {
                player = GridManager.instance.firstTilePlayer;
            }
            else
            {
                Debug.Log("Some Error here. . .");
                player = null;
            }
        }
        return player;
    }
}


/// <summary>
/// Regla No 6
///  Max Score /// maxScoreToWin = 75

///  Score: // The sum of all points is awarded to the player who ran out of chips in his hand.
///  Round Block /// Locked: the lock, that is, the player who has the least is the winner of the round and he will score the points of the remaining chips.

///  Round Tie  /// In case of a tie, none of the players will write the value of the bar and a 0 (zero)
///                 will be placed in the score so as not to alter the points already earned and keep the score of games played.

///  First Turn For Next Round /// The player to the right of the one who wins in the previous round will start with any domino he chooses.
/// </summary>
public class Rule6 : GameRulesManager
{
    public const int maxScoreToWin = 75; // Worst Looser = 0
    public static bool WasLastTileDoubleZero = false;

    /// <summary>
    /// The player to the right of the one who wins in the previous round will start with any domino he chooses.
    /// </summary>
    /// <returns></returns>
    public new static Player FindNextRoundPlayer()
    {
        Player player = null;


        if (lastRoundWinnerPlayer != null)
        {
            player = ReturnNextIndexPlayer(lastRoundWinnerPlayer);
        }
        else
        {
            if (GridManager.instance.wasLastRoundTie)
            {
                player = GridManager.instance.firstTilePlayer;
            }
            else
            {
                Debug.Log("Some Error here. . .");
                player = null;
            }
        }
        return player;
    }

    // <summary>
    ///  Score: // The sum of all points is awarded to the player who ran out of chips in his hand.
    ///  Round Block /// Locked: the lock, that is, the player who has the least is the winner of the round and he will score the points of the remaining chips.

    ///  Round Tie  /// In case of a tie, none of the players will write the value of the bar and a 0 (zero)
    ///                 will be placed in the score so as not to alter the points already earned and keep the score of games played.
    // </summary>
    /// <param name="playerScoresList"></param>
    /// <returns></returns>
    public static PlayerScore/*Winner*/ CalculateScore_RoundBlocked(List<PlayerScore> playerScores)
    {
        List<PlayerScore> playerScoresListCache = playerScores.OrderBy(x => x.Score).ToList(); // Acesding order
        PlayerScore winnerPlayer = null;

        //if tie in scoring
        if (playerScoresListCache[0].Score == playerScoresListCache[1].Score)
        {
            Debug.Log("it's a tie");
            GamePlayUIPanel.instance.PopUpController(GridManager.instance.HeadingPos, "Tie, No Score Distribution");
            lastRoundWinnerPlayer = GridManager.instance.lastTilePlayedPlayer;
            winnerPlayer = playerScoresListCache.Find(x => x.Player.playerPersonalData.playerUserID.Equals(lastRoundWinnerPlayer.playerPersonalData.playerUserID));
            winnerPlayer.Score = 0;
        }
        else
        {
            winnerPlayer = playerScoresListCache[0];
            lastRoundWinnerPlayer = winnerPlayer.Player;
            winnerPlayer.Score = playerScoresListCache.Where(item => item != playerScoresListCache[0]).Sum(item => item.Score); //sum of opponent points
        }

        return winnerPlayer;
    }

    /// <summary>
    /// Check Win State
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static Player DecideWinner()
    {
        List<Player> playersCacheOrderDesc = GamePlayUIPanel.instance.players.OrderByDescending(x => x.playerPersonalData.playerScore).ToList(); // Descending Order

        if (playersCacheOrderDesc[0].playerPersonalData.playerScore >= currentSelectedGame_ScoreToWin)
        {
            //Find the lowest score
            if (playersCacheOrderDesc[0].playerPersonalData.playerScore == playersCacheOrderDesc[1].playerPersonalData.playerScore)
            {
                Debug.Log("It's a tie to decide a winner, let's play another round. . .");
                return null;
            }
            else
            {
                return playersCacheOrderDesc[0];
            }
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    ///  it is used in the Sabaniao modality. If a player places his last piece being double 0, and double the points earned on that hand are added to the winner.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    internal static bool CheckSabanio(Tile tile)
    {
        if ((tile.First + tile.Second) == 0 && tile.SameFace)
        {
            GamePlayUIPanel.instance.PopUpController("It's Sabanio" , GridManager.instance.currentPlayer.playerPersonalData.playerTexture);
            return true;
        }
        else
            return false;
    }
}