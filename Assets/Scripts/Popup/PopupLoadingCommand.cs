using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupLoadingCommand : IPopupCommand
{
	PopupLoading popupLoading;

	public PopupLoadingCommand(PopupLoading instance, string message, string title)
	{
		popupLoading = instance;
		popupLoading.Init(message, title);
	}

	public void Execute()
	{
		popupLoading.Show();
	}

	public void UnExecute()
	{
		popupLoading.Hide();
	}
}
