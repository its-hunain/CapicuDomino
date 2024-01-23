using UnityEngine;
using UnityEngine.UI;

public class MesgBar : MonoBehaviour
{
	public GameObject mesgBar;
	public Text txt;
	public Image holder;

	public Color errorColor;
	public Color mesgColor;

	public static MesgBar instance;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void show(string mesg, bool isError = true)
	{
		if (this.gameObject.activeInHierarchy)
			return;
		//--
		this.gameObject.SetActive(true);
		//--
		mesgBar.transform.localPosition = new Vector3(0, 200, 0);
		LeanTween.moveLocalY(mesgBar, 0, 1f).setEase(LeanTweenType.easeOutExpo);
		//--
		if (isError)
			holder.color = errorColor;
		else
			holder.color = mesgColor;
		//--
		txt.text = mesg;
		//--
		CancelInvoke("hide");
		Invoke("hide", 3);
	}

	public void hide()
	{
		CancelInvoke();
		LeanTween.moveLocalY(mesgBar, 200, .5f).setEase(LeanTweenType.easeOutExpo).setOnComplete(()=> { this.gameObject.SetActive(false); });
	}
}

