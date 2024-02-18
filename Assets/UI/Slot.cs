using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] GameObject item;

    public Inventory inventory;
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
                image.color = new Color(1, 1, 1, 1);
            }
            else
            {
                image.color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void SelectItem()
    {
        inventory.SetSelectedItem(item);
    }
}
