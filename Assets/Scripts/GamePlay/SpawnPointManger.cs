using System;
using System.Collections.Generic;
using UnityEngine;
public class SpawnPointManger : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    public static SpawnPointManger instance;

    private void Awake()
    {
        instance = this;
    }

    public static void ArrangeSpawnPoints(int seatIndex)
    {
        Debug.Log("ArrangeSpawnPoints based on my seat index: " + seatIndex);

        List<int> sequence = new List<int>();

        for (int i = 0; i < GameRulesManager.noOfPlayers; i++)
        {
            sequence.Add(seatIndex);
            seatIndex--;
            if (seatIndex == -1)
            {
                seatIndex = GameRulesManager.noOfPlayers - 1;
            }
        }

        //0,2,1
        for (int i = 0; i < instance.points.Count; i++)
        {
            instance.points[sequence[i]].SetSiblingIndex(i);
        }
    }

    internal static void ArrangeProfilesOrder(int seatIndex)
    {
        Debug.Log("ArrangeProfiles based on my seat index: " + seatIndex);

        List<int> sequence = new List<int>();


        for (int i = 0; i < GameRulesManager.noOfPlayers; i++)
        {
            sequence.Add(seatIndex);
            seatIndex--;
            if (seatIndex == -1)
            {
                seatIndex = GameRulesManager.noOfPlayers - 1;
            }
        }

        //0,2,1
        List<Player> playersSortedOrder = new List<Player>();
        
        for (int i = 0; i < GameRulesManager.noOfPlayers; i++)
        {
            playersSortedOrder.Add(GamePlayUIPanel.instance.players[sequence[i]]);
            //.transform.SetSiblingIndex(i);
            //);
        }
        GamePlayUIPanel.instance.players.Clear();
        GamePlayUIPanel.instance.players.AddRange(playersSortedOrder);
    }
}