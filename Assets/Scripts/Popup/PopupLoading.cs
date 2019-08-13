using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupLoading : Popup
{
	public void Init(string msg, string titleText = null)
	{
		SetupTitle(PopupType.Loading, titleText);

		message.text = msg;
	}
}
