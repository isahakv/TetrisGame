using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private string playerName = "";
    private int score = 0, highScore = 0;

    void Awake()
    {
        instance = this;
        // Initializing Game Manager.
        Init();
    }

	void Start()
	{
		// Starting the Game.
		StartGame();
	}

	void Init()
    {
        playerName = PlayerPrefs.GetString("name");
		highScore = PlayerPrefs.GetInt("highscore");
		// Initializing UI Manager.
		UIManager.GetInstance().Init(playerName, highScore);
	}

    void Update()
    {
        UpdateTime();    
    }

    public void StartGame()
    {
		score = 0;
		Time.timeScale = 1.0f;

        InputController.EnableInput();
        UIManager.GetInstance().HideGameOverPanel();
		UIManager.GetInstance().UpdateScoreText(score);
		TetrisGame.GetInstance().StartNewGame();
    }

	public void GameOver()
	{
		Time.timeScale = 0.0f;

		InputController.DisableInput();
		UIManager.GetInstance().ShowGameOverPanel();
		TetrisGame.GetInstance().GameOver();
	}

	public void PauseGame()
	{
		Time.timeScale = 0.0f;
		InputController.DisableInput();
		TetrisGame.GetInstance().PauseGame();
    }

    public void ResumeGame()
    {
		Time.timeScale = 1.0f;
		InputController.EnableInput();
		TetrisGame.GetInstance().ResumeGame();
    }
	
    public void AddScore(int _score)
    {
        score += _score;
        if (score > highScore)
        {
            highScore = score;
			PlayerPrefs.SetInt("highscore", highScore);
			PlayerPrefs.Save();
			UIManager.GetInstance().UpdateHighScoreText(highScore);
        }
		UIManager.GetInstance().UpdateScoreText(score);
    }

    void UpdateTime()
    {
        int time = (int)Mathf.Floor(Time.time);
        int sec = (time % 3600) % 60, min = (time % 3600) / 60, hour = time / 3600;

        UIManager.GetInstance().UpdateTimeText(sec, min, hour);
    }
    
    public static void NewPlayer(string name)
    {
        PlayerPrefs.SetString("name", name);
		PlayerPrefs.SetInt("highscore", 0);
		PlayerPrefs.Save();
	}

    public static GameManager GetInstance() { return instance; }
}
