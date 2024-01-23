using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetBuilder;
using AvatarBuilder;
using Dominos;
using Nakama.TinyJson;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Contain Player Properties and Behavior.
/// Properties
///     0) PlayerID
///     1) Player Type (Bot/Real)
///     2) Hand Orientation
///     3) Play Turn
///     4) Hand Tiles List
///     5) Hand Tiles Places
///     6) Score
/// Behaviors
///     1) Arranging Hand Tiles after each turn
///     2) Set Score
///     3) Play Turn
/// </summary>
public class Player : MonoBehaviour
{
    public GameObject maleBot;
    public GameObject femaleBot;

    public Button profileStatesBtn;

    [SerializeField] public PlayerData playerPersonalData; //Assigned in Inspector
    [SerializeField] public BotData BotRandomData; //Assigned in Inspector

    [Space]
    public bool isMe; //Assigned in Inspector
    public bool isRotate; //Assigned in Inspector
    public bool AutoPlay = false;
    [Space]
    public Image timmerImage;   //Assigned in Inspector
    public Text tilesCount_Txt; //Assigned in Inspector
    public List<Transform> Places = new List<Transform>();              //Assigned in Inspector
    public List<Transform> UIPlaces = new List<Transform>();              //Assigned in Inspector
    public List<Transform> DisplayTilesPlaces = new List<Transform>();  //Assigned in Inspector
    [Space]
    public List<Tile> dominosCurrentList = new List<Tile>();
    public List<UITile> dominosCurrentListUI = new List<UITile>();

    private bool Playable = false;

    public PlayerPhysicalPosition playerPhysicalPosition;

    [Header("Current Tile and Possibility")]
    public Tile currentPlacedTile = null;
    public Tile currentSelectedTile = null;
    public UITile currentSelected_UI_Tile = null;

    WebServiceManager webServiceManager;

    private IEnumerator Start()
    {

        profileStatesBtn.onClick.AddListener(()=> SetDataToPopUp());

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
        {
            if (webServiceManager == null) //because webServiceManager is on don't destroy, so we have to find it...
            {
                webServiceManager = FindObjectOfType<WebServiceManager>();
            }

            if (!isMe) //Bot Profile
            {
                int randValue = Random.Range(0, 2); // 0 = Female, 1 = Male
                RandomBotGenerator(randValue);
                PlayerStates playerStates = new PlayerStates();
                playerStates.GenerateDummyData();
                //WebServiceManager.instance.FindFlagSprite(playerStates.playerFlagShortCode, playerStates.flagSprite, playerPersonalData.flagImage);

                yield return new WaitForSeconds(2f);
                SetDataToProfile(playerStates, "bot:"+playerPersonalData.playerSeatID, BotRandomData.currentBotName, BotRandomData.currentBotPic);
            }
            else
            {
                if (string.IsNullOrEmpty(PlayerPersonalData.playerUserID))
                {
                    Debug.LogError("User not Found. Login Again.");
                    GamePlayWaitingPopUp.instance.SetData("User Not Found. Login Again." , false);
                    GamePlayWaitingPopUp.instance.EnableDisable(true);
                    yield break;
                }

                SetDataToProfile(PlayerPersonalData.playerStates, PlayerPersonalData.playerUserID, PlayerPersonalData.playerName, PlayerPersonalData.playerTexture);
            }
        }
    }



    public void SetDataToProfile(PlayerStates playerStates, string userID, string name, Texture2D texture2D = null)
    {
        playerPersonalData.playerUserID = userID;
        playerPersonalData.playerName = name;
        playerPersonalData.playerStates = playerStates;
        playerPersonalData.playerNameText.text = name.Split(' ')[0];
        if (texture2D != null) playerPersonalData.playerRawImage.texture = playerPersonalData.playerTexture = texture2D;
        else playerPersonalData.playerRawImage.texture = playerPersonalData.playerTexture = BotRandomData.malePics[0];
        playerPersonalData.playerClassSprite = playerStates.classSprite;
        if (userID.Contains("bot"))
            playerPersonalData.flagImage.sprite = playerPersonalData.playerFlagSprite = playerStates.flagSprite;
        else
            StartCoroutine(_GetFlag(playerStates.playerFlagShortCode));
    }


    public void SetDataToProfile(PlayerStates playerState, string userID, string name, string avatarURL = null)
    {
        playerPersonalData.playerStates = playerState;
        playerPersonalData.playerUserID = userID;
        playerPersonalData.playerName = name;
        playerPersonalData.playerNameText.text = name.Split(' ')[0];
        StartCoroutine(_GetTexture(avatarURL));
        StartCoroutine(_GetFlag(playerState.playerFlagShortCode));
        //playerPersonalData.flagImage.sprite = playerPersonalData.playerFlagSprite = playerStates.flagSprite;
        playerPersonalData.playerClassSprite = playerState.classSprite;
    }


