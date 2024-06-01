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
                    "을(를) " +
                    _itemInfo.count +
                    "개 획득하였습니다."
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
                        "을(를) " +
                        _compressedItemList[i].count +
                        "개 획득하였습니다."
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
            GameManager.instance.AddGameLog("인벤토리의 공간이 충분하지 않습니다.");

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

        // 이미 등록된 콜백함수가 있으면 삭제
        useBtn.onClick.RemoveAllListeners();

        // 선택된 아이템을 누르면 호출될 콜백함수
        useBtn.onClick.AddListener(() => {
            UseItem(_selectedItem);

            // 현재 선택된 객체를 null로 설정하여 포커스 해제
            EventSystem.current.SetSelectedGameObject(null);
        });
    }

    void UseItem(InventoryItem _selectedItem)
    {
        Item usedItem = _selectedItem.Item;

        usedItem.Use(playerObj);

        // 사용할 수 있는 아이템만 사용처리한다.
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

            // 인벤토리를 끝때마다 선택된 아이템 초기화
            ResetSelectedItemSlot();
        }
    }

    public int GetRemainedSlots()
    {
        return remainedSlotCnt;
    }
}