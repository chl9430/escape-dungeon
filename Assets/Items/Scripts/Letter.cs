using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : Item
{
    public override void Use(GameObject target)
    {
        Debug.Log("사용할 수 없는 아이템입니다.");
    }
}
