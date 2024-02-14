using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int enemyMaxHP = 100;
    public int enemyCurrentHP = 0;

    // Start is called before the first frame update
    void Start()
    {
        InitEnemyHP();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitEnemyHP()
    {
        enemyCurrentHP = enemyMaxHP;
    }
}
