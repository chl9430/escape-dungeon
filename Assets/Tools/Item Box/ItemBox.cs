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
                GameManager.instance.AddGameLog("�κ��丮�� ������ ������� �ʽ��ϴ�.");
                return;
            }
        }
    }

    // ���ڰ� ������ �ִϸ��̼� �� �κп� ȣ��ȴ�.
    public void GiveItem()
    {
        if (itemObjs != null)
        {
            inventory.AddItems(itemObjs, itemObjs.Length);

            itemObjs = null;
        }
    }
}