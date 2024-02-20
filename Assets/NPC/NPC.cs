using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NPCQuestState
{
    HAVE_QUEST,
    PROCESS_QUEST,
    SUCCESS_QUEST,
    NONE
}

public class NPC : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] Sprite haveQuest;
    [SerializeField] Sprite processQuest;
    [SerializeField] Sprite successQuest;

    QuestManager questManager;
    Dictionary<int, string[]> talkData;
    List<bool> successList;
    Image questMark;
    int currentQuest = 0;

    NPCQuestState npcQuestState;

    string[] npcFirstHasQuest =
    {
        "안녕하십니까? 이곳은 처음이신가요?",
        "간단한 퀘스트를 드리겠습니다.",
        "몬스터 한마리를 잡고 저에게 다시 와주세요.",
        "당신이 이곳을 탈출하는데 도움이 될만한 것을 드리겠습니다.",
    };

    string[] npcFirstProcessQuest =
    {
        "아직인가요? 제가 그렇게 어려운 부탁을 한 것 같지는 않습니다만..."
    };

    string[] npcFirstSuccessQuest =
    {
        "수고많으셨습니다.",
        "이 열쇠를 받아주세요.",
        "이 열쇠는 당신이 곧 찾게 될 문을 열게 해주는 열쇠입니다.",
        "부디 행운을 빌겠습니다."
    };

    string[] npcSecondHasQuest =
    {
        "잠깐, 가기 전에 드릴 말씀이 있습니다.",
        "다음 장소에 가게 되면 저와 비슷하게 생긴 친구를 볼 수 있을겁니다.",
        "그 친구에게 이 편지를 전달해주세요. 그리고 답장을 받아와주세요.",
        "당신이 이곳을 탈출하는데 또 다른 도움이 될만한 것을 드리겠습니다.",
    };

    string[] npcSecondProcessQuest =
    {
        "답장은 받아오셨나요?",
        "...아 아직이군요.",
    };

    string[] npcSecondSuccessQuest =
    {
        "답장을 받아오셨나요?",
        "아 감사합니다! 어디 한번 볼까요...",
        "흠 그렇군요. 감사합니다.",
        "이건 제 감사의 뜻입니다.",
        "행운을 빌겠습니다. 도와주셔서 감사드립니다."
    };

    public Dictionary<int, string[]> TalkData { get { return talkData; } }
    public int ID { get { return id; } }
    public int CurrentQuest { get { return currentQuest; } }
    public NPCQuestState NPCQuestState { get { return npcQuestState; } }

    void Start()
    {
        questManager = FindObjectOfType<QuestManager>();

        if (questManager != null)
        {
            questManager.successQuest += SetSuccessArr;
        }

        talkData = new Dictionary<int, string[]>();
        successList = new List<bool>();
        questMark = GetComponentInChildren<Image>();
        GenerateData();
    }

    void Update()
    {
        UpdateQuestMark();
    }

    void UpdateQuestMark()
    {
        if (npcQuestState == NPCQuestState.HAVE_QUEST)
        {
            questMark.sprite = haveQuest;
        }
        else if (npcQuestState == NPCQuestState.PROCESS_QUEST)
        {
            questMark.sprite = processQuest;
        }
        else if (npcQuestState == NPCQuestState.SUCCESS_QUEST)
        {
            questMark.sprite = successQuest;
        }
    }

    void GenerateData()
    {
        //첫번째 퀘스트
        talkData.Add(1010, npcFirstHasQuest);
        talkData.Add(1011, npcFirstProcessQuest);
        talkData.Add(1012, npcFirstSuccessQuest);

        // 두번째 퀘스트
        talkData.Add(1020, npcSecondHasQuest);
        talkData.Add(1021, npcSecondProcessQuest);
        talkData.Add(1022, npcSecondSuccessQuest);
    }

    public void IncreaseCurrentQuest()
    {
        currentQuest++;
    }

    public void AddSuccessList()
    {
        successList.Add(false);
    }

    void SetSuccessArr()
    {
        successList[currentQuest] = true;

        if (npcQuestState == NPCQuestState.PROCESS_QUEST)
        {
            npcQuestState = NPCQuestState.SUCCESS_QUEST;
        }
    }

    public void SetNPCQuestState(NPCQuestState state)
    {
        npcQuestState = state;
    }
}
