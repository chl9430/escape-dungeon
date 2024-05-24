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

        // 다음 공격까지 대기시간을 0초로 만든다.
        ResetAttackCoolTime();

        // 플레이어가 인지 범위 내에 있는 지 확인한다.
        RecognizePlayer();

        // 애니메이션을 갱신한다.
        UpdateAnimation();
    }

    // 몬스터의 공격 애니메이션 이벤트에서 호출된다.
    public void Attack()
    {
        // 플레이어가 공격 범위 안에 있다면
        if (canAttack)
        {
            targetObj.GetComponent<PlayerManager>().GetDamaged(10, transform.position);
        }
    }
}
