using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
    public Text userRank;
    public Text userName;
    public Text userWins;
    public Image userImage;

    public void SetData(LeaderboardDatum data)
    {
        userRank.text = data.Rank;
        userName.text = data.userName;
        userWins.text = data.gamesWon;
        StartCoroutine(_GetTexture(data.profilePicUrl));
    }

    static IEnumerator _GetTexture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
          //DOwnloadTexture  userImage.sprite = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }

    }


}
