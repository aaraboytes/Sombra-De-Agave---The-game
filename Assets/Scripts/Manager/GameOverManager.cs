using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public Text score;
    private void Start()
    {
        score.text = "Score : " + GameManager._instance.Score.ToString();
        GameManager._instance.NewGame();
    }
    public void PlayAgain()
    {
        GameManager._instance.GoToScene("TapperGame");
    }
    public void GoToMainMenu()
    {
        GameManager._instance.GoToMainMenu();
    }
}
