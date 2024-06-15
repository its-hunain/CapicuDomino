using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required when using Event data.

public class UITile : MonoBehaviour , IPointerDownHandler // required interface when using the OnPointerDown method.
{
    [Header("Tile Values")]
    public Image tileImage;
    public BoxCollider2D boxCollider2D;
    public RectTransform rectTransform;
    public Tile parentTile;
    public Player player;

    [Header("Tile Selection Shades")]
    public GameObject selectionShade1;
    public GameObject selectionShade2;

    [Header("Tile Disable Shades")]
    public GameObject brownShade;

    [Header("Tile Playable Shades")]
    public GameObject GreyShadowImage;

    internal void SetValues(Sprite sprite , Tile parentTile , Player player)
    {
        this.player = player;
        tileImage.sprite = sprite;
      
        var str = PlayerPrefs.GetString("TileTheme", "TileTheme1");
        UpdateTileTheme(str , tileImage);

        this.parentTile = parentTile;
    }

    public void UpdateTileTheme(string theme, Image tileImage)
    {
        Color color = Color.white;

        if (theme == "TileTheme1")
        {
            color = Color.white;
        }

        else if (theme == "TileTheme2")
        {

            color = new Color(235, 95, 165, 255);
        }
        else if (theme == "TileTheme3")
        {

            color = Color.yellow;// new Color(127, 142, 243, 255);
        }
        else if (theme == "TileTheme4")
        {
            color = new Color(157, 51, 238, 255);
        }
        else
        {
            color = Color.white;
        }

        
        tileImage.color = color;


        Debug.Log("theme: " + theme);
    }

    public void EnableDisable_UI_Tile(bool isEnabled)
    {
        selectionShade1.SetActive(isEnabled);
        selectionShade2.SetActive(isEnabled);
    }

    public void MakeInteractable_UI_Tile(bool isEnabled)
    {
        boxCollider2D.enabled = isEnabled;
        brownShade.SetActive(!isEnabled);
        GreyShadowImage.SetActive(isEnabled);
        rectTransform.sizeDelta = isEnabled ? new Vector2(80, 180) : new Vector2(80, 200);
        //GridManager.instance.playerHandScrollRect.enabled = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!parentTile.Fixed && parentTile.moveable)
        {
            if (GridManager.instance.isFirstTurn)
            {
                if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
                {
                    parentTile.placeFirstTile(); //Bot Game, playing first move
                    DestroyUITile();
                }
                else
                {
                    if (!GridManager.instance.hiddenLayerMask.activeInHierarchy)
                    {
                        TurnTimerController.instance.ResetTimer(GridManager.instance.currentPlayer);
                        if (GridManager.instance.currentPlayer.isMe)
                        {
                            GridManager.instance.hiddenLayerMask.SetActive(true);
                        }
                        GridManager.instance.isFirstTurn = false;

                        //For Multiplayer
                        string tileValue = parentTile.First.ToString() + parentTile.Second.ToString();
                        Debug.Log("Sending Values....");
                        Debug.Log("tileValue, currentSelectedTilePossibility.value.ToString(), currentSelectedTilePossibility.isSamePhase");
                        Debug.Log(tileValue + "," + "-1" + "," + "false");
                        GameManager.instace.SendMatchStateAsync(
                            OpCodes.TURN_MOVE,
                            MatchDataJson.SendMoveValues(tileValue, "-1", "Null", "Null", false, GridManager.instance.currentPlayer.playerPersonalData.playerUserID));
                        DestroyUITile();
                    }
                }
            }
            else
            {
                GridManager.instance.currentPlayer.UpdateShineChild(parentTile,this);
            }
        }
    }

    public void DestroyUITile()
    {
        player.dominosCurrentListUI.Remove(this);
        Destroy(this.gameObject);
    }
}
