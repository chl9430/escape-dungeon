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
        "�ȳ��Ͻʴϱ�? �̰��� ó���̽Ű���?",
        "������ ����Ʈ�� �帮�ڽ��ϴ�.",
        "���� �Ѹ����� ��� ������ �ٽ� ���ּ���.",
        "����� �̰��� Ż���ϴµ� ������ �ɸ��� ���� �帮�ڽ��ϴ�.",
    };

    string[] npcFirstProcessQuest =
    {
        "�����ΰ���? ���� �׷��� ����� ��Ź�� �� �� ������ �ʽ��ϴٸ�..."
    };

    string[] npcFirstSuccessQuest =
    {
        "�������̽��ϴ�.",
        "�� ���踦 �޾��ּ���.",
        "�� ����� ����� �� ã�� �� ���� ���� ���ִ� �����Դϴ�.",
        "�ε� ����� ���ڽ��ϴ�."
    };

    string[] npcSecondHasQuest =
    {
        "���, ���� ���� �帱 ������ �ֽ��ϴ�.",
        "���� ��ҿ� ���� �Ǹ� ���� ����ϰ� ���� ģ���� �� �� �����̴ϴ�.",
        "�� ģ������ �� ������ �������ּ���. �׸��� ������ �޾ƿ��ּ���.",
        "����� �̰��� Ż���ϴµ� �� �ٸ� ������ �ɸ��� ���� �帮�ڽ��ϴ�.",
    };

    string[] npcSecondProcessQuest =
    {
        "������ �޾ƿ��̳���?",
        "...�� �����̱���.",
    };

    string[] npcSecondSuccessQuest =
    {
        "������ �޾ƿ��̳���?",
        "�� �����մϴ�! ��� �ѹ� �����...",
        "�� �׷�����. �����մϴ�.",
        "�̰� �� ������ ���Դϴ�.",
        "����� ���ڽ��ϴ�. �����ּż� ����帳�ϴ�."
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
        //ù��° ����Ʈ
        talkData.Add(1010, npcFirstHasQuest);
        talkData.Add(1011, npcFirstProcessQuest);
        talkData.Add(1012, npcFirstSuccessQuest);

        // �ι�° ����Ʈ
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
