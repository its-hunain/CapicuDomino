using UnityEngine;
using UnityEngine.UI;

public class RulesReadMorePopUp : MonoBehaviour
{
    //Because this script is attached on both popup so this bool will help in independent work
    public bool isFullScreen;

    private string[] RulesName = { "Game Mode 1", "Game Mode 2", "Game Mode 3", "Game Mode 4", "Game Mode 5", "Game Mode 6" };
    private string[] RulesMiniDescription =
        {
            "Objective: The game ends when a player reaches or exceeds 200 points.",
            "Objective: To score the least amount of points, the game ends when 1 player completes 121 points or more.",
            "Objective: The game ends when a player reaches or exceeds 100 points.",
            "Objective: The game ends when a player reaches or exceeds 20 points.",
            "Objective: The player who wins the hand with the fewest points wins the game.",
            "Objective: The game ends when the player reaches or exceeds 75 points."
        };

    public Button disablePopUpBtn;

    [Header("PopUp Items")]
    public Text ruleHeading;
    public Text ruleDescription;
    public Button playNowButton;
    public GameObject lastRuleObjectPopup;

    void Start()
    {
        disablePopUpBtn.onClick.AddListener(() => backButtonDelegate());
        playNowButton.onClick.AddListener(() => MoveToNextScreen());
    }

    private void OnEnable()
    {
        int ruleIndex = (int)GameRulesManager.currentSelectedGame_Rule;
        ruleHeading.text = RulesName[ruleIndex];
        var loaded_text_file = Resources.Load<TextAsset>("Rules/Rule"+(ruleIndex + 1));
        GameRulesManager.currentSelectedGame_RuleDescription = loaded_text_file.text;
        if (!isFullScreen)
        {
            //string originalString = GameRulesManager.currentSelectedGame_RuleDescription;
            //string originalString = RulesMiniDescription[ruleIndex];
            ruleDescription.text = RulesMiniDescription[ruleIndex];
            ruleDescription.text += "<color=#4fd322>\nRead More...</color>";
        }
        else
        {
            ruleDescription.text = GameRulesManager.currentSelectedGame_RuleDescription;
        }
    }

    private void MoveToNextScreen()
    {
        SoundManager.instance.ButtonPressPlayer(true);
        UI_ScreenManager.instance.ruleSelectionScreen.OnReset();
        Invoke("Delay",.6f);
    }

    private void Delay()
    {
        UI_ScreenManager.instance.StartCoroutine(UI_ScreenManager._ChangeGlobeScreen(UI_ScreenManager.instance.no_Of_Player_SelectionScreen.gameObject, UI_ScreenManager.instance.ruleSelectionScreen.gameObject, true));
        UI_ScreenManager.OpenClosePopUp(this.gameObject, false, false);
    }

    private void backButtonDelegate()
    {
        SoundManager.instance.ButtonPressPlayer(true);
        UI_ScreenManager.OpenClosePopUp(gameObject, false, false);
        if (lastRuleObjectPopup != null)
            lastRuleObjectPopup.gameObject.SetActive(true);
            //UI_ScreenManager.OpenClosePopUp(lastRuleObjectPopup, true, false);
    }

    /// <summary>
    /// Direct On Click Inspector
    /// </summary>
    public void ShowPopUpButtonDelegate(RulesReadMorePopUp rulesReadMorePopUp)
    {
        Debug.Log("ShowPopUpButtonDelegate");
        SoundManager.instance.ButtonPressPlayer(true);
        UI_ScreenManager.OpenClosePopUp(gameObject, true, true);
        lastRuleObjectPopup = rulesReadMorePopUp.gameObject;
        lastRuleObjectPopup.gameObject.SetActive(false);

        //UI_ScreenManager.OpenClosePopUp(lastRuleObjectPopup, false, false);
        
    }

}