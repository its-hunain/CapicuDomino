using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Dominos;

/// <summary>
/// Syncs a remotely connected player's character using received network data.
/// </summary>
public class PlayerNetworkRemoteSync : MonoBehaviour
{
    [SerializeField]
    public RemotePlayerNetworkData NetworkData;

    ///// <summary>
    ///// Called when player.
    ///// </summary>
    //public void UpdateLocalPlayerData()
    //{
    //    Player playerProfile = GamePlayUIPanel.instance.players.Find(x => x.isMe);
    //    playerProfile.playerPersonalData.playerUserID = PlayerPersonalData.playerUserID;
    //    playerProfile.playerPersonalData.playerName = PlayerPersonalData.playerName;
    //    playerProfile.playerPersonalData.playerTexture = PlayerPersonalData.playerTexture;
    //    playerProfile.SetDataToProfile(PlayerPersonalData.playerUserID, PlayerPersonalData.playerName, PlayerPersonalData.playerTexture);
    //    Debug.Log("UpdateLocalPlayerData Profile." + playerProfile);
    //}
}

[Serializable]
public class RemotePlayerNetworkData
{
    public bool isLocalUser;
    public string MatchId;
    public NewJsonPlayer User;
    public IApiUser UserDetails;
    public string AvatarURL;
    public string DisplayName;
    public string UserName;
    public string UserID;
    public Texture2D UserTexture;
    public int seatIndex;

    public Player player; //Profile Reference
}
