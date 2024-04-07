using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DogWarrier : QuestNPC
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