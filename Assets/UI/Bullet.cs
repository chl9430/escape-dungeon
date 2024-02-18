using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Bullet : MonoBehaviour, IItem
{
    [SerializeField] string itemName = "Bullet";
    [SerializeField] string itemDescription = "ÃÑ¾ËÀÇ °¹¼ö¸¦ 10°³ Áõ°¡½ÃÅµ´Ï´Ù.";

    public void Use(GameObject target)
    {
        Debug.Log(gameObject.name);
        Debug.Log(target);
    }

    public string GetItemName()
    {
        return itemName;
    }

    public string GetItemDescription()
    {
        return itemDescription;
    }
}
