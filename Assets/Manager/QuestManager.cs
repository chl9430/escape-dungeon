using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [SerializeField] Text currentQuestTitle;

    QuestNPC currentQuestNPC;
    QuestDetail? currentQuestDetail = null; // 구조체 타입의 변수를 null로 만들 수 있는 변수로 만든다.

    [Header("Quest Number 0")]
    int turtleshellDeadCnt;

    void Awake()
    {
        // 어디서든 접근 가능한 정적 변수
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentQuestDetail(QuestDetail _questDetail, QuestNPC _questNPC)
    {
        currentQuestNPC = _questNPC;
        currentQuestDetail = _questDetail;

        currentQuestTitle.text = _questDetail.questSum;
    }

    public void CheckDeadMonName(string _deadMonName)
    {
        // 퀘스트에 따른 하드코딩 부분
        if (currentQuestDetail != null && StoryManager.instance.CurrentQuestNum == 0)
        {
            if (_deadMonName == "Turtle Shell")
            {
                turtleshellDeadCnt++;

                currentQuestTitle.text += ("(" + (2 - turtleshellDeadCnt) + "마리남음)");

                if (turtleshellDeadCnt == 2)
                {
                    currentQuestTitle.text = currentQuestDetail?.questSum + "(성공)";
                    currentQuestNPC.SetQuestState(QuestState.SUCCESS_QUEST);
                }
            }
        }
    }

    public void ResetCurrentQuestDetail()
    {
        currentQuestNPC = null;
        currentQuestDetail = null;

        currentQuestTitle.text = "진행 중인 퀘스트가 없습니다.";
    }
}