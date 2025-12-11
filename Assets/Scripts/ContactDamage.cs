using UnityEngine;

/// <summary>
/// 이 오브젝트가 대상과 접촉해 있는 동안 매초 일정량의 피해를 줍니다.
/// (적에 붙이고, targetTag를 "Player"로 두면 플레이어가 지속 피해를 받음)
/// </summary>
[RequireComponent(typeof(Collider))]
public class ContactDamage : MonoBehaviour
{
    [Tooltip("지속적으로 피해를 줄 대상의 태그")]
    public string targetTag = "Player";

    [Tooltip("1초당 주는 피해량")]
    public float damagePerSecond = 10f;

    [Tooltip("Ice 프로젝타일 여부")]
    public bool isIceProjectile = false;

    [Tooltip("poison 여부")]
    public bool isPoisonProjectile = false;


    void OnCollisionStay(Collision collision)
    {
        if (collision == null) return;
        if (!collision.collider.CompareTag(targetTag)) return;

        ApplyDamage(collision.collider);
    }

    void OnTriggerStay(Collider other)
    {
        if (other == null) return;
        if (!other.CompareTag(targetTag)) return;

        ApplyDamage(other);
    }

    void ApplyDamage(Collider targetCollider)
    {
        if (damagePerSecond <= 0f) return;

        Health health = targetCollider.GetComponent<Health>();
        if (health != null)
        {
            // 접촉 시간에 비례해서 체력이 줄어들도록 Time.deltaTime 사용
            float damageThisFrame = damagePerSecond * Time.deltaTime;
            health.TakeDamage(damageThisFrame);
            
            // Ice 프로젝타일 효과: enemy 속도 감소

            if (isIceProjectile)
            {
                
                EnemyController enemyController = targetCollider.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.ReduceSpeed(0.5f);
                }
            }

                if (isPoisonProjectile)
            {
                
                EnemyController enemyController = targetCollider.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    health.TakePerDamage(0.1f);
                }
            }


        }
    }
}


