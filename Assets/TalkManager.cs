using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
    public static TalkManager instance;

    [SerializeField] GameObject talkBoxObj;
    [SerializeField] GameObject gameUIObj;

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

        if (!npc.TalkData.ContainsKey(key))
        {
            return;
        }
        else
        {
            if (talkIndex == npc.TalkData[key].Length) // ��ȭ�� �����ٸ�
            {
                if (npc.NPCQuestState == NPCQuestState.HAVE_QUEST) // ����Ʈ�� �޾Ҵٸ�
                {
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
                                npc.InteractiveNPCData[questMainKey][i].GetComponent<NPC>().SetNPCQuestState(NPCQuestState.SUCCESS_QUEST);
                            }
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
                        playerManager.CurrentQuest = questMainKey;
                        npc.SetNPCQuestState(NPCQuestState.PROCESS_QUEST);
                        npc.AddSuccessList();
                        questManager.QuestNPCObj = npc.gameObject;
                    }
                }
                else if (npc.NPCQuestState == NPCQuestState.SUCCESS_QUEST) // ����Ʈ�� �Ϸ��ߴٸ�
                {
                    // �������� �ְų�, �κ��丮�� ��ĭ�� Ȯ���Ѵ�. (����)
                    if (GiveItem(npc.RewardData[npc.ID + ((npc.CurrentQuest + 1) * 10)]))
                    {
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

                // ��ȭ ȯ���� �����ϴ� �Լ�
                SetTalkingEnvironment(false, playerManager);
                talkIndex = 0;
            }
            else
            {
                SetTalkingEnvironment(true, playerManager);
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
            }

            return true;
        }
        else // �κ��丮 ���� ������ ���ٸ�
        {
            return false;
            
        }
    }
}