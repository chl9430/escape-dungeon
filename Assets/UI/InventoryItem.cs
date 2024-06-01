using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Item item;
    Inventory inventory;
    Image image;
    Text countText;
    int count;

    public Item Item { get { return item; } }

    void Awake()
    {
        image = GetComponent<Image>();
        countText = GetComponentInChildren<Text>();
    }

    void Start()
    {
        inventory = GetComponentInParent<Inventory>();
    }

    public void UseItem()
    {
        count--;

        if (count == 0)
        {
            image.sprite = null;
            countText.text = "";
            item = null;
        }
        else
        {
            countText.text = count.ToString();
        }
    }

    public void SetItem(ItemInfo _itemInfo)
    {
        item = _itemInfo.item;
        image.sprite = _itemInfo.item.ItemImage;
        count = _itemInfo.count;
        countText.text = count.ToString();
    }

    // 버튼 컴포넌트의 콜백함수
    public void SelectItem()
    {
        inventory.SetSelectedItem(this);

        // 현재 선택된 객체를 null로 설정하여 포커스 해제
        EventSystem.current.SetSelectedGameObject(null);
    }

    public Sprite GetItemImage()
    {
        return image.sprite;
    }

    public int GetItemCount()
    {
        return count;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
