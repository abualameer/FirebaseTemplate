using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    [HideInInspector] public UITransition transition;
    private RTLTMPro.RTLTextMeshPro text;
    public static bool isShown;
    public GameObject loopProgressBar;
    public Slider progressBar;
    public UITransition overlayPrefab;
    private AsyncOperation operation;
    bool isShowingOperationProgress;
    private List<UITransition> anchoredLoadings = new List<UITransition>();
    void Awake()
    {
        Instance = this;
        transition = GetComponent<UITransition>();
        text = GetComponentInChildren<RTLTMPro.RTLTextMeshPro>();
    }
    private void Update()
    {
        if (isShowingOperationProgress && operation!=null)
        {
            progressBar.value = operation.progress * 100;
        }
    }
    /// <summary>
    /// General loading show
    /// </summary>
    /// <param name="loadingText"><B>[Optional]</B> a string to show on loading screen</param>
    /// <param name="loop"><B>[Optional]</B> determine if you want a looped progress bar or not, <B>if not</B> a normal progress bar is showed.</param>
    public void Show(string loadingText = "", bool loop = true)
    {
        text.text = loadingText;
        progressBar.gameObject.SetActive(!loop);
        loopProgressBar.SetActive(loop);
        isShown = true;
        transition.Show();
    }

    /// <summary>
    /// Shows an overlay over the given rect area.
    /// </summary>
    /// <param name="rect">the rect that will be covered by the overlayLoading</param>
    public void Show(RectTransform rect)
    {
        UITransition overlayImage = Instantiate(overlayPrefab, rect);
        RectTransform rt = overlayImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.SetParent(rect);
        rt.SetAsLastSibling();
        if(rt.GetComponentInParent<ScrollRect>()) rt.GetComponentInParent<ScrollRect>().enabled = false;
        overlayImage.Show();
        overlayImage.onHidden.AddListener(delegate { 
            Destroy(overlayImage.gameObject);
            if (rt.GetComponentInParent<ScrollRect>())
                rt.GetComponentInParent<ScrollRect>().enabled = true;
        });
        anchoredLoadings.Add(overlayImage);
    }
    /// <summary>
    /// Show loading screen with certin progress
    /// </summary>
    /// <param name="loadingText"><B>[REQUIRED] </B>a string to show on loading screen</param>
    /// <param name="progress">your manual operation progress <B>100 means 100%</B></param>
    public void Show(string loadingText, float progress)
    {
        if(!progressBar.gameObject.activeInHierarchy) progressBar.gameObject.SetActive(true);
        progressBar.value = progress;
        Show(loadingText, false);
    }
    /// <summary>
    /// Show loading screen with an operation progress
    /// </summary>
    /// <param name="loadingText">[REQUIRED] a string to show on loading screen</param>
    /// <param name="operation">your async operation</param>
    /// <param name="autoHideAfter">[Optional] determine if you want to hide the loading after the operation is done</param>
    public void Show(string loadingText, AsyncOperation operation, bool autoHideAfter = false)
    {
        this.operation = operation;
        isShowingOperationProgress = true;
        operation.completed += delegate { 
            isShowingOperationProgress = false;
            if (autoHideAfter)
                Hide();
        };
        Show(loadingText, false);
    }
    /// <summary>
    /// HIDE LOADING SCREEN
    /// </summary>
    public void Hide()
    {
        isShown = false;
        isShowingOperationProgress = false;
        operation = null;
        progressBar.value = 0;
        transition.Hide();
        foreach (UITransition uITransition in anchoredLoadings)
        {
            uITransition.Hide();
        }
        anchoredLoadings.Clear();
    }
}
