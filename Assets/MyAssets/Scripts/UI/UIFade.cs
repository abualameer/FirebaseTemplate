using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFade : UITransition
{
    [HideInInspector] public CanvasGroup canvasGroup;
    public float duration = 0.3f;
    [Range(0, 1)] public float alpha = 1;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (alpha <= 0)
            Hide();
        else
            Show();
        shown = canvasGroup.alpha == 1;
        SetChildrenActive(shown);
    }
    void Update()
    {
        canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha + ((alpha == 1) ? 1 : -1) * Time.deltaTime / duration);
        canvasGroup.interactable = alpha > 0.5f;
        canvasGroup.blocksRaycasts = alpha > 0.5f;
        if(canvasGroup.alpha == 0)
        {
            SetChildrenActive(false);
        }
    }
    public override void Show()
    {
        SetChildrenActive(true);
        shown = true;
        alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        onShown.Invoke();
    }
    public override void Hide()
    {
        shown = false;
        alpha = 0;
        onHidden.Invoke();
    }

    public override float Duration()
    {
        return duration;
    }
}
