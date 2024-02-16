using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public struct QuestDataStruct
{
    public string[] questArr;
}

public class TalkManager : MonoBehaviour
{
    [SerializeField] GameObject talkBox;

    public static TalkManager instance;
    Dictionary<int, QuestDataStruct> talkData;
    int talkIndex;

    public bool isTalking = false;

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

    void Start()
    {
        instance = this;
        talkData = new Dictionary<int, QuestDataStruct>();
        GenerateData();
    }

    void GenerateData()
    {
        // Quest Talk(0 : ����Ʈ�� �������϶�, 1 : ����Ʈ ���� ���϶�, 2 : ����Ʈ �Ϸ��)
        
        // ù��° ����Ʈ
        QuestDataStruct npcFirstHasQuestData;
        npcFirstHasQuestData.questArr = npcFirstHasQuest;

        talkData.Add(1010, npcFirstHasQuestData);

        QuestDataStruct npcFirstProcessQuestData;
        npcFirstProcessQuestData.questArr = npcFirstProcessQuest;

        talkData.Add(1011, npcFirstProcessQuestData);

        QuestDataStruct npcFirstSuccessQuestData;
        npcFirstSuccessQuestData.questArr = npcFirstSuccessQuest;

        talkData.Add(1012, npcFirstSuccessQuestData);

        // �ι�° ����Ʈ
        QuestDataStruct npcSecondHasQuestData;
        npcSecondHasQuestData.questArr = npcSecondHasQuest;

        talkData.Add(1020, npcSecondHasQuestData);

        QuestDataStruct npcSecondProcessQuestData;
        npcSecondProcessQuestData.questArr = npcSecondProcessQuest;

        talkData.Add(1021, npcSecondProcessQuestData);

        QuestDataStruct npcSecondSuccessQuestData;
        npcSecondSuccessQuestData.questArr = npcSecondSuccessQuest;

        talkData.Add(1022, npcSecondSuccessQuestData);
    }

    public void Talk(GameObject npcObj)
    {
        NPC npc = npcObj.GetComponent<NPC>();
        Text talkText = talkBox.GetComponentInChildren<Text>();

        int index = npc.ID + (int)npc.NPCQuestState + (10 * (npc.CurrentQuest + 1));

        if (talkIndex == talkData[index].questArr.Length)
        {
            isTalking = false;
            talkBox.SetActive(false);
            talkIndex = 0;

            if (npc.NPCQuestState == NPCQuestState.HAVE_QUEST)
            {
                // playerScript.SetCurrentQuest(npcObj);
                // npc.SetProcessingPlayer(player.gameObject);
                npc.SetNPCQuestState(NPCQuestState.PROCESS_QUEST);
                npc.AddSuccessList();
            }
            else if (npc.NPCQuestState == NPCQuestState.SUCCESS_QUEST)
            {
                // currentQuestText.text = "";
                npc.IncreaseCurrentQuest();
                npc.SetNPCQuestState(NPCQuestState.HAVE_QUEST);
            }
        }
        else
        {
            isTalking = true;
            talkBox.SetActive(true);
            talkText.text = GetTalk(index, talkIndex);
            talkIndex++;
        }
    }

    string GetTalk(int id, int talkIndex)
    {
        if (talkIndex == talkData[id].questArr.Length)
            return null;
        else
            return talkData[id].questArr[talkIndex];
    }
}