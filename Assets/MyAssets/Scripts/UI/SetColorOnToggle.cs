using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SetColorOnToggle : MonoBehaviour
{
    public Image target;
    public Color normalColor, activeColor;
    void Start()
    {
        target.color = GetComponent<Toggle> ().isOn ? activeColor : normalColor;
        GetComponent<Toggle> ().onValueChanged.AddListener ((value) => {
            target.color = value ? activeColor : normalColor;
        });
    }

}
