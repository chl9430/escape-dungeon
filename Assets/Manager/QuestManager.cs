using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    [Header("Quest 1010")]
    [SerializeField] int level1MonDeadCnt1010 = 1;

    [SerializeField] GameObject questTextObj;

    GameObject player;
    GameObject questNPCObj;
    PlayerManager playerManager;
    Text currentQuest;

    public GameObject QuestNPCObj { set { questNPCObj = value; } }

    void Start()
    {
        player = FindObjectOfType<PlayerManager>().gameObject;
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
                questNPCObj.GetComponent<NPC>().SetSuccessArr();
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
            if (deadMonName == "Turtle Shell" && level1MonDeadCnt1010 != 0)
            {
                level1MonDeadCnt1010--;
            }
        }
    }
}
