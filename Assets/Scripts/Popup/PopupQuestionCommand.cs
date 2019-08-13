using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupQuestionCommand : IPopupCommand
{
	PopupQuestion popupQuestion;
	PopupType type;
	string message, title;
	PopupQuestionCompleteDelegate completeCallback;

	public PopupQuestionCommand(PopupQuestion instance, PopupType _type, string _message, string _title, PopupQuestionCompleteDelegate[] completeCallbacks)
	{
		popupQuestion = instance;
		type = _type;
		message = _message;
		title = _title;
		foreach (PopupQuestionCompleteDelegate callback in completeCallbacks)
			completeCallback += callback;
	}

	public void Execute()
	{
		popupQuestion.Init(type, message, title, completeCallback);
		popupQuestion.Show();
	}

	public void UnExecute()
	{
		popupQuestion.Hide();
	}
}
