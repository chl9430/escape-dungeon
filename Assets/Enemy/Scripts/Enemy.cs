using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Slider HPBar;

    [SerializeField] protected string monName = "";
    [SerializeField] float enemyMaxHP = 5;
    [SerializeField] protected float attackCoolTime = 5f;

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

    public void GetDamaged(int damage)
    {
        enemyCurrentHP -= damage;

        RefreshHPBar();
    }

    public void RefreshHPBar()
    {
        HPBar.value = enemyCurrentHP / enemyMaxHP;
    }
}
