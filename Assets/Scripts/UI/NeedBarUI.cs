using UnityEngine;
using UnityEngine.UI;

public class NeedBarUI : MonoBehaviour
{
    public Text label;
    public Image fillRoot;
    public Image fill; 

    [Header("Colors")]
    public Color low  = new Color(0.86f, 0.23f, 0.23f);
    public Color mid  = new Color(0.95f, 0.80f, 0.20f);
    public Color high = new Color(0.26f, 0.80f, 0.36f);

    [Header("Smoothing")]
    [Range(0f, 1f)] public float smooth = 0.2f;

    private float displayed = 100f;
    private RectTransform fillRT;
    private RectTransform rootRT;

    void Awake()
    {
        fillRT = fill.rectTransform;
        rootRT = fillRoot.rectTransform;
    }

    public void SetLabel(string text) { if (label) label.text = text; }

    public void SetValue01(float v01)
    {
        displayed = Mathf.Lerp(displayed / 100f, v01, 1f - Mathf.Pow(1f - smooth, Time.deltaTime * 60f)) * 100f;
        float width = rootRT.rect.width * (displayed / 100f);
        fillRT.sizeDelta = new Vector2(width, fillRT.sizeDelta.y);
        
        var c = displayed < 33f ? low : (displayed < 66f ? mid : high);
        fill.color = c;
    }

    public void SetInstant(float v01)
    {
        displayed = v01 * 100f;
        float width = rootRT.rect.width * v01;
        fillRT.sizeDelta = new Vector2(width, fillRT.sizeDelta.y);
        var c = displayed < 33f ? low : (displayed < 66f ? mid : high);
        fill.color = c;
    }
}