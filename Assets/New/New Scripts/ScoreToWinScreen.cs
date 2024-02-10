using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreToWinScreen : MonoBehaviour
{

    public Button fiftyBtn;
    public Button hundredBtn;
    public Button onefiftyBtn;
    public Button twohundredBtn;

    // Start is called before the first frame update
    void Start()
    {
        fiftyBtn.onClick.AddListener(() =>ScoreToWin(50));
      hundredBtn.onClick.AddListener(() =>ScoreToWin(100));
     onefiftyBtn.onClick.AddListener(() =>ScoreToWin(150));
   twohundredBtn.onClick.AddListener(() =>ScoreToWin(200));

    }
    // Update is called once per frame
    void ScoreToWin(int score)
    {
      //  Rule4.maxScoreToWin = score;
    }
}
