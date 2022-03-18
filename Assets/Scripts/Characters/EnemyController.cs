using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStatus { GUARD, PATROL, CHASE, DEAD }

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private EnemyStatus enemyStatus;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;

    private CharacterStats characterStats;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    private GameObject attackTarget;
    private Vector3 initPos;
    private Quaternion initRotation;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;

    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;

    // 配合动画
    private bool isWalk;
    private bool isChase;
    private bool isFollow;
    private bool isDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        initPos = transform.position;
        initRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }

    void Start()
    {
        if (isGuard)
        {
            enemyStatus = EnemyStatus.GUARD;
        }
        else
        {
            enemyStatus = EnemyStatus.PATROL;
            GetNewWayPoint();
        }
    }

    void Update()
    {
        if (characterStats.CurrentHealth == 0)
        {
            isDead = true;
        }
        SwitchStatus();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    private void SwitchStatus()
    {
        if (isDead)
        {
            enemyStatus = EnemyStatus.DEAD;
        }
        else if (FoundPlayer())
        {
            enemyStatus = EnemyStatus.CHASE;
        }
        switch (enemyStatus)
        {
            case EnemyStatus.GUARD:
                isChase = false;
                if (transform.position != initPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = initPos;
                    if (Vector3.SqrMagnitude(initPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, initRotation, 0.01f);
                    }
                }
                break;
            case EnemyStatus.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;

                // 判断是否到了随机巡逻点
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                    }
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStatus.CHASE:
                isWalk = false;
                isChase = true;

                agent.speed = speed;
                if (!FoundPlayer())
                {
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        if (isGuard)
                        {
                            enemyStatus = EnemyStatus.GUARD;
                        }
                        else
                        {
                            enemyStatus = EnemyStatus.PATROL;
                        }
                    }
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                    // 在攻击范围内则攻击
                    if (TargetInSkillRange() || TargetInAttackRange())
                    {
                        isFollow = false;
                        agent.isStopped = true;
                        if (lastAttackTime < 0)
                        {
                            lastAttackTime = characterStats.attackData.coolDown;

                            // 暴击判断
                            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                            // 执行攻击
                            Attack();
                        }
                    }
                }
                break;
            case EnemyStatus.DEAD:
                coll.enabled = false;
                agent.enabled = false;
                Destroy(gameObject, 2f);
                break;
        }
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.attackRange;
        }
        else
        {
            return false;
        }
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.skillRange;
        }
        else
        {
            return false;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            // 近身攻击动画
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            // 技能攻击动画
            anim.SetTrigger("Skill");
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(initPos.x + randomX, transform.position.y, initPos.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    void Hit()
    {
        if (attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}