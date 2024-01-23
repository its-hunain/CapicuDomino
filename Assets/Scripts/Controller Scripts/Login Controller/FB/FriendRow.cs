using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FriendRow : MonoBehaviour
{
    public string userID;
    public string userName;

    public Text friendName_Text;
    public RawImage friend_RawImage;


    public Toggle toggle;

    private void Awake()
    {
    }
    private void Start()
    {
        //requestBtn.onClick.AddListener(()=> SendGameRequest());
    }

    
    public void SendGameRequest(string roomName , string playerName, string playerUserID, string friendUserID)
    {
        Debug.Log(roomName);
        Debug.Log(playerName);
        Debug.Log(playerUserID);
        Debug.Log(friendUserID);

        //SendGameRequest to friend using Pun chat
        //if (PhotonChat.Instance.chatClient == null)
        //{
        //    Debug.LogError("Chat Client was null");
        //    PhotonChat.Instance.Connect();
        //}
    }

    public void Setter(string userID, string userName, Texture2D texture2D)
    {
        this.userID = userID;
        friendName_Text.text = this.userName = userName;
        friend_RawImage.texture = texture2D;
    }
}
