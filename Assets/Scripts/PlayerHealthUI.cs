using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 체력을 Screen Space UI 바에 표시하는 스크립트.
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    public Health playerHealth;
    public Image fillImage;

    void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(playerHealth.currentHealth, playerHealth.maxHealth);
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
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
}


