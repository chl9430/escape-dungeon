using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
    public static TalkManager instance;

    [SerializeField] GameObject talkBoxObj;
    [SerializeField] GameObject gameUIObj;
    [SerializeField] GameObject instUIObj;

    int talkIndex;

    GameObject playerObj;
    GameObject inventoryObj;
    QuestManager questManager;

    void Start()
    {
        instance = this;
        playerObj = FindObjectOfType<PlayerManager>().gameObject;
        inventoryObj = FindObjectOfType<Inventory>().gameObject;
        questManager = FindObjectOfType<QuestManager>();
    }

    public void Talk(GameObject npcObj)
    {
        NPC npc = npcObj.GetComponent<NPC>();
        Text talkText = talkBoxObj.GetComponentInChildren<Text>();
        PlayerManager playerManager = playerObj.GetComponent<PlayerManager>();

        int questMainKey = npc.ID + (10 * (npc.CurrentQuest + 1));
        int key = npc.ID + (int)npc.NPCQuestState + (10 * (npc.CurrentQuest + 1));

        if (npc.InteractiveTalkKey != 0)
        {
            key = npc.InteractiveTalkKey;

            // ����Ʈ�� ���� �ϵ��ڵ� �κ�
            if (npc.InteractiveTalkKey == 1023)
            {
                questMainKey = 1020;
            }
        }

        if (!npc.TalkData.ContainsKey(key))
        {
            return;
        }
        else
        {
            if (talkIndex == npc.TalkData[key].Length) // ��ȭ�� �����ٸ�
            {
                // ��ȭ ȯ���� �����ϴ� �Լ�
                SetTalkingEnvironment(false, playerManager);
                talkIndex = 0;

                if (npc.NPCQuestState == NPCQuestState.HAVE_QUEST) // ����Ʈ�� �޾Ҵٸ�
                {
                    // ����Ʈ�� ���� �ϵ��ڵ� �κ�
                    if (questMainKey == 1020)
                    {
                        // �÷��̾�� ������ ������ �ִٸ� �����Ѵ�.
                        if (GiveItem(npc.RequestObjData[questMainKey]))
                        {
                            playerManager.CurrentQuest = questMainKey;
                            npc.SetNPCQuestState(NPCQuestState.PROCESS_QUEST);
                            npc.AddSuccessList();

                            // ������ �޾ƾ��� NPC�� ����Ʈ �Ϸ���·� ����
                            for (int i = 0; i < npc.InteractiveNPCData[questMainKey].Length; i++)
                            {
                                NPC interactiveNPC = npc.InteractiveNPCData[questMainKey][i].GetComponent<NPC>();
                                interactiveNPC.SetNPCQuestState(NPCQuestState.SUCCESS_QUEST);
                                interactiveNPC.InteractiveTalkKey = 1023;

                                // ������ �޾ƾ��� NPC���Ե� ������ �Ҵ�
                                interactiveNPC.RequestNPCObj = npc.gameObject;
                                interactiveNPC.RequestObjs = npc.RequestObjData[1020];
                            }
                        }
                        else
                        {
                            // �κ��丮�� �� á�ٸ�, ��ȭ �Ϸ�ó�� X
                            return;
                        }
                    }
                    else
                    {
                        playerManager.CurrentQuest = questMainKey;
                        npc.SetNPCQuestState(NPCQuestState.PROCESS_QUEST);
                        npc.AddSuccessList();
                        questManager.QuestNPCObj = npc.gameObject;
                    }
                }
                else if (npc.NPCQuestState == NPCQuestState.SUCCESS_QUEST) // ����Ʈ�� �Ϸ��ߴٸ�
                {
                    // ����Ʈ�� ���� �ϵ��ڵ� �κ�
                    if (npc.InteractiveTalkKey == 1023)
                    {
                        NPC interactiveNPC = npc.RequestNPCObj.GetComponent<NPC>();
                        Inventory inven = playerManager.Inventory.GetComponent<Inventory>();

                        // �������� ������ �����ϰ� �� �� �κ��丮�� ��ĭ�� �ִٸ�
                        if (inven.GetRemainedSlots() + npc.RequestObjs.Length > npc.RequestObjData[1020].Length)
                        {
                            // npc���� �������� ������ �����Ѵ�.
                            for (int i = 0; i < npc.RequestObjs.Length; i++)
                            {
                                inven.PassItemToNPC(npc.RequestObjs[i]);
                            }

                            // npc���� ������ �޴´�.
                            for (int i = 0; i < npc.RequestObjData[1020].Length; i++)
                            {
                                inven.AddItem(npc.RequestObjData[1020][i]);
                            }

                            // ������ �޾ƾ��� NPC���Ե� ������ �Ҵ�
                            interactiveNPC.RequestNPCObj = npc.gameObject;
                            interactiveNPC.RequestObjs = npc.RequestObjData[1020];

                            // ������ �ްԵ� NPC���� ����Ʈ �Ϸ�ó��
                            interactiveNPC.npcQuestState = NPCQuestState.SUCCESS_QUEST;

                            // ���� ��ȭ ���� NPC���� ����Ʈ ����ó��
                            npc.InteractiveTalkKey = 0;
                            npc.RequestNPCObj = null;
                            npc.RequestObjs = null;
                            npc.SetNPCQuestState(NPCQuestState.NONE);
                        }
                        else
                        {
                            // �κ��丮�� �� á�ٸ�, ��ȭ �Ϸ�ó�� X
                            SetTalkingEnvironment(false, playerManager);
                            talkIndex = 0;
                            return;
                        }
                    }
                    else
                    {
                        Inventory inven = playerManager.Inventory.GetComponent<Inventory>();

                        // npc���� �������� ������ �ִٸ�
                        if (npc.RequestObjs != null)
                        {
                            // �������� ������ �����ϰ� �� �� �κ��丮�� ��ĭ�� �ִٸ�
                            if (inven.GetRemainedSlots() + npc.RequestObjs.Length > npc.RewardData[questMainKey].Length)
                            {
                                // ������ npc���� �����Ѵ�.
                                for (int i = 0; i < npc.RequestObjs.Length; i++)
                                {
                                    inven.PassItemToNPC(npc.RequestObjs[i]);
                                }

                                // npc���� ������ �޴´�.
                                for (int i = 0; i < npc.RewardData[questMainKey].Length; i++)
                                {
                                    inven.AddItem(npc.RewardData[questMainKey][i]);
                                }

                                // ����Ʈ �Ϸ�ó��
                                npc.RequestObjs = null;
                                playerManager.CurrentQuest = 0;
                                npc.IncreaseCurrentQuest();
                                npc.CheckNextQuest();
                                questManager.QuestNPCObj = null;
                            }
                            else
                            {
                                // �κ��丮�� �� á�ٸ�, ����Ʈ �Ϸ�ó�� X
                                SetTalkingEnvironment(false, playerManager);
                                talkIndex = 0;
                                return;
                            }
                        }
                        else if (npc.RequestObjs == null) // npc���� �������� ������ ���ٸ�
                        {
                            // ������ �ִµ� �����ߴٸ� (�κ��丮 ��ĭ�� �����ִٸ�)
                            if (GiveItem(npc.RewardData[questMainKey]))
                            {
                                // ����Ʈ �Ϸ�ó��
                                npc.RequestObjs = null;
                                playerManager.CurrentQuest = 0;
                                npc.IncreaseCurrentQuest();
                                npc.CheckNextQuest();
                                questManager.QuestNPCObj = null;
                            }
                            else
                            {
                                // �κ��丮�� �� á�ٸ�, ����Ʈ �Ϸ�ó�� X
                                SetTalkingEnvironment(false, playerManager);
                                talkIndex = 0;
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                if (talkIndex == 0)
                {
                    SetTalkingEnvironment(true, playerManager);
                }
                talkText.text = GetTalk(npc, key, talkIndex);
                talkIndex++;
            }
        }
    }

    void SetTalkingEnvironment(bool isTalking, PlayerManager playerManager)
    {
        playerManager.IsTalking = isTalking;
        talkBoxObj.SetActive(isTalking);
        gameUIObj.SetActive(!isTalking);

        if (isTalking)
        {
            // ���� ���̵�, ���� �α׸� ��� �����Ѵ�.
            GameManager.instance.ClearGameLogInTheList();
        }
    }

    string GetTalk(NPC npc, int key, int talkIndex)
    {
        if (talkIndex == npc.TalkData[key].Length)
            return null;
        else
            return npc.TalkData[key][talkIndex];
    }

    bool GiveItem(GameObject[] items)
    {
        int itemCnt = items.Length;
        Inventory inventory = inventoryObj.GetComponent<Inventory>();

        if (itemCnt <= inventory.GetRemainedSlots())
        {
            for (int i = 0; i < itemCnt; i++)
            {
                inventory.AddItem(items[i]);
                GameManager.instance.AddGameLog(items[i].GetComponent<Item>().GetItemName() + "�� ȹ���Ͽ����ϴ�.");
            }

            return true;
        }
        else // �κ��丮 ���� ������ ���ٸ�
        {
            GameManager.instance.AddGameLog("������ ������� Ȯ�����ּ���.");
            return false;
        }
    }
}