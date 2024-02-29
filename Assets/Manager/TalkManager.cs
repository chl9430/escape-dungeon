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
            if (talkIndex == npc.TalkData[key].Length) // 대화가 끝났다면
            {
                if (npc.NPCQuestState == NPCQuestState.HAVE_QUEST) // 퀘스트를 받았다면
                {
                    if (questMainKey == 1020)
                    {
                        // 플레이어에게 전달할 물건이 있다면 전달한다.
                        if (GiveItem(npc.RequestObjData[questMainKey]))
                        {
                            playerManager.CurrentQuest = questMainKey;
                            npc.SetNPCQuestState(NPCQuestState.PROCESS_QUEST);
                            npc.AddSuccessList();

                            // 물건을 받아야할 NPC를 퀘스트 완료상태로 변경
                            for (int i = 0; i < npc.InteractiveNPCData[questMainKey].Length; i++)
                            {
                                npc.InteractiveNPCData[questMainKey][i].GetComponent<NPC>().SetNPCQuestState(NPCQuestState.SUCCESS_QUEST);
                            }
                        }
                        else
                        {
                            // 인벤토리가 꽉 찼다면, 대화 완료처리 X
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
                else if (npc.NPCQuestState == NPCQuestState.SUCCESS_QUEST) // 퀘스트를 완료했다면
                {
                    // 아이템을 주거나, 인벤토리의 빈칸을 확인한다. (보상)
                    if (GiveItem(npc.RewardData[npc.ID + ((npc.CurrentQuest + 1) * 10)]))
                    {
                        playerManager.CurrentQuest = 0;
                        npc.IncreaseCurrentQuest();
                        npc.CheckNextQuest();
                        questManager.QuestNPCObj = null;
                    }
                    else
                    {
                        // 인벤토리가 꽉 찼다면, 퀘스트 완료처리 X
                        SetTalkingEnvironment(false, playerManager);
                        talkIndex = 0;
                        return;
                    }
                }

                // 대화 환경을 구축하는 함수
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
        else // 인벤토리 남은 공간이 없다면
        {
            return false;
            
        }
    }
}