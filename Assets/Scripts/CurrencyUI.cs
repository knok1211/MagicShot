using UnityEngine;
using TMPro;

/// <summary>
/// CurrencyManager의 재화 값을 TextMeshPro UI에 표시.
/// </summary>
public class CurrencyUI : MonoBehaviour
{
    public TextMeshProUGUI currencyText;

    void Start()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.onCurrencyChanged.AddListener(UpdateCurrencyText);
            UpdateCurrencyText(CurrencyManager.Instance.currentCurrency);
        }
    }

    void UpdateCurrencyText(int value)
    {
        if (currencyText != null)
        {
            currencyText.text = value.ToString();
        }
    }
}


