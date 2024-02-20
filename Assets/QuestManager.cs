using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public Action successQuest;

    GameObject player;

    [Header("Quest 1010")]
    [SerializeField] int level1MonDeadCnt1010 = 5;

    PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerManager = player.GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerManager.currentQuest == 1010 && level1MonDeadCnt1010 == 0)
        {
            successQuest();
        }
    }

    public void CheckDeadMonName(string deadMonName)
    {
        if (playerManager.CurrentQuest == 1010)
        {
            if (deadMonName == "Level 1")
            {
                level1MonDeadCnt1010--;
            }
        }
    }
}
