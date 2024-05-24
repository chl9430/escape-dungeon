using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Beholder : Enemy
{
    [SerializeField] Transform attackPoint;

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
        GameObject razorFXObj = PoolManager.instance.ActiveObj(3, attackPoint.position);
        razorFXObj.transform.rotation = Quaternion.LookRotation(transform.forward);
    }
}
