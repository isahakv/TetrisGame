using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public GameObject gameOver_UIPanel;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /** Start Game Over Menu */
    public void ShowGameOverPanel()
    {
        gameOver_UIPanel.SetActive(true);
    }

    public void HideGameOverPanel()
    {
        gameOver_UIPanel.SetActive(false);
    }
    /** End Game Over Menu */

    public static UIManager GetInstance() { return instance; }
}
