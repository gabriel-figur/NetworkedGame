using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerData
{
    public string uid;
    public string username;
    public int bestScore;
    public string bestScoreDate;
    public int totalPlayersInGame;
    public string roomName;
    public int winCount;

    public PlayerData()
    {
        uid = Guid.NewGuid().ToString();
    }
}
