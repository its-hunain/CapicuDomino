using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TieBreaker : MonoBehaviour
{
    public PlayerScore winnerPlayer;
    public GameObject tieBreakerTilePrefab;
    public int score;
    //53,43,32,40
    [Header("Tie Breaker Dominos List")]
    public List<Domino> dominosList = new List<Domino>();

    public static TieBreaker instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FillingTieBreakerDominosList()
    {
        dominosList.Clear();

        dominosList.AddRange(new List<Domino>()
        {
            new Domino(0, 0), new Domino(0, 1),new Domino(0, 2),new Domino(0, 4),
            new Domino(1, 2),new Domino(1, 5),
            new Domino(2, 3),
            new Domino(3, 4),new Domino(3, 6), new Domino(3, 5),
            new Domino(4, 6),
            new Domino(5, 6),
            new Domino(6, 6),
        });
    }

    List<GameObject> newTiles = new List<GameObject>();

    public IEnumerator DistributeTilesToTiePlayers(List<PlayerScore> playerAreas)
    {
        FillingTieBreakerDominosList();

        newTiles = new List<GameObject>();

        List<PlayerScore> playerScores = new List<PlayerScore>();

        for (int i = 0; i < playerAreas.Count; i++)
        {
            int index = Random.Range(0, dominosList.Count);
            //Tile tile = new Tile(dominosList[index].first, dominosList[index].second);

            GameObject tile = null;
            tile = Instantiate(tieBreakerTilePrefab, GamePlayUIPanel.instance.boneYardPlaces[i].transform.position,Quaternion.identity);

            LeanTween.move(tile, playerAreas[i].Player.DisplayTilesPlaces[i].transform, 0.5f);
            LeanTween.rotate(tile, playerAreas[i].Player.DisplayTilesPlaces[i].eulerAngles, 0.5f);

            Tile tileScript = tile.GetComponent<Tile>();

            //Set bg image, first and last value of tile
            tileScript.Populate(dominosList[index].first, dominosList[index].second);
            tileScript.ShowFront();
            int temp = tileScript.First + tileScript.Second;

            PlayerScore PlayerScoreObj = new PlayerScore { Score = temp, Player = playerAreas[i].Player};
            playerScores.Add(PlayerScoreObj);

            //Rename Spawn Tile
            tile.gameObject.name = tileScript.First.ToString() + tileScript.Second.ToString();

            tile.transform.localScale = Vector3.one;
            dominosList.RemoveAt(index);
            yield return new WaitForSeconds(1f);
            newTiles.Add(tile.gameObject);
        }

        yield return new WaitForSeconds(1f);

        winnerPlayer = FindWinnerInTiedPlayer(playerScores);

        if (winnerPlayer == null)
        {
            Debug.LogError("Winner Not Found...");
            winnerPlayer = playerAreas[0];

        }
        foreach (var item in newTiles)
        {
            Destroy(item);
        }
    }

    public IEnumerator DistributeTilesToTiePlayers_Multiplayer(List<PlayerScore> playerAreas , List<int> tieTilesOrder)
    {
        FillingTieBreakerDominosList();

        newTiles = new List<GameObject>();

        List<PlayerScore> playerScores = new List<PlayerScore>();

        for (int i = 0; i < playerAreas.Count; i++)
        {
            int index = tieTilesOrder[i];
            //Tile tile = new Tile(dominosList[index].first, dominosList[index].second);

            GameObject tile = null;
            tile = Instantiate(tieBreakerTilePrefab, GamePlayUIPanel.instance.boneYardPlaces[i].transform.position,Quaternion.identity);

            LeanTween.move(tile, playerAreas[i].Player.DisplayTilesPlaces[i].transform, 0.5f);
            LeanTween.rotate(tile, playerAreas[i].Player.DisplayTilesPlaces[i].eulerAngles, 0.5f);

            Tile tileScript = tile.GetComponent<Tile>();

            //Set bg image, first and last value of tile
            tileScript.Populate(dominosList[index].first, dominosList[index].second);
            tileScript.ShowFront();
            int temp = tileScript.First + tileScript.Second;

            PlayerScore PlayerScoreObj = new PlayerScore { Score = temp, Player = playerAreas[i].Player};
            playerScores.Add(PlayerScoreObj);

            //Rename Spawn Tile
            tile.gameObject.name = tileScript.First.ToString() + tileScript.Second.ToString();

            tile.transform.localScale = Vector3.one;
            dominosList.RemoveAt(index);
            yield return new WaitForSeconds(1f);
            newTiles.Add(tile.gameObject);
        }

        yield return new WaitForSeconds(1f);

        winnerPlayer = FindWinnerInTiedPlayer(playerScores);

        if (winnerPlayer == null)
        {
            Debug.LogError("Winner Not Found...");
            winnerPlayer = playerAreas[0];

        }
        foreach (var item in newTiles)
        {
            Destroy(item);
        }
    }


    PlayerScore FindWinnerInTiedPlayer(List<PlayerScore> playerScores)
    {
        PlayerScore playerScore = null;

        if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode2)
        {
            if (GameRulesManager.wasTheWinnerTied)
            {
                // Retrieve the object with the lowest value from the list using LINQ
                playerScore = playerScores.OrderBy(obj => obj.Score).First();
            }
            else
            {
                //Highest Scorer Player
                //This is a Round Looser player because he get the highest tile.
                playerScore = playerScores.OrderByDescending(obj => obj.Score).First();
            }
        }
        else
        {
            // Retrieve the object with the lowest value from the list using LINQ
            playerScore = playerScores.OrderBy(obj => obj.Score).First();
        }
        return playerScore;

    }
}
