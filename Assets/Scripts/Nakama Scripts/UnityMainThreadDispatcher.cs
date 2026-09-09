using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// A thread-safe class which holds a queue with actions to execute on recursive Coroutine. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour {

	private static readonly Queue<Action> _executionQueue = new Queue<Action>();
	private static UnityMainThreadDispatcher _instance = null;
	private float delay = 0.1f;


	public IEnumerator ExecuteActions()
    {
		while (true)
		{
			yield return new WaitForEndOfFrame();
			while (_executionQueue.Count > 0)
			{
                if (!GameManager.instace.NakamaConnection.Socket.IsConnected)
                {
					OnQuit();
					yield break;
				}

				yield return new WaitForSeconds(delay);
				_executionQueue.Dequeue().Invoke();
				Debug.Log("delay***********: " + delay);
			}
			if (_executionQueue.Count == 0) { delay = 0;}
			else if(_executionQueue.Count > 1) { delay = 0.2f; }
		}
    }

 //   private void OnApplicationFocus(bool focus)
 //   {
 //       if (GameManager.instace == null) return;
 //       if (GameManager.instace.NakamaConnection == null) return;
 //       if (GameManager.instace.NakamaConnection.Socket == null) return;

 //       if (!GameManager.instace.NakamaConnection.Socket.IsConnected)
 //       {
	//		GamePlayUIPanel.instance.PopUpController(GridManager.instance.HeadingPos, "Socket Disconnect.");
	//		Invoke("OnQuit", 1);
 //       }
	//}

 //   private void OnApplicationPause(bool pause)
 //   {
	//	if (GameManager.instace == null) return;
	//	if (GameManager.instace.NakamaConnection == null) return;
	//	if (GameManager.instace.NakamaConnection.Socket == null) return;

	//	if (!GameManager.instace.NakamaConnection.Socket.IsConnected)
 //       {
	//		GamePlayUIPanel.instance.PopUpController(GridManager.instance.HeadingPos, "Socket Disconnect.");
	//		Invoke("OnQuit", 1);
	//	}
	//}

    private void OnQuit()
    {
		Debug.Log("OnQuit() : Due to Socket Disconnect");
		Debug.Log(gameObject.name + " detect error: ln: 67");
		GameManager.instace.OnRequestQuitMatch.Invoke();
    }


	/// <summary>
	/// Locks the queue and adds the Action to the queue
	/// </summary>
	/// <param name="action">function that will be executed from the main thread.</param>
	public void Enqueue(Action action)
	{
		Debug.Log("Enque new action");
		lock (_executionQueue)
		{
			_executionQueue.Enqueue(action);
		}
    }

	public static UnityMainThreadDispatcher Instance() {
		return _instance;
	}

	void Awake() {

        _instance = this;

		if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
		{
			gameObject.SetActive(false);
			Destroy(this.gameObject);
		}
        else
        {
			StartCoroutine(ExecuteActions());
        }
	}
}