using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 12f;
    public float lifeTime = 5f;
    public float surfaceExitOffset = 0.02f;

    Rigidbody _rigidbody;
    Vector3 _direction = Vector3.forward;
    float _lifeTimer;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody != null)
        {
            _rigidbody.useGravity = false;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
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
            reflector.HandleReflection(this, hitNormal);
            return;
        }

        Destroy(gameObject);
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

