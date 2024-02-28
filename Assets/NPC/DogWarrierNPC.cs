using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogWarrierNPC : NPC
{
    [Header("1010 Quest")]
    [SerializeField] GameObject[] RewardObjs1010;

    [Header("1020 Quest")]
    [SerializeField] GameObject[] requestObjs1020;
    [SerializeField] GameObject[] interactiveNPCObjs1020;

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

    // Start is called before the first frame update
    void Start()
    {
        AddTalkData(1010, npcFirstHasQuest);
        AddTalkData(1011, npcFirstProcessQuest);
        AddTalkData(1012, npcFirstSuccessQuest);

        AddRewardData(1010, RewardObjs1010);

        AddTalkData(1020, npcSecondHasQuest);
        AddTalkData(1021, npcSecondProcessQuest);
        AddTalkData(1022, npcSecondSuccessQuest);

        AddRequestObjData(1020, requestObjs1020);
        AddInteractiveNPCData(1020, interactiveNPCObjs1020);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateQuestMark();
    }
}
