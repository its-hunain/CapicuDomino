using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsScreen : MonoBehaviour
{
    public Button backBtn;
    public Button closeBtn;

    public List<Button> coinBtn;

    void Start()
    {
        backBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.coinsScreen.gameObject, false));

        foreach (var item in coinBtn)
        {
            item.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.noOfPlayers.gameObject, true));

        }

    }

}
