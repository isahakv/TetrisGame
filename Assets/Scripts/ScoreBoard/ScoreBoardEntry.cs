using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardEntry : MonoBehaviour
{
	public Image icon_Image;
	public Text name_Text, highScore_Text;

	public void Init(string iconURL, string name, int highScore)
	{
		name_Text.text = name;
		highScore_Text.text = highScore.ToString();
		StartCoroutine(UserDatabase.LoadImage(iconURL, OnIconLoaded));
	}

	void OnIconLoaded(Texture2D texture)
	{
		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		icon_Image.overrideSprite = sprite;
	}
}
