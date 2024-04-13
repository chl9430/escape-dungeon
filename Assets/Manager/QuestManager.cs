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

    [Header("Quest 0")]
    int turtleshellDeadCnt;

    [Header("Quest 3")]
    int beholderDeadCnt;

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

        // ����Ʈ�� ���� �ϵ��ڵ� �κ�
        // ��ƾ��ϴ� ���� �� �ʱ�ȭ�ϱ�
        turtleshellDeadCnt = 2;
        beholderDeadCnt = 3;
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
        // ���� ���͸� üũ�Ѵ�.
        if (currentQuestDetail != null && StoryManager.instance.CurrentQuestNum == 0)
        {
            if (_deadMonName == "Turtle Shell")
            {
                turtleshellDeadCnt--;

                currentQuestTitle.text = currentQuestDetail?.questSum;

                currentQuestTitle.text += ("\n(" + turtleshellDeadCnt + "��������)");

                if (turtleshellDeadCnt <= 0)
                {
                    turtleshellDeadCnt = 0;
                    currentQuestTitle.text = currentQuestDetail?.questSum + "(����)";
                    currentQuestNPC.SetQuestState(QuestState.SUCCESS_QUEST);
                }
            }
        }
        else if (currentQuestDetail != null && StoryManager.instance.CurrentQuestNum == 3)
        {
            if (_deadMonName == "Beholder")
            {
                beholderDeadCnt--;

                currentQuestTitle.text = currentQuestDetail?.questSum;

                currentQuestTitle.text += ("\n(" + beholderDeadCnt + "��������)");

                if (beholderDeadCnt <= 0)
                {
                    beholderDeadCnt = 0;
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