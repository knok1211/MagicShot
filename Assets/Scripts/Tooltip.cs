using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public Button closeButton;
    public TMPro.TextMeshProUGUI tooltipText;
    [TextArea(1,10)] public string defaultTooltip = "This is a tooltip.";

    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideTooltip);
        }
        ShowTooltip(defaultTooltip);
    }

    public void ShowTooltip(string text)
    {
        if (tooltipText != null)
        {
            tooltipText.text = text;
        }
        gameObject.SetActive(true);
    }


        


    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}