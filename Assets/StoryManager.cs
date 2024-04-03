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
        currentQuestNum = 0;
    }

    void Start()
    {
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
                // 전해줄 물건이 있다면 전해준다.
                if (questDetail.requestItemObjs != null)
                {
                    // 플레이어의 인벤토리의 빈 칸이 충분한지 확인
                    if (!playerManager.Inventory.AddItems(questDetail.requestItemObjs, questDetail.requestItemObjs.Length))
                    {
                        // 충분하지 않으면 바로 대화종료
                        FinishTalk();
                        return;
                    }
                }

                FinishTalk();

                _questNPC.SetQuestState(QuestState.PROCESS_QUEST);

                // 현재 진행중인 퀘스트를 할당
                QuestManager.instance.SetCurrentQuestDetail(questDetail, _questNPC);

                // 퀘스트에 따른 하드코딩 부분
                // 제3자 NPC라면, 제3자 NPC를 퀘스트 완료 상태로 변경
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
                // 플레이어에게 줄 보상이 있다면 인벤토리 빈 칸 체크
                if (questDetail.rewardItemObjs != null)
                {
                    int requiredSlotCnt = questDetail.rewardItemObjs.Length;

                    // NPC가 기다리는 물건이 있다면
                    if (questDetail.requestItemObjs != null)
                    {
                        requiredSlotCnt = questDetail.rewardItemObjs.Length - 1;

                        // 플레이어의 인벤토리의 빈 칸이 충분한지 체크
                        if (playerManager.Inventory.CheckInventorySlots(requiredSlotCnt))
                        {
                            for (int i = 0; i < questDetail.requestItemObjs.Length; i++)
                            {
                                playerManager.Inventory.PassItemToNPC(questDetail.requestItemObjs[i]);
                            }
                        }
                        else
                        {
                            // 바로 대화 종료
                            FinishTalk();
                            GameManager.instance.AddGameLog("인벤토리의 공간이 충분하지 않습니다.");
                            return;
                        }
                    }

                    // 인벤토리의 빈 칸 체크가 끝났다면 아이템 교환
                    if (!playerManager.Inventory.AddItems(questDetail.rewardItemObjs, requiredSlotCnt))
                    {
                        // 바로 대화 종료
                        FinishTalk();
                        return;
                    }
                }

                FinishTalk();

                // 제3자 NPC와 퀘스트를 준 NPC를 모두 퀘스트 없음 상태로 만든다.
                _questNPC.SetQuestState(QuestState.NONE);
                questDetail.questNPC.SetQuestState(QuestState.NONE);

                // 할당 된 퀘스트를 모두 초기화한다.
                QuestManager.instance.ResetCurrentQuestDetail();

                // 다음 퀘스트를 찾는다.
                currentQuestNum++;
                FindNextQuestNPC(currentQuestNum);
            }
        }
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
            questDic[_questNum].questNPC.SetQuestState(QuestState.HAVE_QUEST);
        }
    }

    void GenerateTalkData()
    {
        // quest 0
        QuestDetail mainQuest0 = new QuestDetail
        {
            questSum = "TurtleShell을 2마리 처치하세요.",
            rewardItemObjs = quest0_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
            questNPC = quest0_QuestNPC,
        };

        // have_quest 대사
        mainQuest0.talkDic[0] = new TalkDetail()
        {
            talkingQuestNPC = quest0_QuestNPC,
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
            talkingQuestNPC = quest0_QuestNPC,
            talks = new string[]
            {
                "다 잡아오셨나요?",
                "아... 서둘러주세요."
            }
        };

        // success_quest 대사
        mainQuest0.talkDic[2] = new TalkDetail()
        {
            talkingQuestNPC = quest0_QuestNPC,
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
            requestItemObjs = quest1_RequestItemObjs,
            rewardItemObjs = quest1_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
            questNPC = quest1_QuestNPC,
        };

        // have_quest 대사
        mainQuest1.talkDic[0] = new TalkDetail()
        {
            talkingQuestNPC = quest1_QuestNPC,
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
            talkingQuestNPC = quest1_QuestNPC,
            talks = new string[]
            {
                "답장을 받아오셨나요?",
                "아... 아직이군요."
            }
        };

        // success_quest 대사
        mainQuest1.talkDic[2] = new TalkDetail()
        {
            talkingQuestNPC = quest1_SuccessQuestNPC,
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
            requestItemObjs = quest2_RequestItemObjs,
            rewardItemObjs = quest2_RewardItemObjs,
            talkDic = new Dictionary<int, TalkDetail>(),
            questNPC = quest2_QuestNPC,
        };

        // have_quest 대사
        mainQuest2.talkDic[0] = new TalkDetail()
        {
            talkingQuestNPC = quest2_QuestNPC,
            talks = new string[]
            {
                "여기 답장을 Wizard에게 전달해주세요.",
                "감사합니다.",
            }
        };

        // process_quest 대사
        mainQuest2.talkDic[1] = new TalkDetail()
        {
            talkingQuestNPC = quest2_QuestNPC,
            talks = new string[]
            {
                "Wizard에게 답장을 전달해주세요.",
                "저한테 오실 필요는 없습니다."
            }
        };

        // success_quest 대사
        mainQuest2.talkDic[2] = new TalkDetail()
        {
            talkingQuestNPC = quest2_SuccessQuestNPC,
            talks = new string[]
            {
                "오! 답장을 받아오셨군요.",
                "이건 보상입니다.",
                "받아주세요.",
                "행운을 빕니다."
            }
        };

        questDic.Add(2, mainQuest2);
    }
}