using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public Text playerNameText, timeText, scoreText, highScoreText;
	public GameObject pauseButton, inGameMenu_Panel, gameOver_UIPanel, scoreBoard_UIPanel;
    
    void Awake()
    {
        instance = this;
		DontDestroyOnLoad(this);
    }

    public void Init(string playerName, int highScore)
    {
        playerNameText.text = playerName;
		UpdateHighScoreText(highScore);
    }

    public void UpdateTimeText(int sec, int min, int hour)
    {
        string secStr = sec.ToString(), minStr = min.ToString(), hourStr = hour.ToString();
        if (secStr.Length == 1)
            secStr = secStr.Insert(0, "0");
        if (minStr.Length == 1)
            minStr = minStr.Insert(0, "0");
        if (hourStr.Length == 1)
            hourStr = hourStr.Insert(0, "0");

        timeText.text = hourStr + ":" + minStr + ":" + secStr;
    }

    public void UpdateScoreText(int score)
    {
		scoreText.text = score.ToString();
    }

    public void UpdateHighScoreText(int highScore)
    {
		highScoreText.text = highScore.ToString();
	}

    /** Start InGame Menu */
    public void PauseButton_Pressed()
    {
        GameManager.GetInstance().PauseGame();
        inGameMenu_Panel.SetActive(true);
		pauseButton.SetActive(false);
	}

	public void ContinueButton_Pressed()
	{
		GameManager.GetInstance().ResumeGame();
		inGameMenu_Panel.SetActive(false);
		pauseButton.SetActive(true);
	}

	public void ReturnMainMenuButton_Pressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
    /** End InGame Menu */

    /** Start Game Over Menu */
    public void ShowGameOverPanel()
    {
        gameOver_UIPanel.SetActive(true);
    }

    public void HideGameOverPanel()
    {
        gameOver_UIPanel.SetActive(false);
    }

	public void OpenScoreBoardButton_Pressed()
	{
		scoreBoard_UIPanel.SetActive(true);
	}

	public void CloseScoreBoardButton_Pressed()
	{
		scoreBoard_UIPanel.SetActive(false);
	}
	/** End Game Over Menu */

	public static UIManager GetInstance() { return instance; }
}
