using System;
using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Texture orignalTexture;
    public Sprite originalSprite;
    public Texture emptyTexture;

    [Header("Tile Highlighter")]
    public GameObject shineChild;
    public GameObject greyChild;

    //public int reservedChildCount = 0;

    public enum TileDirection
    {
        LeftUp,
        LeftRight,
        RightDown,
        RightLeft,
        Normal
    }
    public TileDirection currentTileDirection = TileDirection.Normal;

    public bool moveable = true;

    [Header("Tile Values")]
    public int First; //Value 1
    public int Second; //Value 2

    [Header("Renderer Image")]
    public Renderer mat;   //based on first and second value

    [Header("Double Sided Tile")]
    public bool SameFace = false;

    public Vector3 initialPositionOfTile;

    public bool isPlayerHandDomino; //if tile is placed on user hand

    public bool isFrontFace = false; //if tile face is hidden

    [Header("Possiblities For Same Phase Child")]
    public TileChilds childPossibility;

    public void Start()
    {
        //reservedChildCount = 0;
        initialPositionOfTile = transform.position;
    }

    public Tile(int _first, int _second)
    {
        First = _first;
        Second = _second;
    }

    public Tile() { }
    public void Populate(int _first, int _second)
    {
        //Debug.Log("_first: " + _first + " , _second: " + _second);
        First = _first;
        Second = _second;
        if (First == Second)
        {
            SameFace = true;
            childPossibility.leftChild.isSamePhase = childPossibility.rightChild.isSamePhase = childPossibility.topChild.isSamePhase = childPossibility.bottomChild.isSamePhase = !SameFace;
            childPossibility.leftChild.name = childPossibility.rightChild.name = childPossibility.topChild.name = childPossibility.bottomChild.name = First.ToString();
            childPossibility.leftChild.value = childPossibility.rightChild.value = childPossibility.topChild.value = childPossibility.bottomChild.value = First;
        }
        else
        {
            childPossibility.leftChild.name = First.ToString();
            childPossibility.topChild.name = First.ToString();
            childPossibility.leftChild.value = childPossibility.topChild.value = First;

            childPossibility.rightChild.name = Second.ToString();
            childPossibility.bottomChild.name = Second.ToString();
            childPossibility.rightChild.value = childPossibility.bottomChild.value = Second;


            childPossibility.leftChild.isSamePhase = childPossibility.rightChild.isSamePhase = false;
            childPossibility.topChild.isSamePhase = childPossibility.bottomChild.isSamePhase = true;
        }

        orignalTexture = Resources.Load<Texture>("DominoSprites/3DTilesTexture/" + First.ToString() + Second.ToString());
        originalSprite = Resources.Load<Sprite>("DominoSprites/UITilesSprite/" + First.ToString() + Second.ToString());
        mat.materials[1].mainTexture = emptyTexture;
    }

    /// <summary>
    /// Rotate tile, Show Face / Hide Face
    /// </summary>
    #region Front Rotation
    public void ShowFront()
    {
        if (!isFrontFace)
            isFrontFace = true;
        mat.materials[1].mainTexture = orignalTexture;
    }
    #endregion

    public bool Fixed = false;

    public TilePossibilities possibilityChildTile = null;

    private IEnumerator _UpdatePossibilities(bool firstTileOfAllFiveGame)
    {

        Debug.Log("_UpdatePossibilities()");
        yield return new WaitForSeconds(1f);

        if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode1)
        {
            if (firstTileOfAllFiveGame)
            {
                childPossibility.leftChild.ChangeActiveState(true);
                childPossibility.rightChild.ChangeActiveState(true);
            }
            else
            {
                if (this.SameFace)
                {
                    //Debug.Log("childPossibility.leftChild: " + childPossibility.leftChild.name, childPossibility.leftChild);
                    //Debug.Log("childPossibility.rightChild: " + childPossibility.rightChild.name, childPossibility.rightChild);
                    childPossibility.leftChild.ChangeActiveState(true);
                    childPossibility.rightChild.ChangeActiveState(true);
                }
                else
                {
                    //Debug.Log("childPossibility.leftChild: " + childPossibility.leftChild.name, childPossibility.leftChild);
                    //Debug.Log("childPossibility.rightChild: " + childPossibility.rightChild.name, childPossibility.rightChild);
                    //Debug.Log("childPossibility.bottomChild: " + childPossibility.bottomChild.name, childPossibility.bottomChild);
                    //Debug.Log("childPossibility.topChild: " + childPossibility.topChild.name, childPossibility.topChild);
                    childPossibility.leftChild.ChangeActiveState(true);
                    childPossibility.rightChild.ChangeActiveState(true);
                    childPossibility.bottomChild.ChangeActiveState(true);
                    childPossibility.topChild.ChangeActiveState(true);
                }
            }
        }
        else
        {
            if (this.SameFace)
            {
                childPossibility.leftChild.ChangeActiveState(true);
                childPossibility.rightChild.ChangeActiveState(true);
            }
            else
            {
                childPossibility.leftChild.ChangeActiveState(true);
                childPossibility.rightChild.ChangeActiveState(true);
                childPossibility.bottomChild.ChangeActiveState(true);
                childPossibility.topChild.ChangeActiveState(true);
            }
        }

        if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode1)
        {
            yield return new WaitForSeconds(0.2f);
            //Debug.Log("11111111111111111");
            StartCoroutine(GridManager.instance._CheckMultiplayerOfFive());
        }
    }

    //First Turn (Auto Play)
    public void placeFirstTile()
    {

        GridManager.instance.isFirstTurn = false;

        if (SoundManager.instance != null) SoundManager.instance.PlacetileAudioPlayer(true);

        //GameRulesManager.GameStarted = true;
        GridManager.instance.currentPlayer.currentPlacedTile = this;

        bool isFirstTileOfHighFive = false;
        if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode1)
        {
            isFirstTileOfHighFive = HighFiveRules.fillFirstSpinner(this);
            GridManager.instance.highFiveColliders.changeState(isFirstTileOfHighFive);
        }

        moveable = false;
        GridManager.instance.ChangeCollidersState(false);
        ShowFront();
        if (GridManager.instance.addedInMap.Count == 0)
        {
            Fixed = true;
            GridManager.instance.addedInMap.Add(this);

            foreach (var item in GetComponentsInChildren<TilePossibilities>())
            {
                item.gameObject.SetActive(true);
            }

            this.transform.SetParent(GridManager.instance.tableChildParent);
            LeanTween.move(transform.gameObject, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeOutQuint).setOnComplete(() => GetComponent<BoxCollider>().enabled = true);
            LeanTween.rotateLocal(this.gameObject, new Vector3(-90, (SameFace) ? 0 : 90, 0), 0.5f).setEase(LeanTweenType.easeOutQuint);

            transform.localScale = Vector3.one;

            GridManager.instance.addedInMapOnRightSide.Add(this);
            GridManager.instance.addedInMapOnLeftSide.Add(this);
        }

        if (GridManager.instance.currentPlayer.dominosCurrentList.Contains(this))
        {
            Debug.Log("Contains this tile in player list..");
            GridManager.instance.currentPlayer.dominosCurrentList.Remove(this);
        }
        else
        {
            Debug.LogError("Wait here is the error ....");
        }
        GridManager.instance.TurnSwitch = 0;
        GridManager.instance.lastTilePlayedPlayer = GridManager.instance.currentPlayer;


        StartCoroutine(GridManager.instance._ChangeTurn(this));

        //Debug.Log("1");
        StartCoroutine(_UpdatePossibilities(isFirstTileOfHighFive));
    }

    public void PlaceTileOnParticularPosition(TilePossibilities tilePossibilities)
    {
        if (SoundManager.instance != null) SoundManager.instance.PlacetileAudioPlayer(true);

        GridManager.instance.currentPlayer.currentPlacedTile = this;
        GetComponent<BoxCollider>().enabled = true;

        //Turn Off Shines and selection
        GridManager.instance.currentPlayer.UpdateShineChild();
        if (GridManager.instance.currentPlayer.dominosCurrentList.Contains(this))
        {
            Debug.Log("Contains this tile in player list..");
            GridManager.instance.currentPlayer.dominosCurrentList.Remove(this);
        }
        else
        {
            Debug.LogError("Error: Tile not found on particular player list.");
            Debug.Log("Tile = " + this, gameObject);
            Debug.Log("currentPlayer = " + GridManager.instance.currentPlayer, GridManager.instance.currentPlayer);
        }

        GridManager.instance.ChangeCollidersState(false);

        Vector3 newTileRotation = new Vector3();
        currentTileDirection = tilePossibilities.GetComponentInParent<Tile>().currentTileDirection;

        ShowFront();
        possibilityChildTile = tilePossibilities;
        initialPositionOfTile = tilePossibilities.transform.position;
        newTileRotation = tilePossibilities.transform.eulerAngles;

        if (SameFace)
        {
            if (tilePossibilities.isSamePhase)
            {
                switch (currentTileDirection)
                {
                    case TileDirection.Normal:
                        newTileRotation = new Vector3(-90, 0, 0);
                        break;
                    case TileDirection.LeftUp:
                        newTileRotation = new Vector3(-90, 0, 90);
                        break;
                    case TileDirection.LeftRight:
                        newTileRotation = new Vector3(-90, 0, 180);
                        break;
                    case TileDirection.RightDown:
                        newTileRotation = new Vector3(-90, 0, 90);
                        break;
                    case TileDirection.RightLeft:
                        newTileRotation = new Vector3(-90, 0, 180);
                        break;
                }
                Debug.Log("item.transform.eulerAngles: " + newTileRotation);
            }

        }
        else
        {
            if (!tilePossibilities.isSamePhase)
            {
                //Debug.Log(2);

                //possibilityChildTile = tilePossibilities;
                //initialPositionOfTile = tilePossibilities.transform.position;
                //Debug.Log("item.transform.eulerAngles: " + tilePossibilities.transform.eulerAngles);

                float previousTilePosition_X = float.Parse(tilePossibilities.GetComponentInParent<Tile>().transform.position.x.ToString("0.00"));
                float previousTilePosition_Y = float.Parse(tilePossibilities.GetComponentInParent<Tile>().transform.position.y.ToString("0.00"));
                float item_X = float.Parse(tilePossibilities.transform.position.x.ToString("0.00"));
                float item_Y = float.Parse(tilePossibilities.transform.localPosition.y.ToString("0.00"));
                //Debug.Log("previousTilePosition_X : " + previousTilePosition_X);
                //Debug.Log("previousTilePosition_Y : " + previousTilePosition_Y);
                //Debug.Log("item_X : " + item_X);
                //Debug.Log("item_Y : " + item_Y);

                switch (currentTileDirection)
                {
                    case TileDirection.Normal:
                        if (tilePossibilities.value == First)
                        {
                            if (item_X > previousTilePosition_X)
                            {
                                //Debug.Log("1");
                                newTileRotation = new Vector3(-90, 0, 90);
                            }
                            else if (item_X < previousTilePosition_X)
                            {
                                //Debug.Log("2");
                                newTileRotation = new Vector3(-90, 0, -90);
                            }
                            else
                            {
                                if (item_Y > previousTilePosition_Y)
                                {
                                    //Debug.Log("3");
                                    newTileRotation = new Vector3(-90, 0, 180);
                                }
                                else
                                {
                                    //Debug.Log("4");
                                    newTileRotation = new Vector3(-90, 0, 0);
                                }
                            }
                        }
                        else
                        {
                            if (item_X > previousTilePosition_X)
                            {
                                //Debug.Log("5");
                                newTileRotation = new Vector3(-90, 0, -90);
                            }
                            else if (item_X < previousTilePosition_X)
                            {
                                //Debug.Log("6");
                                newTileRotation = new Vector3(-90, 0, 90);
                            }
                            else
                            {
                                if (item_Y > previousTilePosition_Y)
                                {
                                    //Debug.Log("7");
                                    newTileRotation = new Vector3(-90, 0, 0);
                                }
                                else
                                {
                                    //Debug.Log("8");
                                    newTileRotation = new Vector3(-90, 0, 180);
                                }
                            }


                        }
                        break;

                    case TileDirection.RightDown:
                        if (tilePossibilities.value == First)
                        {
                            //Debug.Log("Flip");
                            newTileRotation = new Vector3(-90, 0, 180);
                        }
                        else
                        {
                            newTileRotation = new Vector3(-90, 0, 0);
                            //Debug.Log("Flip");
                        }
                        break;

                    case TileDirection.LeftUp:
                        if (tilePossibilities.value == First)
                        {
                            //Debug.Log("Flip");
                            newTileRotation = new Vector3(-90, 0, 0);
                        }
                        else
                        {
                            //Debug.Log("Flip");
                            newTileRotation = new Vector3(-90, 0, 180);
                        }
                        break;

                    case TileDirection.RightLeft:
                        if (tilePossibilities.value == First)
                        {
                            //Debug.Log("No Flip");
                            newTileRotation = new Vector3(-90, 0, -90);
                        }
                        else
                        {
                            //Debug.Log("Flip");
                            newTileRotation = new Vector3(-90, 0, 90);
                        }
                        break;

                    case TileDirection.LeftRight:
                        if (tilePossibilities.value == First)
                        {
                            //Debug.Log("Flip");
                            newTileRotation = new Vector3(-90, 0, 90);
                        }
                        else
                        {
                            //Debug.Log("No Flip");
                            newTileRotation = new Vector3(-90, 0, 270);
                        }
                        break;
                }
            }

        }

        transform.localScale = Vector3.one;

        LeanTween.move(transform.gameObject, initialPositionOfTile, 0.5f).setEase(LeanTweenType.easeOutQuint).setOnComplete(() => TileMovementComplete());

        //Debug.Log("newTileRotation: " + newTileRotation);
        LeanTween.rotateLocal(this.gameObject, newTileRotation, 0.3f).setEase(LeanTweenType.easeOutQuint);
        //transform.eulerAngles = newTileRotation;

        moveable = false;

        GridManager.instance.addedInMap.Add(this);

        //Add this tile in right direction List
        if (possibilityChildTile.transform.position.x < 0)
            GridManager.instance.addedInMapOnLeftSide.Add(this);
        else
            GridManager.instance.addedInMapOnRightSide.Add(this);

        this.transform.SetParent(GridManager.instance.tableChildParent);


        GridManager.instance.TurnSwitch = 0;
        GridManager.instance.lastTilePlayedPlayer = GridManager.instance.currentPlayer;


        StartCoroutine(GridManager.instance._ChangeTurn());

        int tilesCount = GridManager.instance.addedInMap.Count;

        if (tilesCount == 8)
        {
            GridManager.instance.currentSelectedCollidersParentTransform.gameObject.SetActive(true);
        }
        StartCoroutine(_UpdatePossibilities(false));
        //GridManager.instance.ChangeCollidersState(true);
    }

    void TileMovementComplete()
    {
        Debug.Log("TileMovementComplete");
        GetComponent<BoxCollider>().enabled = true;

        int tilesCount = GridManager.instance.addedInMap.Count;

        if (tilesCount < 8 && tilesCount > 2 && GridManager.instance.autoArrangementAllowed)
        {
            if (GameRulesManager.currentSelectedGame_Rule != GameRulesManager.GameRules.GameMode1)
            {
                TableChildController.instance._ArrangeTableTiles();
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //if (isPlacedOnTable)
        //{
        //    Debug.Log("OnTriggerEnter: but this tile is already Placed On Table: " + this.name, this);
        //    return;
        //}
        if (other.tag.Equals("TileChild"))
        {
            return;
        }
        Debug.Log("OnTriggerEnter(Collider other): " +  other.name, other);

        if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode1)
        {
            bool alreadyArranged = false;
            switch (other.tag)
            {
                case "AllFiveLeftMostPosition":
                    if (SameFace)
                    {
                        Debug.Log("OnTriggerEnter AllFiveLeftMostPosition same face");
                    }
                    else
                    {
                        alreadyArranged = currentTileDirection == TileDirection.LeftUp ? true : false;
                        if (!alreadyArranged)
                        {
                            currentTileDirection = TileDirection.LeftUp;
                            Debug.Log("OnTriggerEnter AllFiveLeftMostPosition same face");
                            if (possibilityChildTile.value == First)
                            {

                                //Debug.Log("Success");
                                //Debug.Log("Flip");
                                transform.eulerAngles = new Vector3(-90, 0, 0);
                            }
                            else
                            {
                                //Debug.Log("Flip");
                                transform.eulerAngles = new Vector3(-90, 0, 180);
                            }

                            //Debug.Log("Yessssssssss");
                            other.GetComponent<BoxCollider>().enabled = false;
                            GridManager.instance.autoArrangementAllowed = false;
                        }
                    }
                    break;
                case "AllFiveRightMostPosition":
                    if (SameFace)
                    {
                        Debug.Log("OnTriggerEnter AllFiveRightMostPosition same face");
                    }
                    else
                    {
                        alreadyArranged = currentTileDirection == TileDirection.RightDown ? true : false;

                        if (!alreadyArranged)
                        {
                            currentTileDirection = TileDirection.RightDown;
                            Debug.Log("OnTriggerEnter AllFiveRightMostPosition not a same face");
                            if (possibilityChildTile.value == Second)
                            {
                                //Debug.Log("Success");
                                //Debug.Log("Flip");
                                transform.eulerAngles = new Vector3(-90, 0, 0);
                            }
                            else
                            {
                                //Debug.Log("No Flip");
                                transform.eulerAngles = new Vector3(-90, 0, 180);

                            }

                            //Debug.Log("Yessssssssss");
                            other.GetComponent<BoxCollider>().enabled = false;
                            GridManager.instance.autoArrangementAllowed = false;
                        }
                    }
                    break;
                case "LeftMostPosition":
                    if (SameFace)
                    {
                        Debug.Log("OnTriggerEnter LeftMostPosition same face");
                    }
                    else
                    {
                        alreadyArranged = currentTileDirection == TileDirection.LeftUp ? true : false;
                        if (!alreadyArranged)
                        {
                            currentTileDirection = TileDirection.LeftUp;
                            Debug.Log("OnTriggerEnter LeftMostPosition not a same face");
                            if (possibilityChildTile.value == First)
                                this.transform.eulerAngles = new Vector3(-90, 0, 0);
                            else
                                this.transform.eulerAngles = new Vector3(-90, 0, 180);

                            transform.position = new Vector3(transform.position.x + 0.06f, transform.position.y, transform.position.z + 0.06f);
                            other.GetComponent<BoxCollider>().enabled = false;
                            GridManager.instance.autoArrangementAllowed = false;
                        }
                    }
                    break;
                case "RightMostPosition":
                    if (SameFace)
                    {
                        Debug.Log("OnTriggerEnter RightMostPosition same face");
                    }
                    else
                    {
                        alreadyArranged = currentTileDirection == TileDirection.RightDown ? true : false;
                        if (!alreadyArranged)
                        {
                            currentTileDirection = TileDirection.RightDown;
                            Debug.Log("OnTriggerEnter RightMostPosition not a same face");
                            if (possibilityChildTile.value == First)
                                this.transform.eulerAngles = new Vector3(-90, 0, 180);
                            else
                                this.transform.eulerAngles = new Vector3(-90, 0, 0);

                            transform.position = new Vector3(transform.position.x - 0.06f, transform.position.y, transform.position.z - 0.06f);
                            other.GetComponent<BoxCollider>().enabled = false;
                            GridManager.instance.autoArrangementAllowed = false;
                        }
                    }
                    break;
                case "TopMostPosition":
                    if (SameFace)
                    {
                        Debug.Log("OnTriggerEnter RightMostPosition same face");
                    }
                    else
                    {
                        alreadyArranged = currentTileDirection == TileDirection.LeftRight ? true : false;
                        if (!alreadyArranged)
                        {
                            currentTileDirection = TileDirection.LeftRight;
                            Debug.Log("OnTriggerEnter TopMostPosition not a same face");
                            if (possibilityChildTile.value == First)
                            {
                                transform.eulerAngles = new Vector3(-90, 0, 90);
                                foreach (var item in GetComponentsInChildren<TilePossibilities>(false))
                                {
                                    if (item.value == First)
                                    {
                                        //Debug.Log("**********Yesss************");
                                        item.gameObject.SetActive(false);
                                    }
                                }
                            }
                            else
                            {
                                this.transform.eulerAngles = new Vector3(-90, 0, -90);
                                foreach (var item in GetComponentsInChildren<TilePossibilities>(false))
                                {
                                    if (item.value == Second)
                                    {
                                        //Debug.Log("**********Yesss************");
                                        item.gameObject.SetActive(false);
                                    }
                                }
                            }

                            transform.position = new Vector3(transform.position.x + 0.06f, transform.position.y, transform.position.z - 0.06f);
                            other.GetComponent<BoxCollider>().enabled = false;
                            GridManager.instance.autoArrangementAllowed = false;
                        }
                    }
                    break;
                case "BottomMostPosition":
                    if (SameFace)
                    {
                        Debug.Log("OnTriggerEnter RightMostPosition same face");
                    }
                    else
                    {
                        alreadyArranged = currentTileDirection == TileDirection.RightLeft ? true : false;
                        if (!alreadyArranged)
                        {
                            currentTileDirection = TileDirection.RightLeft;
                            Debug.Log("OnTriggerEnter RightMostPosition not a same face");
                            if (possibilityChildTile.value == First)
                            {
                                transform.eulerAngles = new Vector3(-90, 0, -90);
                                foreach (var item in GetComponentsInChildren<TilePossibilities>(false))
                                {
                                    if (item.value == First)
                                    {
                                        //Debug.Log("**********Yesss************");
                                        item.gameObject.SetActive(false);
                                    }
                                }
                            }
                            else
                            {
                                transform.eulerAngles = new Vector3(-90, 0, 90);
                                foreach (var item in GetComponentsInChildren<TilePossibilities>(false))
                                {
                                    if (item.value == Second)
                                    {
                                        //Debug.Log("**********Yesss************");
                                        item.gameObject.SetActive(false);
                                    }
                                }
                            }

                            transform.position = new Vector3(transform.position.x - 0.06f, transform.position.y, transform.position.z + 0.06f);
                            other.GetComponent<BoxCollider>().enabled = false;
                            GridManager.instance.autoArrangementAllowed = false;
                        }
                    }
                    break;
            }
        }
        else
        {
            switch (other.tag)
            {
                case "LeftMostPosition":
                    if (SameFace)
                    {
                        Debug.Log("OnTriggerEnter LeftMostPosition same face");
                        //this.transform.eulerAngles = new Vector3(-90, 0, 90);
                        //transform.position = new Vector3(transform.position.x + 0.06f, transform.position.y, transform.position.z + 0.13f);
                    }
                    else
                    {
                        Debug.Log("OnTriggerEnter LeftMostPosition not a same face");

                        if (possibilityChildTile.value == First)
                            this.transform.eulerAngles = new Vector3(-90, 0, 0);
                        else
                            this.transform.eulerAngles = new Vector3(-90, 0, 180);

                        transform.position = new Vector3(transform.position.x + 0.06f, transform.position.y, transform.position.z + 0.06f);
                        currentTileDirection = TileDirection.LeftUp;
                        other.GetComponent<BoxCollider>().enabled = false;
                        GridManager.instance.autoArrangementAllowed = false;
                    }
                    break;

                case "RightMostPosition":
                    if (SameFace)
                    {
                        Debug.Log("OnTriggerEnter RightMostPosition same face");
                        //this.transform.eulerAngles = new Vector3(-90, 0, 90);
                        //transform.position = new Vector3(transform.position.x - 0.06f, transform.position.y, transform.position.z - 0.13f);
                    }
                    else
                    {
                        Debug.Log("OnTriggerEnter RightMostPosition not a same face");
                        if (possibilityChildTile.value == First)
                            this.transform.eulerAngles = new Vector3(-90, 0, 180);
                        else
                            this.transform.eulerAngles = new Vector3(-90, 0, 0);

                        transform.position = new Vector3(transform.position.x - 0.06f, transform.position.y, transform.position.z - 0.06f);
                        currentTileDirection = TileDirection.RightDown;
                        other.GetComponent<BoxCollider>().enabled = false;
                        GridManager.instance.autoArrangementAllowed = false;
                    }
                    break;

                case "TopMostPosition":
                    if (SameFace)
                    {
                        Debug.Log("OnTriggerEnter TopMostPosition same face");
                        //this.transform.eulerAngles = new Vector3(-90, 0, 180);
                        //transform.position = new Vector3(transform.position.x + 0.13f, transform.position.y, transform.position.z - 0.08f);
                    }
                    else
                    {
                        Debug.Log("OnTriggerEnter TopMostPosition not a same face");
                        if (possibilityChildTile.value == First)
                        {
                            this.transform.eulerAngles = new Vector3(-90, 0, 90);
                            foreach (var item in GetComponentsInChildren<TilePossibilities>(false))
                            {
                                if (item.value == First)
                                {
                                    //Debug.Log("**********Yesss************");
                                    item.gameObject.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            this.transform.eulerAngles = new Vector3(-90, 0, -90);
                            foreach (var item in GetComponentsInChildren<TilePossibilities>(false))
                            {
                                if (item.value == Second)
                                {
                                    //Debug.Log("**********Yesss************");
                                    item.gameObject.SetActive(false);
                                }
                            }

                        }
                        transform.position = new Vector3(transform.position.x + 0.06f, transform.position.y, transform.position.z - 0.06f);
                        currentTileDirection = TileDirection.LeftRight;
                        other.GetComponent<BoxCollider>().enabled = false;
                        GridManager.instance.autoArrangementAllowed = false;
                    }
                    break;

                case "BottomMostPosition":
                    if (SameFace)
                    {
                        Debug.Log("OnTriggerEnter BottomMostPosition same face");
                        //this.transform.eulerAngles = new Vector3(-90, 0, 180);
                        //transform.position = new Vector3(transform.position.x -0.13f, transform.position.y, transform.position.z + 0.08f);
                    }
                    else
                    {
                        Debug.Log("OnTriggerEnter RightMostPosition not a same face");
                        if (possibilityChildTile.value == First)
                        {
                            this.transform.eulerAngles = new Vector3(-90, 0, -90);
                            foreach (var item in GetComponentsInChildren<TilePossibilities>(false))
                            {
                                if (item.value == First)
                                {
                                    //Debug.Log("**********Yesss************");
                                    item.gameObject.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            this.transform.eulerAngles = new Vector3(-90, 0, 90);
                            foreach (var item in GetComponentsInChildren<TilePossibilities>(false))
                            {
                                if (item.value == Second)
                                {
                                    //Debug.Log("**********Yesss************");
                                    item.gameObject.SetActive(false);
                                }
                            }
                        }
                        transform.position = new Vector3(transform.position.x - 0.06f, transform.position.y, transform.position.z + 0.06f);
                        currentTileDirection = TileDirection.RightLeft;
                        other.GetComponent<BoxCollider>().enabled = false;
                        GridManager.instance.autoArrangementAllowed = false;
                    }
                    break;
            }
        }
    }
}

[Serializable]
public class TileChilds
{
    public TilePossibilities leftChild;
    public TilePossibilities rightChild;
    public TilePossibilities topChild;
    public TilePossibilities bottomChild;
}

