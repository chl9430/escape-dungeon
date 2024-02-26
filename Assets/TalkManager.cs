using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
    [SerializeField] GameObject talkBoxObj;
    [SerializeField] GameObject gameUIObj;

    public static TalkManager instance;

    PlayerManager playerManager;
    GameObject inventoryObj;
    int talkIndex;

    void Start()
    {
        instance = this;
        playerManager = FindObjectOfType<PlayerManager>();

        if (gameUIObj != null)
        {
            inventoryObj = gameUIObj.GetComponentInChildren<Inventory>(true).gameObject;
        }
    }

    public void Talk(GameObject npcObj)
    {
        NPC npc = npcObj.GetComponent<NPC>();
        Text talkText = talkBoxObj.GetComponentInChildren<Text>();

        int key = npc.ID + (int)npc.NPCQuestState + (10 * (npc.CurrentQuest + 1));

        if (!npc.TalkData.ContainsKey(key))
        {
            return;
        }
        else
        {
            if (talkIndex == npc.TalkData[key].Length) // 대화가 끝났다면
            {
                if (npc.NPCQuestState == NPCQuestState.HAVE_QUEST)
                {
                    playerManager.CurrentQuest = npc.ID + (10 * (npc.CurrentQuest + 1));
                    npc.SetNPCQuestState(NPCQuestState.PROCESS_QUEST);
                    npc.AddSuccessList();
                }
                else if (npc.NPCQuestState == NPCQuestState.SUCCESS_QUEST)
                {
                    // 아이템을 주거나, 인벤토리의 빈칸을 확인한다.
                    int rewardCnt = npc.RewardData[npc.ID + (10 * (npc.CurrentQuest + 1))].Length;
                    Inventory inventory = inventoryObj.GetComponent<Inventory>();

                    if (rewardCnt <= inventory.GetRemainedSlots())
                    {
                        for (int i = 0; i < rewardCnt; i++)
                        {
                            inventory.AddItem(npc.RewardData[npc.ID + (10 * (npc.CurrentQuest + 1))][i]);
                        }
                    }
                    else
                    {
                        // 인벤토리가 꽉 찼다면, 퀘스트 완료처리 X
                        SetTalkingEnvironment(false);
                        talkIndex = 0;
                        return;
                    }

                    playerManager.CurrentQuest = 0;
                    npc.IncreaseCurrentQuest();
                    npc.CheckNextQuest();
                }

                // 대화 환경을 구축하는 함수
                SetTalkingEnvironment(false);
                talkIndex = 0;
            }
            else
            {
                SetTalkingEnvironment(true);
                talkText.text = GetTalk(npc, key, talkIndex);
                talkIndex++;
            }
        }
    }

    void SetTalkingEnvironment(bool isTalking)
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
}