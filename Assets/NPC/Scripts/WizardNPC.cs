using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardNPC : NPC
{
    [Header("1020 Quest")]
    [SerializeField] GameObject[] requestObjs1020;
    string[] receiveMessageTalks =
    {
        "안녕하세요! 무슨일인가요?",
        "나에게 줄 것이 있다고요? 어디 한번...",
        "음 그렇군요. 전달해주셔서 감사합니다.",
        "이것은 답장입니다. 무사히 잘 전달해주세요.",
    };

    // Start is called before the first frame update
    void Start()
    {
        CheckNextQuest();
        AddRequestObjData(1020, requestObjs1020);
        AddTalkData(1023, receiveMessageTalks);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateQuestMark();
    }
}
