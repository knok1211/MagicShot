using UnityEngine;

/// <summary>
/// 적이 죽을 때 재화를 지급하고, 적 오브젝트를 제거하는 컴포넌트.
/// </summary>
[RequireComponent(typeof(Health))]
public class EnemyReward : MonoBehaviour
{
    public int rewardAmount = 10;

    Health _health;

    void Awake()
    {
        _health = GetComponent<Health>();
        if (_health != null)
        {
            _health.onDie.AddListener(HandleDeath);
        }
    }

    void HandleDeath()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCurrency(rewardAmount);
        }

        GameController.Instance.AddCount(1);



        Destroy(gameObject);
    }
}


