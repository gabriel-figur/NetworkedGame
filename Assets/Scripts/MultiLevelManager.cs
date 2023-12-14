using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiLevelManager : MonoBehaviourPunCallbacks
{
    public int maxKills = 3;
    public GameObject gameOverPopup;
    public Text winnerText;
    private void Start()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer,
        ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.GetScore() == maxKills)
        {
            winnerText.text = targetPlayer.NickName;
            gameOverPopup.SetActive(true);
            StorePersonalBest();
        }
    }

    public void StorePersonalBest()
    {
        int currentScore = PhotonNetwork.LocalPlayer.GetScore();
        PlayerData playerData = GameManager.instance.playerData;
        if (currentScore == 3)
        {
            playerData.winCount += 1;
        }
        if (currentScore > playerData.bestScore)
        {
            playerData.username = PhotonNetwork.LocalPlayer.NickName;
            playerData.bestScore = currentScore;
            playerData.bestScoreDate = DateTime.UtcNow.ToString();
            playerData.totalPlayersInGame = PhotonNetwork.CurrentRoom.PlayerCount;
            playerData.roomName = PhotonNetwork.CurrentRoom.Name;
            GameManager.instance.globalLeaderBoard.SubmitScore(playerData.winCount);
            GameManager.instance.SavePlayerData();
        }
    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Arena");
    }

    public void debug()
    {
        Debug.Log("CLICKED");
    }
}
