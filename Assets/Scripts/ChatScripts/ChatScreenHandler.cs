using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class ChatScreenHandler : MonoBehaviour
{
    public Button OpenEmoji;
    public Button OpenChat;
    public Button CloseEmoji;
    public Button CloseChat;
    public GameObject ChatScreen;
    public GameObject EmojiScreen;
    public List<string> messages = new List<string>();
    public List<Sprite> Emojis = new List<Sprite>();
    public static ChatScreenHandler instance;
    public ChatPopUpMessage msgPopUpCanvas;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        OpenEmoji.onClick.AddListener(() => { OpenEmojiScreen(); });
        OpenChat.onClick.AddListener(() => { OpenChatScreen(); });
        CloseEmoji.onClick.AddListener(() => { CloseParticularScreen(EmojiScreen); });
        CloseChat.onClick.AddListener(() => {CloseParticularScreen(ChatScreen); });
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Bot)
        {
            InvokeRepeating("RandomSpawnEmoji", 2f, 10);
        }
    }

    /// <summary>
    /// Invoking this method after every 10 seconds for bot game.
    /// </summary>
    void RandomSpawnEmoji()
    {
        int randomEmoji = Random.Range(0, Emojis.Count);
        int randomPlayerIndex = Random.Range(1, GamePlayUIPanel.instance.players.Count);
        SpawnChatMessage(ChatData.MsgType.Emojis, randomEmoji, GamePlayUIPanel.instance.players[randomPlayerIndex].playerPersonalData.playerUserID);
    }

    void OpenEmojiScreen()
    {
        if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);
        CloseParticularScreen(ChatScreen);
        EmojiScreen.SetActive(!EmojiScreen.activeInHierarchy);
    }
    void OpenChatScreen()
    {
        if (SoundManager.instance != null) SoundManager.instance.ButtonPressPlayer(true);
        CloseParticularScreen(EmojiScreen);
        ChatScreen.SetActive(!ChatScreen.activeInHierarchy);
    }

    void CloseParticularScreen(GameObject Screen)
    {
        Screen.SetActive(false);
    }

    public void CloseChatScreens() { CloseParticularScreen(ChatScreen); CloseParticularScreen(EmojiScreen); }

    Transform parent = null;
    public void SpawnChatMessage(ChatData.MsgType msgType, int index , string playerID)
    {
        //Debug.Log("SpawnChatMessage: \n playerID: " + playerID + "\n index:" + index + "\n msgType:" + msgType.ToString());
        var player = GamePlayUIPanel.instance.players.Find(x => x.playerPersonalData.playerUserID.Contains(playerID));

        if (player != null)
        {
            parent = player.playerPhysicalPosition.playerMsgPosTransform;
        }
        GameObject msgPopUp = Instantiate(msgPopUpCanvas.gameObject, parent.transform);
        ChatPopUpMessage chatPopUpMessage = msgPopUp.GetComponent<ChatPopUpMessage>();

        Transform dynamicMsgTransform = null;
        if (msgType == ChatData.MsgType.TextMsg)
        {
            dynamicMsgTransform = chatPopUpMessage.msgTxt.transform;
            chatPopUpMessage.msgTxt.text = messages[index];
            chatPopUpMessage.emoteImageBG.transform.gameObject.SetActive(false);
            chatPopUpMessage.msgTxtBG.transform.gameObject.SetActive(true);
        }
        else
        {
            dynamicMsgTransform = chatPopUpMessage.emoteImage.transform;
            chatPopUpMessage.emoteImage.sprite = Emojis[index];
            chatPopUpMessage.msgTxtBG.transform.gameObject.SetActive(false);
            chatPopUpMessage.emoteImageBG.transform.gameObject.SetActive(true);
        }

        if (player.playerPersonalData.playerDefaultSeatID == 1)
        {
            chatPopUpMessage.transform.localScale = new Vector3(-1, 1, 1);
            dynamicMsgTransform.localScale = new Vector3(-1, 1, 1);
        }
        else if (player.playerPersonalData.playerDefaultSeatID == 2)
        {
            chatPopUpMessage.transform.localScale = new Vector3(1, -1, 1);
            dynamicMsgTransform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            chatPopUpMessage.transform.localScale = new Vector3(1, 1, 1);
            dynamicMsgTransform.localScale = new Vector3(1, 1, 1);
        }
    }
}
