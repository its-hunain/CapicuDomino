using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinButton : MonoBehaviour
{
    public Button thisBtn;
    public int value;

    // Start is called before the first frame update
    void Start()
    {
        thisBtn.onClick.AddListener(() => GetComponentInParent<SelectCoinsToPlayScreen>().MoveToNextScreen(value));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
