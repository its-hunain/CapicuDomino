using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon;

public class FriendRow : MonoBehaviour
{
    public string userID;
    public string userName;

    public Text friendName_Text;
    public Text coins_Text;
    public Image friend_Image;


    public Button inviteBtn;
    public Image inviteBtnImage;

    public Sprite unselectedImage;
    public Sprite selectedImage;

    public bool doInvite;


    private void Start()
        =>  inviteBtn.onClick.AddListener(()=> SelectPartner());

    private void SelectPartner() 
    {
        doInvite = !doInvite;
        inviteBtnImage.sprite = doInvite ? selectedImage : unselectedImage;
    }

    public void SendGameRequest(string roomName, string playerName, string playerUserID, string friendUserID)
    {
        Debug.Log("roomName: " + roomName + 
                    "\n" + "playerName: " + playerName + 
                    "\n" + "playerUserID: " + playerUserID + 
                    "\n" + "friendUserID: " + friendUserID 
                    );
    }

    public void Setter(string userID, string userName, Texture2D texture2D)
    {
        this.userID = userID;
        friendName_Text.text = this.userName = userName;
        var friendPic = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100.0f);
        friend_Image.sprite = friendPic;
    }
}
