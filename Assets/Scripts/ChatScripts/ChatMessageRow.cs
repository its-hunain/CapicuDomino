    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageRow : MonoBehaviour
{
    public int index = -1;
    public ChatData.MsgType _chatType = ChatData.MsgType.TextMsg;

    Button m_button;
    private void Start()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(()=>{ SendChatMessage(); });
    }



    void SendChatMessage()
    {
        FindObjectOfType<ChatScreenHandler>().CloseChatScreens();
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
            ChatScreenHandler.instance.SpawnChatMessage(_chatType,index,PlayerPersonalData.playerUserID);
        else
            GameManager.instace.SendChatMessage(_chatType, index);

    }

    internal void SetData(int index, ChatData.MsgType _chatType)
    {
        this.index = index;
        this._chatType = _chatType;


        if (_chatType == ChatData.MsgType.TextMsg)
        {
            GetComponentInChildren<Text>().text = ChatScreenHandler.instance.messages[index];
        }
        else
        {
            GetComponentInChildren<Image>().sprite = ChatScreenHandler.instance.Emojis[index];

        }
    }
}
