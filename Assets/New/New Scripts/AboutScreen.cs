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
}
