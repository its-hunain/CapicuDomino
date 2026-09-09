using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Controls the behaviour of the in-game menu.
/// </summary>
public class InGameMenu : MonoBehaviour
{
    public UnityEvent OnRequestQuitMatch;

    public Button ResumeButton;
    public Button ExitButton;

    private bool isOpen;

    /// <summary>
    /// Called by Unity when this GameObject starts.
    /// </summary>
    private void Start()
    {
        // Initialise the OnRequestQuitMatch event if required.
        if (OnRequestQuitMatch == null)
        {
            OnRequestQuitMatch = new UnityEvent();
        }

        // Add button event listeners.
        ResumeButton.onClick.AddListener(Close);
        ExitButton.onClick.AddListener(QuitMatch);
    }



    /// <summary>
    /// Change State
    /// </summary>
    /// <param name="state"></param>
    public void ChangeGameObjectState(bool state)
    {
        gameObject.SetActive(state);
    }

    /// <summary>
    /// Called by Unity every frame.
    /// </summary>
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }



    /// <summary>
    /// Called by Unity when this GameObject is being destroyed.
    /// </summary>
    private void OnDestroy()
    {
        // Remove button event listeners.
        ResumeButton.onClick.RemoveListener(Close);
        ExitButton.onClick.RemoveListener(QuitMatch);
    }

    /// <summary>
    /// Opens the in-game menu.
    /// </summary>
    public void Open()
    {
        gameObject.GetComponent<Canvas>().enabled = true;
        //GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<PlayerInputController>().enabled = false;
        isOpen = true;
        if (Time.timeScale != 0) Time.timeScale = 0;

    }


    /// <summary>
    /// Closes the in-game menu.
    /// </summary>
    public void Close()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
        //GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<PlayerInputController>().enabled = true;
        isOpen = false;
        if (Time.timeScale != 1) Time.timeScale = 1;
    }

    /// <summary>
    /// Quits the current match and closes the in-game menu.
    /// </summary>
    /// <returns></returns>
    public void QuitMatch()
    {
        OnRequestQuitMatch.Invoke();
        Close();
    }
}
