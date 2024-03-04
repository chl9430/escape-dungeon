using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardNPC : NPC
{
    [Header("1020 Quest")]
    [SerializeField] GameObject[] requestObjs1020;
    string[] receiveMessageTalks =
    {
        "�ȳ��ϼ���! �������ΰ���?",
        "������ �� ���� �ִٰ��? ��� �ѹ�...",
        "�� �׷�����. �������ּż� �����մϴ�.",
        "�̰��� �����Դϴ�. ������ �� �������ּ���.",
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
