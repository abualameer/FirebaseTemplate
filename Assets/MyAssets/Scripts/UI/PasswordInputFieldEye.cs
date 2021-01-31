using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordInputFieldEye : MonoBehaviour
{
    private TMP_InputField input;
    private Text eyeIcon;
    private bool show = false;
    private void Awake () {
        input = transform.parent.GetComponent<TMP_InputField> ();
        eyeIcon = GetComponent<Text> ();
    }
    public void Toggle () {
        show = !show;
        eyeIcon.text = show ? "" : "";
        input.contentType = show ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        input.ForceLabelUpdate ();
    }
}
