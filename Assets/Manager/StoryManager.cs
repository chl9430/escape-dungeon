using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct TalkDetail
{
    public string talkingNPCName;
    public string[] talks;
}

public struct QuestDetail
{
    public string questSum;
    public string requestNPCName;
    public GameObject[] requestItemObjs;
    public GameObject[] rewardItemObjs;
    public Dictionary<int, TalkDetail> talkDic;
}

public class StoryManager : MonoBehaviour
{
    public static StoryManager instance;

    [SerializeField] GameObject gameUIObj;
    [SerializeField] GameObject talkBoxObj;

    [Header("Quest 0")]
    [SerializeField] GameObject[] quest0_RequestItemObjs;
    [SerializeField] GameObject[] quest0_RewardItemObjs;
    [SerializeField] string quest0_RequestNPCName;
    [SerializeField] string quest0_SuccessNPCName;

    [Header("Quest 1")]
    [SerializeField] GameObject[] quest1_RequestItemObjs;
    [SerializeField] GameObject[] quest1_RewardItemObjs;
    [SerializeField] string quest1_RequestNPCName;
    [SerializeField] string quest1_SuccessNPCName;

    [Header("Quest 2")]
    [SerializeField] GameObject[] quest2_RequestItemObjs;
    [SerializeField] GameObject[] quest2_RewardItemObjs;
    [SerializeField] string quest2_RequestNPCName;
    [SerializeField] string quest2_SuccessNPCName;

    [Header("Quest 3")]
    [SerializeField] GameObject[] quest3_RequestItemObjs;
    [SerializeField] GameObject[] quest3_RewardItemObjs;
    [SerializeField] string quest3_RequestNPCName;
    [SerializeField] string quest3_SuccessNPCName;

