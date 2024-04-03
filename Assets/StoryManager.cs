using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct TalkDetail
{
    public QuestNPC talkingQuestNPC;
    public string[] talks;
}

public struct QuestDetail
{
    public string questSum;
    public QuestNPC questNPC;
    public GameObject[] requestItemObjs;
    public Dictionary<int, TalkDetail> talkDic;
    public GameObject[] rewardItemObjs;
}

public class StoryManager : MonoBehaviour
{
    public static StoryManager instance;

    [SerializeField] GameObject gameUIObj;
    [SerializeField] GameObject talkBoxObj;

    [Header("Quest 0")]
    [SerializeField] GameObject[] quest0_RewardItemObjs;
    [SerializeField] QuestNPC quest0_QuestNPC;

    [Header("Quest 1")]
    [SerializeField] GameObject[] quest1_RequestItemObjs;
    [SerializeField] GameObject[] quest1_RewardItemObjs;
    [SerializeField] QuestNPC quest1_SuccessQuestNPC;
    [SerializeField] QuestNPC quest1_QuestNPC;

    [Header("Quest 2")]
    [SerializeField] GameObject[] quest2_RequestItemObjs;
    [SerializeField] GameObject[] quest2_RewardItemObjs;
    [SerializeField] QuestNPC quest2_SuccessQuestNPC;
    [SerializeField] QuestNPC quest2_QuestNPC;

    Dictionary<int, QuestDetail> questDic;
    PlayerManager playerManager;
    int currentQuestNum;
    int currentTalkIdx = 0;

    public Dictionary<int, QuestDetail> QuestDic { get { return questDic; } }
    public int CurrentQuestNum { get { return currentQuestNum; } }

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

