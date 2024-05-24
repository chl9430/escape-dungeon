using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Slider HPBar;
    [SerializeField] protected Text enemyName;

    [SerializeField] protected string monName = "";
    [SerializeField] protected float enemyMaxHP = 5;
    [SerializeField] protected float recogPlayerRange = 15;
    [SerializeField] protected float wanderCoolTime = 5;
    [SerializeField] protected float attackCoolTime = 5f;

    protected NavMeshAgent agent;
    protected Animator animator;
    protected GameObject targetObj;
    protected CapsuleCollider enemyCollider;
    protected Vector3 initialPos;
    protected bool isDead = false;
    protected float enemyCurrentHP = 0;
    protected float currentAttackCoolTime;
    protected bool canAttack = false;
    protected float currentWanderCooltime;

    public float EnemyCurrentHP { get { return enemyCurrentHP; } }

    public bool CanAttack { set { canAttack = value; } get { return canAttack; } }

    public void InitEnemyHP()
    {
        enemyCurrentHP = enemyMaxHP;

        RefreshHPBar();
    }

    public virtual void GetDamaged(int _damage)
    {
        enemyCurrentHP -= _damage;

        RefreshHPBar();

        if (enemyCurrentHP <= 0)
        {
            isDead = true;
            QuestManager.instance.CheckDeadMonName(monName);
            GameManager.instance.AddGameLog(monName + "를(을) 처치하였습니다.");

            GameManager.instance.MonSpawnInRandomPos();
        }

        if (enemyCurrentHP <= 0)
        {
            // 죽는 애니메이션이 끝난 후 적을 삭제하기 위해 코루틴 사용
            StartCoroutine(EnemyDie());
        }
    }

    public void RefreshHPBar()
    {
        HPBar.value = enemyCurrentHP / enemyMaxHP;
    }

    public void ResetAttackCoolTime()
    {
        if (currentAttackCoolTime != 0f)
        {
            currentAttackCoolTime -= Time.deltaTime;

            if (currentAttackCoolTime <= 0f)
            {
                currentAttackCoolTime = 0f;
            }
        }
    }

    public void RecognizePlayer()
    {
        if (Vector3.Distance(transform.position, targetObj.transform.position) <= recogPlayerRange)
        {
            agent.destination = targetObj.transform.position;
            transform.LookAt(targetObj.transform.position);

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
            if (currentWanderCooltime >= 0f)
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

                    currentWanderCooltime = Random.Range(0f, wanderCoolTime);
                }
            }
        }
    }

    public void UpdateAnimation()
    {
        if (agent.velocity.magnitude != 0f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        // 위치 (0, 0, 0)을 기준으로 반지름이 dist인 구 내에 랜덤 위치 생성
        Vector3 randDirection = Random.insideUnitSphere * dist;

        // 몬스터의 원래 포지션에 생성된 랜덤 좌표를 더한다.
        randDirection += origin;

        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, NavMesh.AllAreas);
        return navHit.position;
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
}
