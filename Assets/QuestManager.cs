using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public Action successQuest;

    [Header("Quest 1010")]
    [SerializeField] int level1MonDeadCnt1010 = 5;

    [SerializeField] GameObject questTextObj;

    GameObject player;
    PlayerManager playerManager;
    Text currentQuest;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerManager = player.GetComponent<PlayerManager>();

        if (questTextObj != null)
        {
            currentQuest = questTextObj.GetComponent<Text>();
        }
    }

    void Update()
    {
        if (playerManager.CurrentQuest == 1010)
        {
            if (level1MonDeadCnt1010 == 0)
            {
                successQuest();
                currentQuest.color = Color.green;
                currentQuest.text = "���͸� 5���� óġ�ϼ���." + " (�Ϸ�)";
            }
            else
            {
                currentQuest.color = Color.white;
                currentQuest.text = "���͸� 5���� óġ�ϼ���." + " (" + level1MonDeadCnt1010 + "���� ����)";
            }
        }
        else
        {
            currentQuest.color = Color.white;
            currentQuest.text = "���� ���� ����Ʈ�� �����ϴ�.";
        }
    }

    public void CheckDeadMonName(string deadMonName)
    {
        if (playerManager.CurrentQuest == 1010)
        {
            if (deadMonName == "Level 1" && level1MonDeadCnt1010 != 0)
            {
                level1MonDeadCnt1010--;
            }
        }
    }
}
