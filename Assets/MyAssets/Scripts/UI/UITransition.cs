using UnityEngine;
using UnityEngine.Events;

public abstract class UITransition : MonoBehaviour
{
    public UnityEvent onShown, onHidden;
    public abstract void Show();
    public abstract void Hide();
    public abstract float Duration();

    [HideInInspector] public bool shown = false;
    public void SetVisible(bool visible)
    {
        if (visible)
            Show();
        else
            Hide();
    }
    public void SetChildrenActive (bool active)
    {
        gameObject.SetActive(active);
    }
}