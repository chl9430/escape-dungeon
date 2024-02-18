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

    private void OnValidate()
    {
        slots = slotParent.GetComponentsInChildren<Slot>();
    }

    void OnEnable()
    {
        playerObj = FindObjectOfType<PlayerManager>().gameObject;
        items = playerObj.GetComponent<PlayerManager>().ItemList;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddItem(item);
        }
    }

    void OnDisable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
            print("full");
        }
    }

    public void SetSelectedItem(GameObject itemObj)
    {
        selectedItem = itemObj;

        IItem item = selectedItem.GetComponent<IItem>();

        selectedItemSlot.sprite = selectedItem.GetComponent<Image>().sprite;
        selectedItemDescription.text = item.GetItemName() + " : " + item.GetItemDescription();
        useBtn.onClick.AddListener(() => {
            item.Use(playerObj);
        });
    }
}