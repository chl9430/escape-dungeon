using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Bullet : MonoBehaviour, IItem
{
    [SerializeField] string itemName = "Max Bullet";
    [SerializeField] string itemDescription = "�Ѿ��� �ִ� �������� 10�� ������ŵ�ϴ�.";

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
