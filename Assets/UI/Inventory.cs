using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject item;

    [SerializeField] Transform slotParent;
    [SerializeField] Slot[] slots;
    [SerializeField] Image selectedItemSlot;
    [SerializeField] Text selectedItemDescription;
    [SerializeField] Button useBtn;

    GameObject playerObj;
    GameObject selectedItem;
    List<GameObject> items;

    void Start()
    {
        playerObj = FindObjectOfType<PlayerManager>().gameObject;
        items = playerObj.GetComponent<PlayerManager>().ItemList;
        slots = slotParent.GetComponentsInChildren<Slot>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddItem(item);
        }
    }

    public void FreshSlot()
    {
        int i = 0;
        for (; i < items.Count && i < slots.Length; i++)
        {
            slots[i].Item = item;
            slots[i].GetComponent<Button>().onClick.AddListener(slots[i].SelectItem);
        }
        for (; i < slots.Length; i++)
        {
            slots[i].Item = null;
        }
    }

    public void AddItem(GameObject _item)
    {
        if (items.Count < slots.Length)
        {
            items.Add(_item);
            FreshSlot();
        }
        else
        {
            print("Inventory is full!");
        }
    }

    public void SetSelectedItem(GameObject _itemObj)
    {
        selectedItem = _itemObj;

        IItem item = selectedItem.GetComponent<IItem>();

        selectedItemSlot.sprite = selectedItem.GetComponent<Image>().sprite;
        selectedItemDescription.text = item.GetItemName() + " : " + item.GetItemDescription();
        useBtn.onClick.AddListener(() => {
            item.Use(playerObj);
        });
    }

    public void SetIsShowingInven(bool _isShowing)
    {
        if (_isShowing)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1200);
        }
    }

    public int GetRemainedSlots()
    {
        return slots.Length - items.Count;
    }
}