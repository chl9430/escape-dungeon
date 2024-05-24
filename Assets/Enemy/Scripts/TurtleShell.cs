using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TurtleShell : Enemy
{
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<CapsuleCollider>();
        currentWanderCooltime = Random.Range(0f, wanderCoolTime);
        enemyName.text = monName;

        InitEnemyHP();
    }

    void Start()
    {
        initialPos = transform.position;

        if (!GameManager.instance.IsDead)
        {
            targetObj = FindObjectOfType<PlayerManager>().gameObject;
        }
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        // ���� ���ݱ��� ���ð��� 0�ʷ� �����.
        ResetAttackCoolTime();

        // �÷��̾ ���� ���� ���� �ִ� �� Ȯ���Ѵ�.
        RecognizePlayer();

        // �ִϸ��̼��� �����Ѵ�.
        UpdateAnimation();
    }

    // ������ ���� �ִϸ��̼� �̺�Ʈ���� ȣ��ȴ�.
    public void Attack()
    {
        // �÷��̾ ���� ���� �ȿ� �ִٸ�
        if (canAttack)
        {
            targetObj.GetComponent<PlayerManager>().GetDamaged(10, transform.position);
        }
    }
}
