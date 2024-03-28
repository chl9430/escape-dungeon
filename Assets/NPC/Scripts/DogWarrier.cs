using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct QuestInfo
{
    List<string> questTalkList;
}

public class DogWarrier : MonoBehaviour
{
    Dictionary<int, QuestInfo> questData;

    void Awake()
    {
    }
}