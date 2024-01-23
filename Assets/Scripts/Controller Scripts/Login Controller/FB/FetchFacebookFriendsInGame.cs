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
        foreach (var item in friendRows)
        {
            if (item.toggle.isOn)
            {
                //item.SendGameRequest(PhotonNetwork.CurrentRoom.Name , PlayerProfile.Player_UserName , PlayerProfile.Player_UserID , item.userID);
            } 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}