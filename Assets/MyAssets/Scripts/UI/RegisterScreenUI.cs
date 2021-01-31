using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RegisterScreenUI : MonoBehaviour
{
    public TMP_InputField firstname;
    public TMP_InputField lastname;
    public TMP_InputField email;
    public TMP_InputField password;
    public TMP_Dropdown country;
    public Toggle agreedToPolicy;
    public void Register()
    {
        if (string.IsNullOrEmpty(firstname.text) ||
            string.IsNullOrEmpty(lastname.text) ||
            string.IsNullOrEmpty(email.text) ||
            string.IsNullOrEmpty(password.text))
        {
            ILogger.Instance.ShowMessage("Please fill in your data.");
            return;
        }
        if (!agreedToPolicy.isOn)
        {
            ILogger.Instance.ShowMessage("Please read and agree to our terms and privacy policy.");
            return;
        }
        StartCoroutine(AuthManager.Instance.Register(email.text, password.text, firstname.text+ " " + lastname.text));
    }
}
