using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] GameObject itemObj;

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


    public void InteractObject()
    {
        if (itemObj != null)
        {
            anim.SetTrigger("OpenBox");
        }
    }

    // ���ڰ� ������ �ִϸ��̼� �� �κп� ȣ��ȴ�.
    public void GiveItem()
    {
        if (itemObj != null)
        {
            inventory.AddItem(itemObj);

            itemObj = null;
        }
    }
}
