using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : Tool
{
    [SerializeField] GameObject[] items;

    Animator anim;
    Inventory inventory;

    List<ItemCountInfo> compressedItemList;

    void Awake()
    {
        anim = GetComponent<Animator>();
        compressedItemList = new List<ItemCountInfo>();
    }

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    public override void InteractObject()
    {
        if (items != null)
        {
            compressedItemList = inventory.MakeCompressedItemCntList(items);

            if (inventory.CheckInventorySlots(compressedItemList.Count))
            {
                anim.SetTrigger("OpenBox");
            }
            else
            {
                GameManager.instance.AddGameLog("�κ��丮�� ������ ������� �ʽ��ϴ�.");
                return;
            }
        }
    }

    // ���ڰ� ������ �ִϸ��̼� �� �κп� ȣ��ȴ�.
    public void GiveItem()
    {
        if (items != null)
        {
            inventory.AddItems(compressedItemList);

            items = null;
        }
    }
}