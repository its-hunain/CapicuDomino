using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour
{
    public static CountDownTimer instance;

    public Text timeText;
    public Button backButton;


    private void Awake()
    {
        instance = this;

        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot || GameRulesManager.currentSelectedGame_GameType == GameRulesManager.GameType.SingleMatch) //No need for a timer.
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        backButton.onClick.AddListener(() => SceneManager.LoadScene(Global.UIScene));
        Debug.Log("GameRulesManager.matchDetails.timeDifference: " + GameRulesManager.matchDetails.matchDate);
        StartCountdown(double.Parse(GameRulesManager.matchDetails.matchDate));
    }
    public void StartCountdown(double matchStartingTime)
    {
        Epoch.UpdateDiff(double.Parse(GameRulesManager.matchDetails.serverTime));
        //Debug.Log("Server diff " + Epoch.instance.serverTimeDiff);
        StartCoroutine(CountdownTimer(matchStartingTime));
    }
    private IEnumerator CountdownTimer(double matchStartTime)
    {
        while (true)
        {
            double timeLeft = Epoch.SecondsLeft(matchStartTime);
            if (timeLeft > 0)
            {
                GamePlayWaitingPopUp.instance.EnableDisable(false);
                int totalMinute = (int)timeLeft / 60;
                int toatalHours = totalMinute / 60;
                int days = (int)toatalHours / 24;

                int remainingMinutes = totalMinute % 60;
                int remainingHours = toatalHours % 24;

                timeText.text = days.ToString("00") + ":" + remainingHours.ToString("00") + ":" + remainingMinutes.ToString("00") + ":" + ((int)(timeLeft % 60)).ToString("00");


                //timeText.text = "00:" + timeLeft.ToString("00");
            }
            else
            {
                Debug.Log("ended");
                GamePlayWaitingPopUp.instance.EnableDisable(true);
                gameObject.SetActive(false);
                GameManager.instace.FindTournamentGame();
                break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
