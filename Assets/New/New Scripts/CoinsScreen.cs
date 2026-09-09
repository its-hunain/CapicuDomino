using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsScreen : MonoBehaviour
{
    public Button closeBtn;


    void Start()
    {
        closeBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.coinsScreen.gameObject, false));
    }
   
     


}
