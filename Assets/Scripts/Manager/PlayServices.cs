using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
public class PlayServices : MonoBehaviour
{
    public static PlayServices _instance;
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        SignIn();
    }

    void SignIn()
    {
        Social.localUser.Authenticate(success=>
        {
            if(success)
            {
                UnlockAchivements(GPGSIds.achievement_bienvenido);
            }
            else
            {
                GameManager._instance.FailureOnConnection();
            }
        });
    }
    #region Achivements
    public void UnlockAchivements(string id)
    {
        Social.ReportProgress(id, 100, (bool success) => { });
    }
    public void IncrementAchivements(string id,int stepsToIncrement)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { });
    }
    public void ShowAchivementsUI()
    {
        Social.ShowAchievementsUI();
    }
    #endregion
    #region Leaderboards
    public void AddScoreToLeaderboard(string leaderboardId,long score)
    {
        Social.ReportScore(score, leaderboardId, success => { });
    }
    public void ShowLeaderboardUI()
    {
        Social.ShowLeaderboardUI();
    }
    public int GetMaxScore(string leaderboardId)
    {
        int score = 0;
        PlayGamesPlatform.Instance.LoadScores(
             leaderboardId,
             LeaderboardStart.PlayerCentered,
             1,
             LeaderboardCollection.Public,
             LeaderboardTimeSpan.AllTime,
         (LeaderboardScoreData data) => {
             Debug.Log(data.Valid);
             Debug.Log(data.Id);
             Debug.Log(data.PlayerScore);
             Debug.Log(data.PlayerScore.userID);
             Debug.Log(data.PlayerScore.formattedValue);
             score = int.Parse(data.PlayerScore.formattedValue);
         });
        return score;
    }
    #endregion
}
