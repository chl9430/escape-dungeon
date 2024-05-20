using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct ItemCountInfo
{
    public Item item;
    public int count;
}

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject[] items;

    [SerializeField] Transform slotParent;
    [SerializeField] Slot[] slots;
    [SerializeField] Image selectedItemSlot;
    [SerializeField] Text selectedItemDescription;
    [SerializeField] Button useBtn;

    GameObject playerObj;
    Sprite emptyImageSprite;
    int remainedSlotCnt;

    void Start()
    {
        playerObj = FindObjectOfType<PlayerManager>().gameObject;
        slots = slotParent.GetComponentsInChildren<Slot>();
        remainedSlotCnt = slots.Length;
        emptyImageSprite = selectedItemSlot.sprite;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            List<ItemCountInfo> compressedItemList = MakeCompressedItemCntList(items);

            if (CheckInventorySlots(compressedItemList.Count))
            {
                AddItems(compressedItemList);
            }
        }
    }

    void ResetSelectedItemSlot()
    {
        selectedItemSlot.sprite = emptyImageSprite;
        selectedItemDescription.text = "";
        useBtn.onClick.RemoveAllListeners();
    }

    public List<ItemCountInfo> MakeCompressedItemCntList(GameObject[] _itemObjs)
    {
        List<ItemCountInfo> compressedItemCntList = new();

        for (int i = 0; i < _itemObjs.Length; i++)
        {
            Item item = _itemObjs[i].GetComponent<Item>();
            int existingItemIdx = compressedItemCntList.FindIndex(itemInfo => itemInfo.item.GetItemName() == item.GetItemName());

            ItemCountInfo newItemInfo;

            if (existingItemIdx == -1 || compressedItemCntList[existingItemIdx].count == 50)
            {
                newItemInfo.item = Instantiate(_itemObjs[i]).GetComponent<Item>();
                newItemInfo.count = 1;

                compressedItemCntList.Add(newItemInfo);
            }
            else
            {
                newItemInfo = compressedItemCntList[existingItemIdx];
                newItemInfo.count++;

                compressedItemCntList[existingItemIdx] = newItemInfo;
            }
        }

        return compressedItemCntList;
    }

    public bool AddItems(List<ItemCountInfo> _compressedItemList)
    {
        for (int i = 0; i < _compressedItemList.Count; i++)
        {
            for (int j = 0; j < slots.Length; j++)
            {
                if (slots[j].Item == null)
                {
                    slots[j].SetItem(_compressedItemList[i]);
                    GameManager.instance.AddGameLog(
                        _compressedItemList[i].item.GetItemName() +
                        "��(��) " +
                        _compressedItemList[i].count +
                        "�� ȹ���Ͽ����ϴ�."
                    );
                    slots[j].GetComponent<Button>().onClick.AddListener(slots[j].SelectItem);
                    remainedSlotCnt--;
                    break;
                }
            }
        }

        return true;
    }

    public bool CheckInventorySlots(int _requireSlots)
    {
        if (GetRemainedSlots() < _requireSlots)
        {
            GameManager.instance.AddGameLog("�κ��丮�� ������ ������� �ʽ��ϴ�.");

            return false;
        }
        else
        {
            return true;
        }
    }

    public void SetSelectedItem(Slot _selectedSlot)
    {
        Item selectedItem = _selectedSlot.Item;

        selectedItemSlot.sprite = _selectedSlot.GetComponent<Image>().sprite;
        selectedItemDescription.text = selectedItem.GetItemName() + " : " + selectedItem.GetItemDescription();

        // �̹� ��ϵ� �ݹ��Լ��� ������ ����
        useBtn.onClick.RemoveAllListeners();

        // ���õ� �������� ������ ȣ��� �ݹ��Լ�
        useBtn.onClick.AddListener(() => {
            UseItem(_selectedSlot);

            // ���� ���õ� ��ü�� null�� �����Ͽ� ��Ŀ�� ����
            EventSystem.current.SetSelectedGameObject(null);
        });
    }

    void UseItem(Slot _slot)
    {
        Item usedItem = _slot.Item;

        usedItem.Use(playerObj);

        // ����� �� �ִ� �����۸� ���ó���Ѵ�.
        if (usedItem.IsUsable())
        {
            _slot.UseItem();

            if (_slot.ItemCount == 0)
            {
                ResetSelectedItemSlot();
                remainedSlotCnt++;
            }
        }
    }

    public void PassItemToNPC(GameObject _requestItemObj)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            string itemName = slots[i].Item.GetComponent<Item>().GetItemName();
            if (itemName == _requestItemObj.GetComponent<Item>().GetItemName())
            {
                slots[i].UseItem();
                remainedSlotCnt++;
                return;
            }
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

            // �κ��丮�� �������� ���õ� ������ �ʱ�ȭ
            ResetSelectedItemSlot();
        }
    }

    public int GetRemainedSlots()
    {
        return remainedSlotCnt;
    }
}