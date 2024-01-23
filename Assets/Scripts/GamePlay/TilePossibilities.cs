using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TilePossibilities : MonoBehaviour
{
    public string directionOfTile;
    public bool isReserved  = false;
    public bool isSamePhase = false;
    public int value;

    public void ChangeActiveState(bool state)
    {
        //this.enabled = false;
        gameObject.SetActive(state);
    }

    private void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tile")
        {
            isReserved = true;
            ChangeActiveState(false);


            if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode1)
            {

                var parentTile = GetComponentInParent<Tile>();

                //parentTile.reservedChildCount += 1;
                Debug.Log("++++++++++ " + parentTile.name, parentTile);

                if (parentTile == HighFiveRules.firstSpinner)
                {
                    CheckReservedChilds();
                }
                else
                {
                    //Disable Old Tiles Scripts because it's not required anymore and they might raise some error.
                    //if (parentTile.reservedChildCount >= 4)
                    //{
                    //    Debug.Log("++++++++++False: " + parentTile.name, parentTile);
                    //    parentTile.GetComponent<Collider>().enabled = false; 
                    //}
                }
            }
        }
    }

    /// <summary>
    /// if left and right are reserved turn on the top and bottom child.
    /// </summary>
    private void CheckReservedChilds()
    {
        bool leftChildReserved      = HighFiveRules.firstSpinner.childPossibility.leftChild.isReserved;
        bool rightChildReserved     = HighFiveRules.firstSpinner.childPossibility.rightChild.isReserved;
        bool topChildReserved       = HighFiveRules.firstSpinner.childPossibility.topChild.isReserved;
        bool bottomChildReserved    = HighFiveRules.firstSpinner.childPossibility.bottomChild.isReserved;

        if (leftChildReserved && rightChildReserved && !topChildReserved && !bottomChildReserved)
        {
            Debug.Log("Left and right are reserved turn on the top and bottom childs.");
            HighFiveRules.firstSpinner.childPossibility.topChild.ChangeActiveState(true);
            HighFiveRules.firstSpinner.childPossibility.bottomChild.ChangeActiveState(true);
        }        
    }


    public void OnMouseDown()
    {
        if (GetComponent<MeshRenderer>().enabled && GridManager.instance.currentPlayer.currentSelectedTile != null && !EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("OnMouseDown() child" + gameObject.name , gameObject);
            PlayMove();
        }
    }


    public void PlayMove()
    {
        Debug.Log("PlayMove");
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
        {
            GridManager.instance.currentPlayer.currentSelectedTile.PlaceTileOnParticularPosition(this); //Place Tile
            GridManager.instance.currentPlayer.currentSelected_UI_Tile.DestroyUITile();
        }
        else
        {
            //For Multiplayer
            //1) Tile
            //2) TilePossibility
            if (!GridManager.instance.hiddenLayerMask.activeInHierarchy)
            {
                GridManager.instance.hiddenLayerMask.SetActive(true);
                TurnTimerController.instance.ResetTimer(GridManager.instance.currentPlayer);

                Tile currentSelectedTile = GridManager.instance.currentPlayer.currentSelectedTile;
                TilePossibilities currentSelectedTilePossibility = this;
                Tile tilePossibilityParent = this.GetComponentInParent<Tile>();
                string selectedTileValue = currentSelectedTile.First.ToString() + currentSelectedTile.Second.ToString();
                string tilePossibilityParentValue = tilePossibilityParent.First.ToString() + tilePossibilityParent.Second.ToString();
                Debug.Log(selectedTileValue + "," + currentSelectedTilePossibility.value.ToString() + "," + currentSelectedTilePossibility.isSamePhase);
                if(GridManager.instance.currentPlayer.currentSelected_UI_Tile) GridManager.instance.currentPlayer.currentSelected_UI_Tile.DestroyUITile();
                GameManager.instace.SendMatchStateAsync(
                    OpCodes.TURN_MOVE,
                    MatchDataJson.SendMoveValues(selectedTileValue, currentSelectedTilePossibility.value.ToString(), currentSelectedTilePossibility.directionOfTile.ToString(), tilePossibilityParentValue.ToString(), currentSelectedTilePossibility.isSamePhase, GridManager.instance.currentPlayer.playerPersonalData.playerUserID)
                );
            }
        }

    }
}