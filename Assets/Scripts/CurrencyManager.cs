using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 재화(코인/점수) 관리 매니저. 싱글톤으로 사용.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public int currentCurrency = 0;
    public UnityEvent<int> onCurrencyChanged; // (현재 재화)

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        onCurrencyChanged?.Invoke(currentCurrency);
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        if (currentCurrency < 0) currentCurrency = 0;

        onCurrencyChanged?.Invoke(currentCurrency);
    }
}


