using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateJoinRoomButtonPanel : MonoBehaviour
{
    public Button backBtn;
    public Button createBtn;
    public Button joinBtn;

    // Start is called before the first frame update
    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnCallBack());
        createBtn.onClick.AddListener(() => CreateBtnCallBack());
        joinBtn.onClick.AddListener(() => JoinBtnCallBack());

    }


    public void CreateBtnCallBack()
    {
        GameRulesManager.isCreatingRoom = true;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, true);
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.createJoinRoomButtonPanel.gameObject, false);
    }

    public void JoinBtnCallBack()
    {
        GameRulesManager.isCreatingRoom = false;
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.joinRoomScreen.gameObject, true);
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.createJoinRoomButtonPanel.gameObject, false);
    }

    public void BackBtnCallBack()
    {
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.createJoinRoomButtonPanel.gameObject, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
