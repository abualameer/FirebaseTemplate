using UnityEngine;

public class Constants : MonoBehaviour
{
    #region Strings
    public const string EMAIL_PREFS = "Email";
    public const string PASSWORD_PREFS = "Password";
    public const string GRAPHICS_LEVEL_PREFS = "Graphics";
    public const string LOGIN_TYPE_PREFS = "LoginType";
    #endregion
    public static Constants Instance;
    void Awake()
    {
        if (!Instance)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
