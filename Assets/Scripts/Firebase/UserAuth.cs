using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;

public class UserAuth : MonoBehaviour
{
	private static UserAuth instance;
	FirebaseAuth auth;
	FirebaseUser user;

	public static UserAuth Get() { return instance; }
	public FirebaseAuth GetAuth() { return auth; }
	public FirebaseUser GetUser() { return user; }

	void Awake()
	{
		if (instance == null)
		{
			DontDestroyOnLoad(this);
			instance = this;

			if (!FB.IsInitialized)
				FB.Init(OnFBInitComplete);
		}
		else if (instance != this)
			DestroyImmediate(gameObject);
	}

	void OnFBInitComplete()
	{
		if (FB.IsInitialized)
		{
			Debug.Log("FB Init Complete!");
			FB.ActivateApp();

			InitFirebase();
		}
		else
			Debug.Log("FB Init Failed!");
	}

	void InitFirebase()
	{
		auth = FirebaseAuth.DefaultInstance;
		auth.StateChanged += AuthStateChanged;
		// AuthStateChanged(this, null);
		/*FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			DependencyStatus dependencyStatus = task.Result;
			if (dependencyStatus == DependencyStatus.Available)
			{
				Debug.Log("Firebase dependencies are available.");

				auth = FirebaseAuth.DefaultInstance;
				FirebaseApp.LogLevel = LogLevel.Debug;
			}
			else
				Debug.Log("Could not resolve all Firebase dependencies: " + dependencyStatus.ToString());
		});*/
	}

	void AuthStateChanged(object sender, System.EventArgs eventArgs)
	{
		if (auth.CurrentUser != user)
		{
			user = auth.CurrentUser;
			if (auth.CurrentUser == null)
			{
				// SignedOut
				PlayerPrefs.SetInt("highscore", 0);
				PlayerPrefs.Save();
			}
			else
			{
				// SignedIn
				UserDatabase.Get().SyncUserHighscore();
			}
		}
	}

	/*public static void SignInWithGoogle()
	{
		GoogleSignIn.Configuration = new GoogleSignInConfiguration {
			RequestIdToken = true,
			WebClientId = "814756471280-m1mecq40nremh4uuc6dn8lnfv05curqb.apps.googleusercontent.com" // "814756471280-t2logvkl2g7kvdj6fpkvv9nnt8gi7set.apps.googleusercontent.com"
		};

		Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
		TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

		signIn.ContinueWith(task =>
		{
			if (task.IsCanceled)
			{
				resultText.text = "Pass 1!";
				signInCompleted.SetCanceled();
			}
			else if (task.IsFaulted)
			{
				resultText.text = "Pass 2! - " + task.Exception.ToString();
				signInCompleted.SetException(task.Exception);
			}
			else
			{
				resultText.text = "Pass 3!";

				Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
				Task<FirebaseUser> userTask = FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);
				userTask.ContinueWith(authTask =>
				{
					if (authTask.IsCanceled)
					{
						resultText.text = "Pass 4!";
						signInCompleted.SetCanceled();
					}
					else if (authTask.IsFaulted)
					{
						resultText.text = "Pass 5!";
						signInCompleted.SetException(authTask.Exception);
					}
					else
					{
						resultText.text = "Pass 6!";

						signInCompleted.SetResult(authTask.Result);

						FirebaseUser newUser = authTask.Result;
						if (newUser != null)
							resultText.text = newUser.DisplayName + " : " + newUser.Email;
						else
							resultText.text = "Fuckin user is null!";
					}
				});
			}
		});
	}*/

	public static bool IsUserLoggedIn()
	{
		return Get().GetUser() != null;
	}

	public static string GetUserName()
	{
		if (!IsUserLoggedIn())
			return "Not Logged In!";

		string longName = Get().GetUser().DisplayName;
		string[] names = longName.Split(' ');
		if (names.Length > 0)
			return names[0];
		return longName;
	}

	public void SignInWithFacebook()
	{
		string[] permissions = new string[] { "public_profile", "email" };
		FB.LogInWithReadPermissions(permissions, OnFacebookLoginResult);
	}

	private void OnFacebookLoginResult(ILoginResult result)
	{
		Debug.Log("Pass 2!");

		if (!FB.IsLoggedIn)
		{
			Debug.Log("User Cancel Login!");
			return;
		}

		AccessToken accessToken = AccessToken.CurrentAccessToken;
		Debug.Log("accessToken.UserId = " + accessToken.UserId);

		SignInFirebase(accessToken.TokenString);
	}

	private void SignInFirebase(string accessToken)
	{
		Debug.Log("Pass 100!");
		// auth = FirebaseAuth.DefaultInstance;
		// auth.StateChanged += AuthStateChanged;

		Credential credential = FacebookAuthProvider.GetCredential(accessToken);
		Task<FirebaseUser> userTask = auth.SignInWithCredentialAsync(credential);
		if (userTask != null)
		{
			Debug.Log("SignInFirebase Succeed!");

			userTask.ContinueWith(task =>
			{
				if (task.IsCanceled)
					Debug.Log("Pass 4!");
				else if (task.IsFaulted)
					Debug.Log("Pass 5!");
				else
				{
					Debug.Log("Pass 6!");

					FirebaseUser newUser = task.Result;
					if (newUser != null)
					{
						Debug.Log(newUser.DisplayName + " : " + newUser.Email);
					}
					else
						Debug.Log("Fuckin user is null!");
				}
			});
		}
		else
			Debug.Log("userTask == null");
	}

	public void SignInAnonymously()
	{
		Debug.Log("Starting to Sign-In Anonymously!");

		auth.SignInAnonymouslyAsync().ContinueWith(task => {
			if (task.IsCanceled)
			{
				Debug.LogError("SignInAnonymouslyAsync was canceled.");
				return;
			}
			if (task.IsFaulted)
			{
				Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
				return;
			}

			FirebaseUser newUser = task.Result;
			Debug.LogFormat("User signed in successfully: {0} ({1})",
				newUser.DisplayName, newUser.UserId);
		});
	}

	public void SignOut()
	{
		PopupManager.Get().NewMessage(PopupType.Info, null, "If you Sign Out your local stats will be TERMINATIED!");
		FB.LogOut();
		GetAuth().SignOut();
	}
}
