using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 12f;
    public float lifeTime = 2f;
    public float surfaceExitOffset = 0.02f;

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

        // 블록 색상 확인 및 Point Light 색상 변경
        ChangeColorByBlock(collision.gameObject);

        if (collision.collider.TryGetComponent(out ProjectileReflector reflector))
        {
            reflector.HandleReflection(this, hitNormal);
            return;
        }

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
        }
        else if (block.name.Contains("Fire"))
        {
            _pointLight.color = new Color(1f, 0.5f, 0f); // 주황색
            // Fire 블록에 닿으면 0.1초 후 추가 발사체 생성
            SpawnSplitProjectile();
        }
        else if (block.name.Contains("Poison"))
        {
            _pointLight.color = new Color(1.5f, 0f, 1.5f); // 보라색
        }
    }

    void SpawnSplitProjectile()
    {
        // 본체의 속도를 80%로 감소
        speed *= 0.8f;
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
        if (_rigidbody == null)
            return;

        Vector3 velocity = _direction * speed;
        velocity.y = 0f;
        _rigidbody.linearVelocity = velocity;
    }
}

