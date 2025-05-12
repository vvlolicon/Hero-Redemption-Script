using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ProjectileControl : MonoBehaviour
{
    Component Sender;
    public ProjectileStats _projectileStats;
    List<GameObject> damagedEnemy = new List<GameObject>();
    public Color _dmgColor = Color.cyan;
    float gravity = 0f;
    public float distanceTraveled = 0;
    Vector3 _velocity;
    Vector3 _initialPosition;

    Vector3 target;
    void FixedUpdate()
    {
        if (_projectileStats == null) return;
        // calculate movement (keep movement on XZ plane)
        Vector3 horizontalDirection = new Vector3(
            target.x - _initialPosition.x,
            0,
            target.z - _initialPosition.z).normalized;

        // keep horizontal speed
        _velocity.x = horizontalDirection.x * _projectileStats.speed;
        _velocity.z = horizontalDirection.z * _projectileStats.speed;

        RaycastHit hit;
        if (gravity > 0)
        {
            // use SphereCast to detect collision on obstacles
            Vector3 predictedPosition = transform.position + _velocity * Time.fixedDeltaTime;
            if (Physics.SphereCast(transform.position, 3f, predictedPosition - transform.position, out hit,
                _projectileStats.speed * Time.fixedDeltaTime * 10f, ~LayerMask.GetMask("Character")))
            {
                //Debug.Log("Hit something");
                if (CheckProjectileHit(hit.collider))
                {
                    OnProjectileCollides();
                    return;
                }
            }
        }
        else
        {
            // use Linecast to detect collision on obstacles
            Vector3 targetPosition = transform.position + _velocity * Time.fixedDeltaTime * 5f;
            if (Physics.Linecast(transform.position, targetPosition, out hit,
                ~LayerMask.GetMask("Character")))
            {
                //Debug.Log("Hit something");
                if (CheckProjectileHit(hit.collider))
                {
                    OnProjectileCollides();
                    return;
                }
            }
        }
        _velocity.y -= gravity * Time.fixedDeltaTime;
        transform.Translate(_velocity * Time.fixedDeltaTime, Space.World);

        // update flight distance
        distanceTraveled += _velocity.magnitude * Time.fixedDeltaTime;

        // apply gravity
        if (gravity > 0)
        {
            transform.Translate(gravity * Time.fixedDeltaTime * Vector3.down, Space.World);
        }

        // if the projectile is out of range, destroy it
        if (distanceTraveled >= _projectileStats.flyingRange)
        {
            Destroy(gameObject);
        }

        bool CheckProjectileHit(Collider collider)
        {
            if (collider == null) return false;
            // ignore self and weapon
            if (collider.gameObject == Sender.gameObject) return false;
            if (collider.CompareTag("Weapon")) return false;
            return true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Sender.gameObject) return;
        if (other.CompareTag("Weapon")) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            Damage(other.gameObject);
            OnProjectileCollides();
        }
    }

    void OnProjectileCollides()
    {
        if (_projectileStats.DestroyOnTouch)
        {
            if (_projectileStats.IsAOE)
            {
                Explode();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    readonly Collider[] hitColliders = new Collider[10]; 
    void Explode()
    {
        Physics.OverlapSphereNonAlloc(transform.position, _projectileStats.AOERadius, hitColliders);
        // Get all Collider in range
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider == null) continue;
            if (hitCollider.gameObject == Sender.gameObject) continue;

            Damage(hitCollider.gameObject);
        }
        _isExploding = true;
        StartCoroutine(ExtendIEnumerator.DelayAction(3f, () => { 
            _isExploding = false; 
            Destroy(gameObject);
        }));
    }

    bool _isExploding = false;
    // visualize explosion range
    void OnDrawGizmos()
    {
        if (_isExploding)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _projectileStats.AOERadius); 
        }
    }

    void Damage(GameObject victim)
    {
        if (victim.layer != LayerMask.NameToLayer("Character")) return;
        if (Sender is PlayerStateExecutor player)
        {
            if (victim.CompareTag("Enemy"))
            {
                if (!damagedEnemy.Contains(victim))
                {
                    damagedEnemy.Add(victim);
                    player.DamageEnemy(victim, _dmgColor);
                }
            }
        }
        if(Sender is EnemyStateExecutor enemy)
        {
            if (victim.CompareTag("Player"))
            {
                if (!damagedEnemy.Contains(victim))
                {
                    damagedEnemy.Add(victim);
                    enemy.DamagePlayer(victim.transform.position - transform.position);
                }
            }
        }
    }

    public void SetStats(EnemyStateExecutor executor, ProjectileStats stats, Vector3 target)
    {
        Sender = executor;
        _projectileStats = stats;
        this.target = target;
        InitializeVelocity();
    }
    public void SetStats(PlayerStateExecutor executor, ProjectileStats stats, Vector3 target)
    {
        Sender = executor;
        _projectileStats = stats;
        this.target = target;
        InitializeVelocity();
    }

    void InitializeVelocity()
    {
        _initialPosition = transform.position;
        Vector3 toTarget = target - _initialPosition;

        float speed = _projectileStats.speed;
        if (_projectileStats.useGravity) gravity = 10f;

        if (gravity > 0)
        {
            float horizontalDistance = new Vector3(toTarget.x, 0, toTarget.z).magnitude;
            float verticalDistance = toTarget.y;

            float angle = CalculateLaunchAngle(horizontalDistance, verticalDistance, speed, gravity);
            if (float.IsNaN(angle) || float.IsInfinity(angle))
            {
                Debug.LogError("no valid fire angle");
                Destroy(gameObject);
                return;
            }
            _velocity = new Vector3(toTarget.x, 0, toTarget.z).normalized * speed;
            _velocity.y = speed * Mathf.Sin(angle * Mathf.Deg2Rad);
        }
        else
        {
            Vector3 direction = toTarget.normalized;
            _velocity = direction * speed;
        }
    }

    float CalculateLaunchAngle(float horizontalDistance, float verticalDistance, float speed, float gravity)
    {
        if (horizontalDistance < 5f) // distance thresholds when distance is too small
        {// return smaller angle if distance is too small
            return 30f;
        }
        if (horizontalDistance < 10f) 
        {
            return 45f;
        }

        // bunch of complicated Quadratic equation here
        float a = -0.5f * gravity / (speed * speed);
        float b = verticalDistance / horizontalDistance;
        float c = 0.5f;
        float discriminant = b * b - 4 * a * c;

        // Check whether the discriminant is non-negative.
        if (discriminant < 0)
        {
            Debug.LogError("no valid fire angle");
            return float.NaN;
        }

        // Calculate the two possible angles.
        float angle1 = Mathf.Atan((-b + Mathf.Sqrt(discriminant)) / (2 * a)) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan((-b - Mathf.Sqrt(discriminant)) / (2 * a)) * Mathf.Rad2Deg;

        // choose valid angles, smaller angle is optimized
        if (angle2 >= 0 && angle2 <= 90)
        {
            return angle2; 
        }
        else if (angle1 >= 0 && angle1 <= 90)
        {
            return angle1;
        }
        else
        {
            Debug.LogError("no valid fire angle");
            return float.NaN;
        }
    }



}
