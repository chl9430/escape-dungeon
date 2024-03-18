using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TurtleShell : Enemy
{
    [SerializeField] float recogPlayerRange = 15;

    GameObject targetPlayerObj;
    NavMeshAgent agent;
    Animator animator;
    QuestManager questManager;
    CapsuleCollider enemyCollider;

    Vector3 initialPos;

    bool isDead = false;

    void Awake()
    {
        initialPos = transform.position;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<CapsuleCollider>();

        InitEnemyHP();
    }

    void Start()
    {
        if (FindObjectOfType<PlayerManager>().IsAlive)
        {
            targetPlayerObj = FindObjectOfType<PlayerManager>().gameObject;
        }

        questManager = FindObjectOfType<QuestManager>();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (enemyCurrentHP <= 0)
        {
            isDead = true;
            questManager.CheckDeadMonName(monName);
            // �״� �ִϸ��̼��� ���� �� ���� �����ϱ� ���� �ڷ�ƾ ���
            StartCoroutine(EnemyDie());
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
            animator.SetBool("isWalking", true);

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
            agent.destination = initialPos;

            if (agent.velocity.magnitude == 0f)
            {
                animator.SetBool("isWalking", false);
            }
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
}
