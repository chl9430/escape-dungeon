using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] GameObject item2;

    [SerializeField] Transform slotParent;
    [SerializeField] Slot[] slots;
    [SerializeField] Slot selectedItemSlot;
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

        if (Input.GetKeyDown(KeyCode.L))
        {
            AddItem(item2);
        }
    }

    public void AddItem(GameObject _item)
    {
        if (items.Count < slots.Length)
        {
            items.Add(_item);

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Item == null)
                {
                    slots[i].Item = _item;
                    slots[i].GetComponent<Button>().onClick.AddListener(slots[i].SelectItem);
                    return;
                }
            }
        }
        else
        {
            print("Inventory is full!");
        }
    }

    public void SetSelectedItem(GameObject _itemObj, Slot _selectedSlot)
    {
        selectedItem = _itemObj;

        IItem item = selectedItem.GetComponent<IItem>();

        selectedItemSlot.Item = _itemObj;
        selectedItemDescription.text = item.GetItemName() + " : " + item.GetItemDescription();

        // 이미 등록된 콜백함수가 있으면 삭제
        useBtn.onClick.RemoveAllListeners();

        // 선택된 아이템을 누르면 호출될 콜백함수
        useBtn.onClick.AddListener(() => {
            UseItem(item, _selectedSlot);
            useBtn.onClick.RemoveAllListeners();
        });
    }

    void UseItem(IItem _item, Slot _slot)
    {
        _item.Use(playerObj);

        // 사용할 수 없는 아이템이라면
        if (_item.IsUsable())
        {
            _slot.Item = null;
            selectedItemSlot.Item = null;
            selectedItemDescription.text = "";
            items.Remove(selectedItem);
        }
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