using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCQuestState
{
    HAVE_QUEST,
    PROCESS_QUEST,
    SUCCESS_QUEST,
    NONE
}

public class NPC : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] int currentQuest = 0;
    [SerializeField] List<bool> successList;
    public NPCQuestState npcQuestState;

    public int ID {  get { return id; } }
    public int CurrentQuest { get { return currentQuest; } }
    public NPCQuestState NPCQuestState { get { return npcQuestState; } }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SetSuccessArr();
        }
    }

    public void IncreaseCurrentQuest()
    {
        currentQuest++;
    }

    public void AddSuccessList()
    {
        successList.Add(false);
    }

    public void SetSuccessArr()
    {
        successList[currentQuest] = true;

        if (npcQuestState == NPCQuestState.PROCESS_QUEST)
        {
            npcQuestState = NPCQuestState.SUCCESS_QUEST;
        }
    }

    public void SetNPCQuestState(NPCQuestState state)
    {
        npcQuestState = state;
    }
}
