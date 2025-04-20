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

    // 新增速度矢量变量
    Vector3 _velocity;
    Vector3 _initialPosition;

    Vector3 target;
    void FixedUpdate()
    {
        if (_projectileStats == null) return;
        // 计算移动方向（保持XZ平面朝向目标）
        Vector3 horizontalDirection = new Vector3(
            target.x - _initialPosition.x,
            0,
            target.z - _initialPosition.z).normalized;

        // 应用重力到速度矢量
        _velocity.y -= gravity * Time.fixedDeltaTime;
        // 保持水平速度恒定
        _velocity.x = horizontalDirection.x * _projectileStats.speed;
        _velocity.z = horizontalDirection.z * _projectileStats.speed;

        RaycastHit hit;
        if (gravity > 0)
        {
            // 使用 SphereCast 检测与场景障碍物的碰撞
            Vector3 predictedPosition = transform.position + _velocity * Time.fixedDeltaTime;
            if (Physics.SphereCast(transform.position, 2f, predictedPosition - transform.position, out hit,
                _projectileStats.speed * Time.fixedDeltaTime * 15f, ~LayerMask.GetMask("Character")))
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
            // 使用 Linecast 检测与场景障碍物的碰撞
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

        // 执行位移
        transform.Translate(_velocity * Time.fixedDeltaTime, Space.World);

        // 更新飞行距离（使用实际移动距离）
        distanceTraveled += _velocity.magnitude * Time.fixedDeltaTime;

        // 应用重力
        if (gravity > 0)
        {
            transform.Translate(gravity * Time.fixedDeltaTime * Vector3.down, Space.World);
        }

        // 如果投射物飞出了指定距离且不会爆炸，则销毁它
        if (distanceTraveled >= _projectileStats.flyingRange)
        {
            Destroy(gameObject);
        }

        bool CheckProjectileHit(Collider collider)
        {
            if (collider == null) return false;

            // 排除发射者和无效碰撞对象
            if (collider.gameObject == Sender.gameObject) return false;
            if (collider.CompareTag("Weapon")) return false;

            return true;
        }

        void OnProjectileCollides()
        {
            // 如果检测到碰撞且 DestroyOnTouch 为 true，则销毁投射物或爆炸
            if (_projectileStats.DestroyOnTouch)
            {
                if (_projectileStats.IsAOE)
                {
                    Explode();
                }
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Sender.gameObject) return;
        if(other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            Damage(other.gameObject);
            if (_projectileStats.DestroyOnTouch)
            {
                if (_projectileStats.IsAOE)
                {
                    Explode();
                }
                Destroy(gameObject);
            }
        }
    }

    void Explode()
    {
        // 获取所有在爆炸范围内的 Collider
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _projectileStats.AOERadius);

        foreach (var hitCollider in hitColliders)
        {
            // 确保不是投射物的发射者
            if (hitCollider.gameObject == Sender.gameObject) continue;

            // 对每个在范围内的对象调用 Damage 方法
            Damage(hitCollider.gameObject);
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
                    GeneralStatsObj _playerStats = player.PlayerStats;
                    damagedEnemy.Add(victim);
                    EnemyDmgInfo dmgInfo = new EnemyDmgInfo(
                        _playerStats.ATK, 
                        _playerStats.CritChance, 
                        _playerStats.CritDmgMult, 
                        _dmgColor, transform, victim);
                    dmgInfo.CallDamageable(victim);
                }
            }
        }
        if(Sender is EnemyStateExecutor enemy)
        {
            if (victim.CompareTag("Player"))
            {
                PlayerDmgInfo dmgInfo = new PlayerDmgInfo(
                    enemy.ATK, victim.transform.position - transform.position, 250f);
                dmgInfo.CallDamageable(victim);
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
        // 计算目标向量
        Vector3 toTarget = target - _initialPosition;

        float speed = _projectileStats.speed;
        if (_projectileStats.useGravity) gravity = 10f;

        if (gravity > 0)
        {
            float horizontalDistance = new Vector3(toTarget.x, 0, toTarget.z).magnitude;
            float verticalDistance = toTarget.y;

            // 计算抛射角度
            float angle = CalculateLaunchAngle(horizontalDistance, verticalDistance, speed, gravity);
            if (float.IsNaN(angle) || float.IsInfinity(angle))
            {
                Debug.LogError("无法计算有效的发射角度");
                Destroy(gameObject);
                return;
            }

            // 计算初始速度
            _velocity = new Vector3(toTarget.x, 0, toTarget.z).normalized * speed;
            _velocity.y = speed * Mathf.Sin(angle * Mathf.Deg2Rad);
        }
        else
        {
            // 如果没有重力，直接朝向目标点
            Vector3 direction = toTarget.normalized;
            _velocity = direction * speed;
        }
    }

    float CalculateLaunchAngle(float horizontalDistance, float verticalDistance, float speed, float gravity)
    {
        // 如果目标距离非常近，直接返回一个较小的角度以避免过高抛射
        if (horizontalDistance < 5f) // 添加距离阈值
        {
            // 小距离时，使用较小的抛射角度
            return 30f;
        }
        if (horizontalDistance < 10f) // 添加距离阈值
        {
            // 小距离时，使用较小的抛射角度
            return 45f;
        }

        // 使用二次公式解抛体运动方程
        float a = -0.5f * gravity / (speed * speed);
        float b = verticalDistance / horizontalDistance;
        float c = 0.5f;

        // 二次方程的系数
        float discriminant = b * b - 4 * a * c;

        // 检查判别式是否非负
        if (discriminant < 0)
        {
            Debug.LogError("没有有效的发射角度");
            return float.NaN;
        }

        // 计算两个可能的角度
        float angle1 = Mathf.Atan((-b + Mathf.Sqrt(discriminant)) / (2 * a)) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan((-b - Mathf.Sqrt(discriminant)) / (2 * a)) * Mathf.Rad2Deg;

        // 选择一个有效的角度
        // 优先选择较小的角度，以减少垂直速度分量
        if (angle2 >= 0 && angle2 <= 90)
        {
            return angle2; // 选择较小的角度
        }
        else if (angle1 >= 0 && angle1 <= 90)
        {
            return angle1;
        }
        else
        {
            Debug.LogError("没有有效的发射角度");
            return float.NaN;
        }
    }



}
