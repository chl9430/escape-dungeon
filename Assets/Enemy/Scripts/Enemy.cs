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
            GameManager.instance.AddGameLog(monName + "��(��) óġ�Ͽ����ϴ�.");

            GameManager.instance.MonSpawnInRandomPos();
        }

        if (enemyCurrentHP <= 0)
        {
            // �״� �ִϸ��̼��� ���� �� ���� �����ϱ� ���� �ڷ�ƾ ���
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

            // ���� �����Ÿ� �ȿ� �÷��̾ �ְ�, ���� ���ݱ��� ���ð��� 0�� ���϶��
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

                    // �ڽ��� ������ ����� �ʾҴٸ�
                    if (Vector3.Distance(transform.position, initialPos) <= 5)
                    {
                        // �� ���� ������ ������ ������ �����δ�.
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
        // ��ġ (0, 0, 0)�� �������� �������� dist�� �� ���� ���� ��ġ ����
        Vector3 randDirection = Random.insideUnitSphere * dist;

        // ������ ���� �����ǿ� ������ ���� ��ǥ�� ���Ѵ�.
        randDirection += origin;

        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, NavMesh.AllAreas);
        return navHit.position;
    }

    IEnumerator EnemyDie()
    {
        agent.speed = 0;
        animator.SetTrigger("Dead");
        enemyCollider.enabled = false;

        // �ִϸ��̼� ��� �� ����(��Ȱ��ȭ)�Ѵ�.
        yield return new WaitForSeconds(3f);

        gameObject.SetActive(false);
        InitEnemyHP();
        agent.speed = 1;
        enemyCollider.enabled = true;
        isDead = false;
    }
}
