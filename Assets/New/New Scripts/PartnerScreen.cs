using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartnerScreen : MonoBehaviour
{
    public Button closeBtn;

    private void Start()
    {
        closeBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.partnerScreen.gameObject, false));

    }
}
