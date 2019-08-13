using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessage : Popup
{
	PopupMessageCompleteDelegate onConfirmedCallBack;

	public void Init(PopupType type, string msg, string titleText = null, PopupMessageCompleteDelegate _onConfirmedCallBack = null)
	{
		SetupTitle(type, titleText);

		message.text = msg;
		onConfirmedCallBack = _onConfirmedCallBack;
	}

	public void OnConfirmButton_Pressed()
	{
		onConfirmedCallBack?.Invoke();
	}
}
