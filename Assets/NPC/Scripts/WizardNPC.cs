using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardNPC : NPC
{
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

        AddTalkData(1023, receiveMessageTalks);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateQuestMark();
    }
}
