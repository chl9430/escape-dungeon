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

    void Start()
    {
        instance = this;
        talkData = new Dictionary<int, QuestDataStruct>();
        GenerateData();
    }

    void GenerateData()
    {
        // Quest Talk(0 : 퀘스트를 보유중일때, 1 : 퀘스트 진행 중일때, 2 : 퀘스트 완료시)
        
        // 첫번째 퀘스트
        QuestDataStruct npcFirstHasQuestData;
        npcFirstHasQuestData.questArr = npcFirstHasQuest;

        talkData.Add(1010, npcFirstHasQuestData);

        QuestDataStruct npcFirstProcessQuestData;
        npcFirstProcessQuestData.questArr = npcFirstProcessQuest;

        talkData.Add(1011, npcFirstProcessQuestData);

        QuestDataStruct npcFirstSuccessQuestData;
        npcFirstSuccessQuestData.questArr = npcFirstSuccessQuest;

        talkData.Add(1012, npcFirstSuccessQuestData);

        // 두번째 퀘스트
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