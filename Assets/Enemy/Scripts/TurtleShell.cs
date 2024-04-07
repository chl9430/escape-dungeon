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

        // ���� ���ݱ��� ���ð��� 0�ʷ� �����.
        if (currentAttackCoolTime != 0f)
        {
            currentAttackCoolTime -= Time.deltaTime;

            if (currentAttackCoolTime <= 0f)
            {
                currentAttackCoolTime = 0f;
            }
        }

        // �÷��̾ ���� ���� ���� �ִ� �� Ȯ���Ѵ�.
        if (Vector3.Distance(transform.position, targetPlayerObj.transform.position) <= recogPlayerRange)
        {
            agent.destination = targetPlayerObj.transform.position;
            transform.LookAt(targetPlayerObj.transform.position);

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
            if (currentWanderCooltime != 0f)
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
            // �״� �ִϸ��̼��� ���� �� ���� �����ϱ� ���� �ڷ�ƾ ���
            StartCoroutine(EnemyDie());
        }
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

    // ������ ���� �ִϸ��̼� �̺�Ʈ���� ȣ��ȴ�.
    public void Attack()
    {
        // �÷��̾ ���� ���� �ȿ� �ִٸ�
        if (canAttack)
        {
            targetPlayerObj.GetComponent<PlayerManager>().GetDamaged(10, transform.position);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        // ��ġ (0, 0, 0)�� �������� �������� dist�� �� ���� ���� ��ġ ����
        Vector3 randDirection = Random.insideUnitSphere * dist;

        // ������ ���� �����ǿ� ������ ���� ��ǥ�� ���Ѵ�.
        randDirection += origin;

        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, NavMesh.AllAreas);
        return navHit.position;
    }
}
