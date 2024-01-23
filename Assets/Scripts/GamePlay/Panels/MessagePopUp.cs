using UnityEngine;
using UnityEngine.UI;

public class MessagePopUp : MonoBehaviour
{
    public Text msgText;
    public Image image;

    public void ShowData(string msg = null)
    {
        if (msg != null)
        {
            msgText.text = msg;
        }
    }

    public void ShowData(string msg = null, Texture2D tex = null)
    {
        if (msg != null)
        {
            msgText.text = msg;

            
           if(tex!=null) image.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
