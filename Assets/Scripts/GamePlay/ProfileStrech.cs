using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileStrech : MonoBehaviour
{
    public GameObject bgPanel;
    public Button profileBtn;

    // Start is called before the first frame update
    void Start()
    {
        profileBtn.onClick.AddListener(()=> EnableDisableBgPanel(!bgPanel.activeInHierarchy));
    }

    // Update is called once per frame
    public void EnableDisableBgPanel(bool state)
    {
        bgPanel.SetActive(state);
    }
}
