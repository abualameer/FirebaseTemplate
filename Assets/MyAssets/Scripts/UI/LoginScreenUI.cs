using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScreenUI : MonoBehaviour
{
    public static LoginScreenUI Instance;

    public TMP_InputField email;
    public TMP_InputField password;
    public UITransition container;
    private LoginType loginType;
    private void Awake()
    {
        Instance = this;
        loginType = (LoginType) PlayerPrefs.GetInt(Constants.LOGIN_TYPE_PREFS, 0);
    }
    private void Start()
    {
        TryAutoLogin();
    }
    public void TryAutoLogin()
    {
        if(true)//AuthManager.Instance.User == null)
        {
            switch (loginType)
            {
                case LoginType.basic:
                    email.text = PlayerPrefs.GetString(Constants.EMAIL_PREFS, "");
                    password.text = PlayerPrefs.GetString(Constants.PASSWORD_PREFS, "");
                    if (!string.IsNullOrEmpty(email.text))
                    {
                        Login();
                    }
                    else
                    {
                        LoadingScreen.Instance.Hide();
                    }
                    break;
                case LoginType.facebook:
                    LoadingScreen.Instance.Hide();
                    break;
                default:
                    LoadingScreen.Instance.Hide();
                    break;
            }
            
        }
        else
        {
            AfterLogin();
        }

    }
    public void Login()
    {
        if (string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text))
        {
            ILogger.Instance.ShowMessage("Please fill in your <B>email or phone number</B> and <B>password</B>");
            return;
        }
        StartCoroutine(AuthManager.Instance.Login(email.text, password.text));
    }
    public void LoginWithFacebook()
    {
        StartCoroutine(AuthManager.Instance.LoginWithFacebook());
    }
    public void AfterLogin()
    {
        LoadingScreen.Instance.Show("Loading Game...", SceneManager.LoadSceneAsync(1),true);
    }
}
enum LoginType
{
    basic = 0,
    facebook = 1
}