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
    int remainedSlotCnt;

    void Start()
    {
        playerObj = FindObjectOfType<PlayerManager>().gameObject;
        slots = slotParent.GetComponentsInChildren<Slot>();
        remainedSlotCnt = slots.Length;
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
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Item == null)
            {
                slots[i].Item = _item;
                GameManager.instance.AddGameLog(_item.GetComponent<Item>().GetItemName() + "을(를) 획득하였습니다.");
                slots[i].GetComponent<Button>().onClick.AddListener(slots[i].SelectItem);
                remainedSlotCnt--;
                return;
            }
        }

        GameManager.instance.AddGameLog("인벤토리의 공간이 충분하지 않습니다.");
    }

    public void SetSelectedItem(GameObject _itemObj, Slot _selectedSlot)
    {
        IItem item = _itemObj.GetComponent<IItem>();

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

        // 사용할 수 있는 아이템만 사용처리한다.
        if (_item.IsUsable())
        {
            _slot.Item = null;
            selectedItemSlot.Item = null;
            selectedItemDescription.text = "";
            remainedSlotCnt++;
        }
    }

    public void PassItemToNPC(GameObject _requestItemObj)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Item == _requestItemObj)
            {
                slots[i].Item = null;
                remainedSlotCnt++;
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
            selectedItemSlot.Item = null;
            selectedItemDescription.text = "";
            useBtn.onClick.RemoveAllListeners();
        }
    }

    public int GetRemainedSlots()
    {
        return remainedSlotCnt;
    }
}