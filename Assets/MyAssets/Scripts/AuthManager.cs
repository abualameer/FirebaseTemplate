using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;

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
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
    }
    public IEnumerator Login(string _email, string _password)
    {
        LoadingScreen.Instance.Show("Loging in...");
        //Call the Firebase auth signin function passing the email and password
        yield return new WaitUntil(() => auth != null);
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
        LoadingScreen.Instance.Hide();
        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
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
                //Create a user profile and set the username
                UserProfile profile = new UserProfile { DisplayName = _displayName };

                //Call the Firebase auth update user profile function passing the profile with the username
                var ProfileTask = User.UpdateUserProfileAsync(profile);
                //Wait until the task completes
                yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);
                if (ProfileTask.Exception != null)
                {
                    //If there are errors handle them
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
}