    IEnumerator _GetTexture(string url)
    {
        Debug.Log("Downloading. . . ");
        if (string.IsNullOrEmpty(url))
        {
            playerPersonalData.playerRawImage.texture = playerPersonalData.playerTexture = BotRandomData.malePics[0];
            yield break;
        }

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D texture2D = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (texture2D != null) playerPersonalData.playerRawImage.texture = playerPersonalData.playerTexture = texture2D;
            else playerPersonalData.playerRawImage.texture = playerPersonalData.playerTexture = BotRandomData.malePics[0];

            //remotePlayerNetworkData.UserTexture = await DownLoadPicture(remotePlayerNetworkData.AvatarURL);
            Debug.Log("Downloaded. . . ");
        }
    }



    IEnumerator _GetFlag(string flagShortCode)
    {
        Debug.Log("Downloading. . . Flag: " + flagShortCode);

        //https://flagcdn.com/h20/af.jpg
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://flagcdn.com/h20/" + flagShortCode.ToLower() + ".jpg");
        //UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://countryflagsapi.com/png/" + flagShortCode);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D texture2D = ((DownloadHandlerTexture)www.downloadHandler).texture;

            Sprite sprite = Sprite.Create((Texture2D)texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100.0f);

            playerPersonalData.flagImage.sprite = playerPersonalData.playerFlagSprite = playerPersonalData.playerStates.flagSprite = sprite;
            Debug.Log("Downloaded. . . ");
        }
    }



    public IEnumerator SpawnPlayerScoreBadge(int score , RectTransform parentTransform , Transform targetTransform)
    {

        GameObject Badge = Instantiate(GamePlayUIPanel.instance.ScoreBadgePrefab , parentTransform , false);

        if (!Badge.activeInHierarchy) Badge.SetActive(true);
        Badge.GetComponentInChildren<Text>().text = "+" + score.ToString();
        LeanTween.move(Badge, targetTransform, 1).setOnComplete(() =>
        {
            if (SoundManager.instance != null) SoundManager.instance.PointsAudioPlayer(true);
            Destroy(Badge);
        });
        LeanTween.scale(Badge, new Vector2(.5f,.5f), 1);
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// Rearranging tiles of hand, move them, scale them and change their orientation if required.
    /// </summary>
    public void Shuffle()
    {
        Debug.Log("Shuffle");
        for (int i = 0; i < dominosCurrentList.Count; i++)
        {
            dominosCurrentList[i].GetComponent<BoxCollider>().enabled = false;

            LeanTween.move(dominosCurrentList[i].gameObject, Places[i].transform.position, 0.5f).setDelay(i * 0.04f);//.setOnComplete(()=> dominosCurrentList[i].GetComponent<BoxCollider>().enabled = true);
            LeanTween.rotateLocal(dominosCurrentList[i].gameObject, Places[i].transform.eulerAngles, 0.5f).setDelay(i * 0.04f); ;
            dominosCurrentList[i].transform.localScale = Vector3.one;
            if (isMe)
            {
                dominosCurrentList[i].isPlayerHandDomino = true; //Set this tile property to true
                dominosCurrentList[i].initialPositionOfTile = Places[i].transform.position;
                //dominosCurrentList[i].ShowFront();
            }
            else
            {
                dominosCurrentList[i].isPlayerHandDomino = false;
            }
        }

        UpdateTilesCount();
        if (isMe)
            StartCoroutine(_ShuffleUI());
    }

    private IEnumerator _ShuffleUI()
    {
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("Shuffle UI Tiles");

        //if (this.dominosCurrentListUI.Count > 10) GridManager.instance.playerHandScrollRect.enabled = true;
        //else

        for (int i = 0; i < dominosCurrentListUI.Count; i++)
        {
            if (dominosCurrentListUI[i].transform.parent == GamePlayUIPanel.instance.boneyardTileImage)
                dominosCurrentListUI[i].transform.SetParent(GridManager.instance.playerHandScrollRect.content);
            //LeanTween.move(dominosCurrentListUI[i].gameObject, UIPlaces[i].transform.position, 0.5f);
            dominosCurrentListUI[i].transform.SetSiblingIndex(i);
            dominosCurrentListUI[i].transform.localScale = Vector3.one;
        }
    }

    public void UpdateTilesCount()
    {
        tilesCount_Txt.text = dominosCurrentList.Count.ToString(); //Update Cards Count
    }

    /// <summary>
    /// Rearrange Tiles To Display
    /// </summary>
    /// <returns></returns>
    public IEnumerator _ShowTiles()
    {
        
        for (int i = 0; i < dominosCurrentList.Count; i++)
        {
            dominosCurrentList[i].ShowFront();
            LeanTween.rotateLocal(dominosCurrentList[i].gameObject, DisplayTilesPlaces[i].eulerAngles, 0.3f);

            LeanTween.move(dominosCurrentList[i].gameObject, DisplayTilesPlaces[i], 0.3f);

            dominosCurrentList[i].transform.localScale = Vector3.one;
            yield return new WaitForSeconds(0.2f);
        }
    }


    /// <summary>
    /// Show Double Faces Tiles
    /// </summary>
    /// <returns></returns>
    public IEnumerator _ShowDoubleFaceTiles()
    {
        for (int i = 0; i < dominosCurrentList.Count; i++)
        {
            if (!dominosCurrentList[i].SameFace) continue;
            dominosCurrentList[i].ShowFront();
            LeanTween.rotateLocal(dominosCurrentList[i].gameObject, DisplayTilesPlaces[i].eulerAngles, 0.3f);

            LeanTween.move(dominosCurrentList[i].gameObject, DisplayTilesPlaces[i], 0.3f);

            dominosCurrentList[i].transform.localScale = Vector3.one;
            yield return new WaitForSeconds(0.2f);
        }
    }


    /// <summary>
    /// Rearrange Tiles To Display
    /// </summary>
    /// <returns></returns>
    public IEnumerator _ShowTileScoreToBadge(int localScore)
    {
        int iterator = 0;
        GameObject scoreBadge = playerPersonalData.DisplayerScoreArea;
        if (!scoreBadge.activeInHierarchy) scoreBadge.SetActive(true);
        scoreBadge.GetComponentInChildren<Text>().text = iterator.ToString();

        while (localScore > 0)
        {
            iterator += 1;
            localScore--;
            scoreBadge.GetComponentInChildren<Text>().text = iterator.ToString();
            yield return null;
        }
    }



    public IEnumerator _GiveScoreBadgeToPlayerDirectly()
    {
        foreach (var player in GamePlayUIPanel.instance.players)
        {
            Vector3 oldPos = player.playerPersonalData.DisplayerScoreArea.transform.position;
            LeanTween.move(player.playerPersonalData.DisplayerScoreArea, player.playerPersonalData.ScoreText.transform, 0.5f);

            //Reset
            yield return new WaitForSeconds(0.5f);
            player.playerPersonalData.DisplayerScoreArea.SetActive(false); //Hide
            player.playerPersonalData.DisplayerScoreArea.transform.position = oldPos; // Reset Local Badge Pos
            player.playerPersonalData.DisplayerScoreArea.GetComponentInChildren<Text>().text = "0";
        }
    }


    /// <summary>
    /// Tiles Finish
    /// </summary>
    /// <param name="newScore"></param>
    /// <returns></returns>
    public IEnumerator _SetScore(int newScore)
    {
        while (newScore > 0)
        {
            playerPersonalData.playerScore += 1;
            newScore--;
            playerPersonalData.ScoreText.text = playerPersonalData.playerScore.ToString();
            yield return null;
        }
    }


    /// <summary>
    /// Game Locked
    /// </summary>
    /// <param name="playerScores"></param>
    /// <returns></returns>
    public IEnumerator _GiveScoreToWinner(List<PlayerScore> playerScores)
    {
        PlayerScore playerScore = playerScores.Find(player => player.Player.Equals(this));
        int sum = 0;
        foreach (var item in playerScores)
        {
            if (playerScore == item)
                sum -= item.Score;
            else
                sum += item.Score;
        }

        while (sum > 0)
        {
            playerPersonalData.playerScore += 1;
            sum--;
            playerPersonalData.ScoreText.text = playerPersonalData.playerScore.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }


    public void DestroyOldDominoTiles()
    {
        foreach (var tile in dominosCurrentList)
        {
            Destroy(tile.gameObject);
        }
        dominosCurrentList.Clear();

        foreach (var tile in dominosCurrentListUI)
        {
            Destroy(tile.gameObject);
        }
        dominosCurrentListUI.Clear();
    }

    public void DestroyPlayer()
    {
        playerPhysicalPosition.currentplayerPosition.GetComponentInChildren<AvatarParent_FbxHolder>().gameObject.SetActive(false);
        gameObject.SetActive(false);
        DestroyOldDominoTiles();
        GamePlayUIPanel.instance.players.Remove(this);

        if (GamePlayUIPanel.instance.players.Count == 1)
        {
            GridManager.FinishTheGame(GamePlayUIPanel.instance.players[0]);//GamePlayUIPanel.instance.players.Find(x => x.playerPersonalData.playerUserID.Equals(PlayerPersonalData.playerUserID)));
        }
    }

    public void MakeTileInteractableToMove(Tile tile, bool enable)
    {
        if(enable) Debug.Log("MakeTileInteractableToMove");
        bool changeState = !enable;
        tile.moveable = enable;
        //tile.greyChild.gameObject.SetActive(changeState);

        UITile uiTile = dominosCurrentListUI.Find(x => x.parentTile == tile);
        if(uiTile) uiTile.MakeInteractable_UI_Tile(enable);
    }

    public void UpdateShineChild(Tile tile = null , UITile uITile = null)
    {
        Debug.Log("UpdateShineChild");
        GridManager.instance.playerHandScrollRect.scrollSensitivity = 20;
        GridManager.instance.playerHandScrollRect.enabled = false;
        GridManager.instance.playerHandScrollRect.enabled = true;


        //currentSelectedTile = null;

        //Turn Off Shine 3D Tiles
        foreach (var item in dominosCurrentList)
        {
            item.shineChild.SetActive(false);
        }
        //Turn Off Shine UI Tiles
        foreach (var item in dominosCurrentListUI)
        {
            item.EnableDisable_UI_Tile(false);
        }
        foreach (var tilePossibility in FindObjectsOfType<TilePossibilities>(false))
        {
            tilePossibility.GetComponent<MeshRenderer>().enabled = false;
            tilePossibility.GetComponent<BoxCollider>().enabled = true;
        }

        if (tile != null)
        {
            currentSelectedTile = tile;
            //tile.shineChild.SetActive(true);

            int noOfPossibilities = 0;

            foreach (var item in FindObjectsOfType<TilePossibilities>(false))
            {
                item.GetComponent<MeshRenderer>().enabled = false;
                item.GetComponent<BoxCollider>().enabled = false;
                if (tile.SameFace)
                {
                    if (item.isSamePhase)
                    {
                        if (item.value == tile.First)
                        {
                            noOfPossibilities++;
                            item.GetComponent<MeshRenderer>().enabled = true;
                            item.GetComponent<BoxCollider>().enabled = true;
                        }
                    }
                }
                else
                {
                    if (!item.isSamePhase)
                    {
                        if (item.value == tile.First || item.value == tile.Second)
                        {
                            noOfPossibilities++;
                            item.GetComponent<MeshRenderer>().enabled = true;
                            item.GetComponent<BoxCollider>().enabled = true;
                        }
                    }
                }
            }

            //Check Capicua
            if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode4 &&  noOfPossibilities >= 2 && GridManager.instance.currentPlayer.dominosCurrentList.Count == 1)
            {
                if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
                {
                    Debug.LogError("********************* No Of Possibilities: " + noOfPossibilities);
                    Rule4.canLastTilePlayedOnBothSides = Rule4.CheckCapicua(noOfPossibilities);
                }
                else
                {
                    UpdateMessage updateMessage = new UpdateMessage
                    {
                        code = GameUpdates.Capicua,
                        data = "Capicua"
                    };
                    GameManager.instace.SendMatchStateAsync(OpCodes.UPDATE, updateMessage.ToJson());
                }
            }

        }

        if (uITile != null)
        {
            currentSelected_UI_Tile = uITile;
            uITile.EnableDisable_UI_Tile(true);
        }
    }

    /// <summary>
    /// Play your Turn
    /// </summary>
    /// <returns></returns>
    public IEnumerator _StartMove()
    {
        yield return new WaitForSeconds(1);
        Playable = false;
        //movePlayed = false;
        //Player

        TilePossibilities[] tilePossibilities = FindObjectsOfType<TilePossibilities>();

        if (isMe)
        {
            Debug.Log("My Turn");
            for (int i = 0; i < dominosCurrentList.Count; i++)
            {
                bool canPlayThisTile = false;
                foreach (var item in tilePossibilities)
                {

                    Debug.Log("1:Possibilities(first loop for finding possiblities)");
                    if (dominosCurrentList[i].First == item.value || dominosCurrentList[i].Second == item.value)
                    {   
                        Debug.Log("2: Possibilities(second loop for finding possiblities)");
                        canPlayThisTile = true;
                        MakeTileInteractableToMove(dominosCurrentList[i], true);
                        Playable = true;
                        break;
                    }
                }
                if (!canPlayThisTile)
                {
                    MakeTileInteractableToMove(dominosCurrentList[i], false);
                }
            }

            //if player don't have any tile to play. (No match found)
            if (!Playable)
            {
                Debug.Log("Can't Play");
                GridManager.instance.hiddenLayerMask.SetActive(true);
                //Taking new tile from boneyard
                if (GridManager.instance.dominosCurrentList.Count > 0)
                {
                    if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
                    {

                        PlayerIdParse playerIdParse = new PlayerIdParse
                        {
                            playerID = playerPersonalData.playerUserID
                        };

                        UpdateMessage updateMessage = new UpdateMessage
                        {
                            code = GameUpdates.PickUpNewTile,
                            //data = MatchDataJson.PickUpNewTileFromBoneyard(playerPersonalData.playerUserID)
                            data = playerIdParse.ToJson()
                        };
                        GameManager.instace.SendMatchStateAsync(OpCodes.UPDATE, updateMessage.ToJson());
                    }
                    else
                    {
                        //reset timer
                        //ResetTimerImageValue();
                        Debug.Log("Picking Up New Tile");
                        TurnTimerController.instance.StartTimer(this);
                        dominosCurrentList.Add(GridManager.instance.dominosCurrentList[0]);
                        GridManager.instance.spawnUITile(this);
                        Shuffle();
                        GridManager.instance.dominosCurrentList.RemoveAt(0);
                        GamePlayUIPanel.UpdateBoneYardText(GridManager.instance.dominosCurrentList.Count);

                        StartCoroutine(_StartMove());
                    }
                }
                //Pass the Turn, this player can not play
                else
                {
                    if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
                    {
                        Debug.LogError("3 Bug here");

                        UpdateMessage updateMessage = new UpdateMessage
                        {
                            code = GameUpdates.SkipTurn,
                            data = new SkipTurnUpdate
                            {
                                PlayerId = PlayerPersonalData.playerUserID,
                                //PlayerId = playerPersonalData.playerUserID,
                                SkipReason = "Can't Play!"
                            }.ToJson()
                        };
                        GameManager.instace.SendMatchStateAsync(OpCodes.UPDATE, updateMessage.ToJson());
                    }
                    else
                    {
                        Debug.Log("No Tile Left, Turn Change");
                        //GamePlayUIPanel.instance.PopUpController(GetComponent<RectTransform>(), "Can't Play!");
                        Debug.Log("3");
                        SkipMyTurn("Can't Play!");

                        //GridManager.instance.TurnSwitch += 1;
                        //StartCoroutine(GridManager.instance._ChangeTurn());
                    }
                }
            }
            else
            {
                Debug.Log("All Players are waiting for my Move.");
                GridManager.instance.hiddenLayerMask.SetActive(false);
                //Wait for this player to play it's turn. Turn will be played in update method.
            }
        }
        else
        {
            if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
            {
                //Bot playing turn
                int randomWait = Random.Range(2, 5);
                yield return new WaitForSeconds(randomWait);
                AutoPlayTurn();
            }
            else
            {
                Debug.Log("Waiting For Opponent Move.");
            }
        }
    }



    /// <summary>
    /// Calling by Nakama
    /// </summary>
    public void PickUpNewTileMultiplayer()
    {
        //reset timer
        //ResetTimerImageValue();

        TurnTimerController.instance.StartTimer(this);
        Debug.Log("Picking Up New Tile");
        dominosCurrentList.Add(GridManager.instance.dominosCurrentList[0]);

        if (isMe)
        {
            //GridManager.instance.hiddenLayerMask.SetActive(true);
            GridManager.instance.spawnUITile(this);
        }

        Shuffle();
        GridManager.instance.dominosCurrentList.RemoveAt(0);
        GamePlayUIPanel.UpdateBoneYardText(GridManager.instance.dominosCurrentList.Count);

        StartCoroutine(_StartMove());
    }

    /// <summary>
    /// This method is specially for Computer Mode.
    /// But, This method will also help when player's time over, then Auto Play the Turn
    /// </summary>
    public void AutoPlayTurn()
    {

        TilePossibilities[] tilePossibilities = FindObjectsOfType<TilePossibilities>(false);

        Playable = false;

        if (isMe)
        {
            List<Tile> moveableTiles = dominosCurrentList.FindAll(x => x.moveable);

            var minValuedTile = moveableTiles.OrderBy(p => p.First + p.Second).First();

            var tilePossibility = tilePossibilities.ToList().Find(x => x.isSamePhase.Equals(minValuedTile.SameFace) && (x.value.Equals(minValuedTile.First) || x.value.Equals(minValuedTile.Second)));
            if (tilePossibility != null)
            {
                Debug.Log("its me, item.value: " + tilePossibility.value, tilePossibility);
                Playable = true;

                minValuedTile.PlaceTileOnParticularPosition(tilePossibility); //Place Tile
                UITile uiTile = GridManager.instance.currentPlayer.dominosCurrentListUI.Find(x => x.parentTile == minValuedTile);
                if (uiTile) uiTile.DestroyUITile();
            }
        }
        else
        {
            //Decide whether can play turn or not. if match found break the loop and place that tile
            for (int i = 0; i < dominosCurrentList.Count; i++)
            {
                foreach (var item in tilePossibilities)
                {
                    if (dominosCurrentList[i].First == item.value || dominosCurrentList[i].Second == item.value)
                    {
                        if (dominosCurrentList[i].SameFace == item.isSamePhase)
                        {
                            Playable = true;

                            dominosCurrentList[i].PlaceTileOnParticularPosition(item); //Place Tile
                            break;
                        }
                    }
                }
                if (Playable)
                {
                    break;
                }
            }
        }


        //if Player don't have any tile to play. (No match found)
        if (!Playable)
        {
            Debug.Log("Can't Play");
            //Taking new tile from boneyard
            if (GridManager.instance.dominosCurrentList.Count > 0)
            {
                //reset timer
                //ResetTimerImageValue();
                TurnTimerController.instance.StartTimer(this);
                Debug.Log("Picking Up New Tile");
                dominosCurrentList.Add(GridManager.instance.dominosCurrentList[0]);
                Shuffle();
                GridManager.instance.dominosCurrentList.RemoveAt(0);
                GamePlayUIPanel.UpdateBoneYardText(GridManager.instance.dominosCurrentList.Count);

                StartCoroutine(_StartMove());
            }
            //Pass the Turn, this Bot can not play
            else
            {
                Debug.Log("4");
                SkipMyTurn("Can't Play!");
                Debug.Log("No Tile Left, Turn Change");
                //GridManager.instance.TurnSwitch += 1;
                //StartCoroutine(GridManager.instance._ChangeTurn());
            }
        }
    }


    /// <summary>
    /// This method is specially for Multiplayer.
    /// But, This method will also help when player's time over, then Auto Play the Turn
    /// </summary>
    public void AutoPlayTurnMultiplayer()
    {
        TilePossibilities[] tilePossibilities = FindObjectsOfType<TilePossibilities>(false);
        Playable = false;

        //Decide whether can play turn or not. if match found break the loop and place that tile
        Debug.Log("AutoPlayTurnMultiplayer. ");

        List<Tile> moveableTiles = dominosCurrentList.FindAll(x => x.moveable);


        //moveableTiles.OrderBy(p => p.First + p.Second).First();
        try
        {
            var minTile = moveableTiles.OrderBy(p => p.First + p.Second).First();

            if (minTile != null)
            {
                var tilePossibility = tilePossibilities.ToList().Find(x => x.isSamePhase.Equals(minTile.SameFace) && (x.value.Equals(minTile.First) || x.value.Equals(minTile.Second)));
                if (tilePossibility != null)
                {
                    Debug.Log("tilePossibility.value: " + tilePossibility.value, tilePossibility);
                    Playable = true;
                    GridManager.instance.currentPlayer.currentSelectedTile = minTile;
                    UITile uiTile = GridManager.instance.currentPlayer.dominosCurrentListUI.Find(x => x.parentTile == minTile);
                    GridManager.instance.currentPlayer.currentSelected_UI_Tile = uiTile;
                    GridManager.instance.hiddenLayerMask.SetActive(false);
                    tilePossibility.PlayMove();
                }
                else
                {
                    Debug.LogError("Error tilePossibility not found for this tile: " + minTile.First + "," + minTile.Second);
                }
            }
            else
            {
                Debug.LogError("Error To Find Smallest Tile.");
                var tilePossibility = tilePossibilities.ToList().Find(x => x.isSamePhase.Equals(minTile.SameFace) && (x.value.Equals(moveableTiles[0].First) || x.value.Equals(moveableTiles[0].Second)));
                if (tilePossibility != null)
                {
                    Debug.Log("tilePossibility.value: " + tilePossibility.value, tilePossibility);
                    Playable = true;
                    GridManager.instance.currentPlayer.currentSelectedTile = moveableTiles[0];
                    UITile uiTile = GridManager.instance.currentPlayer.dominosCurrentListUI.Find(x => x.parentTile == moveableTiles[0]);
                    GridManager.instance.currentPlayer.currentSelected_UI_Tile = uiTile;
                    GridManager.instance.hiddenLayerMask.SetActive(false);
                    tilePossibility.PlayMove();
                }
                else
                {
                    Debug.LogError("Error tilePossibility not found for this tile: " + moveableTiles[0].First + "," + moveableTiles[0].Second);
                }
            }

            if (!Playable)
            {

                Debug.LogError("1 Bug here");
                UpdateMessage updateMessage = new UpdateMessage
                {
                    code = GameUpdates.SkipTurn,
                    data = new SkipTurnUpdate
                    {
                        PlayerId = playerPersonalData.playerUserID,
                        SkipReason = "Can't Play!"
                    }.ToJson()
                };
                GameManager.instace.SendMatchStateAsync(OpCodes.UPDATE, updateMessage.ToJson());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("2 Bug here" + ex);
            UpdateMessage updateMessage = new UpdateMessage
            {
                code = GameUpdates.SkipTurn,
                data = new SkipTurnUpdate
                {
                    PlayerId = playerPersonalData.playerUserID,
                    SkipReason = "Can't Play!"
                }.ToJson()
            };
            GameManager.instace.SendMatchStateAsync(OpCodes.UPDATE, updateMessage.ToJson());
        }
    }

    public void SkipMyTurn(string reason)
    {
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
        {
            if (GameManager.instace.isTurnInProgress)
            {
                SkipTurnBody(reason);
            }
            else
            {
                Debug.LogError("........Bug Here.........");
            }
        }
        else
        {
            SkipTurnBody(reason);
        }
    }


    void SkipTurnBody(string reason)
    {
        GamePlayUIPanel.instance.PopUpController(reason, playerPersonalData.playerTexture);

        GridManager.instance.TurnSwitch += 1;
        StartCoroutine(GridManager.instance._ChangeTurn());

        //Check Pass
        if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode4)
        {
            Debug.LogError(Rule4.CheckPassPoints() == true ? "Yes it's a pass, rewarding 2 points" : "No pass");
        }
        else if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode3)
        {
            Debug.LogError(Rule3.FirstImmediatePassPoints() == true ? "Yes it's a first immediate pass point, rewarding 25 points" : "No immediate pass");
        }

    }

    public Image ResetTimerImageValue()
    {
        Image image = GridManager.instance.currentPlayer.timmerImage;
        image.fillAmount = 1;
        image.gameObject.SetActive(true);
        return image;
    }

    public void RandomBotGenerator(int value)
    {
        if (value == 0) //Female
        {
            int randNameValue = Random.Range(0, BotRandomData.femaleNames.Length);
            BotRandomData.currentBotName = BotRandomData.femaleNames[randNameValue];

            int randPicValue = Random.Range(0, BotRandomData.femalePics.Count);
            BotRandomData.currentBotPic = BotRandomData.femalePics[randPicValue];

            BotRandomData.botGender = BotData.Gender.Female;
        }
        else //Male
        {
            int randNameValue = Random.Range(0, BotRandomData.maleNames.Length);
            BotRandomData.currentBotName = BotRandomData.maleNames[randNameValue];

            int randPicValue = Random.Range(0, BotRandomData.malePics.Count);
            BotRandomData.currentBotPic = BotRandomData.malePics[randPicValue];

            BotRandomData.botGender = BotData.Gender.Male;
        }
    }

    public void SetDataToPopUp()
    {

        ProfileStatesPopUpData playerStatesPopUp = new ProfileStatesPopUpData();
        Texture2D tex = playerPersonalData.playerTexture;

        playerStatesPopUp.profilePic = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        PlayerData playerData = playerPersonalData;

        playerStatesPopUp.playerName = playerData.playerName;
        playerStatesPopUp.playerCoins = playerData.playerScore.ToString();


        PlayerStates playerStats = playerData.playerStates;

        playerStatesPopUp.userGameModeInfo = playerStats.userGameModeInfo;

        playerStatesPopUp.gamesPlayed = playerStats.gamesPlayed.ToString();
        playerStatesPopUp.gamesWon = playerStats.gamesWon.ToString();
        playerStatesPopUp.gamesWonPercentage = playerStats.gamesWonPercentage.ToString();

        playerStatesPopUp.tournamentPlayed = playerStats.tournnamentPlayed.ToString();
        playerStatesPopUp.tournamentWon = playerStats.tournnamentWon.ToString();
        playerStatesPopUp.tournamentLost = playerStats.tournnamentLost.ToString();

        playerStatesPopUp.championShipWon = playerStats.championshipsWon.ToString();

        playerStatesPopUp.flagName = playerStats.playerFlagShortCode.ToString();
        playerStatesPopUp.className = playerStats.playerClass.ToString();

        //Find Flag by shortcode
        //Find Class by class name
        playerStatesPopUp.playerFlagIcon = playerPersonalData.playerFlagSprite;
        playerStatesPopUp.playerClassIcon = playerPersonalData.playerClassSprite;

        GamePlayUIPanel.instance.playerStatesPopUp.EnableProfilePanel(playerStatesPopUp);
    }



    public void OnSuccessfullyAssetsDownloaded(JObject arg1, long arg2, NFTUriAndToken uri)
    {
        if (ResponseStatus.Check(arg2))
        {
            Debug.Log("OnSuccessfullyAssetsDownloaded" + arg1.ToString());
            MintData mintData = MintData.FromJson(arg1.ToString());

            //SpawnCharacter(mintData.Attributes.gender);

            //Gender selectedGender = (Gender)Enum.Parse(typeof(Gender), mintData.Attributes.gender);

            //LoadCharacterBlends(selectedGender, mintData.Attributes.DataList);
        }
        else
        {
            Debug.Log("OnSuccessfullyAssetsDownloaded Fail: " + arg1.ToString());
        }
    }


    public void OnFail(string obj)
    {
        Debug.LogError("On Fail+++++++: " + obj);
    }

}

