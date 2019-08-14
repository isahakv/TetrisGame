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
        ScoreBoardMenu
    }

    public GameObject mainMenu_Panel, settings_Panel, scoreBoard_Panel;

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

	public void ScoreBoardButton_Pressed()
	{
		currentState = MenuState.ScoreBoardMenu;
		mainMenu_Panel.SetActive(false);
		scoreBoard_Panel.SetActive(true);
	}
	public void ExitButton_Pressed()
    {
        Application.Quit();
    }

    public void BackButton_Pressed()
    {
        switch(currentState)
        {
            case MenuState.ScoreBoardMenu:
                currentState = MenuState.MainMenu;
                mainMenu_Panel.SetActive(true);
				scoreBoard_Panel.SetActive(false);
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
		PopupManager.Get().NewQuestion(PopupType.Warning, "", "Reset Your Local and Server Stats?",(PopupQuestionResult result) =>
		{
			if (result == PopupQuestionResult.Accept)
			{
				PlayerPrefs.DeleteAll();
				StartCoroutine(TaskExtension.YieldWait(UserDatabase.Get().PostUserHighScore(0)));
			}
		});
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
		if (!UserAuth.IsUserLoggedIn())
		{
			PopupManager.Get().NewMessage(PopupType.Error, "", "You are not Logged-in!");
			return;
		}

		PopupManager.Get().NewQuestion(PopupType.Warning, null, "If you Sign Out your local stats will be TERMINATIED!", (PopupQuestionResult result) =>
		{
			if (result == PopupQuestionResult.Accept)
				UserAuth.Get().SignOut();
		});
	}
}
