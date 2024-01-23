using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUIPanel : MonoBehaviour
{


    [Header("Black Image")]
    public GameObject showBlackScreen;

    [Header("BONE YARD TEXT")]
    public Text boneYardText;

    [Header("Top Right Game Buttons")]
    public Button leaveGameBtn;
    public Button soundBtn;

    [Header("Sprites")]
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    [Header("Screen/PopUp")]
    public QuitGamePopup quitGamePopup;
    public PlayerStatesPopUp playerStatesPopUp;

    [Space]
    public GameObject RoundTotalScoreBadge;
    public GameObject ScoreBadgePrefab;
    public WinnerScreen WinnerScreen;

        //player tiles container
    public List<Player> players = new List<Player>();

    //Extra boneyard places
    public List<Transform> boneYardPlaces = new List<Transform>();


    public Text ResultText;

    [Header("PopUps")]
    public MessagePopUp messagePopUp;
    public MessagePopUp messagePopUp2;

    public RectTransform FinalPopUpPos;

    public GameObject Environment;

    [Header("Headings")]
    public Text gameNameHeading;
    public Text roundNumberHeading;
    public Text scoreToWinHeading;
    public Transform boneyardTileImage;

    //[Header("Sprites")]
    //public List<Sprite> flagSprites = new List<Sprite>();
    //public List<Sprite> classSprites = new List<Sprite>();

    public static GamePlayUIPanel instance;

    void Awake()
    {
        instance = this;
        if(SoundManager.instance!=null) SoundManager.instance.GameplayBGPlayer(true);
    }

    private void Start()
    {
        leaveGameBtn.onClick.AddListener(()=> OpenClosePopUp(quitGamePopup.gameObject,true,true));
        soundBtn.onClick.AddListener(()=> soundBtnClickedEvent());
    }
    public static void UpdateBoneYardText(int value)
    {
        instance.boneYardText.text = value.ToString();
    }
    private void soundBtnClickedEvent()
    {
        if (soundBtn.image.sprite == soundOnSprite)
        {
            soundBtn.image.sprite = soundOffSprite;
            AudioListener.volume = 0;
        }
        else
        {
            soundBtn.image.sprite = soundOnSprite;
            AudioListener.volume = 1;
        }
    }

    public void EnvironmentToggling()
    {
        Environment.SetActive(!Environment.activeInHierarchy);
    }

    public void SetData(bool isWin, Texture2D tex, double Coins, Player winner = null)
    {
        StartCoroutine(_SetDataAfterDelay(isWin, tex, Coins , winner)); //Because this game object is off, so coroutine is in gamePlay UI Script
    }

    public IEnumerator _SetDataAfterDelay(bool isWin, Texture2D tex, double Coins, Player winner)
    {
        Debug.Log("_SetDataAfterDelay Start");

        yield return new WaitForSeconds(2f);
        WinnerScreen.Status.transform.GetChild(0).gameObject.SetActive(isWin);   //victory ribbon
        WinnerScreen.Status.transform.GetChild(1).gameObject.SetActive(!isWin);  //loss ribbon

        if (isWin)
        {
            WinnerScreen.DomiCoinStatus.text = "Domicoin Won";
            WinnerScreen.WinBorder.gameObject.SetActive(true);
            WinnerScreen.LostBorder.gameObject.SetActive(false);
        }
        else
        {
            WinnerScreen.DomiCoinStatus.text = "Domicoin Lost";
            WinnerScreen.WinBorder.gameObject.SetActive(false);
            WinnerScreen.LostBorder.gameObject.SetActive(true);
        }

        if (quitGamePopup.gameObject.activeInHierarchy)
        {
            quitGamePopup.gameObject.SetActive(false);
        }
        if (playerStatesPopUp.gameObject.activeInHierarchy)
        {
            playerStatesPopUp.gameObject.SetActive(false);
        }


        OpenClosePopUp(WinnerScreen.gameObject, true, true);
        //gameObject.SetActive(true);
        if(tex!=null) WinnerScreen.Avatar.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        WinnerScreen.DomiCoins.text = Coins.ToString();


        if(winner!=null) ShowWinnerEffect(winner);
        Debug.Log("_SetDataAfterDelay End");
    }

    public void PopUpController(RectTransform initialPos_Transform , string msg)
    {
        Debug.Log("PopUpController: " + initialPos_Transform.transform.position);
        messagePopUp.transform.position = initialPos_Transform.position;
        messagePopUp.ShowData(msg);
        messagePopUp.gameObject.SetActive(true);
        StartCoroutine(FadeTo(messagePopUp.GetComponent<Image>() ,1, 0 , 1));
    }
    
    public void PopUpController(string msg , Texture2D texture2D)
    {
        //Debug.Log("PopUpController: " + initialPos_Transform.transform.position);
        
        messagePopUp2.transform.position = GridManager.instance.HeadingPos.position;
        //messagePopUp2.transform.position = initialPos_Transform.position;
        messagePopUp2.ShowData(msg , texture2D);
        messagePopUp2.gameObject.SetActive(true);
        StartCoroutine(FadeTo(messagePopUp2.GetComponent<Image>() ,1, 0 , 1));
    }
    
    IEnumerator FadeTo(Image MessagePopUp, float fromValue, float toValue, float alphaTime)
    {
        //MessagePopUp.color = new Color(1,1,1,fromValue);
        Vector3 initialPos = MessagePopUp.gameObject.GetComponent<RectTransform>().position;
        LeanTween.move(MessagePopUp.gameObject, FinalPopUpPos, 0.5f);
        yield return new WaitForSeconds(1.5f);
        MessagePopUp.gameObject.SetActive(false);
        MessagePopUp.gameObject.GetComponent<RectTransform>().position = initialPos;
        //float alpha = MessagePopUp.color.a;
        //for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / alphaTime)
        //{
        //    Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, toValue, t));
        //    MessagePopUp.color = newColor;
        //    yield return null;
        //}
    }

    public void ShowWinnerEffect(Player winner)
    {
        Debug.Log("ShowWinnerEffect");

        StartCoroutine(_PartyPopper(winner));
    }

    IEnumerator _PartyPopper(Player winner)
    {
        Debug.Log("_PartyPopper");
        winner.playerPhysicalPosition.playerProfileParticleSystem.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        winner.playerPhysicalPosition.playerProfileParticleSystem.gameObject.SetActive(false);
        winner.playerPhysicalPosition.playerProfileParticleSystem.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        winner.playerPhysicalPosition.playerProfileParticleSystem.gameObject.SetActive(false);
        if (Time.timeScale != 0) Time.timeScale = 0;
    }

    public void UpdateRound()
    {
        GridManager.roundNum++;
        UpdateHeadingTexts();
    }

    public void UpdateHeadingTexts()
    {
        int ruleIndex = (int)GameRulesManager.currentSelectedGame_Rule + 1;
        //if (ruleIndex == 1)
        //{
        //    gameNameHeading.text = GameRulesManager.currentSelectedGame_Rule.ToString(); //High Five
        //}
        //else
        {
            gameNameHeading.text = ruleIndex.ToString(); //2, 3, 4 ,5 ,6
        }

        roundNumberHeading.text = GridManager.roundNum.ToString();
        scoreToWinHeading.text = GameRulesManager.currentSelectedGame_ScoreToWin.ToString();
    }

    public static void OpenClosePopUp(GameObject panel, bool doOpen, bool showBlackBG)
    {
        if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);

        if (doOpen)
        {
            instance.showBlackScreen.SetActive(showBlackBG);
            panel.SetActive(true);
            LeanTween.scale(panel, Vector3.one, .5f).setEaseOutBack();
        }
        else
        {
            LeanTween.scale(panel, Vector3.zero, .5f).setEaseInBack().setOnComplete(() => OnComplete(panel, showBlackBG));
        }
    }

    static void OnComplete(GameObject panel, bool showBlackBG)
    {
        panel.SetActive(false);
        instance.showBlackScreen.SetActive(showBlackBG);
    }

    public static void arrangeProfilesOrderBasedOnMySeatNumber(int mySeatIndex)
    {
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
        {
            return; //Do Nothing
        }

        int noOfPlayers = GameRulesManager.noOfPlayers;
        int mySeatNum   = mySeatIndex;
        foreach (var item in instance.players)
        {
            item.gameObject.SetActive(true);
        }

        if (noOfPlayers == 3 || noOfPlayers == 4)
        {
            switch (mySeatNum)
            {
                case 0:
                    //Default Arrangement.
                    instance.players[0].playerPersonalData.playerSeatID = 0;
                    instance.players[1].playerPersonalData.playerSeatID = 1;
                    instance.players[2].playerPersonalData.playerSeatID = 2;
                    instance.players[3].playerPersonalData.playerSeatID = 3;

                    instance.players[3].gameObject.SetActive(noOfPlayers == 4);
                    break;
                case 1:
                    instance.players[0].playerPersonalData.playerSeatID = 1;
                    instance.players[1].playerPersonalData.playerSeatID = 2; 
                    instance.players[2].playerPersonalData.playerSeatID = 3;
                    instance.players[3].playerPersonalData.playerSeatID = 0;

                    instance.players[2].gameObject.SetActive(noOfPlayers == 4);
                    break;

                case 2:
                    instance.players[0].playerPersonalData.playerSeatID = 2;
                    instance.players[1].playerPersonalData.playerSeatID = 3; 
                    instance.players[2].playerPersonalData.playerSeatID = 0;
                    instance.players[3].playerPersonalData.playerSeatID = 1;

                    instance.players[1].gameObject.SetActive(noOfPlayers == 4);
                    break;
                case 3:
                    instance.players[0].playerPersonalData.playerSeatID = 3;
                    instance.players[1].playerPersonalData.playerSeatID = 0;
                    instance.players[2].playerPersonalData.playerSeatID = 1;
                    instance.players[3].playerPersonalData.playerSeatID = 2;
                    break;
            }
        }
        else //2 Player game special arrangement
        {
            //Default Arrangement.
            instance.players[1].gameObject.SetActive(false);
            instance.players[3].gameObject.SetActive(false);

            switch (mySeatNum)
            {
                case 0:
                    instance.players[0].playerPersonalData.playerSeatID = 0;//
                    instance.players[1].playerPersonalData.playerSeatID = 2;
                    instance.players[2].playerPersonalData.playerSeatID = 1;//
                    instance.players[3].playerPersonalData.playerSeatID = 3;

                    break;

                case 1:
                    instance.players[0].playerPersonalData.playerSeatID = 1;//
                    instance.players[1].playerPersonalData.playerSeatID = 2;
                    instance.players[2].playerPersonalData.playerSeatID = 0;//
                    instance.players[3].playerPersonalData.playerSeatID = 3;

                    ////Assign Physical Seat
                    //instance.players[0].playerPhysicalPosition.currentplayerPosition = SpawnPointManger.instance.points[1];
                    //instance.players[1].playerPhysicalPosition.currentplayerPosition = SpawnPointManger.instance.points[2];
                    //instance.players[2].playerPhysicalPosition.currentplayerPosition = SpawnPointManger.instance.points[0];
                    //instance.players[3].playerPhysicalPosition.currentplayerPosition = SpawnPointManger.instance.points[3];
                    break;
            }
        }

        List<Player> playersCache = instance.players;//.OrderBy(x => x.playerPersonalData.playerSeatID).ToList();

        instance.players = instance.players.OrderBy(x => x.playerPersonalData.playerSeatID).ToList();

        foreach (var item in playersCache)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                instance.players.Remove(item);
                item.playerPhysicalPosition.currentplayerPosition.gameObject.SetActive(false);
            }
        }


    }
}
