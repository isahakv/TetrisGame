using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ScoreBoardMenu : MonoBehaviour
{
	public ScoreBoardEntry scoreBoardEntryPrefab;
	public Transform scoreBoardTable;

	List<ScoreBoardEntry> scoreBoardEntries = new List<ScoreBoardEntry>();

	private void OnEnable()
	{
		StartCoroutine(LoadUserDatas());
	}

	IEnumerator LoadUserDatas()
	{
		PopupManager.Get().OpenLoading("", "Loading...");

		Task<UserData[]> task = UserDatabase.Get().GetAllUserData();
		yield return TaskExtension.YieldWait(task);
		if (task.IsCompleted)
		{
			UserData[] userDatas = task.Result;
			if (userDatas.Length > 0)
			{
				foreach(UserData userData in userDatas)
				{
					ScoreBoardEntry entry = Instantiate(scoreBoardEntryPrefab, scoreBoardTable);
					entry.Init(userData.iconURL, userData.name, userData.highScore);
					scoreBoardEntries.Add(entry);
				}
			}
		}
		PopupManager.Get().CloseLoading();
	}

	private void OnDisable()
	{
		foreach (ScoreBoardEntry entry in scoreBoardEntries)
			Destroy(entry.gameObject);
		scoreBoardEntries.Clear();
	}
}
