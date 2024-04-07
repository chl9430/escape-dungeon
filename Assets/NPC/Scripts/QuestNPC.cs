using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum QuestState
{
    HAVE_QUEST,
    PROCESS_QUEST,
    SUCCESS_QUEST,
    NONE
}

public class QuestNPC : MonoBehaviour
{
    [SerializeField] Sprite haveQuest;
    [SerializeField] Sprite processQuest;
    [SerializeField] Sprite successQuest;
    [SerializeField] Image questMark;
    [SerializeField] protected Text npcName;
    [SerializeField] protected string questNPCName;

    public string QuestNPCName {  get { return questNPCName; } }

    protected QuestState questState;

    public QuestState QuestState { get { return questState; } set { questState = value; } }

    public void SetQuestState(QuestState _state)
    {
        questState = _state;

        Color newColor = questMark.color;

        if (_state == QuestState.NONE)
        {
            questMark.sprite = null;
            newColor.a = 0;
            questMark.color = newColor;
        }
        else
        {
            if (_state == QuestState.HAVE_QUEST)
            {
                questMark.sprite = haveQuest;

            }
            else if (_state == QuestState.PROCESS_QUEST)
            {
                questMark.sprite = processQuest;
            }
            else if (_state == QuestState.SUCCESS_QUEST)
            {
                questMark.sprite = successQuest;
            }

            newColor.a = 1;
            questMark.color = newColor;
        }
    }
}
