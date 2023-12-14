using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

public class LeaderboardPopup : MonoBehaviour
{
    public GameObject scoreHolder;
    public GameObject noScoreText;
    public GameObject leaderBoardItem;

    private void OnEnable()
    {
        GameManager.instance.globalLeaderBoard.GetLeaderboard();
    }

    public void UpdateUI(List<PlayerLeaderboardEntry> playerLeaderboardEntries)
    {
        if (playerLeaderboardEntries.Count > 0)
        {
            DestroyChildren(scoreHolder.transform);
            for (int i = 0; i < playerLeaderboardEntries.Count; i++)
            {
                GameObject newLeaderboardItem = Instantiate(leaderBoardItem, Vector3.zero, Quaternion.identity, scoreHolder.transform);
                newLeaderboardItem.GetComponent<LeaderboardItem>().SetScores(i + 1, playerLeaderboardEntries[i].DisplayName, playerLeaderboardEntries[i].StatValue);
            }
            
            scoreHolder.SetActive(true);
            noScoreText.SetActive(false);
        }
        else
        {
            scoreHolder.SetActive(false);
            noScoreText.SetActive(true);
        }
    }

    void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
