using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private string playerName = "";
    private int score = 0, highScore = 0;
    bool isGameOver = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        isGameOver = false;
        UIManager.GetInstance().HideGameOverPanel();
        TetrisGame.GetInstance().StartNewGame();
    }

    public void GameOver()
    {
        isGameOver = true;
        UIManager.GetInstance().ShowGameOverPanel();
        TetrisGame.GetInstance().GameOver();
    }

    public bool IsGameOver() { return isGameOver; }

    void AddScore()
    {

    }

    public static void NewPlayer(string name)
    {
        PlayerPrefs.SetString("name", name);
    }

    public static GameManager GetInstance() { return instance; }
}
