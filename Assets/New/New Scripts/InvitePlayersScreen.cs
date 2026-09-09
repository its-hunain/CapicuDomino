using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InvitePlayersScreen : MonoBehaviour
{
    public Button backBtn;
    public Button nextBtn;
    public Button inviteBtn;

    public InputField roomID;


    // Start is called before the first frame update
    void Start()
    {
        backBtn.onClick.AddListener(() => BackBtnCallBack());
        nextBtn.onClick.AddListener(() => NextBtnCallBack());
        inviteBtn.onClick.AddListener(() => InviteBtnCallBack());

    }

    private void OnEnable()
    {
        roomID.text = GenerateRandomRoomId();
    }

    private string GenerateRandomRoomId()
    {
        int num = Random.Range(500,5000); 
        string roomId = GameRulesManager.privateRoomId = "CAPICU" + num.ToString();
        return roomId;
    }

    public void InviteBtnCallBack()
    {
        if (string.IsNullOrEmpty(roomID.text.Trim()))
        {
            MesgBar.instance.show("Invalid Room Id", true);
        }
        else
        {
            //UI_Manager.instance.ChangeScreen(UI_Manager.instance.createJoinRoomButtonPanel.gameObject, false);
            //UI_Manager.instance.ChangeScreen(UI_Manager.instance.joinRoomScreen.gameObject, false);
          
            NativeShare nativeShare = new NativeShare();

            string GameRule = "Capicu";

            if (Rule5.isNines)
            {
                GameRule = "NINES";
            }
            else
            {
                if (GameRulesManager.currentSelectedGame_Rule == GameRulesManager.GameRules.GameMode4)
                {
                    GameRule = "BLOCK";
                }
                else 
                { 
                    GameRule = "CAPICU";
                }
            }
            string gameDetails = "Your friend invited you to join a Capicu Table."
                                + "\n Room Id: " + roomID.text
                                + "\n No Of Players: " + GameRulesManager.noOfPlayers
                                + "\n Game Type: " + GameRule;


            Debug.Log(gameDetails);
            nativeShare.SetSubject("Game Request")
                .SetTitle("Capicu Game")
                .SetText(gameDetails).Share();
        }
    }

    public void BackBtnCallBack()
    {
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.invitePlayersScreen.gameObject, false);
    }

    public void NextBtnCallBack()
    {
        SceneManager.LoadScene(Global.GameScene);
    }

}
