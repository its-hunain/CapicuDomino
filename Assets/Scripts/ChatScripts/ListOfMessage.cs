using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ListOfMessage : MonoBehaviour
{
    public ChatData.MsgType _chatType;

    public GameObject Button;
    public Transform parent;
    //public GameObject player;
    public static ListOfMessage instance;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < (_chatType == ChatData.MsgType.TextMsg ? ChatScreenHandler.instance.messages.Count : ChatScreenHandler.instance.Emojis.Count); i++)
        {

            GameObject temp = Instantiate(Button, parent.transform);
            temp.GetComponent<ChatMessageRow>().SetData(i,_chatType);
        }
    }
}
