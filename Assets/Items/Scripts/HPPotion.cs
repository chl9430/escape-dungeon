using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPotion : Item
{
    [SerializeField] int restoreHPValue;

    void Awake()
    {
        itemName = "HP Potion";
        itemDescription = "�÷��̾��� HP�� " + restoreHPValue + "��ŭ ȸ����ŵ�ϴ�.";
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
