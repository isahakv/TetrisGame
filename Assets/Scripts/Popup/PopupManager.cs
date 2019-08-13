using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PopupType
{
	Info,
	Warning,
	Error,
	Loading
}

public enum PopupQuestionResult
{
	Accept,
	Decline
}

public delegate void PopupMessageCompleteDelegate();
public delegate void PopupQuestionCompleteDelegate(PopupQuestionResult result);

public class PopupManager : MonoBehaviour
{
	private static PopupManager instance;

	public PopupMessage popupMessagePrefab;
	public PopupQuestion popupQuestionPrefab;
	public PopupLoading popupLoadingPrefab;

	PopupMessage popupMessageInstance;
	PopupQuestion popupQuestionInstance;
	PopupLoading popupLoadingInstance;

	Canvas canvas;
	LinkedList<IPopupCommand> commandQueue = new LinkedList<IPopupCommand>();

	PopupMessage PopupMessageInstance
	{
		get
		{
			if (popupMessageInstance == null)
				popupMessageInstance = Instantiate(popupMessagePrefab, GetCanvas().transform);
			return popupMessageInstance;
		}
	}
	PopupQuestion PopupQuestionInstance
{
		get
		{
			if (popupQuestionInstance == null)
				popupQuestionInstance = Instantiate(popupQuestionPrefab, GetCanvas().transform);
			return popupQuestionInstance;
		}
	}
	PopupLoading PopupLoadingInstance
{
		get
		{
			if (popupLoadingInstance == null)
				popupLoadingInstance = Instantiate(popupLoadingPrefab, GetCanvas().transform);
			return popupLoadingInstance;
		}
	}

	Canvas GetCanvas()
	{
		if (canvas == null)
			canvas = FindObjectOfType<Canvas>();
		return canvas;
	}

	void Awake()
	{
		if (instance == null)
		{
			DontDestroyOnLoad(this);
			instance = this;
		}
		else if (instance != this)
			DestroyImmediate(gameObject);
	}

	public void NewMessage(PopupType type, string title, string message, PopupMessageCompleteDelegate onMessagePopupCompeted = null)
	{
		PopupMessageCompleteDelegate[] completeCallbacks = { OnMessagePopupCompeted, onMessagePopupCompeted };
		PopupMessageCommand messageCommand = new PopupMessageCommand(PopupMessageInstance, type, message, title, completeCallbacks);
		commandQueue.AddLast(messageCommand);
		onMessagePopupCompeted = OnMessagePopupCompeted;
		// If there is only one command in queue, then execute it.
		if (commandQueue.Count == 1)
			messageCommand.Execute();
	}

	public void NewQuestion(PopupType type, string title, string message, PopupQuestionCompleteDelegate onQuestionPopupCompeted)
	{
		PopupQuestionCompleteDelegate[] completeCallbacks = { OnQuestionPopupCompeted, onQuestionPopupCompeted };
		PopupQuestionCommand questionCommand = new PopupQuestionCommand(PopupQuestionInstance, type, message, title, completeCallbacks);
		commandQueue.AddLast(questionCommand);
		onQuestionPopupCompeted = OnQuestionPopupCompeted;
		// If there is only one command in queue, then execute it.
		if (commandQueue.Count == 1)
			questionCommand.Execute();
	}

	public void OpenLoading(string title, string message)
	{
		if (commandQueue.Count > 0)
			commandQueue.First.Value.UnExecute();

		PopupLoadingCommand loadingCommand = new PopupLoadingCommand(PopupLoadingInstance, message, title);
		commandQueue.AddFirst(loadingCommand);
		loadingCommand.Execute();
	}

	void OnMessagePopupCompeted()
	{
		PopupMessageCommand messageCommand = (PopupMessageCommand)(commandQueue.First.Value);
		commandQueue.RemoveFirst();
		messageCommand.UnExecute();

		// Execute next command, if there is.
		ExecuteNextCommand();
	}

	void OnQuestionPopupCompeted(PopupQuestionResult result)
	{
		PopupQuestionCommand questionCommand = (PopupQuestionCommand)(commandQueue.First.Value);
		commandQueue.RemoveFirst();
		questionCommand.UnExecute();

		// Execute next command, if there is.
		ExecuteNextCommand();
	}

	public void CloseLoading()
	{
		PopupLoadingCommand loadingCommand = (PopupLoadingCommand)(commandQueue.First.Value);
		if (loadingCommand == null)
			return;

		commandQueue.RemoveFirst();
		loadingCommand.UnExecute();

		// Execute next command, if there is.
		ExecuteNextCommand();
	}

	void ExecuteNextCommand()
	{
		if (commandQueue.Count > 0)
			commandQueue.First.Value.Execute();
	}



	public static PopupManager Get() { return instance; }
}
