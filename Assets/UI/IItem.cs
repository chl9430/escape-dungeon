using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu]
//public class Item : ScriptableObject
//{
//    public string itemName;
//    public Sprite itemImage;
//}

// ������ Ÿ�Ե��� �ݵ�� �����ؾ��ϴ� �������̽�
public interface IItem
{
    // �Է����� �޴� target�� ������ ȿ���� ����� ���
    void Use(GameObject target);

    string GetItemName();
    string GetItemDescription();
}