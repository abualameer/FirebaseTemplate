using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ILogger : MonoBehaviour
{
	public static ILogger Instance;
	public Text icon;
    public TMP_Text typeTitle;
    public Color errorColor, infoColor, warningColor, successColor;
	private UITransition transition;
	public TMP_Text text;
	private string previousMessage;
    private Image img;

	private void Awake()
	{
		Instance = this;
		transition = GetComponent<UITransition>();
        img = GetComponent<Image> ();
	}
	public void ShowMessage(string msg, LoggerType type = LoggerType.error, float duration = 5f)
	{
		if (msg.Equals(previousMessage))
		{
			Debug.Log("duplicated error");
			return;
		}
		if(type == LoggerType.info) {
            icon.text = "";
            typeTitle.text = "INFO";
            icon.color = infoColor;
        } else if( type == LoggerType.warning) {
            icon.text = "";
            typeTitle.text = "WARNING";
            icon.color = warningColor;
        } else if (type == LoggerType.success) {
            icon.text = "";
            typeTitle.text = "SUCCESS";
            icon.color = successColor;
        } else {
            icon.text = "";
            typeTitle.text = "Error";
            icon.color = errorColor;
        }

        typeTitle.color = icon.color;
        img.color = icon.color;
		transition.CancelInvoke();
		previousMessage = msg;
		Invoke("ForgetPreviousMessage", duration + transition.Duration());
		text.text = msg;
        text.gameObject.SetActive (false);
        Invoke ("ShowText", 0.1f);
		transition.Invoke("Hide", duration);
	}
    private void ShowText () {
        text.gameObject.SetActive (true);
        transition.Show ();
    }

    private void ForgetPreviousMessage()
	{
		previousMessage = "";
	}
}
public enum LoggerType
{
	error,
	info,
	warning,
    success
}