using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hovering;
    private ItemSO heldItem;
    private int itemAmount;

    private Image itemIcon;
    private TextMeshProUGUI itemAmountText;
    
    public void Awake()
    {
        itemIcon = transform.GetChild(0).GetComponent<Image>();
        itemAmountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public ItemSO GetItem()
    {
        return heldItem;
    }

    public int GetItemAmount()
    {
        return itemAmount;
    }

    public void SetItem(ItemSO item, int amount = 1)
    {
        heldItem = item;
        itemAmount = amount;

        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (itemIcon == null)
        {
            itemIcon = transform.GetChild(0).GetComponent<Image>();
            itemAmountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
        
        if (heldItem != null)
        {
            itemIcon.sprite = heldItem.itemIcon;
            itemIcon.enabled = true;
            itemAmountText.enabled = true;
            itemAmountText.text = itemAmount.ToString();
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            itemAmountText.enabled = false;
        }
    }

    public int AddAmount(int amount)
    {
        if (heldItem == null)
            return 0;

        int spaceLeft = heldItem.maxStack - itemAmount;
        int amountToAdd = Mathf.Min(spaceLeft, amount);
        itemAmount += amountToAdd;

        UpdateSlot();
        return amountToAdd;
    }

    public int RemoveAmount(int amount)
    {
        if (heldItem == null)
            return 0;

        int amountToRemove = Mathf.Min(itemAmount, amount);
        itemAmount -= amountToRemove;

        if (itemAmount <= 0)
        {
            ClearSlot();
        }

        UpdateSlot();
        return amountToRemove;
    }

    public void ClearSlot()
    {
        heldItem = null;
        itemAmount = 0;
        UpdateSlot();
    }

    public bool HasItem()
    {
        return heldItem != null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
