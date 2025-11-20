using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
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

    Rigidbody _rigidbody;
    Vector3 _moveInput;
    Vector3 _aimDirection = Vector3.forward;
    float _lastShotTime;

    public Vector3 AimDirection => _aimDirection;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

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
        _moveInput = Vector3.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                _moveInput.z -= 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                _moveInput.z += 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                _moveInput.x -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                _moveInput.x += 1f;
        }

        _moveInput = Vector3.ClampMagnitude(_moveInput, 1f);
    }

    void MoveCharacter()
    {
        Vector3 targetVelocity = new Vector3(_moveInput.x, 0f, _moveInput.z) * moveSpeed;
        Vector3 newVelocity = Vector3.Lerp(_rigidbody.linearVelocity, targetVelocity, Time.fixedDeltaTime * damping);
        newVelocity.y = 0f;
        _rigidbody.linearVelocity = newVelocity;
    }

    void AimTowardsPointer()
    {
        if (gameplayCamera == null)
            return;

        if (Mouse.current == null)
            return;

        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = gameplayCamera.ScreenPointToRay(mouseScreenPosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                _aimDirection = direction.normalized;
                float angle = Mathf.Atan2(_aimDirection.x, _aimDirection.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(120, 0f, -angle);
            }
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
        spawnPosition.y = transform.position.y;
        Quaternion spawnRotation = Quaternion.LookRotation(_aimDirection, Vector3.up);
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);

        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(_aimDirection);
        }
    }
}
