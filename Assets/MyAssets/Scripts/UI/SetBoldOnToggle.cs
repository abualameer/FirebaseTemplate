using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SetBoldOnToggle : MonoBehaviour
{
    public bool setBold = true;
    public RTLTMPro.RTLTextMeshPro target;
    public bool setColor = false;
    public Color normalColor, activeColor;
    void Start()
    {
        Toggle toggle = GetComponent<Toggle>();
        if (setBold)
            target.fontStyle = toggle.isOn? TMPro.FontStyles.Bold : TMPro.FontStyles.Normal;
        if (setColor)
            target.color = toggle.isOn ? activeColor : normalColor;
        toggle.onValueChanged.AddListener ((value) => {
            if (setBold)
                target.fontStyle = value ? TMPro.FontStyles.Bold : TMPro.FontStyles.Normal;
            if (setColor)
                target.color = value ? activeColor : normalColor;
        });
    }

}
