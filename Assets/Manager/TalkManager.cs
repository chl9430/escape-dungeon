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

            // 퀘스트에 따른 하드코딩 부분
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
            if (talkIndex == npc.TalkData[key].Length) // 대화가 끝났다면
            {
                // 대화 환경을 구축하는 함수
                SetTalkingEnvironment(false, playerManager);
                talkIndex = 0;

                if (npc.NPCQuestState == NPCQuestState.HAVE_QUEST) // 퀘스트를 받았다면
                {
                    // 퀘스트에 따른 하드코딩 부분
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
                                NPC interactiveNPC = npc.InteractiveNPCData[questMainKey][i].GetComponent<NPC>();
                                interactiveNPC.SetNPCQuestState(NPCQuestState.SUCCESS_QUEST);
                                interactiveNPC.InteractiveTalkKey = 1023;

                                // 물건을 받아야할 NPC에게도 참조를 할당
                                interactiveNPC.RequestNPCObj = npc.gameObject;
                                interactiveNPC.RequestObjs = npc.RequestObjData[1020];
                            }
                        }
                        else
                        {
                            // 인벤토리가 꽉 찼다면, 대화 완료처리 X
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
                    // 퀘스트에 따른 하드코딩 부분
                    if (npc.InteractiveTalkKey == 1023)
                    {
                        NPC interactiveNPC = npc.RequestNPCObj.GetComponent<NPC>();
                        Inventory inven = playerManager.Inventory.GetComponent<Inventory>();

                        // 전달해줄 물건을 전달하고 난 후 인벤토리에 빈칸이 있다면
                        if (inven.GetRemainedSlots() + npc.RequestObjs.Length > npc.RequestObjData[1020].Length)
                        {
                            // npc에게 전달해줄 물건을 전달한다.
                            for (int i = 0; i < npc.RequestObjs.Length; i++)
                            {
                                inven.PassItemToNPC(npc.RequestObjs[i]);
                            }

                            // npc에게 물건을 받는다.
                            for (int i = 0; i < npc.RequestObjData[1020].Length; i++)
                            {
                                inven.AddItem(npc.RequestObjData[1020][i]);
                            }

                            // 물건을 받아야할 NPC에게도 참조를 할당
                            interactiveNPC.RequestNPCObj = npc.gameObject;
                            interactiveNPC.RequestObjs = npc.RequestObjData[1020];

                            // 물건을 받게될 NPC에게 퀘스트 완료처리
                            interactiveNPC.npcQuestState = NPCQuestState.SUCCESS_QUEST;

                            // 현재 대화 중인 NPC에게 퀘스트 없음처리
                            npc.InteractiveTalkKey = 0;
                            npc.RequestNPCObj = null;
                            npc.RequestObjs = null;
                            npc.SetNPCQuestState(NPCQuestState.NONE);
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
                        Inventory inven = playerManager.Inventory.GetComponent<Inventory>();

                        // npc에게 전달해줄 물건이 있다면
                        if (npc.RequestObjs != null)
                        {
                            // 전달해줄 물건을 전달하고 난 후 인벤토리에 빈칸이 있다면
                            if (inven.GetRemainedSlots() + npc.RequestObjs.Length > npc.RewardData[questMainKey].Length)
                            {
                                // 물건을 npc에게 전달한다.
                                for (int i = 0; i < npc.RequestObjs.Length; i++)
                                {
                                    inven.PassItemToNPC(npc.RequestObjs[i]);
                                }

                                // npc에게 보상을 받는다.
                                for (int i = 0; i < npc.RewardData[questMainKey].Length; i++)
                                {
                                    inven.AddItem(npc.RewardData[questMainKey][i]);
                                }

                                // 퀘스트 완료처리
                                npc.RequestObjs = null;
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
                        else if (npc.RequestObjs == null) // npc에게 전달해줄 물건이 없다면
                        {
                            // 보상을 주는데 성공했다면 (인벤토리 빈칸이 남아있다면)
                            if (GiveItem(npc.RewardData[questMainKey]))
                            {
                                // 퀘스트 완료처리
                                npc.RequestObjs = null;
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
            // 게임 가이드, 게임 로그를 모두 삭제한다.
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
                GameManager.instance.AddGameLog(items[i].GetComponent<Item>().GetItemName() + "을 획득하였습니다.");
            }

            return true;
        }
        else // 인벤토리 남은 공간이 없다면
        {
            GameManager.instance.AddGameLog("가방의 빈공간을 확보해주세요.");
            return false;
        }
    }
}