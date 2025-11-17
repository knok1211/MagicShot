using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 5f;

    Rigidbody2D _rigidbody2D;
    Vector2 _direction = Vector2.right;
    float _lifeTimer;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
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
    }

    public void Initialize(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
            _direction = direction.normalized;

        ApplyVelocity();
    }

    public void Reflect(Vector2 normal)
    {
        _direction = Vector2.Reflect(_direction, normal).normalized;
        ApplyVelocity();
    }

    void ApplyVelocity()
    {
        if (_rigidbody2D == null)
            return;

        _rigidbody2D.linearVelocity = _direction * speed;
    }
}

