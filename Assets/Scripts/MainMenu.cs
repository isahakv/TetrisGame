using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    enum MenuState
    {
        MainMenu,
        SettingsMenu,
        PlayerNameInputMenu
    }

    public GameObject mainMenu_Panel, playerNameInput_Panel, settings_Panel;

    MenuState currentState = MenuState.MainMenu;

    void Start()
    {
        // Reset InputField placeholder color to default, in case if it is red.
        InputField playerNameInputField = playerNameInput_Panel.GetComponentInChildren<InputField>();
        playerNameInputField.onValueChanged.AddListener((string text) =>
        {
            if (text == "")
                playerNameInputField.placeholder.color = new Color(0.196f, 0.196f, 0.196f, 0.5f);
        });
    }

    public void PlayButton_Pressed()
    {
        if (!(PlayerPrefs.HasKey("name")))
        {
            if (currentState == MenuState.MainMenu)
            {
                currentState = MenuState.PlayerNameInputMenu;
                mainMenu_Panel.SetActive(false);
                playerNameInput_Panel.SetActive(true);
                return;
            }
            else // if in Player Name Input Mode.
            {
                InputField playerNameInputField = playerNameInput_Panel.GetComponentInChildren<InputField>();
                if (playerNameInputField.text == "")
                {
                    playerNameInputField.placeholder.color = new Color(1.0f, 0.0f, 0.0f);
                    return;
                }
                else
                    GameManager.NewPlayer(playerNameInputField.text);
            }
        }

        SceneManager.LoadScene("MainScene");
    }

    public void SettingsButton_Pressed()
    {
        currentState = MenuState.SettingsMenu;
        mainMenu_Panel.SetActive(false);
        settings_Panel.SetActive(true);
    }

    public void ExitButton_Pressed()
    {
        Application.Quit();
    }

    public void BackButton_Pressed()
    {
        switch(currentState)
        {
            case MenuState.PlayerNameInputMenu:
                currentState = MenuState.MainMenu;
                mainMenu_Panel.SetActive(true);
                playerNameInput_Panel.SetActive(false);
                break;
            case MenuState.SettingsMenu:
                currentState = MenuState.MainMenu;
                mainMenu_Panel.SetActive(true);
                settings_Panel.SetActive(false);
                break;
        }
    }

    public void ResetStatsButton_Pressed()
    {
        PlayerPrefs.DeleteAll();
    }
}
