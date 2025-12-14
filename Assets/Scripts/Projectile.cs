using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 12f;
    public float lifeTime = 2f;
    public float surfaceExitOffset = 0.02f;

    [Header("Damage")]
    [Tooltip("적에게 주는 데미지량")]
    public float damage = 10f;

    [Header("Projectile Type")]
    [Tooltip("Ice 발사체 여부 (속도 감소 효과)")]
    public bool isIceProjectile = false;
    
    [Tooltip("Poison 발사체 여부 (지속 데미지 효과)")]
    public bool isPoisonProjectile = false;

    Rigidbody _rigidbody;
    Vector3 _direction = Vector3.forward;
    float _lifeTimer;
    Light _pointLight;



    

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody != null)
        {
            _rigidbody.useGravity = false;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        _pointLight = GetComponentInChildren<Light>();
    }

    void OnEnable()
    {
        _lifeTimer = lifeTime;
        ApplyVelocity();
    }

    void Update()
    {
        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }

        // 현재 위치를 위험 지역으로 표시
        MarkCurrentPositionAsDanger();
    }

    void MarkCurrentPositionAsDanger()
    {
        if (GameController.Instance == null)
            return;

        int x = Mathf.RoundToInt(transform.position.x);
        int z = Mathf.RoundToInt(transform.position.z);

        GameController.Instance.MarkDangerZone(x, z, 2f);
    }

    public void Initialize(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
            _direction = direction.normalized;

        ApplyVelocity();
    }

    public void Reflect(Vector3 normal)
    {
        normal.y = 0f;
        if (normal.sqrMagnitude < 0.0001f)
        {
            normal = -_direction;
        }

        _direction = Vector3.Reflect(_direction, normal.normalized).normalized;
        transform.position += normal.normalized * surfaceExitOffset;
        ApplyVelocity();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision == null)
            return;

        Vector3 hitNormal = collision.contacts.Length > 0
            ? collision.contacts[0].normal
            : -_direction;

        if (collision.collider.TryGetComponent(out ProjectileReflector reflector))
        {
            // 반사 표면: 색상 변경 및 반사 처리

            speed -= 3f;
            if (speed < 2)
            {
                Destroy(gameObject);
                return;
            } 


            ChangeColorByBlock(collision.gameObject);
            reflector.HandleReflection(this, hitNormal);
            return;
        }

        // 적에 닿았는지 확인하고 데미지 적용
        if (TryDamageEnemy(collision.collider))
        {
            // 적에게 데미지를 주었으면 발사체 파괴
            Destroy(gameObject);
            return;
        }

        // 반사되지 않는 경우: 블록 색상 확인 및 Point Light 색상 변경
        ChangeColorByBlock(collision.gameObject);

        Destroy(gameObject);
    }



    void ChangeColorByBlock(GameObject block)
    {
        if (_pointLight == null || block == null)
            return;

        // 블록 이름에서 타입 확인
        if (block.name.Contains("Ice"))
        {
            _pointLight.color = new Color(0.25f, 0.25f, 1f); // 파란색
            isIceProjectile = true;
            
            // ContactDamage 컴포넌트에 Ice 상태 전달
            ContactDamage contactDamage = GetComponent<ContactDamage>();
            if (contactDamage != null)
            {
                contactDamage.isIceProjectile = true;
            }
        }
        else if (block.name.Contains("FireBlock"))
        {
            _pointLight.color = new Color(1f, 0.5f, 0f); // 주황색
            SpawnSplitProjectile();
        }
        else if (block.name.Contains("Poison"))
        {
            _pointLight.color = new Color(1.5f, 0f, 1.5f); // 보라색
            isPoisonProjectile = true;

            ContactDamage contactDamage = GetComponent<ContactDamage>();
            if (contactDamage != null)
            {
                contactDamage.isPoisonProjectile = true;
            }
        }
    }

    void SpawnSplitProjectile()
    {
        
        ApplyVelocity();
        
        Invoke(nameof(CreateSplitProjectile), 0.05f);
    }

    void CreateSplitProjectile()
    {
        if (this == null || gameObject == null)
            return;

        // 현재 발사체를 복제
        GameObject splitProjectile = Instantiate(gameObject, transform.position, transform.rotation);
        
        // 크기를 절반으로
        splitProjectile.transform.localScale = transform.localScale * 0.5f;

        Projectile splitComponent = splitProjectile.GetComponent<Projectile>();
        if (splitComponent != null)
        {
            // 속도와 수명을 40%으로
            splitComponent.speed = speed * 0.4f;
            splitComponent.lifeTime = lifeTime * 0.4f;
            splitComponent._lifeTimer = lifeTime * 0.4f;
            splitComponent.Initialize(_direction);
        }
    }

    void ApplyVelocity()
    {

        GetComponent<AudioSource>().Play();
        if (_rigidbody == null)
            return;

        Vector3 velocity = _direction * speed;
        velocity.y = 0f;
        _rigidbody.linearVelocity = velocity;
    }

    /// <summary>
    /// 적에게 데미지를 주는 함수. 성공하면 true 반환
    /// </summary>
    bool TryDamageEnemy(Collider hitCollider)
    {
        if (hitCollider == null)
            return false;

        // 적 태그 확인 (Enemy 또는 Enemy2 등)
        if (!hitCollider.CompareTag("Enemy"))
            return false;

        // Health 컴포넌트 찾기
        Health enemyHealth = hitCollider.GetComponent<Health>();
        if (enemyHealth == null)
        {
            // Health가 없으면 부모 오브젝트에서 찾기
            enemyHealth = hitCollider.GetComponentInParent<Health>();
        }

        if (enemyHealth != null)
        {
            // 기본 데미지 적용
            enemyHealth.TakeDamage(damage);
            Debug.Log($"적에게 {damage} 데미지를 입혔습니다. (남은 체력: {enemyHealth.currentHealth}/{enemyHealth.maxHealth})");

            // Ice 발사체 효과: 적 속도 감소
            if (isIceProjectile)
            {
                EnemyController enemyController = hitCollider.GetComponent<EnemyController>();
                if (enemyController == null)
                {
                    enemyController = hitCollider.GetComponentInParent<EnemyController>();
                }
                
                if (enemyController != null)
                {
                    enemyController.ReduceSpeed(0.5f, 3f); // 50% 속도 감소, 3초 지속
                    Debug.Log("Ice 효과: 적 속도 감소");
                }
            }

            // Poison 발사체 효과: 최대 체력의 10% 추가 데미지
            if (isPoisonProjectile)
            {
                float poisonDamage = enemyHealth.maxHealth * 0.1f;
                enemyHealth.TakeDamage(poisonDamage);
                Debug.Log($"Poison 효과: 추가 {poisonDamage} 데미지");
            }

            return true;
        }

        return false;
    }
}

