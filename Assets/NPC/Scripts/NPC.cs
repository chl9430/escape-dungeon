using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] Sprite haveQuest;
    [SerializeField] Sprite processQuest;
    [SerializeField] Sprite successQuest;

    int currentQuest = 0;
    int interactiveTalkKey;

    public NPCQuestState npcQuestState;

    Dictionary<int, string[]> talkData;
    Dictionary<int, GameObject[]> requestObjData;
    Dictionary<int, GameObject[]> interactiveNPCData;
    Dictionary<int, GameObject[]> rewardData;

    GameObject requestNPCObj;
    GameObject[] requestObjs;

    List<bool> successList;
    Image questMark;

    public int ID { get { return id; } }
    public int CurrentQuest { get { return currentQuest; } }
    public int InteractiveTalkKey { get { return interactiveTalkKey; } set { interactiveTalkKey = value; } }
    public NPCQuestState NPCQuestState { get { return npcQuestState; } }
    public Dictionary<int, string[]> TalkData { get { return talkData; } }
    public Dictionary<int, GameObject[]> RewardData {  get {  return rewardData; } }
    public Dictionary<int, GameObject[]> RequestObjData { get { return requestObjData; } }
    public Dictionary<int, GameObject[]> InteractiveNPCData { get { return interactiveNPCData; } }
    public GameObject RequestNPCObj { get { return requestNPCObj; } set { requestNPCObj = value; } }
    public GameObject[] RequestObjs { get { return requestObjs; } set { requestObjs = value; } }

    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        requestObjData = new Dictionary<int, GameObject[]>();
        interactiveNPCData = new Dictionary<int, GameObject[]>();
        rewardData = new Dictionary<int, GameObject[]>();
        successList = new List<bool>();
        questMark = GetComponentInChildren<Image>();
    }

    public void AddTalkData(int _key, string[] _talks)
    {
        talkData.Add(_key, _talks);
    }

    public void AddInteractiveNPCData(int _key, GameObject[] _npcObjs)
    {
        interactiveNPCData.Add(_key, _npcObjs);
    }

    public void AddRewardData(int _key, GameObject[] _rewardObjs)
    {
        rewardData.Add(_key, _rewardObjs);
    }

    public void AddRequestObjData(int _key, GameObject[] _requestObjs)
    {
        requestObjData.Add(_key, _requestObjs);
    }

    protected void UpdateQuestMark()
    {
        Color newColor = questMark.color;

        if (npcQuestState == NPCQuestState.NONE)
        {
            questMark.sprite = null;
            newColor.a = 0;
            questMark.color = newColor;
        }
        else
        {
            if (npcQuestState == NPCQuestState.HAVE_QUEST)
            {
                questMark.sprite = haveQuest;

            }
            else if (npcQuestState == NPCQuestState.PROCESS_QUEST)
            {
                questMark.sprite = processQuest;
            }
            else if (npcQuestState == NPCQuestState.SUCCESS_QUEST)
            {
                questMark.sprite = successQuest;
            }

            newColor.a = 1;
            questMark.color = newColor;
        }
    }

    public void SetSuccessArr()
    {
        successList[currentQuest] = true;

        if (npcQuestState == NPCQuestState.PROCESS_QUEST)
        {
            npcQuestState = NPCQuestState.SUCCESS_QUEST;
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

    public void SetNPCQuestState(NPCQuestState _state)
    {
        npcQuestState = _state;
    }

    public void CheckNextQuest()
    {
        if (talkData.ContainsKey(id + ((currentQuest + 1) * 10)))
        {
            npcQuestState = NPCQuestState.HAVE_QUEST;
        }
        else
        {
            npcQuestState = NPCQuestState.NONE;
        }
    }
}
