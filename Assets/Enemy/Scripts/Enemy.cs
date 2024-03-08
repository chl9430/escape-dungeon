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

    QuestManager questManager;
    GameObject targetPlayer;

    NavMeshAgent agent;
    Animator animator;
    CapsuleCollider enemyCollider;

    PlayerManager playerManager;
    float enemyCurrentHP = 0;
    float targetDelay = 0.5f;
    bool isDead = false;

    public PlayerManager PlayerManager { set { playerManager = value; } }

    public float EnemyCurrentHP { get { return enemyCurrentHP; } }

    // Start is called before the first frame update
    void Start()
    {
        questManager = FindObjectOfType<QuestManager>();

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<CapsuleCollider>();

        targetPlayer = FindObjectOfType<PlayerManager>().gameObject;

        InitEnemyHP();
    }

    // Update is called once per frame
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

        if (targetPlayer != null)
        {
            float maxDelay = 0.5f;
            targetDelay += Time.deltaTime;

            // 플레이어(타겟)의 위치를 인식하기 전, 잠깐의 딜레이를 준다.
            if (targetDelay < maxDelay)
            {
                return;
            }

            agent.destination = targetPlayer.transform.position;
            transform.LookAt(targetPlayer.transform.position);

            // bool isRange = Vector3.Distance(transform.position, targetPlayer.transform.position) <= agent.stoppingDistance;

            // 사정거리 안에 플레이어가 있다면
            if (playerManager)
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                animator.SetFloat("MoveSpeed", agent.velocity.magnitude);
            }

            targetDelay = 0f;
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
        if (playerManager)
        {
            playerManager.GetDamaged(10, transform.position);
        }
    }
}
