using System;
using UnityEngine;
using UnityEngine.UI;
using static GameRulesManager;

public class RuleSelectionButton : MonoBehaviour
{
    public GameRules gameRule;
    public Button thisBtn;
    public GameObject greenCircle;
    public GameObject popUp;

    public Sprite greySprite;
    public Sprite RedSprite;
    public Image btnImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSelectable(bool state)
    {
        UI_ScreenManager.OpenClosePopUp(greenCircle, state, false);
        UI_ScreenManager.OpenClosePopUp(popUp, state, false);
    }

    public void EnableDisableBtn(bool state)
    {
        thisBtn.interactable = state;
        btnImage.sprite = (state == true) ? RedSprite : greySprite;
    }

    internal void ChangeRuleButtonState(bool state)
    {
        EnableDisableBtn(state);

    }
}
