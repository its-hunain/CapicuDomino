using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeScreen : MonoBehaviour
{
    public Button closeBtn;

    public Button blocking; //replace with gameMode type object
    public Button draw; //replace with gameMode type object
    public Button muggins; //replace with gameMode type object


    void Start()
    {
        closeBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, false));

        blocking.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.coinsScreen.gameObject, true));
        draw.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.coinsScreen.gameObject, true));
        muggins.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.coinsScreen.gameObject, true));
    }

}