[Serializable]
public class PlayerData
{

    public int playerDefaultSeatID;
    public string playerUserID;
    public int playerSeatID;
    public int playerScore;
    public string playerName;
    public Texture2D playerTexture;
    public Sprite playerFlagSprite;
    public Sprite playerClassSprite;
    [Space]
    [Header("Inspector Fields")]
    public Text playerNameText;
    public RawImage playerRawImage;
    public Text ScoreText;
    public GameObject DisplayerScoreArea;
    public PlayerStates playerStates;
    public Image flagImage;

    public PlayerStates UpdatePlayerStats(MetaData metaData)
    {
        Debug.Log("UpdatePlayerStats");
        this.playerStates.blockChainData = BlockChainData.FromJson(metaData.blockChainData);
        this.playerStates.userGameModeInfo = UserGameModeInfo.FromJson(metaData.userGameModeInfo);

        this.playerStates.gamesPlayed = (metaData.GamesPlayed);
        this.playerStates.gamesWon = (metaData.GamesWon);
        this.playerStates.gamesWonPercentage = (metaData.GamesWonPercentage);

        this.playerStates.tournnamentPlayed = int.Parse(metaData.TournnamentPlayed);
        this.playerStates.tournnamentWon = int.Parse(metaData.TournnamentWon);
        this.playerStates.tournnamentLost = int.Parse(metaData.TournnamentLost);

        this.playerStates.championshipsWon = int.Parse(metaData.ChampionshipsWon);

        this.playerStates.playerClass = metaData.PlayerClass;
        this.playerStates.playerFlagShortCode = metaData.PlayerFlagShortCode;
        Sprite classSprite = WebServiceManager.instance.FindClassSprite(metaData.PlayerClass);
        Debug.Log("this.playerStates.playerClass: " + this.playerStates.playerClass);
        Debug.Log("this.playerStates.playerFlagShortCode: " + this.playerStates.playerFlagShortCode);
        Debug.Log((classSprite == null) ? "classSprite Null" : "classSprite NotNull");
        this.playerStates.classSprite = classSprite;
        //WebServiceManager.instance.FindFlagSprite(metaData.PlayerFlagShortCode, this.playerStates.flagSprite , flagImage);
        //this.flagImage.sprite = this.playerFlagSprite = GamePlayUIPanel.instance.FindFlagSprite(metaData.PlayerFlagShortCode);
        return this.playerStates;
    }
}


