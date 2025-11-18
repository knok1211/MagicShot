using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float damping = 10f;

    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float fireCooldown = 0.2f;

    [Header("References")]
    public Camera gameplayCamera;

    Rigidbody2D _rigidbody2D;
    Vector2 _moveInput;
    Vector2 _aimDirection = Vector2.right;
    float _lastShotTime;

    public Vector2 AimDirection => _aimDirection;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        if (gameplayCamera == null)
            gameplayCamera = Camera.main;

        if (projectileSpawnPoint == null)
            projectileSpawnPoint = transform;
    }

    void Update()
    {
        ReadInput();
        AimTowardsPointer();
        TryFire();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    void ReadInput()
    {
        _moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                _moveInput.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                _moveInput.y -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                _moveInput.x += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                _moveInput.x -= 1f;
        }

        _moveInput = Vector2.ClampMagnitude(_moveInput, 1f);
    }

    void MoveCharacter()
    {
        Vector2 targetVelocity = _moveInput * moveSpeed;
        _rigidbody2D.linearVelocity = Vector2.Lerp(_rigidbody2D.linearVelocity, targetVelocity, Time.fixedDeltaTime * damping);
    }

    void AimTowardsPointer()
    {
        if (gameplayCamera == null)
            return;

        if (Mouse.current == null)
            return;

        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 world = gameplayCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 direction = (world - transform.position);
        direction.Normalize();

        if (direction.sqrMagnitude > 0.001f)
        {
            _aimDirection = direction;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    void TryFire()
    {
        if (projectilePrefab == null)
            return;

        bool isFiring = false;

        if (Mouse.current != null)
            isFiring |= Mouse.current.leftButton.isPressed;
        if (Gamepad.current != null)
            isFiring |= Gamepad.current.rightTrigger.isPressed || Gamepad.current.leftTrigger.isPressed;

        if (!isFiring)
            return;

        if (Time.time - _lastShotTime < fireCooldown)
            return;

        _lastShotTime = Time.time;

        Vector3 spawnPosition = projectileSpawnPoint.position;
        Quaternion spawnRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg - 90f);
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);

        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(_aimDirection);
        }
    }
}
