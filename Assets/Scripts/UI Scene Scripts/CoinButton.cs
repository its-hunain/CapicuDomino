using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinButton : MonoBehaviour
{
    public Button thisBtn;
    public Text btnTxt;
    private int coins = 0;

    // Start is called before the first frame update
    void Start()
    {
        thisBtn.onClick.AddListener(() => GetComponentInParent<SelectCoinsToPlayScreen>().MoveToNextScreen(thisBtn, coins));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValue(int value)
    {
        this.coins = value;
        btnTxt.text = value.ToString();
    }
}
