using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] Sprite emptyImgSprite;

    Inventory inventory;
    Image image;

    void Start()
    {
        inventory = GetComponentInParent<Inventory>();
        image = GetComponent<Image>();
    }

    public GameObject Item
    {
        get { return item; }
        set
        {
            item = value;
            if (item != null)
            {
                image.sprite = item.GetComponent<Image>().sprite;
            }
            else
            {
                image.sprite = emptyImgSprite;
            }
        }
    }

    public void SelectItem()
    {
        inventory.SetSelectedItem(item, this);
    }
}
