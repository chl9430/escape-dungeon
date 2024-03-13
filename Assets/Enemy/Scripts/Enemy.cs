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
            // 죽는 애니메이션이 끝난 후 적을 삭제하기 위해 코루틴 사용
            StartCoroutine(EnemyDie());
            return;
        }

        // 적이 본인 영역 내에 있다면
        if (Vector3.Distance(initialPos, transform.position) <= moveRange)
        {
            // 플레이어가 인지 범위 내에 있는 지 확인한다.
            if (Vector3.Distance(transform.position, targetPlayerObj.transform.position) <= recogPlayerRange)
            {
                float maxDelay = 0.5f;
                targetDelay += Time.deltaTime;

                // 플레이어(타겟)의 위치를 인식하기 전, 잠깐의 딜레이를 준다.
                if (targetDelay < maxDelay)
                {
                    return;
                }

                agent.destination = targetPlayerObj.transform.position;
                transform.LookAt(targetPlayerObj.transform.position);

                // 공격 사정거리 안에 플레이어가 있다면
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

        // 애니메이션 재생 후 삭제(비활성화)한다.
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

    // 몬스터의 공격 애니메이션 이벤트에서 호출된다.
    public void Attack()
    {
        // 플레이어가 범위 안에 있다면
        if (canAttack)
        {
            targetPlayerObj.GetComponent<PlayerManager>().GetDamaged(30, transform.position);
        }
    }
}
