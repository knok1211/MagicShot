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
        //Debug.Log($"HealthBar Start - targetHealth: {targetHealth}, fillImage: {fillImage}");
        if (targetHealth != null)
        {
            targetHealth.onHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(targetHealth.currentHealth, targetHealth.maxHealth);
            //Debug.Log($"HealthBar 초기화 완료 - 체력: {targetHealth.currentHealth}/{targetHealth.maxHealth}");
        }

    }

    void UpdateHealthBar(float current, float max)
    {
        Debug.Log($"HealthBar UpdateHealthBar 호출 - current: {current}, max: {max}");
        if (fillImage == null) 
        {
            return;
        }

        float ratio = max > 0f ? current / max : 0f;
        fillImage.fillAmount = ratio;
        Debug.Log($"HealthBar fillAmount 설정: {ratio}");
    }

    void OnDestroy()
    {
        if (targetHealth != null)
        {
            targetHealth.onHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
}


