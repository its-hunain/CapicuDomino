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
    public Image userImage = null;


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
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
          //  userImage.sprite = sprite;

        }

    }


}
