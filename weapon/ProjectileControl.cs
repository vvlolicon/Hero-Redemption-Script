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

    // �����ٶ�ʸ������
    Vector3 _velocity;
    Vector3 _initialPosition;

    Vector3 target;
    void FixedUpdate()
    {
        if (_projectileStats == null) return;
        // �����ƶ����򣨱���XZƽ�泯��Ŀ�꣩
        Vector3 horizontalDirection = new Vector3(
            target.x - _initialPosition.x,
            0,
            target.z - _initialPosition.z).normalized;

        // Ӧ���������ٶ�ʸ��
        _velocity.y -= gravity * Time.fixedDeltaTime;
        // ����ˮƽ�ٶȺ㶨
        _velocity.x = horizontalDirection.x * _projectileStats.speed;
        _velocity.z = horizontalDirection.z * _projectileStats.speed;

        RaycastHit hit;
        if (gravity > 0)
        {
            // ʹ�� SphereCast ����볡���ϰ������ײ
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
            // ʹ�� Linecast ����볡���ϰ������ײ
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

        // ִ��λ��
        transform.Translate(_velocity * Time.fixedDeltaTime, Space.World);

        // ���·��о��루ʹ��ʵ���ƶ����룩
        distanceTraveled += _velocity.magnitude * Time.fixedDeltaTime;

        // Ӧ������
        if (gravity > 0)
        {
            transform.Translate(gravity * Time.fixedDeltaTime * Vector3.down, Space.World);
        }

        // ���Ͷ����ɳ���ָ�������Ҳ��ᱬը����������
        if (distanceTraveled >= _projectileStats.flyingRange)
        {
            Destroy(gameObject);
        }

        bool CheckProjectileHit(Collider collider)
        {
            if (collider == null) return false;

            // �ų������ߺ���Ч��ײ����
            if (collider.gameObject == Sender.gameObject) return false;
            if (collider.CompareTag("Weapon")) return false;

            return true;
        }

        void OnProjectileCollides()
        {
            // �����⵽��ײ�� DestroyOnTouch Ϊ true��������Ͷ�����ը
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
        // ��ȡ�����ڱ�ը��Χ�ڵ� Collider
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _projectileStats.AOERadius);

        foreach (var hitCollider in hitColliders)
        {
            // ȷ������Ͷ����ķ�����
            if (hitCollider.gameObject == Sender.gameObject) continue;

            // ��ÿ���ڷ�Χ�ڵĶ������ Damage ����
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
        // ����Ŀ������
        Vector3 toTarget = target - _initialPosition;

        float speed = _projectileStats.speed;
        if (_projectileStats.useGravity) gravity = 10f;

        if (gravity > 0)
        {
            float horizontalDistance = new Vector3(toTarget.x, 0, toTarget.z).magnitude;
            float verticalDistance = toTarget.y;

            // ��������Ƕ�
            float angle = CalculateLaunchAngle(horizontalDistance, verticalDistance, speed, gravity);
            if (float.IsNaN(angle) || float.IsInfinity(angle))
            {
                Debug.LogError("�޷�������Ч�ķ���Ƕ�");
                Destroy(gameObject);
                return;
            }

            // �����ʼ�ٶ�
            _velocity = new Vector3(toTarget.x, 0, toTarget.z).normalized * speed;
            _velocity.y = speed * Mathf.Sin(angle * Mathf.Deg2Rad);
        }
        else
        {
            // ���û��������ֱ�ӳ���Ŀ���
            Vector3 direction = toTarget.normalized;
            _velocity = direction * speed;
        }
    }

    float CalculateLaunchAngle(float horizontalDistance, float verticalDistance, float speed, float gravity)
    {
        // ���Ŀ�����ǳ�����ֱ�ӷ���һ����С�ĽǶ��Ա����������
        if (horizontalDistance < 5f) // ��Ӿ�����ֵ
        {
            // С����ʱ��ʹ�ý�С������Ƕ�
            return 30f;
        }
        if (horizontalDistance < 10f) // ��Ӿ�����ֵ
        {
            // С����ʱ��ʹ�ý�С������Ƕ�
            return 45f;
        }

        // ʹ�ö��ι�ʽ�������˶�����
        float a = -0.5f * gravity / (speed * speed);
        float b = verticalDistance / horizontalDistance;
        float c = 0.5f;

        // ���η��̵�ϵ��
        float discriminant = b * b - 4 * a * c;

        // ����б�ʽ�Ƿ�Ǹ�
        if (discriminant < 0)
        {
            Debug.LogError("û����Ч�ķ���Ƕ�");
            return float.NaN;
        }

        // �����������ܵĽǶ�
        float angle1 = Mathf.Atan((-b + Mathf.Sqrt(discriminant)) / (2 * a)) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan((-b - Mathf.Sqrt(discriminant)) / (2 * a)) * Mathf.Rad2Deg;

        // ѡ��һ����Ч�ĽǶ�
        // ����ѡ���С�ĽǶȣ��Լ��ٴ�ֱ�ٶȷ���
        if (angle2 >= 0 && angle2 <= 90)
        {
            return angle2; // ѡ���С�ĽǶ�
        }
        else if (angle1 >= 0 && angle1 <= 90)
        {
            return angle1;
        }
        else
        {
            Debug.LogError("û����Ч�ķ���Ƕ�");
            return float.NaN;
        }
    }



}
