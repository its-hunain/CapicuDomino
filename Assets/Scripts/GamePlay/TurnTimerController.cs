using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurnTimerController : MonoBehaviour
{
    public static TurnTimerController instance;
    public static int nextTurnNumber = 0;
    private void Awake()
    {
        if(instance == null) instance = this;
    }

    //public IEnumerator timerCoroutine;

    [Header("Timmer")]
    public float targetTime;
    public double currentTime;
    public float timer;
    private float tempTime;
    private double currentActionTime = 0;

    private void OnTurnStart()
    {
        currentTime = Epoch.Now + GameManager.GetServerTime();
    }


    public Image image = null;
    public void StartTimer(Player player)
    {
        tempTime = targetTime;
        timer = 0;
        player.AutoPlay = true;
        GameRulesManager.TurnStarted = true;
        image = player.ResetTimerImageValue();
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer) OnTurnStart();
    }


    void Update()
    {
        if (GameRulesManager.TurnStarted && GridManager.instance.currentPlayer.AutoPlay)
        {
            if(GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
            {
                tempTime -= Time.deltaTime; 
                timer = tempTime;
                if (timer <= 0)
                {
                    OnTimerEnd();
                }
                if (GameRulesManager.TurnStarted)
                {
                    image.fillAmount = timer/targetTime;
                }
            }
            else
            {
                float currentTime = (float)Utilities.GetTimeDiffInSeconds(currentActionTime);
                //Debug.Log("currentTime: " + currentTime);
                float timeLeft = targetTime - currentTime;

                if (timeLeft >= 0)
                {
                    float amount = timeLeft / targetTime;
                    image.fillAmount = amount;
                }
                else
                {
                    if (GameRulesManager.TurnStarted)
                    {
                        Debug.Log("TIME END: Player Name: " + GridManager.instance.currentPlayer.playerPersonalData.playerName);
                        OnTimerEnd();
                    }
                }
            }
        }
    }


    public void UpdateActionTime(double actionTime)
    {
        this.currentActionTime = actionTime;
    }
    public void ResetTimer(Player player)
    {
        GameRulesManager.TurnStarted = false;
        player.AutoPlay = false;
        player.timmerImage.gameObject.SetActive(false);
    }

    public void StopTimer(Player currentPlayer)
    {
        //Not a first turn
        if (currentPlayer.AutoPlay)
        {
            ResetTimer(currentPlayer);
        }
        else
        {
            //Debug.Log("First Turn");
        }
    }

    private void OnTimerEnd()
    {
        //End Time
        Debug.Log("OnEnd: Time End 2 ", this.gameObject);
        ResetTimer(GridManager.instance.currentPlayer);

        if (GridManager.instance.currentPlayer.isMe)
        {
            GridManager.instance.hiddenLayerMask.SetActive(true);
        }

        if (GridManager.instance.isFirstTurn)
        {
            if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
            {
                if (GridManager.instance.currentPlayer.isMe)
                {
                    Debug.Log("My First Turn, but time End so auto play.....");
                    //For Multiplayer
                    string tileValue = GridManager.instance.firstTile.First.ToString() + GridManager.instance.firstTile.Second.ToString();

                    //string tileValue = parentTile.First.ToString() + parentTile.Second.ToString();
                    Debug.Log("Sending Values first tile.. time End.....");
                    GameManager.instace.SendMatchStateAsync(
                        OpCodes.TURN_MOVE,
                        MatchDataJson.SendMoveValues(tileValue, "-1", "Null", "Null", GridManager.instance.firstTile.SameFace, GridManager.instance.currentPlayer.playerPersonalData.playerUserID)
                    );
                    UITile uiTile = GridManager.instance.currentPlayer.dominosCurrentListUI.Find(x => x.parentTile == GridManager.instance.firstTile);
                    if (uiTile) uiTile.DestroyUITile();
                }
                else
                {
                    Debug.Log("First move opponent, but time End .....");
                }

            }
            else
            {
                GridManager.instance.firstTile.placeFirstTile(); //Bot Game, playing first move. . .timmer end
                UITile uiTile = GridManager.instance.currentPlayer.dominosCurrentListUI.Find(x => x.parentTile == GridManager.instance.firstTile);
                if(uiTile) uiTile.DestroyUITile();
            }
        }
        else
        {
            if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
            {
                Debug.Log("Turn Time Out");

                //Remove this code if turns are still having problem.
                //and uncomment the server side code for Turn Time Out.
                //Testing
                ResetTimer(GridManager.instance.currentPlayer);
                if (GridManager.instance.currentPlayer.isMe)
                {
                    if (GameManager.instace.isTurnInProgress)
                    {
                        Debug.Log("still exist 1 isTurnInProgress = .." + GameManager.instace.isTurnInProgress);
                        GridManager.instance.currentPlayer.AutoPlayTurnMultiplayer();
                    }
                    else
                    {
                        Debug.LogError("still exist 2 isTurnInProgress = .." + GameManager.instace.isTurnInProgress);
                    }
                }
                //End
            }
            else
            {
                GridManager.instance.currentPlayer.AutoPlayTurn();
                //GridManager.instance.currentPlayer.SkipMyTurn("Time End!");
            }
        }
    }

}
