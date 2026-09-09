using UnityEngine;
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
        ImageCacheManager.instance.CheckOrDownloadImage(data.profilePicUrl, null, UpdatePic);
        //StartCoroutine(_GetTexture(data.profilePicUrl));
    }

    private void UpdatePic(Texture2D texture2D)
    {
        if (texture2D != null)
        {
            userImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(texture2D.width / 2, texture2D.height / 2));
        }
    }

    //static IEnumerator _GetTexture(string url)
    //{
    //    UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
    //    yield return www.SendWebRequest();
    //    if (www.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
    //        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
    //        userImage.sprite = sprite;

    //    }

    //}


}
