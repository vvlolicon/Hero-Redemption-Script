using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectiles", menuName = "Enemy/projectiles")]
public class ProjectileStats : ScriptableObject
{
    [Tooltip("The projectile prefab to be fired")]
    public GameObject ProjectilePrefab;
    //public float Damage;
    [Tooltip("If true, the projectile will damage enemy in range")]
    public bool IsAOE;
    [Tooltip("The radius of the AOE damage")]
    public float AOERadius;
    [Tooltip("If true, the projectile will be destoryed on impact")]
    public bool DestroyOnTouch;
    [Tooltip("If projectile will not explode on touch, the maximum range it will fly")]
    public float flyingRange;
    [Tooltip("gravity of the projectile, 0 means no gravity")]
    public bool useGravity = false;
    [Tooltip("speed of the projectile")]
    public float speed;

    private void OnEnable()
    {
        if (!DestroyOnTouch)
        {
            IsAOE = false;
        }
        if (!IsAOE)
        {
            AOERadius = 0;
        }
    }

}
