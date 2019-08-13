using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMessageCommand : IPopupCommand
{
	PopupMessage popupMessage;
	PopupType type;
	string message, title;
	PopupMessageCompleteDelegate completeCallback;

	public PopupMessageCommand(PopupMessage instance, PopupType _type, string _message, string _title, PopupMessageCompleteDelegate[] completeCallbacks)
	{
		popupMessage = instance;
		type = _type;
		message = _message;
		title = _title;
		foreach (PopupMessageCompleteDelegate callback in completeCallbacks)
			completeCallback += callback;
	}

	public void Execute()
	{
		popupMessage.Init(type, message, title, completeCallback);
		popupMessage.Show();
	}

	public void UnExecute()
	{
		popupMessage.Hide();
	}
}
