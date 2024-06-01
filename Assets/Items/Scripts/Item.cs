using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IItem
{
    [SerializeField] protected string itemName;
    [SerializeField] protected string itemDescription;
    [SerializeField] protected bool isUsable;
    [SerializeField] protected Sprite itemImg;

    protected ItemInfo itemInfo;
    protected int count = 1;
    public Sprite ItemImage { get { return itemImg; } }

    public virtual void Use(GameObject target)
    {
        Debug.Log(itemName);
        Debug.Log(itemDescription);
    }

    public string GetItemName()
    {
        return itemName;
    }

    public string GetItemDescription()
    {
        return itemDescription;
    }

    public bool IsUsable()
    {
        return isUsable;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObj = collision.gameObject;

        if (collisionObj.CompareTag("Player"))
        {
            Inventory inventory = collisionObj.GetComponent<PlayerManager>().Inventory;

            inventory.AddItem(itemInfo);
        }
    }
}