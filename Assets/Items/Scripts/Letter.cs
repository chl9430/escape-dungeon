using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : Item
{
    public override void Use(GameObject target)
    {
        GameManager.instance.AddGameLog("����� �� ���� �������Դϴ�.");
    }
}
