using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public ItemSO testItem1;
    public ItemSO testItem2;


    public GameObject hotBarObj;
    public GameObject inventorySlotParent;
    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> hotBarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private void Awake()
    {
        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        hotBarSlots.AddRange(hotBarObj.GetComponentsInChildren<Slot>());
        allSlots.AddRange(inventorySlots);
        allSlots.AddRange(hotBarSlots);
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            AddItem(testItem1, 1);
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            AddItem(testItem2, 1);
        }
    }

    void AddItem(ItemSO item, int amount = 1)
    {
        foreach (Slot slot in allSlots)
        {
            if (slot.GetItem() == item && slot.GetItemAmount() < item.maxStack)
            {
                int addedAmount = slot.AddAmount(amount);
                amount -= addedAmount;

                if (amount <= 0)
                    return;
            }
        }

        foreach (Slot slot in allSlots)
        {
            if (slot.GetItem() == null)
            {
                slot.SetItem(item, amount);
                return;
            }
        }
    }
}
