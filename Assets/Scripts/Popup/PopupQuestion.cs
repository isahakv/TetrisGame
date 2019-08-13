using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupQuestion : Popup
{
	PopupQuestionCompleteDelegate onCompleteCallBack;

	public void Init(PopupType type, string msg, string titleText = null, PopupQuestionCompleteDelegate _onCompleteCallBack = null)
	{
		SetupTitle(type, titleText);

		message.text = msg;
		onCompleteCallBack = _onCompleteCallBack;
	}

	public void OnAcceptButton_Pressed()
	{
		onCompleteCallBack?.Invoke(PopupQuestionResult.Accept);
	}

	public void OnDeclineButton_Pressed()
	{
		onCompleteCallBack?.Invoke(PopupQuestionResult.Decline);
	}
}