        questDic = new Dictionary<int, QuestDetail>();
        currentQuestNum = 0;
    }

    void Start()
    {
        GenerateTalkData();

        playerManager = FindObjectOfType<PlayerManager>();

        // ù��° ����Ʈ�� ������ �ִ� NPC�� ã���ϴ�.
        FindNextQuestNPC(currentQuestNum);
    }

    public void Talk(QuestNPC _questNPC)
    {
        QuestDetail questDetail = questDic[currentQuestNum];

        // ��ȭȯ���� ����
        gameUIObj.SetActive(false);
        talkBoxObj.SetActive(true);

        if (_questNPC.QuestState == QuestState.HAVE_QUEST)
        {
            TalkDetail talkDetail = questDetail.talkDic[0];

            if (currentTalkIdx < talkDetail.talks.Length)
            {
                // ��ȭ���ڿ� ��縦 ǥ���Ѵ�.
                talkBoxObj.GetComponentInChildren<Text>().text = talkDetail.talks[currentTalkIdx];

                currentTalkIdx++;
            }
            else if (currentTalkIdx == talkDetail.talks.Length) // ������ ������ ���������� �о��ٸ�
            {
                // ������ ������ �ִٸ� �����ش�.
                if (questDetail.requestItemObjs != null)
                {
                    // �÷��̾��� �κ��丮�� �� ĭ�� ������� Ȯ��
                    if (!playerManager.Inventory.AddItems(questDetail.requestItemObjs, questDetail.requestItemObjs.Length))
                    {
                        // ������� ������ �ٷ� ��ȭ����
                        FinishTalk();
                        return;
                    }
                }

                FinishTalk();

                _questNPC.SetQuestState(QuestState.PROCESS_QUEST);

                // ���� �������� ����Ʈ�� �Ҵ�
                QuestManager.instance.SetCurrentQuestDetail(questDetail, _questNPC);

                // ����Ʈ�� ���� �ϵ��ڵ� �κ�
                // ��3�� NPC���, ��3�� NPC�� ����Ʈ �Ϸ� ���·� ����
                if (currentQuestNum == 1)
                {
                    QuestNPC successQuestNPC = questDetail.talkDic[2].talkingQuestNPC;
                    successQuestNPC.SetQuestState(QuestState.SUCCESS_QUEST);
                }
                else if (currentQuestNum == 2)
                {
                    QuestNPC successQuestNPC = questDetail.talkDic[2].talkingQuestNPC;
                    successQuestNPC.SetQuestState(QuestState.SUCCESS_QUEST);
                }
            }
        }
        else if (_questNPC.QuestState == QuestState.PROCESS_QUEST)
        {
            TalkDetail talkDetail = questDetail.talkDic[1];

            if (currentTalkIdx < talkDetail.talks.Length)
            {
                // ��ȭ���ڿ� ��縦 ǥ���Ѵ�.
                talkBoxObj.GetComponentInChildren<Text>().text = talkDetail.talks[currentTalkIdx];

                currentTalkIdx++;
            }
            else if (currentTalkIdx == talkDetail.talks.Length) // ������ ������ ���������� �о��ٸ�
            {
                FinishTalk();
            }
        }
        else if (_questNPC.QuestState == QuestState.SUCCESS_QUEST)
        {
            TalkDetail talkDetail = questDetail.talkDic[2];

            if (currentTalkIdx < talkDetail.talks.Length)
            {
                // ��ȭ���ڿ� ��縦 ǥ���Ѵ�.
                talkBoxObj.GetComponentInChildren<Text>().text = talkDetail.talks[currentTalkIdx];

                currentTalkIdx++;
            }
            else if (currentTalkIdx == talkDetail.talks.Length) // ������ ������ ���������� �о��ٸ�
            {
                // �÷��̾�� �� ������ �ִٸ� �κ��丮 �� ĭ üũ
                if (questDetail.rewardItemObjs != null)
                {
                    int requiredSlotCnt = questDetail.rewardItemObjs.Length;

                    // NPC�� ��ٸ��� ������ �ִٸ�
                    if (questDetail.requestItemObjs != null)
                    {
                        requiredSlotCnt = questDetail.rewardItemObjs.Length - 1;

                        // �÷��̾��� �κ��丮�� �� ĭ�� ������� üũ
                        if (playerManager.Inventory.CheckInventorySlots(requiredSlotCnt))
                        {
                            for (int i = 0; i < questDetail.requestItemObjs.Length; i++)
                            {
                                playerManager.Inventory.PassItemToNPC(questDetail.requestItemObjs[i]);
                            }
                        }
                        else
                        {
                            // �ٷ� ��ȭ ����
                            FinishTalk();
                            GameManager.instance.AddGameLog("�κ��丮�� ������ ������� �ʽ��ϴ�.");
                            return;
                        }
                    }

                    // �κ��丮�� �� ĭ üũ�� �����ٸ� ������ ��ȯ
                    if (!playerManager.Inventory.AddItems(questDetail.rewardItemObjs, requiredSlotCnt))
                    {
                        // �ٷ� ��ȭ ����
                        FinishTalk();
                        return;
                    }
                }

                FinishTalk();

                // ��3�� NPC�� ����Ʈ�� �� NPC�� ��� ����Ʈ ���� ���·� �����.
                _questNPC.SetQuestState(QuestState.NONE);
                questDetail.questNPC.SetQuestState(QuestState.NONE);

                // �Ҵ� �� ����Ʈ�� ��� �ʱ�ȭ�Ѵ�.
                QuestManager.instance.ResetCurrentQuestDetail();

                // ���� ����Ʈ�� ã�´�.
                currentQuestNum++;
                FindNextQuestNPC(currentQuestNum);
            }
        }
    }

    void FinishTalk()
    {
        currentTalkIdx = 0;
        playerManager.IsTalking = false;

        // ��ȭȯ���� ����
        gameUIObj.SetActive(true);
        talkBoxObj.SetActive(false);
    }

    void FindNextQuestNPC(int _questNum)
    {
        if (questDic.ContainsKey(_questNum))
        {
            questDic[_questNum].questNPC.SetQuestState(QuestState.HAVE_QUEST);
        }
    }

    void GenerateTalkData()
    {
        // quest 0
        QuestDetail mainQuest0 = new QuestDetail
        {
            questSum = "TurtleShell�� 2���� óġ�ϼ���.",
            rewardItemObjs = quest0_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
            questNPC = quest0_QuestNPC,
        };

        // have_quest ���
        mainQuest0.talkDic[0] = new TalkDetail()
        {
            talkingQuestNPC = quest0_QuestNPC,
            talks = new string[]
            {
                "�ȳ��ϼ���",
                "Turtle Shell�� 2���� ����ּ���.",
                "��Ź�帳�ϴ�.",
            }
        };

        // process_quest ���
        mainQuest0.talkDic[1] = new TalkDetail()
        {
            talkingQuestNPC = quest0_QuestNPC,
            talks = new string[]
            {
                "�� ��ƿ��̳���?",
                "��... ���ѷ��ּ���."
            }
        };

        // success_quest ���
        mainQuest0.talkDic[2] = new TalkDetail()
        {
            talkingQuestNPC = quest0_QuestNPC,
            talks = new string[]
            {
                "����ϼ̽��ϴ�.",
                "�̰� �����Դϴ�.",
                "�޾��ּ���.",
                "����� ���ϴ�."
            }
        };

        questDic.Add(0, mainQuest0);

        // quest 1
        QuestDetail mainQuest1 = new()
        {
            questSum = "Dog Warrier���� ������ �����ϼ���.",
            requestItemObjs = quest1_RequestItemObjs,
            rewardItemObjs = quest1_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
            questNPC = quest1_QuestNPC,
        };

        // have_quest ���
        mainQuest1.talkDic[0] = new TalkDetail()
        {
            talkingQuestNPC = quest1_QuestNPC,
            talks = new string[]
            {
                "�ȳ��ϼ���",
                "�� ������ Dog Warrior���� ������ �ּ���.",
                "�׸��� ������ �޾ƿ��ּ���.",
                "��Ź�帳�ϴ�."
            }
        };

        // process_quest ���
        mainQuest1.talkDic[1] = new TalkDetail()
        {
            talkingQuestNPC = quest1_QuestNPC,
            talks = new string[]
            {
                "������ �޾ƿ��̳���?",
                "��... �����̱���."
            }
        };

        // success_quest ���
        mainQuest1.talkDic[2] = new TalkDetail()
        {
            talkingQuestNPC = quest1_SuccessQuestNPC,
            talks = new string[]
            {
                "�������ΰ���?",
                "��? ������?",
                "�����. �������ּż� �����մϴ�."
            }
        };

        questDic.Add(1, mainQuest1);

        // quest 2
        QuestDetail mainQuest2 = new QuestDetail
        {
            questSum = "Wizard���� ������ �����ϼ���.",
            requestItemObjs = quest2_RequestItemObjs,
            rewardItemObjs = quest2_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
            questNPC = quest2_QuestNPC,
        };

        // have_quest ���
        mainQuest2.talkDic[0] = new TalkDetail()
        {
            talkingQuestNPC = quest2_QuestNPC,
            talks = new string[]
            {
                "���� ������ Wizard���� �������ּ���.",
                "�����մϴ�.",
            }
        };

        // process_quest ���
        mainQuest2.talkDic[1] = new TalkDetail()
        {
            talkingQuestNPC = quest2_QuestNPC,
            talks = new string[]
            {
                "Wizard���� ������ �������ּ���.",
                "������ ���� �ʿ�� �����ϴ�."
            }
        };

        // success_quest ���
        mainQuest2.talkDic[2] = new TalkDetail()
        {
            talkingQuestNPC = quest2_SuccessQuestNPC,
            talks = new string[]
            {
                "��! ������ �޾ƿ��̱���.",
                "�̰� �����Դϴ�.",
                "�޾��ּ���.",
                "����� ���ϴ�."
            }
        };

        questDic.Add(2, mainQuest2);
    }
}