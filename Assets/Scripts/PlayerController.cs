using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float damping = 10f;
    public float fixedHeight = 1f;
    public float rotationOffset = 9f;

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
    bool _isRecoiling = false;
    float _baseRotationY = 0f;

    public Vector3 AimDirection => _aimDirection;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        // Y 위치와 모든 회전 고정
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        if (gameplayCamera == null)
            gameplayCamera = Camera.main;

        if (projectileSpawnPoint == null)
            projectileSpawnPoint = transform;
    }

    void Start()
    {
        
        Vector3 pos = transform.position;
        pos.y = fixedHeight;
        transform.position = pos;
        
        // Rigidbody 위치도 동기화
        if (_rigidbody != null)
        {
            _rigidbody.position = pos;
        }
    }

    void Update()
    {
        // Y 위치 강제 고정 (떠오르는 문제 방지)
        Vector3 pos = transform.position;
        pos.y = fixedHeight;

        transform.position = pos;
        
        ReadInput();
        AimTowardsPointer();
        TryFire();
    }

    void FixedUpdate()
    {
        // FixedUpdate에서도 Y 위치 고정 (물리 시뮬레이션 후 위치 보정)
        Vector3 pos = _rigidbody.position;
        pos.y = fixedHeight;
        _rigidbody.position = pos;
        
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
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                _aimDirection = direction.normalized;
                float angle = Mathf.Atan2(_aimDirection.x, _aimDirection.z) * Mathf.Rad2Deg;
                angle += rotationOffset;
                
                if (_isRecoiling)
                {
                    float currentY = transform.eulerAngles.y;
                    transform.rotation = Quaternion.Euler(0f, currentY, 0f);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }
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
        spawnPosition.y = 1;
        Quaternion spawnRotation = Quaternion.LookRotation(_aimDirection, Vector3.up);
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);

        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(_aimDirection);
        }

        if (!_isRecoiling)
        {
            StartCoroutine(RecoilAnimation());
        }
    }

    System.Collections.IEnumerator RecoilAnimation()
    {
        _isRecoiling = true;
        _baseRotationY = transform.eulerAngles.y;

        float recoilAngle = 20f;
        float duration = 0.1f;
        float returnDuration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentY = Mathf.Lerp(_baseRotationY, _baseRotationY + recoilAngle, t);
            transform.rotation = Quaternion.Euler(0f, currentY, 0f);
            yield return null;
        }

        elapsed = 0f;
        float startY = transform.eulerAngles.y;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;
            float currentY = Mathf.Lerp(startY, _baseRotationY, t);
            transform.rotation = Quaternion.Euler(0f, currentY, 0f);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, _baseRotationY, 0f);
        _isRecoiling = false;
    }
}
