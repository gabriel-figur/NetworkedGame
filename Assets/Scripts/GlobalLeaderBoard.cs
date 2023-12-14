using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class GlobalLeaderBoard : MonoBehaviour
{
    private int maxResults = 5;
    public LeaderboardPopup leaderboardPopup;
    
    public void SubmitScore(int playerScore)
    {
        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate()
                {
                    StatisticName = "Most Kills",
                    Value = playerScore,
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, PlayFabUpdateStatsResult, PlayFabUpdateStatsError);
    }

    void PlayFabUpdateStatsResult(UpdatePlayerStatisticsResult updatePlayerStatisticsResult)
    {
        Debug.Log("PlayFab - Score submitted");
    }

    void PlayFabUpdateStatsError(PlayFabError updatePlayerStatisticsError)
    {
        Debug.Log("PlayFab - Error occurred while submitting score: " + updatePlayerStatisticsError.ErrorMessage);
    }

    public void GetLeaderboard()
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest()
        {
            MaxResultsCount = maxResults,
            StatisticName = "Most Kills"
        };

        PlayFabClientAPI.GetLeaderboard(request, PlayFabGetLeaderboardResult, PlayFabLeaderboardError);
    }

    void PlayFabGetLeaderboardResult(GetLeaderboardResult getLeaderboardResult)
    {
        Debug.Log("PlayFab - Get Leaderboard completed.");
        leaderboardPopup.UpdateUI(getLeaderboardResult.Leaderboard);
    }

    void PlayFabLeaderboardError(PlayFabError getLeaderboardError)
    {
        Debug.Log("PlayFab - Error occurred while getting Leaderboard: " + getLeaderboardError.ErrorMessage);
    }
}
