using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wizard : QuestNPC
{
    void Awake()
    {
        questMark = GetComponentInChildren<Image>();
        questState = QuestState.NONE;
    }
}
