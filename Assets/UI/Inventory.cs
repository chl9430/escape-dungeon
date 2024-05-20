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
                        "을(를) " +
                        _compressedItemList[i].count +
                        "개 획득하였습니다."
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
            GameManager.instance.AddGameLog("인벤토리의 공간이 충분하지 않습니다.");

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

        // 이미 등록된 콜백함수가 있으면 삭제
        useBtn.onClick.RemoveAllListeners();

        // 선택된 아이템을 누르면 호출될 콜백함수
        useBtn.onClick.AddListener(() => {
            UseItem(_selectedSlot);

            // 현재 선택된 객체를 null로 설정하여 포커스 해제
            EventSystem.current.SetSelectedGameObject(null);
        });
    }

    void UseItem(Slot _slot)
    {
        Item usedItem = _slot.Item;

        usedItem.Use(playerObj);

        // 사용할 수 있는 아이템만 사용처리한다.
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

            // 인벤토리를 끝때마다 선택된 아이템 초기화
            ResetSelectedItemSlot();
        }
    }

    public int GetRemainedSlots()
    {
        return remainedSlotCnt;
    }
}