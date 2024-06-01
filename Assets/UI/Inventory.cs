using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct ItemInfo
{
    public Item item;
    public int count;
}

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject[] items;

    [SerializeField] GameObject itemOmgObj;
    [SerializeField] Transform slotParent;
    [SerializeField] List<GameObject> slotObjs;
    [SerializeField] Image selectedItemSlot;
    [SerializeField] Text selectedItemDescription;
    [SerializeField] Button useBtn;

    GameObject playerObj;
    int remainedSlotCnt;

    void Start()
    {
        playerObj = FindObjectOfType<PlayerManager>().gameObject;
        for (int i = 0; i < slotParent.transform.childCount; i++)
        {
            GameObject child = slotParent.GetChild(i).gameObject;

            slotObjs.Add(child);
        }
        remainedSlotCnt = slotObjs.Count;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            List<ItemInfo> compressedItemList = MakeCompressedItemCntList(items);

            if (CheckInventorySlots(compressedItemList.Count))
            {
                AddItems(compressedItemList);
            }
        }
    }

    void ResetSelectedItemSlot()
    {
        selectedItemSlot.sprite = null;

        Color color = selectedItemSlot.color;
        color.a = 0f;
        selectedItemSlot.color = color;

        selectedItemDescription.text = "";
        useBtn.onClick.RemoveAllListeners();
    }

    public List<ItemInfo> MakeCompressedItemCntList(GameObject[] _itemObjs)
    {
        List<ItemInfo> compressedItemCntList = new();

        for (int i = 0; i < _itemObjs.Length; i++)
        {
            Item item = _itemObjs[i].GetComponent<Item>();
            int existingItemIdx = compressedItemCntList.FindIndex(itemInfo => itemInfo.item.GetItemName() == item.GetItemName());

            ItemInfo newItemInfo;

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

    public void AddItem(ItemInfo _itemInfo)
    {
        for (int i = 0; i < slotObjs.Count; i++)
        {
            if (slotObjs[i].transform.childCount == 0)
            {
                InventoryItem instIvenItem = Instantiate(itemOmgObj).GetComponent<InventoryItem>();
                instIvenItem.transform.SetParent(slotObjs[i].transform, false);

                instIvenItem.SetItem(_itemInfo);

                GameManager.instance.AddGameLog(
                    _itemInfo.item.GetItemName() +
                    "��(��) " +
                    _itemInfo.count +
                    "�� ȹ���Ͽ����ϴ�."
                );

                remainedSlotCnt--;
                break;
            }
        }
    }

    public bool AddItems(List<ItemInfo> _compressedItemList)
    {
        for (int i = 0; i < _compressedItemList.Count; i++)
        {
            for (int j = 0; j < slotObjs.Count; j++)
            {
                if (slotObjs[j].transform.childCount == 0)
                {
                    InventoryItem instIvenItem = Instantiate(itemOmgObj).GetComponent<InventoryItem>();
                    instIvenItem.transform.SetParent(slotObjs[j].transform, false);

                    instIvenItem.SetItem(_compressedItemList[i]);

                    GameManager.instance.AddGameLog(
                        _compressedItemList[i].item.GetItemName() +
                        "��(��) " +
                        _compressedItemList[i].count +
                        "�� ȹ���Ͽ����ϴ�."
                    );

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

    public void SetSelectedItem(InventoryItem _selectedItem)
    {
        selectedItemSlot.sprite = _selectedItem.GetItemImage();

        Color selectedItemSlotColor = selectedItemSlot.color;
        selectedItemSlotColor.a = 255;
        selectedItemSlot.color = selectedItemSlotColor;

        selectedItemDescription.text = _selectedItem.Item.GetItemName() + " : " + _selectedItem.Item.GetItemDescription();

        // �̹� ��ϵ� �ݹ��Լ��� ������ ����
        useBtn.onClick.RemoveAllListeners();

        // ���õ� �������� ������ ȣ��� �ݹ��Լ�
        useBtn.onClick.AddListener(() => {
            UseItem(_selectedItem);

            // ���� ���õ� ��ü�� null�� �����Ͽ� ��Ŀ�� ����
            EventSystem.current.SetSelectedGameObject(null);
        });
    }

    void UseItem(InventoryItem _selectedItem)
    {
        Item usedItem = _selectedItem.Item;

        usedItem.Use(playerObj);

        // ����� �� �ִ� �����۸� ���ó���Ѵ�.
        if (usedItem.IsUsable())
        {
            _selectedItem.UseItem();

            if (_selectedItem.GetItemCount() == 0)
            {
                Destroy(_selectedItem.gameObject);
                ResetSelectedItemSlot();
                remainedSlotCnt++;
            }
        }
    }

    public void PassItemToNPC(GameObject _requestItemObj)
    {
        for (int i = 0; i < slotObjs.Count; i++)
        {
            if (slotObjs[i].transform.childCount != 0)
            {
                InventoryItem invenItem = slotObjs[i].transform.GetChild(0).GetComponent<InventoryItem>();
                string itemName = invenItem.Item.GetComponent<Item>().GetItemName();

                if (itemName == _requestItemObj.GetComponent<Item>().GetItemName())
                {
                    invenItem.UseItem();
                    remainedSlotCnt++;
                    return;
                }
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