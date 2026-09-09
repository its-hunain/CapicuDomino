using System;
using System.Collections.Generic;
/// <summary>
/// Defines the various network operations that can be sent/received.
/// </summary>
public class OpCodes
{
    public const long UNKNOWN = 0;
    public const long READY = 1;
    public const long START_ROUND = 2; // For spawning and initializing game data
    public const long GAME_ENDED = 3;
    public const long TURN_MOVE = 4;
    public const long UPDATE = 5;
    public const long Tie_Breaker = 6;
    public const long ResetTurnIndex = 7;
}

public class GameUpdates 
{
    public const long TILES_Order = 0;
    public const long SpawnPlayers = 1;
    public const long PickUpNewTile = 2;
    public const long SkipTurn = 3;
    public const long TurnTimeout = 4;
    public const long UpdateTime = 5;
    public const long RoundEnd = 6;
    public const long PlayerLeft = 7;
    public const long RestartRound = 8;
    public const long KickPlayers = 9;
    public const long Capicua = 10;
    public const long MultipleOfFive = 11;
}


public enum SkipTurnReason
{
    TIME_OUT = 0,
    NO_TILE_AVAILABLE = 1
}

[Serializable]
public class UpdateMessage
{
    public long code { get; set; }

    public string data { get; set; }
}
[Serializable]
public class ShuffleMessage
{
    public int[] shuffleArray { get; set; }
    public int[] tieShuffleArray { get; set; }
}

[Serializable]
public class PlayersData
{
    public long code { get; set; }

    public string data { get; set; }
}