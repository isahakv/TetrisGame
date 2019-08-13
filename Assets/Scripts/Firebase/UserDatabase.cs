using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;

[System.Serializable]
public class UserData
{
	public string userID;
	public string name;
	public string iconURL;
	public int highScore;

	public UserData() { }

	public UserData(string _userID, string _name, string _iconURL, int _highScore)
	{
		userID = _userID;
		name = _name;
		iconURL = _iconURL;
		highScore = _highScore;
	}

	public Dictionary<string, object> ToDictionary()
	{
		Dictionary<string, object> result = new Dictionary<string, object>();
		result["userID"] = userID;
		result["name"] = name;
		result["iconURL"] = iconURL;
		result["highScore"] = highScore;
		return result;
	}

	public static UserData FromDictionary(Dictionary<string, object> dictionary)
	{
		UserData result = new UserData();
		result.userID = (string)dictionary["userID"];
		result.name = (string)dictionary["name"];
		result.iconURL = (string)dictionary["iconURL"];
		result.highScore = int.Parse((dictionary["highScore"]).ToString());
		return result;
	}
}

public class UserDatabase : MonoBehaviour
{
	static UserDatabase instance;
	DatabaseReference usersRef;

	public static UserDatabase Get() { return instance; }

	void Awake()
	{
		if (instance == null)
		{
			DontDestroyOnLoad(this);
			instance = this;

			FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://tetrisgame-2019.firebaseio.com/");
			Init();
		}
		else if (instance != this)
			DestroyImmediate(gameObject);
	}

	public void Init()
	{
		 DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
		 if (dbRef != null)
		 	usersRef = dbRef.Child("users");
	}

	public void SyncUserHighscore()
	{
		TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
		PopupManager.Get().OpenLoading("", "Synchronizing Your Data With Server...");
		int localHighScore = PlayerPrefs.GetInt("highscore", 0);
		ReceiveUserHighScore().ContinueWith(task =>
		{
			int serverHighScore = task.Result;
			if (serverHighScore != -1 && serverHighScore != localHighScore)
			{
				if (serverHighScore < localHighScore)
					PostUserHighScore(localHighScore).ContinueWith(t => { });
				else
					localHighScore = serverHighScore;
			}
			StartCoroutine(SyncUserHighscoreCoroutine(localHighScore));
		}, taskScheduler);
	}

	private IEnumerator SyncUserHighscoreCoroutine(int newHighScore)
	{
		yield return null;
		PopupManager.Get().CloseLoading();
		PlayerPrefs.SetInt("highscore", newHighScore);
		PlayerPrefs.Save();
	}

	private async Task<int> ReceiveUserHighScore()
	{
		if (!UserAuth.IsUserLoggedIn())
			return -1;

		FirebaseUser user = UserAuth.Get().GetUser();

		Dictionary<string, object> userDictionary = await GetUserDictionaryByID(user.UserId);
		if (userDictionary == null || userDictionary.Count == 0)
			return -1;

		return GetUserHighscore(userDictionary);
	}

	public async Task PostUserHighScore(int newHighScore)
	{
		if (!UserAuth.IsUserLoggedIn())
			return;

		FirebaseUser user = UserAuth.Get().GetUser();

		Debug.Log("UserId: " + user.UserId + ", DisplayName: " + user.DisplayName + ",  jkhjkyuiyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy");

		Dictionary<string, object> userDictionary = await GetUserDictionaryByID(user.UserId);
		if (userDictionary == null || userDictionary.Count == 0) // If there is no user data in database, then create it.
		{
			Debug.Log("Heheheheeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeyyyyyyyyyyyyyyyyyyyyyy");

			await CreateNewUserData(new UserData(user.UserId, user.DisplayName, user.PhotoUrl.ToString(), newHighScore));
		}
		else
			SetUserHighscore(userDictionary, newHighScore);
	}

	private void SetUserHighscore(Dictionary<string, object> userDictionary, int newHighScore)
	{
		foreach (KeyValuePair<string, object> item in userDictionary)
		{
			Dictionary<string, object> dict = (Dictionary<string, object>)item.Value;
			dict["highScore"] = newHighScore;
			usersRef.UpdateChildrenAsync(userDictionary);
		}
	}

	private int GetUserHighscore(Dictionary<string, object> userDictionary)
	{
		foreach (KeyValuePair<string, object> item in userDictionary)
		{
			Dictionary<string, object> dict = (Dictionary<string, object>)item.Value;
			return int.Parse((dict["highScore"]).ToString());
		}
		return -1;
	}

	public async Task<UserData[]> GetAllUserData(bool sortByHighScore = true)
	{
		Query query = usersRef.OrderByChild("highScore");
		DataSnapshot dataSnapshot = await query.GetValueAsync();
		Dictionary<string, object> dictionary = (Dictionary<string, object>)dataSnapshot.Value;
		List<UserData> userDatas = new List<UserData>();

		foreach (KeyValuePair<string, object> item in dictionary)
		{
			Dictionary<string, object> dict = (Dictionary<string, object>)item.Value;
			UserData userData = UserData.FromDictionary(dict);
			userDatas.Add(userData);
		}

		if (sortByHighScore)
		{
			userDatas.Sort((x, y) => { return x.highScore.CompareTo(y.highScore); });
			userDatas.Reverse();
		}

		return userDatas.ToArray();
	}

	private async Task<Dictionary<string, object>> GetDictionaryByReference(DatabaseReference reference)
	{
		Task<DataSnapshot> task = reference.GetValueAsync();
		DataSnapshot data = await task;
		if (task.IsFaulted)
			Debug.Log("GetDictionaryByReference Failed: " + task.Exception);
		else if (task.IsCompleted && data != null)
			return (Dictionary<string, object>)data.Value;
		return null;
	}

	private async Task<Dictionary<string, object>> GetDictionaryByQuery(Query query)
	{
		Task<DataSnapshot> task = query.GetValueAsync();
		DataSnapshot data = await task;
		if (task.IsFaulted)
			Debug.Log("GetDictionaryByQuery Failed: " + task.Exception);
		else if (task.IsCompleted && data != null)
			return (Dictionary<string, object>)data.Value;
		return null;
	}

	private async Task<Dictionary<string, object>> GetUserDictionaryByID(string userid)
	{
		Query query = usersRef.OrderByChild("userID").EqualTo(userid);
		return await GetDictionaryByQuery(query);
	}

	async Task<DatabaseReference> CreateNewUserData(UserData userData)
	{
		DatabaseReference userRef = usersRef.Push();
		string userDataJson = JsonUtility.ToJson(userData);
		await userRef.SetRawJsonValueAsync(userDataJson);
		return userRef;
	}

	public static IEnumerator LoadImage(string url, Action<Texture2D> callback)
	{
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError)
			Debug.Log(request.error);
		else
		{
			Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
			callback(texture);
		}
	}
}
