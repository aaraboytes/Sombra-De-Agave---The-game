using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;
    private void Awake()
    {
        _instance = this;
    }
    [Header("Main UI")]
    public Text score;
    public GameObject[] health;
    public Text level;
    public Image buttonImg;
    public Sprite emptyItemSlot;
    [Header("Advices UI")]
    public GameObject advicer;
    Animator advicerAnim;
    Text advicerText;
    private void Start()
    {
        //Advicer UI
        advicerAnim = advicer.GetComponent<Animator>();
        advicerText = advicer.transform.GetChild(0).GetComponent<Text>();

        //Main UI
        score.text = GameManager._instance.Score.ToString();
        int levelvalue = GameManager._instance.Level;
        level.text = (levelvalue+1).ToString();
    }
    #region Score
    public void ScoreChanged()
    {
        float currentScore = GameManager._instance.Score;
        score.text = currentScore.ToString();
    }
    public void LevelChanged()
    {
        int levelvalue = GameManager._instance.Level;
        level.text = (levelvalue + 1).ToString();
    }
    #endregion
    #region Health
    public void HealthChanged()
    {
        int currentHealth = GameManager._instance.Life;
        health[currentHealth].SetActive(false);
    }
    #endregion
    #region Items
    public void ItemBtn()
    {
        FindObjectOfType<PlayerController>().ActivateItem();
    }
    public void ItemUsed()
    {
        buttonImg.sprite = emptyItemSlot;
    }
    public void ItemAdquired(Item item)
    {
        buttonImg.sprite = item.sprite;
    }
    #endregion
    #region Advice
    public void Message(string msg)
    {
        advicerText.text = msg;
        advicerAnim.SetTrigger("Appear");
    }
    #endregion
}
