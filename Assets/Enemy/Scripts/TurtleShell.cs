using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TurtleShell : Enemy
{
    [SerializeField] float recogPlayerRange = 15;
    [SerializeField] float wanderCoolTime = 5;

    GameObject targetPlayerObj;
    NavMeshAgent agent;
    Animator animator;
    CapsuleCollider enemyCollider;

    Vector3 initialPos;

    float currentWanderCooltime;

    void Awake()
    {
        initialPos = transform.position;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<CapsuleCollider>();
        currentWanderCooltime = Random.Range(0f, wanderCoolTime);

        InitEnemyHP();
    }

    void Start()
    {
        if (FindObjectOfType<PlayerManager>().IsAlive)
        {
            targetPlayerObj = FindObjectOfType<PlayerManager>().gameObject;
        }
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        // 다음 공격까지 대기시간을 0초로 만든다.
        if (currentAttackCoolTime != 0f)
        {
            currentAttackCoolTime -= Time.deltaTime;

            if (currentAttackCoolTime <= 0f)
            {
                currentAttackCoolTime = 0f;
            }
        }

        // 플레이어가 인지 범위 내에 있는 지 확인한다.
        if (Vector3.Distance(transform.position, targetPlayerObj.transform.position) <= recogPlayerRange)
        {
            agent.destination = targetPlayerObj.transform.position;
            transform.LookAt(targetPlayerObj.transform.position);

            // 공격 사정거리 안에 플레이어가 있고, 다음 공격까지 대기시간이 0초 이하라면
            if (canAttack && currentAttackCoolTime <= 0f)
            {
                animator.SetBool("isAttacking", true);
                currentAttackCoolTime = attackCoolTime;
            }
            else
            {
                animator.SetBool("isAttacking", false);
            }
        }
        else
        {
            if (currentWanderCooltime != 0f)
            {
                currentWanderCooltime -= Time.deltaTime;

                if (currentWanderCooltime <= 0f)
                {
                    Vector3 newPos = initialPos;

                    // 자신의 영역을 벗어나지 않았다면
                    if (Vector3.Distance(transform.position, initialPos) <= 5)
                    {
                        // 그 영역 내에서 랜덤한 값으로 움직인다.
                        newPos = RandomNavSphere(transform.position, 5);
                    }

                    agent.SetDestination(newPos);

                    currentWanderCooltime = Random.Range(0, wanderCoolTime);
                }
            }
        }

        if (agent.velocity.magnitude == 0f)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }
    }

    public override void GetDamaged(int _damage)
    {
        base.GetDamaged(_damage);

        if (enemyCurrentHP <= 0)
        {
            // 죽는 애니메이션이 끝난 후 적을 삭제하기 위해 코루틴 사용
            StartCoroutine(EnemyDie());
        }
    }

    IEnumerator EnemyDie()
    {
        agent.speed = 0;
        animator.SetTrigger("Dead");
        enemyCollider.enabled = false;

        // 애니메이션 재생 후 삭제(비활성화)한다.
        yield return new WaitForSeconds(3f);

        gameObject.SetActive(false);
        InitEnemyHP();
        agent.speed = 1;
        enemyCollider.enabled = true;
        isDead = false;
    }

    // 몬스터의 공격 애니메이션 이벤트에서 호출된다.
    public void Attack()
    {
        // 플레이어가 공격 범위 안에 있다면
        if (canAttack)
        {
            targetPlayerObj.GetComponent<PlayerManager>().GetDamaged(10, transform.position);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        // 위치 (0, 0, 0)을 기준으로 반지름이 dist인 구 내에 랜덤 위치 생성
        Vector3 randDirection = Random.insideUnitSphere * dist;

        // 몬스터의 원래 포지션에 생성된 랜덤 좌표를 더한다.
        randDirection += origin;

        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, NavMesh.AllAreas);
        return navHit.position;
    }
}
