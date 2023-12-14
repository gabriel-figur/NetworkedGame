using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalBestPopup : MonoBehaviour
{
    public GameObject scoreHolder;
    public GameObject noScoreText;
    public Text userName;
    public Text bestScore;
    public Text date;
    public Text totalPlayers;
    public Text roomName;
    
    public void UpdatePersonalBestUI()
    {
        PlayerData playerData = GameManager.instance.playerData;
        if (playerData.username != null)
        {
            userName.text = playerData.username;
            bestScore.text = playerData.bestScore.ToString();
            date.text = playerData.bestScoreDate;
            totalPlayers.text = playerData.totalPlayersInGame.ToString();
            roomName.text = playerData.roomName;
            
            scoreHolder.SetActive(true);
            noScoreText.SetActive(false);
        }
        else
        {
            scoreHolder.SetActive(false);
            noScoreText.SetActive(true);
        }
    }

    public void OnEnable()
    {
        UpdatePersonalBestUI();
    }
}
