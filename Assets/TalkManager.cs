using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
    [SerializeField] GameObject talkBox;

    public static TalkManager instance;

    PlayerManager player;
    int talkIndex;
    public bool isTalking = false;

    void Start()
    {
        instance = this;
        player = FindObjectOfType<PlayerManager>();
    }

    public void Talk(GameObject npcObj)
    {
        NPC npc = npcObj.GetComponent<NPC>();
        Text talkText = talkBox.GetComponentInChildren<Text>();

        int key = npc.ID + (int)npc.NPCQuestState + (10 * (npc.CurrentQuest + 1));

        if (talkIndex == npc.TalkData[key].Length)
        {
            isTalking = false;
            talkBox.SetActive(false);
            talkIndex = 0;

            if (npc.NPCQuestState == NPCQuestState.HAVE_QUEST)
            {
                player.SetCurrentQuest(npc.ID + (10 * (npc.CurrentQuest + 1)));
                npc.SetNPCQuestState(NPCQuestState.PROCESS_QUEST);
                npc.AddSuccessList();
            }
            else if (npc.NPCQuestState == NPCQuestState.SUCCESS_QUEST)
            {
                // currentQuestText.text = "";
                player.currentQuest = 0;
                npc.IncreaseCurrentQuest();
                npc.SetNPCQuestState(NPCQuestState.HAVE_QUEST);
            }
        }
        else
        {
            isTalking = true;
            talkBox.SetActive(true);
            talkText.text = GetTalk(npc, key, talkIndex);
            talkIndex++;
        }
    }

    string GetTalk(NPC npc, int key, int talkIndex)
    {
        if (talkIndex == npc.TalkData[key].Length)
            return null;
        else
            return npc.TalkData[key][talkIndex];
    }
}