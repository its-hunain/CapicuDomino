using UnityEngine;
using UnityEngine.UI;

public class PlayOrReject : MonoBehaviour
{


    public string roomID;
    public Button playButton;
    public Button rejectedButton;

    public RawImage OppPlayerImage;
    public Text DescriptionText;

    private void Awake()
    {
        playButton.onClick.AddListener(() => OnPlayButton());
        rejectedButton.onClick.AddListener(() => OnRejectButton());
    }

    public void fetchANDSetFriendPic(string userID)
    {
        FriendDetail friendInfo =  FacebookManager.friendList.Find(x=> x.friendUserID.Equals(userID));
        OppPlayerImage.texture = friendInfo.friendPic; // error raised
        DescriptionText.text = friendInfo.friendName + " Invited you to Play..";

    }

    private void OnPlayButton()
    {
        //if (roomID != null)
        //{
        //    PlayerPersonalData.GameId = Global.currentGameId;
        //    //leave current room if joined
        //    //agr mein kisi room mein hn or mujhe new game request aagai or mene accept krli, tw pichla room left hoga or new join hoga
        //    if (PhotonNetwork.InRoom)
        //    {
        //        Debug.Log("user is in room, calling _joinNewRoomAfterLeaving");
        //    }
        //    else
        //    {
        //            //PhotonRoomCreator.instance.joinDedicatedRoom(roomID);
        //            this.gameObject.SetActive(false);
        //    }
        //}
    }

    //public override void OnLeftRoom()
    //{
    //    base.OnLeftRoom();

    //}

    private void OnRejectButton()
    {
        if (roomID != null)
        {
            this.gameObject.SetActive(false);
        }

        OppPlayerImage.texture = null;
        DescriptionText.text = "";
    }
}
