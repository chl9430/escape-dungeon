using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IItem
{
    [SerializeField] protected string itemName;
    [SerializeField] protected string itemDescription;
    [SerializeField] protected bool isUsable;

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
}
