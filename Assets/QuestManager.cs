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
                currentQuest.text = "몬스터를 5마리 처치하세요." + " (완료)";
            }
            else
            {
                currentQuest.color = Color.white;
                currentQuest.text = "몬스터를 5마리 처치하세요." + " (" + level1MonDeadCnt1010 + "마리 남음)";
            }
        }
        else
        {
            currentQuest.color = Color.white;
            currentQuest.text = "진행 중인 퀘스트가 없습니다.";
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
