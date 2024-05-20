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
        // 어디서든 접근 가능한 정적 변수
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

            // NPC가 중복된 위치에 생성되지 않게 한다.
            while (randNumList.Contains(randPosIndex))
            {
                randPosIndex = Random.Range(0, npcSpawnPoints.Length);
            }

            randNumList.Add(randPosIndex);

            // 생성한 NPC를 인스턴스화 한다.
            questNPCList.Add(Instantiate(npcObjs[i],
                npcSpawnPoints[randPosIndex]).GetComponent<QuestNPC>());
        }

        GenerateTalkData();

        playerManager = FindObjectOfType<PlayerManager>();

        // 첫번째 퀘스트를 가지고 있는 NPC를 찾습니다.
        FindNextQuestNPC(currentQuestNum);
    }

    public void Talk(QuestNPC _questNPC)
    {
        QuestDetail questDetail = questDic[currentQuestNum];

        // 대화환경을 구축
        gameUIObj.SetActive(false);
        talkBoxObj.SetActive(true);

        if (_questNPC.QuestState == QuestState.HAVE_QUEST)
        {
            TalkDetail talkDetail = questDetail.talkDic[0];

            if (currentTalkIdx < talkDetail.talks.Length)
            {
                // 대화상자에 대사를 표시한다.
                talkBoxObj.GetComponentInChildren<Text>().text = talkDetail.talks[currentTalkIdx];

                currentTalkIdx++;
            }
            else if (currentTalkIdx == talkDetail.talks.Length) // 마지막 대사까지 성공적으로 읽었다면
            {
                List<ItemCountInfo> compressedItemList = playerManager.Inventory.MakeCompressedItemCntList(questDetail.requestItemObjs);

                // 인벤토리에 여유가 있으면
                if (playerManager.Inventory.CheckInventorySlots(compressedItemList.Count))
                {
                    playerManager.Inventory.AddItems(compressedItemList);

                    // 퀘스트에 따른 하드코딩
                    // 제3자 NPC를 퀘스트 완료 상태로 변경
                    if (currentQuestNum == 1 || currentQuestNum == 2)
                    {
                        FindNPCByName(questDetail.talkDic[2].talkingNPCName).SetQuestState(QuestState.SUCCESS_QUEST);
                    }
                }
                else
                {
                    // 바로 대화 종료
                    FinishTalk();
                    return;
                }

                FinishTalk();

                _questNPC.SetQuestState(QuestState.PROCESS_QUEST);

                // 현재 진행중인 퀘스트를 할당
                QuestManager.instance.SetCurrentQuestDetail(questDetail, _questNPC);
            }
        }
        else if (_questNPC.QuestState == QuestState.PROCESS_QUEST)
        {
            TalkDetail talkDetail = questDetail.talkDic[1];

            if (currentTalkIdx < talkDetail.talks.Length)
            {
                // 대화상자에 대사를 표시한다.
                talkBoxObj.GetComponentInChildren<Text>().text = talkDetail.talks[currentTalkIdx];

                currentTalkIdx++;
            }
            else if (currentTalkIdx == talkDetail.talks.Length) // 마지막 대사까지 성공적으로 읽었다면
            {
                FinishTalk();
            }
        }
        else if (_questNPC.QuestState == QuestState.SUCCESS_QUEST)
        {
            TalkDetail talkDetail = questDetail.talkDic[2];

            if (currentTalkIdx < talkDetail.talks.Length)
            {
                // 대화상자에 대사를 표시한다.
                talkBoxObj.GetComponentInChildren<Text>().text = talkDetail.talks[currentTalkIdx];

                currentTalkIdx++;
            }
            else if (currentTalkIdx == talkDetail.talks.Length) // 마지막 대사까지 성공적으로 읽었다면
            {
                List<ItemCountInfo> compressedItemList = playerManager.Inventory.MakeCompressedItemCntList(questDetail.rewardItemObjs);

                // 인벤토리 공간 확인
                if (playerManager.Inventory.CheckInventorySlots(compressedItemList.Count - questDetail.requestItemObjs.Length))
                {
                    // 퀘스트에 따른 하드코딩
                    // 퀘스트가 끝나면 별도로 할 액션 추가하기
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
                    // 인벤토리에 여유가 없다면
                    // 바로 대화 종료
                    FinishTalk();
                    return;
                }

                FinishTalk();

                // 제3자 NPC와 퀘스트를 준 NPC를 모두 퀘스트 없음 상태로 만든다.
                _questNPC.SetQuestState(QuestState.NONE);
                FindNPCByName(questDetail.requestNPCName).SetQuestState(QuestState.NONE);

                // 할당 된 퀘스트를 모두 초기화한다.
                QuestManager.instance.ResetCurrentQuestDetail();

                // 다음 퀘스트를 찾는다.
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
        // NPC에게 물건을 전달
        for (int i = 0; i < _requestItemObjs.Length; i++)
        {
            playerManager.Inventory.PassItemToNPC(_requestItemObjs[i]);
        }

        // NPC로부터 보상을 수령
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

        // 대화환경을 해제
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
            questSum = "TurtleShell을 2마리 처치하세요.",
            requestNPCName = quest0_RequestNPCName,
            requestItemObjs = quest0_RequestItemObjs,
            rewardItemObjs = quest0_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
        };

        // have_quest 대사
        mainQuest0.talkDic[0] = new TalkDetail()
        {
            talkingNPCName = quest0_RequestNPCName,
            talks = new string[]
            {
                "안녕하세요",
                "Turtle Shell을 2마리 잡아주세요.",
                "부탁드립니다.",
            }
        };

        // process_quest 대사
        mainQuest0.talkDic[1] = new TalkDetail()
        {
            talkingNPCName = quest0_RequestNPCName,
            talks = new string[]
            {
                "다 잡아오셨나요?",
                "아... 서둘러주세요."
            }
        };

        // success_quest 대사
        mainQuest0.talkDic[2] = new TalkDetail()
        {
            talkingNPCName = quest0_SuccessNPCName,
            talks = new string[]
            {
                "고생하셨습니다.",
                "이건 보상입니다.",
                "받아주세요.",
                "행운을 빕니다."
            }
        };

        questDic.Add(0, mainQuest0);

        // quest 1
        QuestDetail mainQuest1 = new()
        {
            questSum = "Dog Warrier에게 편지를 전달하세요.",
            requestNPCName = quest1_RequestNPCName,
            requestItemObjs = quest1_RequestItemObjs,
            rewardItemObjs = quest1_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
        };

        // have_quest 대사
        mainQuest1.talkDic[0] = new TalkDetail()
        {
            talkingNPCName = quest1_RequestNPCName,
            talks = new string[]
            {
                "안녕하세요",
                "이 편지를 Dog Warrior에게 가져다 주세요.",
                "그리고 답장을 받아와주세요.",
                "부탁드립니다."
            }
        };

        // process_quest 대사
        mainQuest1.talkDic[1] = new TalkDetail()
        {
            talkingNPCName = quest1_RequestNPCName,
            talks = new string[]
            {
                "답장을 받아오셨나요?",
                "아... 아직이군요."
            }
        };

        // success_quest 대사
        mainQuest1.talkDic[2] = new TalkDetail()
        {
            talkingNPCName = quest1_SuccessNPCName,
            talks = new string[]
            {
                "무슨일인가요?",
                "엥? 편지요?",
                "어디보자. 전달해주셔서 감사합니다."
            }
        };

        questDic.Add(1, mainQuest1);

        // quest 2
        QuestDetail mainQuest2 = new QuestDetail
        {
            questSum = "Wizard에게 답장을 전달하세요.",
            requestNPCName = quest2_RequestNPCName,
            requestItemObjs = quest2_RequestItemObjs,
            rewardItemObjs = quest2_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
        };

        // have_quest 대사
        mainQuest2.talkDic[0] = new TalkDetail()
        {
            talkingNPCName = quest2_RequestNPCName,
            talks = new string[]
            {
                "여기 답장을 Wizard에게 전달해주세요.",
                "감사합니다.",
            }
        };

        // process_quest 대사
        mainQuest2.talkDic[1] = new TalkDetail()
        {
            talkingNPCName = quest2_RequestNPCName,
            talks = new string[]
            {
                "Wizard에게 답장을 전달해주세요.",
                "저한테 오실 필요는 없습니다."
            }
        };

        // success_quest 대사
        mainQuest2.talkDic[2] = new TalkDetail()
        {
            talkingNPCName = quest2_SuccessNPCName,
            talks = new string[]
            {
                "오! 답장을 받아오셨군요.",
                "이건 보상입니다.",
                "받아주세요.",
                "행운을 빕니다."
            }
        };

        questDic.Add(2, mainQuest2);

        // quest 3
        QuestDetail mainQuest3 = new()
        {
            questSum = "Beholder을 3마리 처치하세요.",
            requestNPCName = quest3_RequestNPCName,
            requestItemObjs = quest3_RequestItemObjs,
            rewardItemObjs = quest3_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
        };

        // have_quest 대사
        mainQuest3.talkDic[0] = new TalkDetail()
        {
            talkingNPCName = quest3_RequestNPCName,
            talks = new string[]
            {
                "부탁을 하나 더 들어주세요.",
                "Beholder라는 몬스터를 3마리 잡아주세요.",
                "조심하세요. 그들은 원거리 공격을 합니다.",
            }
        };

        // process_quest 대사
        mainQuest3.talkDic[1] = new TalkDetail()
        {
            talkingNPCName = quest3_RequestNPCName,
            talks = new string[]
            {
                "다 잡아오셨나요?",
                "좀 어려운 부탁이긴 합니다.",
                "당신의 한계는 여기까지 인가요?"
            }
        };

        // success_quest 대사
        mainQuest3.talkDic[2] = new TalkDetail()
        {
            talkingNPCName = quest3_SuccessNPCName,
            talks = new string[]
            {
                "오! 다 잡아오셨군요.",
                "감사합니다.",
                "여기를 나가기 위해서는 게이트를 활성화해야합니다.",
                "게이트를 활성화하기 위해서는 게이트버튼을 찾아야해요.",
                "이곳 어딘가에 해골모양의 게이트버튼이 있습니다.",
                "그 버튼을 누르면 게이트가 활성화 될겁니다.",
                "행운을 빕니다!"
            }
        };

        questDic.Add(3, mainQuest3);
    }
}