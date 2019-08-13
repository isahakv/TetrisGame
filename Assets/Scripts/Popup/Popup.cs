using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
	public Text title, message;
	public Color infoColor = Color.blue, warningColor = Color.yellow, errorColor = Color.red;

	protected void SetupTitle(PopupType type, string titleStr)
	{
		// Setup text.
		if (string.IsNullOrEmpty(titleStr))
			title.text = type.ToString();
		else
			title.text = titleStr;

		// Setup color.
		switch (type)
		{
			case PopupType.Info:
				title.color = infoColor;
				break;
			case PopupType.Warning:
				title.color = warningColor;
				break;
			case PopupType.Error:
				title.color = errorColor;
				break;
		}
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