[Serializable]
public class BotData
{
    public enum Gender {Male,Female}
    public Gender botGender = Gender.Male;

    public string currentBotName;
    public Texture2D currentBotPic;

    [Header("Bots")]
    public List<Texture2D> malePics = new List<Texture2D>();
    public List<Texture2D> femalePics = new List<Texture2D>();
    public string[] maleNames = new string[] { "Jacob", "Michael", "Matthew", "Joshua", "Christopher", "Nicholas", "Andrew", "Joseph", "Daniel", "Tyler", "William", "Brandon", "Ryan", "John", "Zachary", "David", "Anthony", "James", "Justin", "Alexander", "Jonathan", "Christian", "Austin", "Dylan", "Ethan", "Benjamin", "Noah", "Samuel", "Robert", "Nathan", "Cameron", "Kevin", "Thomas", "Jose", "Hunter", "Jordan", "Kyle", "Caleb", "Jason", "Logan", "Aaron", "Eric", "Brian", "Gabriel", "Adam", "Jack", "Isaiah", "Juan", "Luis", "Connor", "Charles", "Elijah", "Isaac", "Steven", "Evan", "Jared", "Sean", "Timothy", "Luke", "Cody", "Nathaniel", "Alex", "Seth", "Mason", "Richard", "Carlos", "Angel", "Patrick", "Devin", "Bryan", "Cole", "Jackson", "Ian", "Garrett", "Trevor", "Jesus", "Chase", "Adrian", "Mark", "Blake", "Sebastian", "Antonio", "Lucas", "Jeremy", "Gavin" };
    public string[] femaleNames = new string[] { "Emily", "Hannah", "Madison", "Ashley", "Sarah", "Alexis", "Samantha", "Jessica", "Elizabeth", "Taylor", "Lauren", "Alyssa", "Kayla", "Abigail", "Brianna", "Olivia", "Emma", "Megan", "Grace", "Victoria", "Rachel", "Anna", "Sydney", "Destiny", "Morgan", "Jennifer", "Jasmine", "Haley", "Julia", "Kaitlyn", "Nicole", "Amanda", "Katherine", "Natalie", "Hailey", "Alexandra", "Savannah", "Chloe", "Rebecca", "Stephanie", "Maria", "Sophia", "Mackenzie", "Allison", "Isabella", "Amber", "Mary", "Danielle", "Gabrielle", "Jordan", "Brooke", "Michelle", "Sierra", "Katelyn", "Andrea", "Madeline", "Sara", "Kimberly", "Courtney", "Erin", "Brittany", "Vanessa", "Jenna", "Jacqueline", "Caroline", "Faith", "Makayla", "Bailey", "Paige", "Shelby", "Melissa", "Kaylee", "Christina", "Trinity", "Mariah", "Caitlin", "Autumn", "Marissa", "Breanna" };
}

[Serializable]
public class PlayerPhysicalPosition
{
    public Transform currentplayerPosition;
    public RectTransform playerMsgPosTransform;
    public ParticleSystem playerProfileParticleSystem;
}

[Serializable]
public class WinnerData
{
    public string playerUserID;
    public GameEndPlayers[] players; 
    public WinnerData(string winnerId, GameEndPlayers[] players)
    {
        this.playerUserID = winnerId;
        this.players = players;
    }
}

public class GameEndPlayers
{
    public int playerScore;
    public string playerUserID;
}