    [Header("NPC Spawn Point")]
    [SerializeField] Transform[] npcSpawnPoints;
    [SerializeField] GameObject[] npcObjs;
    List<QuestNPC> questNPCList;

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
        questNPCList = new List<QuestNPC>();
        currentQuestNum = 0;
    }

    void Start()
    {
        // NPC
        List<int> randNumList = new List<int>();

        for (int i = 0; i < npcObjs.Length; i++)
        {
            int randPosIndex = Random.Range(0, npcSpawnPoints.Length);

            // NPC�� �ߺ��� ��ġ�� �������� �ʰ� �Ѵ�.
            while (randNumList.Contains(randPosIndex))
            {
                randPosIndex = Random.Range(0, npcSpawnPoints.Length);
            }

            randNumList.Add(randPosIndex);

            // ������ NPC�� �ν��Ͻ�ȭ �Ѵ�.
            questNPCList.Add(Instantiate(npcObjs[i],
                npcSpawnPoints[randPosIndex]).GetComponent<QuestNPC>());
        }

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
                List<ItemCountInfo> compressedItemList = playerManager.Inventory.MakeCompressedItemCntList(questDetail.requestItemObjs);

                // �κ��丮�� ������ ������
                if (playerManager.Inventory.CheckInventorySlots(compressedItemList.Count))
                {
                    playerManager.Inventory.AddItems(compressedItemList);

                    // ����Ʈ�� ���� �ϵ��ڵ�
                    // ��3�� NPC�� ����Ʈ �Ϸ� ���·� ����
                    if (currentQuestNum == 1 || currentQuestNum == 2)
                    {
                        FindNPCByName(questDetail.talkDic[2].talkingNPCName).SetQuestState(QuestState.SUCCESS_QUEST);
                    }
                }
                else
                {
                    // �ٷ� ��ȭ ����
                    FinishTalk();
                    return;
                }

                FinishTalk();

                _questNPC.SetQuestState(QuestState.PROCESS_QUEST);

                // ���� �������� ����Ʈ�� �Ҵ�
                QuestManager.instance.SetCurrentQuestDetail(questDetail, _questNPC);
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
                List<ItemCountInfo> compressedItemList = playerManager.Inventory.MakeCompressedItemCntList(questDetail.rewardItemObjs);

                // �κ��丮 ���� Ȯ��
                if (playerManager.Inventory.CheckInventorySlots(compressedItemList.Count - questDetail.requestItemObjs.Length))
                {
                    // ����Ʈ�� ���� �ϵ��ڵ�
                    // ����Ʈ�� ������ ������ �� �׼� �߰��ϱ�
                    if (currentQuestNum == 3)
                    {
                        StartCoroutine(StartQuest3Action(questDetail.requestItemObjs, compressedItemList));
                    }
                    else
                    {
                        ExchangeItemsWithNPC(questDetail.requestItemObjs, compressedItemList);
                    }
                }
                else
                {
                    // �κ��丮�� ������ ���ٸ�
                    // �ٷ� ��ȭ ����
                    FinishTalk();
                    return;
                }

                FinishTalk();

                // ��3�� NPC�� ����Ʈ�� �� NPC�� ��� ����Ʈ ���� ���·� �����.
                _questNPC.SetQuestState(QuestState.NONE);
                FindNPCByName(questDetail.requestNPCName).SetQuestState(QuestState.NONE);

                // �Ҵ� �� ����Ʈ�� ��� �ʱ�ȭ�Ѵ�.
                QuestManager.instance.ResetCurrentQuestDetail();

                // ���� ����Ʈ�� ã�´�.
                currentQuestNum++;
                FindNextQuestNPC(currentQuestNum);
            }
        }
    }

    IEnumerator StartQuest3Action(GameObject[] _requestItemObjs, List<ItemCountInfo> _compressedRewardItemList)
    {
        GameManager.instance.ActivateGateBtn();

        yield return new WaitForSeconds(3f);

        ExchangeItemsWithNPC(_requestItemObjs, _compressedRewardItemList);
    }

    void ExchangeItemsWithNPC(GameObject[] _requestItemObjs, List<ItemCountInfo> _compressedRewardItemList)
    {
        // NPC���� ������ ����
        for (int i = 0; i < _requestItemObjs.Length; i++)
        {
            playerManager.Inventory.PassItemToNPC(_requestItemObjs[i]);
        }

        // NPC�κ��� ������ ����
        playerManager.Inventory.AddItems(_compressedRewardItemList);
    }

    QuestNPC FindNPCByName(string _name)
    {
        foreach (QuestNPC questNPC in questNPCList)
        {
            if (questNPC.QuestNPCName == _name)
            {
                return questNPC;
            }
        }

        return null;
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
            FindNPCByName(questDic[_questNum].requestNPCName).SetQuestState(QuestState.HAVE_QUEST);
        }
    }

    void GenerateTalkData()
    {
        // quest 0
        QuestDetail mainQuest0 = new QuestDetail
        {
            questSum = "TurtleShell�� 2���� óġ�ϼ���.",
            requestNPCName = quest0_RequestNPCName,
            requestItemObjs = quest0_RequestItemObjs,
            rewardItemObjs = quest0_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
        };

        // have_quest ���
        mainQuest0.talkDic[0] = new TalkDetail()
        {
            talkingNPCName = quest0_RequestNPCName,
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
            talkingNPCName = quest0_RequestNPCName,
            talks = new string[]
            {
                "�� ��ƿ��̳���?",
                "��... ���ѷ��ּ���."
            }
        };

        // success_quest ���
        mainQuest0.talkDic[2] = new TalkDetail()
        {
            talkingNPCName = quest0_SuccessNPCName,
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
            requestNPCName = quest1_RequestNPCName,
            requestItemObjs = quest1_RequestItemObjs,
            rewardItemObjs = quest1_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
        };

        // have_quest ���
        mainQuest1.talkDic[0] = new TalkDetail()
        {
            talkingNPCName = quest1_RequestNPCName,
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
            talkingNPCName = quest1_RequestNPCName,
            talks = new string[]
            {
                "������ �޾ƿ��̳���?",
                "��... �����̱���."
            }
        };

        // success_quest ���
        mainQuest1.talkDic[2] = new TalkDetail()
        {
            talkingNPCName = quest1_SuccessNPCName,
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
            requestNPCName = quest2_RequestNPCName,
            requestItemObjs = quest2_RequestItemObjs,
            rewardItemObjs = quest2_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
        };

        // have_quest ���
        mainQuest2.talkDic[0] = new TalkDetail()
        {
            talkingNPCName = quest2_RequestNPCName,
            talks = new string[]
            {
                "���� ������ Wizard���� �������ּ���.",
                "�����մϴ�.",
            }
        };

        // process_quest ���
        mainQuest2.talkDic[1] = new TalkDetail()
        {
            talkingNPCName = quest2_RequestNPCName,
            talks = new string[]
            {
                "Wizard���� ������ �������ּ���.",
                "������ ���� �ʿ�� �����ϴ�."
            }
        };

        // success_quest ���
        mainQuest2.talkDic[2] = new TalkDetail()
        {
            talkingNPCName = quest2_SuccessNPCName,
            talks = new string[]
            {
                "��! ������ �޾ƿ��̱���.",
                "�̰� �����Դϴ�.",
                "�޾��ּ���.",
                "����� ���ϴ�."
            }
        };

        questDic.Add(2, mainQuest2);

        // quest 3
        QuestDetail mainQuest3 = new()
        {
            questSum = "Beholder�� 3���� óġ�ϼ���.",
            requestNPCName = quest3_RequestNPCName,
            requestItemObjs = quest3_RequestItemObjs,
            rewardItemObjs = quest3_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
        };

        // have_quest ���
        mainQuest3.talkDic[0] = new TalkDetail()
        {
            talkingNPCName = quest3_RequestNPCName,
            talks = new string[]
            {
                "��Ź�� �ϳ� �� ����ּ���.",
                "Beholder��� ���͸� 3���� ����ּ���.",
                "�����ϼ���. �׵��� ���Ÿ� ������ �մϴ�.",
            }
        };

        // process_quest ���
        mainQuest3.talkDic[1] = new TalkDetail()
        {
            talkingNPCName = quest3_RequestNPCName,
            talks = new string[]
            {
                "�� ��ƿ��̳���?",
                "�� ����� ��Ź�̱� �մϴ�.",
                "����� �Ѱ�� ������� �ΰ���?"
            }
        };

        // success_quest ���
        mainQuest3.talkDic[2] = new TalkDetail()
        {
            talkingNPCName = quest3_SuccessNPCName,
            talks = new string[]
            {
                "��! �� ��ƿ��̱���.",
                "�����մϴ�.",
                "���⸦ ������ ���ؼ��� ����Ʈ�� Ȱ��ȭ�ؾ��մϴ�.",
                "����Ʈ�� Ȱ��ȭ�ϱ� ���ؼ��� ����Ʈ��ư�� ã�ƾ��ؿ�.",
                "�̰� ��򰡿� �ذ����� ����Ʈ��ư�� �ֽ��ϴ�.",
                "�� ��ư�� ������ ����Ʈ�� Ȱ��ȭ �ɰ̴ϴ�.",
                "����� ���ϴ�!"
            }
        };

        questDic.Add(3, mainQuest3);
    }
}