using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Mine
public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    enum PageState
    {
        None,
        Start,
        GameOver,
        CountDown
    }

    int score = 0;
    bool gameOver = true;

    public bool GameOver { get { return gameOver; } }
    public int Score { get { return score; } }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
    }

    private void OnDisable()
    {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted(); //event sent to TapController
        score = 0;
        gameOver = false;
    }

    void OnPlayerDied() 
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if(score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);

    }

    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            case PageState.CountDown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
        }
    }
    public void ConfirmGameOver() 
    {
        //Activated when replay button is hit
        OnGameOverConfirmed(); //event ent to TapController
        scoreText.text = "0";
        SetPageState(PageState.Start);
    
    }

    public void StartGame()
    {
        //Activated when play button is hit
        SetPageState(PageState.CountDown);
    }
}
