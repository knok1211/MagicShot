using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health 컴포넌트의 체력 변화를 UI Image(fillAmount)로 표시.
/// </summary>
public class HealthBar : MonoBehaviour
{
    public Health targetHealth;
    public Image fillImage;

    void Start()
    {
        if (targetHealth != null)
        {
            targetHealth.onHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(targetHealth.currentHealth, targetHealth.maxHealth);
        }
    }

    void UpdateHealthBar(float current, float max)
    {
        if (fillImage == null) return;

        float ratio = max > 0f ? current / max : 0f;
        fillImage.fillAmount = ratio;
    }

    void OnDestroy()
    {
        if (targetHealth != null)
        {
            targetHealth.onHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
}


