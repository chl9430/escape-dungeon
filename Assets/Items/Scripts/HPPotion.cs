using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPotion : Item
{
    [SerializeField] int restoreHPValue;

    void Awake()
    {
        itemName = "HP Potion";
        itemDescription = "플레이어의 HP를 " + restoreHPValue + "만큼 회복시킵니다.";
        itemInfo = new ItemInfo
        {
            item = this,
            count = count
        };
    }

    public override void Use(GameObject _playerObj)
    {
        _playerObj.GetComponent<PlayerManager>().RestoreHP(restoreHPValue);
    }
}
