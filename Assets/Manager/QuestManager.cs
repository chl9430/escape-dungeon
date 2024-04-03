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
    QuestDetail? currentQuestDetail = null; // ����ü Ÿ���� ������ null�� ���� �� �ִ� ������ �����.

    [Header("Quest Number 0")]
    int turtleshellDeadCnt;

    void Awake()
    {
        // ��𼭵� ���� ������ ���� ����
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
        // ����Ʈ�� ���� �ϵ��ڵ� �κ�
        if (currentQuestDetail != null && StoryManager.instance.CurrentQuestNum == 0)
        {
            if (_deadMonName == "Turtle Shell")
            {
                turtleshellDeadCnt++;

                currentQuestTitle.text += ("(" + (2 - turtleshellDeadCnt) + "��������)");

                if (turtleshellDeadCnt == 2)
                {
                    currentQuestTitle.text = currentQuestDetail?.questSum + "(����)";
                    currentQuestNPC.SetQuestState(QuestState.SUCCESS_QUEST);
                }
            }
        }
    }

    public void ResetCurrentQuestDetail()
    {
        currentQuestNPC = null;
        currentQuestDetail = null;

        currentQuestTitle.text = "���� ���� ����Ʈ�� �����ϴ�.";
    }
}