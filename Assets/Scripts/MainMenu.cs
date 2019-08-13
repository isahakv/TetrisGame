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

    public GameObject mainMenu_Panel, playerLogin_Panel, settings_Panel;
	public InputField playerEmailInput_Panel, playerPasswordInput_Panel;

	MenuState currentState = MenuState.MainMenu;

    public void PlayButton_Pressed()
    {
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
				playerLogin_Panel.SetActive(false);
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
		UserDatabase.Get().PostUserHighScore(0).ContinueWith(task => { });
	}

	public Text googleSignIn_Text;

	public void SignInWithGoogle()
	{
		// UserAuth.SignInWithGoogle();
	}

	public void SignInWithFacebook()
	{
		UserAuth.Get().SignInWithFacebook();
	}

	public void SignInAnonymously()
	{
		UserAuth.Get().SignInAnonymously();
	}

	public void SignOut()
	{
		UserAuth.Get().SignOut();
	}

	public void PopupTestButton_Pressed()
	{
		UserDatabase.Get().SyncUserHighscore();
	}
}
