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
