using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class FetchFacebookFriendsInGame : MonoBehaviour
{
    [Space]
    [Header("Friend Rows")]
    public GameObject FriendRows;
    public Transform FriendRowsParent; //facebook friend row popuop/friend scroll view/ content

    public Button RequestBtn;


    // Start is called before the first frame update
    void Start()
    {
        RequestBtn.onClick.AddListener(()=> SendRequestToFriends());

        Debug.Log("friends: " + JsonConvert.SerializeObject(FacebookManager.friendList));
        foreach (var item in FacebookManager.friendList)
        {
            GameObject Friend = Instantiate(FriendRows, Vector3.zero, Quaternion.identity, FriendRowsParent);
            FriendRow FriendInfo = Friend.GetComponent<FriendRow>();
            FriendInfo.Setter(item.friendUserID, item.friendName, item.friendPic);
        }
    }

    //Send request to all selected friends
    private void SendRequestToFriends()
    {
        FriendRow[] friendRows = FriendRowsParent.GetComponentsInChildren<FriendRow>();
        string roomName = GenerateRandomRoomId();

        foreach (var item in friendRows)
        {
            if (item.doInvite)
            {
                item.SendGameRequest(roomName , PlayerPersonalData.playerName , PlayerPersonalData.playerUserID, item.userID);
            } 
        }
    }

    private string GenerateRandomRoomId()
    {
        int num = Random.Range(500, 5000);
        string roomId = GameRulesManager.privateRoomId = "CAPICU" + num.ToString();
        return roomId;
    }
}