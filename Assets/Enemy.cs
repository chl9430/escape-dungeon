using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Slider HPBar;

    float enemyMaxHP = 10;
    public float enemyCurrentHP = 0;

    NavMeshAgent agent;
    Animator animator;

    GameObject targetPlayer;
    float targetDelay = 0.5f;

    CapsuleCollider enemyCollider;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<CapsuleCollider>();

        targetPlayer = GameObject.FindWithTag("Player");

        InitEnemyHP();
    }

    // Update is called once per frame
    void Update()
    {
        HPBar.value = enemyCurrentHP / enemyMaxHP;

        if (enemyCurrentHP <= 0)
        {
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

            bool isRange = Vector3.Distance(transform.position, targetPlayer.transform.position) <= agent.stoppingDistance;

            // �����Ÿ� �ȿ� �÷��̾ �ִٸ�
            if (isRange)
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
        // Destroy(gameObject);
        gameObject.SetActive(false);
        InitEnemyHP();
        agent.speed = 1;
        enemyCollider.enabled = true;
    }
}
