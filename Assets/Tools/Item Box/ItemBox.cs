using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : Tool
{
    [SerializeField] GameObject[] items;

    Animator anim;
    Inventory inventory;

    List<ItemInfo> compressedItemList;

    void Awake()
    {
        anim = GetComponent<Animator>();
        compressedItemList = new List<ItemInfo>();
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
                GameManager.instance.AddGameLog("인벤토리의 공간이 충분하지 않습니다.");
                return;
            }
        }
    }

    // 상자가 열리는 애니메이션 끝 부분에 호출된다.
    public void GiveItem()
    {
        if (items != null)
        {
            inventory.AddItems(compressedItemList);

            items = null;
        }
    }
}