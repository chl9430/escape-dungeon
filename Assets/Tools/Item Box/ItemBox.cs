using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : Tool
{
    [SerializeField] GameObject[] itemObjs;

    Animator anim;
    Inventory inventory;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    public override void InteractObject()
    {
        if (itemObjs != null)
        {
            if (inventory.CheckInventorySlots(itemObjs.Length))
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
        if (itemObjs != null)
        {
            inventory.AddItems(itemObjs, itemObjs.Length);

            itemObjs = null;
        }
    }
}