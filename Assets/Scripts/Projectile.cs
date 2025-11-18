using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 5f;

    Rigidbody _rigidbody;
    Vector3 _direction = Vector3.forward;
    float _lifeTimer;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody != null)
        {
            _rigidbody.useGravity = false;
        }
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

    public void Initialize(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
            _direction = direction.normalized;

        ApplyVelocity();
    }

    public void Reflect(Vector3 normal)
    {
        _direction = Vector3.Reflect(_direction, normal).normalized;
        ApplyVelocity();
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

