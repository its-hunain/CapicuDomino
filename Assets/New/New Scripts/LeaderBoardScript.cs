using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardScript : MonoBehaviour
{
    public LeaderboardData data;


    public Button closeBtn;
    public Transform content;
    public LeaderboardItem prefab;

    void Start()
    {
        WebServiceManager.instance.APIRequest(WebServiceManager.instance.getLeaderboards, Method.GET, null, null, OnSucess);

        closeBtn.onClick.AddListener(() => Close());
    }
    void OnSucess(string keyValuePairs, long successCode)
    {
        Debug.Log("OnSuccessfullygetLeaderboards: " + keyValuePairs.ToString());
        if (!ResponseStatus.Check(successCode))
        {
            Debug.LogError("successCode Error: " + successCode.ToString());
            Debug.LogError("Some Error: " + keyValuePairs.ToString());
        }
        WebglUserSession.userLoggedIn = true;

        data = LeaderboardData.FromJson(keyValuePairs.ToString());


        foreach (var item in data.data)
        {
            var temp = Instantiate(prefab, content);
            temp.SetData(item);
        }


    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}

[Serializable]
public partial class LeaderboardData
{
    public List<LeaderboardDatum> data;
}

[Serializable]
public partial class LeaderboardDatum
{
    public string profilePicUrl;
    public string _id;
    public string userName;
    public string Rank;
    public string gamesWon;

}

public partial class LeaderboardData
{
    public static LeaderboardData FromJson(string json) => JsonConvert.DeserializeObject<LeaderboardData>(json, Dominos.Converter.Settings);
}
