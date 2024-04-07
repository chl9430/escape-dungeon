using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Slider HPBar;
    [SerializeField] protected Text enemyName;

    [SerializeField] protected string monName = "";
    [SerializeField] float enemyMaxHP = 5;
    [SerializeField] protected float attackCoolTime = 5f;

    protected bool isDead = false;
    protected float enemyCurrentHP = 0;
    protected float currentAttackCoolTime;
    protected bool canAttack = false;


    public float EnemyCurrentHP { get { return enemyCurrentHP; } }
    public bool CanAttack { set { canAttack = value; } get { return canAttack; } }

    public void InitEnemyHP()
    {
        enemyCurrentHP = enemyMaxHP;

        RefreshHPBar();
    }

    public virtual void GetDamaged(int _damage)
    {
        enemyCurrentHP -= _damage;

        RefreshHPBar();

        if (enemyCurrentHP <= 0)
        {
            isDead = true;
            QuestManager.instance.CheckDeadMonName(monName);
            GameManager.instance.AddGameLog(monName + "를(을) 처치하였습니다.");
        }
    }

    public void RefreshHPBar()
    {
        HPBar.value = enemyCurrentHP / enemyMaxHP;
    }
}
