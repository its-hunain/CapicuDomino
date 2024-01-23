using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    public Button gameSoundBtn;
    public Button musicBtn;
    public Button logoutBtn;
    public Button aboutBtn;
    public Button termsBtn;
    public Button policyBtn;
    public Button closeBtn;

    private void Start()
    {
        closeBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.settingScreen.gameObject, false));

        aboutBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.aboutScreen.gameObject, true));
        logoutBtn.onClick.AddListener(() => LogOut());
    }

    public void LogOut()
    {
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.settingScreen.gameObject, false);
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.menuScreen.gameObject, false);
    }

}
