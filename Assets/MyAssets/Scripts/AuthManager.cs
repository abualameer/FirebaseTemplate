using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Facebook.Unity;
using System.Collections.Generic;

public class AuthManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    public static AuthManager Instance;
    void Awake()
    {
        Instance = this;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
        if (!FB.IsInitialized)
            FB.Init(InitCallback, null);
        else
            FB.ActivateApp();
    }
    private void InitCallback()
    {
        if (FB.IsInitialized)
            FB.ActivateApp();
        else
            Debug.Log("Failed to Initialize the Facebook SDK");
    }
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
    }
    public IEnumerator Login(string _email, string _password)
    {
        LoadingScreen.Instance.Show("Loging in...");
        yield return new WaitUntil(() => auth != null);
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
        LoadingScreen.Instance.Hide();
        if (LoginTask.Exception != null)
        {
            Debug.LogError(LoginTask.Exception);
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            ILogger.Instance.ShowMessage(message, LoggerType.error);
        }
        else
        {
            User = LoginTask.Result;
            PlayerPrefs.SetInt(Constants.LOGIN_TYPE_PREFS, 0);
            PlayerPrefs.SetString(Constants.EMAIL_PREFS, _email);
            PlayerPrefs.SetString(Constants.PASSWORD_PREFS, _password);
            ILogger.Instance.ShowMessage("Logged in...", LoggerType.info);
            LoginScreenUI.Instance.AfterLogin();
        }
    }

    public IEnumerator Register(string _email, string _password, string _displayName)
    {
        LoadingScreen.Instance.Show("Signing up...");
        yield return new WaitUntil(() => auth != null);
        var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);
        LoadingScreen.Instance.Hide();
        if (RegisterTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = "Register Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;
            }
            ILogger.Instance.ShowMessage(message, LoggerType.error);
        }
        else
        {
            User = RegisterTask.Result;
            if (User != null)
            {
                UserProfile profile = new UserProfile { DisplayName = _displayName };
                var ProfileTask = User.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
                if (ProfileTask.Exception != null)
                {
                    Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                    FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                    ILogger.Instance.ShowMessage("Diplay Name Set Failed!", LoggerType.error);
                }
                else
                {
                    PlayerPrefs.SetString(Constants.EMAIL_PREFS, _email);
                    PlayerPrefs.SetString(Constants.PASSWORD_PREFS, _password);
                    LoginScreenUI.Instance.AfterLogin();
                }
            }
        }
    }
    public URLRawImage img;
    public IEnumerator LoginWithFacebook()
    {
        auth.SignOut();
        LoadingScreen.Instance.Show("Siging in with Facebook...");
        yield return new WaitUntil(() => FB.IsInitialized);
        string accessToken = "";
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        permissions.Add("email");
        FB.LogInWithReadPermissions(permissions, delegate (ILoginResult result)
        {
            if (!string.IsNullOrEmpty(result.Error) || !FB.IsLoggedIn)
            {
                ILogger.Instance.ShowMessage("Failed to login in with facebook", LoggerType.error);
                LoadingScreen.Instance.Hide();
                Debug.LogError(result.Error);
                return;
            }

            accessToken = AccessToken.CurrentAccessToken.TokenString;
            Debug.Log(accessToken);
            FB.API("me?fields=first_name", HttpMethod.GET, delegate (IGraphResult res)
            {
                if (res.Error == null)
                {
                    Debug.Log(res.ResultDictionary["first_name"] + ": " + AccessToken.CurrentAccessToken.UserId + "\nExpires in: " + AccessToken.CurrentAccessToken.ExpirationTime);
                }
            });
            Credential credential = FacebookAuthProvider.GetCredential(accessToken);
            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                LoadingScreen.Instance.Hide();
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }
                User = task.Result;
                if (User != null)
                {
                    string name = User.DisplayName;
                    string email = User.Email;
                    System.Uri photo_url = User.PhotoUrl;
                    Debug.Log(name + ": " + email + ": " + photo_url);
                    LoginScreenUI.Instance.AfterLogin();
                }
            });
        });
    }
}