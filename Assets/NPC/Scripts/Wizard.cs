using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wizard : QuestNPC
{
    void Awake()
    {
        questState = QuestState.NONE;

        if (npcName != null)
        {
            npcName.text = questNPCName;
        }
    }
}
