using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeScreen : MonoBehaviour
{
    public Button closeBtn;

    public Button blocking; //replace with gameMode type object
    public Button capicu; //replace with gameMode type object
    public Button nines; //replace with gameMode type object

    public Button blockingInfo; //replace with gameMode type object
    public Button capicuInfo; //replace with gameMode type object
    public Button ninesInfo; //replace with gameMode type object
    
    void Start()
    {
        closeBtn.onClick.AddListener(() => BackBtnCallBack());

        capicu.onClick.AddListener(() =>    GameModeSelection(GameRulesManager.GameRules.GameMode5, capicu,false));
        blocking.onClick.AddListener(() =>GameModeSelection(GameRulesManager.GameRules.GameMode4, blocking,false));
        nines.onClick.AddListener(() => GameModeSelection(GameRulesManager.GameRules.GameMode5, nines,true));

        capicuInfo.onClick.AddListener(() =>     AboutBtnCallBack(capicuInfo));
        blockingInfo.onClick.AddListener(() =>   AboutBtnCallBack(blockingInfo));
        ninesInfo.onClick.AddListener(() =>      AboutBtnCallBack(ninesInfo));
    }

    void GameModeSelection(GameRulesManager.GameRules gameRules,Button btn,bool isNines)
    {
        GameRulesManager.currentSelectedGame_Rule = gameRules;
        if (GameRulesManager.currentSelectedGame_MatchType == GameRulesManager.MatchType.Multiplayer)
        {
            UI_Manager.instance.ChangeScreen(UI_Manager.instance.selectCoinsToPlayScreen.gameObject, true);
        }
        else
        {
            UI_Manager.instance.ChangeScreen(UI_Manager.instance.noOfPlayers.gameObject, true);
        }
        ChangeNoOfPlayers(btn) ;
        Rule5.isNines = isNines;
    }

    void ChangeNoOfPlayers(Button btn)
    {
        UI_Manager.instance.noOfPlayers.ResetBtns();
        if (btn == blocking)
        {
           // UI_Manager.instance.ChangeScreen(UI_Manager.instance.scoreToWinScreen.gameObject, true);
            UI_Manager.instance.noOfPlayers.twoPlayerBtn.interactable = true;
        }
        else if (btn == capicu)
        {
            UI_Manager.instance.noOfPlayers.twoPlayerBtn.interactable = true;
            UI_Manager.instance.noOfPlayers.fourPlayerBtn.interactable = true;

        }
        else
        {
            UI_Manager.instance.noOfPlayers.threePlayerBtn.interactable = true;

        }
    }

    void BackBtnCallBack() 
    {    
        if (GameRulesManager.isPrivateRoom)
        {
            UI_Manager.instance.ChangeScreen(UI_Manager.instance.createJoinRoomButtonPanel.gameObject, true);
        }
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.gameModeScreen.gameObject, false);
    }
    void AboutBtnCallBack(Button btn)
    {
        UI_Manager.instance.ChangeScreen(UI_Manager.instance.aboutScreen.gameObject, true);

        if (btn== blockingInfo)
            UI_Manager.instance.aboutScreen.UpdateUIText(
                "BLOCKS",
                "Welcome to <b>BLOCKS</b> - the <b>No-Draw, All-Skill Battle of Strategy and Swagger!</b>\n\n" +
                "Think you've got the brains and boldness to run the table? In BLOCKS, every move matters - no luck, no draw pile, just pure domino dominance.\n\n" +
                "<b>Goal:</b>\n" +
                "Be the first to play all your tiles, or have the lowest total when the game gets blocked. Every point counts!\n\n" +
                "<b>Setup:</b>\n" +
                "- Each player starts with 7 dominoes\n" +
                "- Highest double hits the board first\n" +
                "- No drawing - what's in your hand is what you've got, so make it count!\n\n" +
                "<b>Gameplay:</b>\n" +
                "- Match one open end of the board\n" +
                "- Can't play? Pass and hope for a comeback.\n" +
                "- Keep it moving until someone drops their last tile or the game locks up.\n\n" +
                "<b>Scoring:</b>\n" +
                "- The winner scores the total value of all opponents' leftover dominoes.\n" +
                "- If the game is blocked, the lowest hand wins the round.\n\n" +
                "<b>Pro Tip:</b>\n" +
                "Own the board - control both ends and force your opponents into tight spots. Smart play and sharper memory make champions in BLOCKS!"
            );

        if (btn== capicuInfo)
            UI_Manager.instance.aboutScreen.UpdateUIText(
                "CAPICU",
                "<b>CAPICU - The One-of-a-Kind Domino Experience!</b>\n\n" +
                "Get ready for fast-paced, high-energy domino action that'll keep you hooked from the very first tile! " +
                "Play up to 200 points in this electrifying version of the classic game loved across the world - now with <b>flavor, flair, and fire!</b>\n\n" +

                "<b>How to Play:</b>\n" +
                "- Match and play your tiles to score big!\n" +
                "- Be the first to finish your hand and stack up the points.\n" +
                "- Land a <b>Capicu</b> (when both open ends match) and score a sweet <b>+25 bonus!</b>\n" +
                "- Make everyone pass with no moves? That's another <b>+25!</b>\n" +
                "- Drop that final <b>double blank</b> tile that can be played on both ends to earn a <b>Chuchazo</b> and flex with <b>+25 more!</b>\n\n" +

                "<b>Game Modes:</b>\n" +
                "- <b>1v1 Mode:</b> Go head-to-head and show who's boss.\n" +
                "- <b>2v2 Partner Mode:</b> Squad up with your partner, vibe together, and run the table - double the skill, double the hype!\n\n" +

                "This is dominoes with <b>flavor, style, and bragging rights.</b>\n" +
                "<b>Play smart. Play bold. Play CAPICU</b> - where every move can make you a legend!\n\n" +

                "<b>Pro Tip:</b>\n" +
                "Keep track of every tile played - knowing what's left in the game is your secret weapon. " +
                "A smart player doesn't just play tiles... they set traps."
            );

        if (btn== ninesInfo)
            UI_Manager.instance.aboutScreen.UpdateUIText(
                "NINES",
                "<b>NINES GAME MODE - The Most Intense 3-Player Domino Showdown Ever!</b>\n\n" +
                "Get ready for a one-of-a-kind domino battle that flips the table on tradition! " +
                "In <b>NINES</b>, it's three players, nine dominoes each, and no second chances.\n\n" +

                "<b>How It Works:</b>\n" +
                "- Each player starts with <b>9 dominoes</b> - that's right, just you vs. two rivals in a full-blown strategy war.\n" +
                "- The game uses <b>27 dominoes total</b>, with the <b>double blank</b> kicked out - no fillers, just pure skill.\n" +
                "- There's <b>no draw pile</b>, no lucky pick-ups - every move counts, every mistake costs you the crown.\n" +
                "- Be the first to play all your dominoes and claim victory... or get left behind as the last one standing!\n\n" +

                "<b>Fast. Ruthless. Addictive.</b>\n" +
                "NINES isn't just a game - it's the <b>ultimate domino survival match.</b>\n" +
                "Think you've got what it takes to conquer the table?\n\n" +
                "<b>Welcome to NINES</b> - where legends are made, one tile at a time.\n\n" +

                "<b>Pro Tip:</b>\n" +
                "Keep your eyes on the board - in NINES, <b>memory is your greatest weapon.</b> " +
                "Track every play and lock down your opponents before they even see it coming."
            );


    }
}
