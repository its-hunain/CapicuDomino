using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MonoBehaviour
{
    public Button coinsBtn;
    public Button tilesBtn;
    public Button tablesBtn;
    public Button backBtn;
    public Button settingsBtn;


    public GameObject coinsScroll;
    public GameObject tilesScroll;
    public GameObject tablesScroll;

    public Image selectionImage;


    // Start is called before the first frame update
    void Start()
    {
        coinsBtn.onClick.AddListener(() => SwitchPanels(coinsBtn,coinsScroll));
        tilesBtn.onClick.AddListener(() => SwitchPanels(tilesBtn,tilesScroll));
        tablesBtn.onClick.AddListener(() => SwitchPanels(tablesBtn,tablesScroll));

        backBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.shopScreen.gameObject, false));
        settingsBtn.onClick.AddListener(() => UI_Manager.instance.ChangeScreen(UI_Manager.instance.settingScreen.gameObject, true));

    }

    void SwitchPanels(Button btn, GameObject panel)
    {
        print("karwa");
        coinsScroll.SetActive(false);
        tilesScroll.SetActive(false);
        tablesScroll.SetActive(false);

        panel.SetActive(true);
        selectionImage.transform.position = btn.transform.position;
    }
}
