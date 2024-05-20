using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] Item item;

    Inventory inventory;
    Sprite emptyImgSprite;
    Image image;
    Text countText;

    int itemCnt;

    public int ItemCount { get { return itemCnt; } }

    void Start()
    {
        inventory = GetComponentInParent<Inventory>();
        image = GetComponent<Image>();
        emptyImgSprite = image.sprite;
        countText = GetComponentInChildren<Text>();
    }

    public Item Item { get { return item; } }

    public void SetItem(ItemCountInfo _itemInfo)
    {
        item = _itemInfo.item;
        image.sprite = item.GetComponent<Image>().sprite;
        itemCnt = _itemInfo.count;
        countText.text = itemCnt.ToString();
    }

    public void UseItem()
    {
        itemCnt--;

        if (itemCnt == 0)
        {
            Destroy(item);
            image.sprite = emptyImgSprite;
            countText.text = "";
            item = null;
        }
        else
        {
            countText.text = itemCnt.ToString();
        }
    }

    public void SelectItem()
    {
        inventory.SetSelectedItem(this);

        // 현재 선택된 객체를 null로 설정하여 포커스 해제
        EventSystem.current.SetSelectedGameObject(null);
    }
}
