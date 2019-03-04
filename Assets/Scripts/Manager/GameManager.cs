using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public int maxLifes;
    int score;
    int lifes;
    int level;
    [SerializeField]
    bool connected = true;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        Invoke("GoToMainMenu", 3.0f);
        NewGame();
    }
    #region Setters & getters
    public int Score { get { return score; } }
    public int Life { get { return lifes; } }
    public int Level { get { return level; } set { level = value; } }
    #endregion
    #region Calling gameplay methods
    public void IncreaseScore()
    {
        score++;
    }
    public void Damage()
    {
        lifes-=1;
        if(lifes<= 0)
        {
            GameOver();
        }
    }
    public void NewGame()
    {
        score = 0;
        lifes = maxLifes;
        level = 0;
    }
    void GameOver()
    {
        if (connected)
        {
            PlayServices._instance.AddScoreToLeaderboard(GPGSIds.leaderboard_puntajes, score);
            //Sirvio mas de 50 tequilas
            if (score >= 50)
            {
                PlayServices._instance.UnlockAchivements(GPGSIds.achievement_servidor_profesional);
            }
            //Mejor score global
            if (PlayServices._instance.GetMaxScore(GPGSIds.leaderboard_puntajes) <= score)
            {
                PlayServices._instance.UnlockAchivements(GPGSIds.achievement_supremo_lider);
            }
        }
        GoToScene("GameOver");
    }
    #endregion
    #region Calling scene methods
    public void GoToMainMenu()
    {
        if (connected)
            SceneManager.LoadScene("MainMenu");
        else
            SceneManager.LoadScene("MainMenuOffline");
    }
    public void GoToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
    #region Google Play
    public void ShowLeaderboard()
    {
        PlayServices._instance.ShowLeaderboardUI();
    }
    public void ShowAchievements()
    {
        PlayServices._instance.ShowAchivementsUI();
    }
    public void FailureOnConnection()
    {
        connected = false;
        SceneManager.LoadScene("MainMenuOffline");
    }
    #endregion
}
