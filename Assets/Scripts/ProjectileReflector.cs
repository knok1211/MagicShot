using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProjectileReflector : MonoBehaviour
{
    [Tooltip("If enabled, the projectile reflects when hitting this surface.")]
    public bool enableReflection = true;

    [Tooltip("Optional tag for future logic (damage, scoring, etc.).")]
    public string descriptor = "Block";

    public void HandleReflection(Projectile projectile, Vector3 hitNormal)
    {
        if (!enableReflection || projectile == null)
            return;

        projectile.Reflect(hitNormal);
    }
}

