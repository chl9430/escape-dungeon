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
            // �״� �ִϸ��̼��� ���� �� ���� �����ϱ� ���� �ڷ�ƾ ���
            StartCoroutine(EnemyDie());
            return;
        }

        if (targetPlayer != null)
        {
            float maxDelay = 0.5f;
            targetDelay += Time.deltaTime;

            // �÷��̾�(Ÿ��)�� ��ġ�� �ν��ϱ� ��, ����� �����̸� �ش�.
            if (targetDelay < maxDelay)
            {
                return;
            }

            agent.destination = targetPlayer.transform.position;
            transform.LookAt(targetPlayer.transform.position);

            // bool isRange = Vector3.Distance(transform.position, targetPlayer.transform.position) <= agent.stoppingDistance;

            // �����Ÿ� �ȿ� �÷��̾ �ִٸ�
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
        if (playerManager)
        {
            playerManager.GetDamaged(10, transform.position);
        }
    }
}
