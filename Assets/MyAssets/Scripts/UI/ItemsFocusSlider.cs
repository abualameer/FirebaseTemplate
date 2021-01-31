using UnityEngine;
using UnityEngine.UI;

public class ItemsFocusSlider : MonoBehaviour
{
    public RectTransform content;
    public float itemWidth;
    public float itemSpace;
    public float margins;
    private float fullItemSize;
    private bool isUserControlling = false;
    private Vector2 distention;
    private int forceItem = -1;
    private int currentItem;
    private UITransition uITransition;
    
    void Awake()
    {
        uITransition = GetComponentInParent<UITransition>();
    }
    void Start()
    {
        Init();
    }
    public void Init()
    {
        float width = itemWidth * content.childCount + itemSpace * (content.childCount - 1) + 2 * margins;
        content.sizeDelta = new Vector2(width, content.sizeDelta.y);
        fullItemSize = (itemWidth + itemSpace);
    }
    void Update()
    {
        if (!uITransition.shown)
            return;
        float position = Mathf.Abs (content.anchoredPosition.x);
        currentItem = Mathf.RoundToInt (position / fullItemSize);
        currentItem = Mathf.Clamp (currentItem, 0, content.childCount - 1);
        if (forceItem != -1) {
            if (currentItem == forceItem)
                forceItem = -1;
            else
                currentItem = forceItem;
        }
        if(content.childCount > 0) content.GetChild (currentItem).GetComponent<Toggle> ().isOn = true;
        if (!isUserControlling) {
            distention = new Vector2 (-currentItem * fullItemSize, content.anchoredPosition.y);
            content.anchoredPosition = Vector2.Lerp (content.anchoredPosition, distention, Time.deltaTime * 5);
        }
    }
    public void PointerDownEvent () {
        isUserControlling = true;
    }
    public void PointerUpEvent() {
        isUserControlling = false;
    }
    public void Prev() {
        forceItem = Mathf.Max (currentItem - 1, 0);
    }
    public void Next () {
        forceItem = Mathf.Min (currentItem + 1, content.childCount - 1);
    }
}
