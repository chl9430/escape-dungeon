using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Slider HPBar;

    [SerializeField] string monName = "";
    [SerializeField] float enemyMaxHP = 5;

    [SerializeField] float recogPlayerRange = 5;
    [SerializeField] float moveRange = 10;

    QuestManager questManager;
    GameObject targetPlayerObj;

    NavMeshAgent agent;
    Animator animator;
    CapsuleCollider enemyCollider;
    Vector3 initialPos;
    float targetDelay = 0.5f;
    float enemyCurrentHP = 0;

    bool isDead = false;
    bool canAttack = false;
    public bool CanAttack { set { canAttack = value; } }

    public float EnemyCurrentHP { get { return enemyCurrentHP; } }

    void Start()
    {
        questManager = FindObjectOfType<QuestManager>();

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<CapsuleCollider>();

        initialPos = transform.position;

        if (FindObjectOfType<PlayerManager>().IsAlive)
        {
            targetPlayerObj = FindObjectOfType<PlayerManager>().gameObject;
        }

        InitEnemyHP();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        HPBar.value = enemyCurrentHP / enemyMaxHP;

        if (enemyCurrentHP <= 0)
        {
            isDead = true;
            questManager.CheckDeadMonName(monName);
            // �״� �ִϸ��̼��� ���� �� ���� �����ϱ� ���� �ڷ�ƾ ���
            StartCoroutine(EnemyDie());
            return;
        }

        // ���� ���� ���� ���� �ִٸ�
        if (Vector3.Distance(initialPos, transform.position) <= moveRange)
        {
            // �÷��̾ ���� ���� ���� �ִ� �� Ȯ���Ѵ�.
            if (Vector3.Distance(transform.position, targetPlayerObj.transform.position) <= recogPlayerRange)
            {
                float maxDelay = 0.5f;
                targetDelay += Time.deltaTime;

                // �÷��̾�(Ÿ��)�� ��ġ�� �ν��ϱ� ��, ����� �����̸� �ش�.
                if (targetDelay < maxDelay)
                {
                    return;
                }

                agent.destination = targetPlayerObj.transform.position;
                transform.LookAt(targetPlayerObj.transform.position);

                // ���� �����Ÿ� �ȿ� �÷��̾ �ִٸ�
                if (canAttack)
                {
                    animator.SetTrigger("Attack");
                }
                else
                {
                    animator.SetFloat("MoveSpeed", agent.velocity.magnitude);
                }

                targetDelay = 0f;
            }
            else
            {
                agent.destination = initialPos;
            }
        }
        else
        {
            agent.destination = initialPos;
        }
    }

    void InitEnemyHP()
    {
        enemyCurrentHP = enemyMaxHP;
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

    public void GetDamaged(int damage)
    {
        enemyCurrentHP -= damage;
    }

    // ������ ���� �ִϸ��̼� �̺�Ʈ���� ȣ��ȴ�.
    public void Attack()
    {
        // �÷��̾ ���� �ȿ� �ִٸ�
        if (canAttack)
        {
            targetPlayerObj.GetComponent<PlayerManager>().GetDamaged(30, transform.position);
        }
    }
}
