using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GameRulesManager;

public class GameCenterSelectionScreen : MonoBehaviour
{
    public static string selectedGameCenterID;

    [SerializeField]
    public GameCenters gameCenters;
    public GameCenter_ItemRow gameCenterItemPrefab;
    public GameCenterReadMorePopUp gameCenterReadMorePopUp;

    public Button backBtn;
    public Transform contentParent;

    Animation anim;
    AnimationClip animClip;

    private void Awake()
    {
        if(anim == null) anim = GetComponent<Animation>();
        if(animClip == null) animClip = anim.GetClip("GameCenterSelectionScreen");
    }

    public void OnEnable()
    {
        Debug.Log("OnEnable", gameObject);

        anim[animClip.name].speed = 1;
        anim.Play(animClip.name);
    }

    private void Start()
    {
        //LoadGameCenterData();
        backBtn.onClick.AddListener(() => backButtonDelegate());
    }

    private void backButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);
        gameObject.SetActive(false);
        UI_ScreenManager.instance.playGameScreen.gameObject.SetActive(true);
        //StartCoroutine(UI_ScreenManager._Change1stScreen(UI_ScreenManager.instance.playGameScreen.gameObject, gameObject, false));
    }  

    private void LoadGameCenterData()
    {
        foreach (var item in gameCenters.data)
        {
            GameCenter_ItemRow gameCenter_ItemRow = Instantiate(gameCenterItemPrefab, contentParent).GetComponent<GameCenter_ItemRow>();
            gameCenter_ItemRow.gameCenterID = item._id;
            gameCenter_ItemRow.gameDescription = item.gameCenterDescription;
            gameCenter_ItemRow.gameCenterName.text = item.gameCenterName;
            gameCenter_ItemRow.gameCenterDescription.text = item.gameCenterDescription.Length >= 81 ? item.gameCenterDescription.Substring(0, 81) + " ..." : item.gameCenterDescription +" ...";
            string imageURL = item.gameCenterImageURL;
            WebServiceManager.instance.DownloadAndSetTexture(imageURL, false, gameCenter_ItemRow.gameCenterImage);
        }
    }

    public void OnSuccessfullyDataDownloaded(JObject data, long code)
    {
        if (ResponseStatus.Check(code))
        {
            var response = JsonConvert.DeserializeObject<GameCenters>(data.ToString());
            gameCenters = response;
            LoadGameCenterData();
        }
        else
        {
            Debug.LogError(code);
        }
    }
}

[Serializable]
public class GameCenters
{
    public List<GameCenterDetails> data = new List<GameCenterDetails>();
}

[Serializable]
public class GameCenterDetails
{
    public string _id;
    public string gameCenterName;
    public string gameCenterDescription;
    public string gameCenterImageURL;
    public List<RuleDetails> gameCenterGames = new List<RuleDetails>();
}

[Serializable]
public class RuleDetails
{
    public string _id = "";
    public List<Rule> Rules = new List<Rule>();
}

[Serializable]
public partial class Rule
{
    public string _id = "";
    public GameRules gameRulesName;
    public string gameRulesDescription = "";
    public List<int> noOfPlayers = new List<int>();
    public List<int> coins = new List<int>();
}