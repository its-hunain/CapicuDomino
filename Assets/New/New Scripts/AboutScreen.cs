using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AboutScreen : MonoBehaviour
{
    public Button closeBtn;

    public Text headingTxt;
    public Text contentTxt;

    private void Start()
    {
        closeBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.aboutScreen.gameObject, false));

    }
    public void UpdateUI(string heading=null,string context=null)
    {
        if (heading != null)
        {
            headingTxt.text = heading;
        }
        if (context != null)
        {
            contentTxt.text = context;
        }

    }
}
