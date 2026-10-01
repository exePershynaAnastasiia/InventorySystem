using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public ItemSO testItem1;
    public ItemSO testItem2;

    public Image dragIcon;


    public GameObject hotBarObj;
    public GameObject inventorySlotParent;
    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> hotBarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private Slot draggedSlot = null;
    private bool isDragging = false;

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

        StartDrag();
        UpdateDragIconPosition();
        EndDrag();
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

    private void StartDrag()
    {
        if (Mouse.current.leftButton.isPressed && !isDragging)
        {
            Slot hovered = GetHoveredSlot();
            Debug.Log("Hovered Slot: " + hovered);
            if (hovered != null && hovered.HasItem())
            {
                draggedSlot = hovered;
                isDragging = true;
                dragIcon.sprite = hovered.GetItem().itemIcon;
                dragIcon.color = new Color(1, 1, 1, 0.5f);
                dragIcon.enabled = true;
            }
        }
    }

    private void EndDrag()
    {
        if (!Mouse.current.leftButton.isPressed && isDragging)
        {
            Slot hovered = GetHoveredSlot();
            if (hovered != null && hovered != draggedSlot)
            {
                HandleDrop(draggedSlot, hovered);
            }

            isDragging = false;
            draggedSlot = null;
            dragIcon.enabled = false;
        }
    }

    private void HandleDrop(Slot fromSlot, Slot toSlot)
    {
        if (fromSlot.GetItem() == toSlot.GetItem())
        {
            Debug.Log($"Swapping items!!!");
            int totalAmount = fromSlot.GetItemAmount() + toSlot.GetItemAmount();
            int maxStack = fromSlot.GetItem().maxStack;

            if (totalAmount <= maxStack)
            {
                toSlot.SetItem(fromSlot.GetItem(), totalAmount);
                fromSlot.ClearSlot();
            }
            else
            {
                toSlot.SetItem(fromSlot.GetItem(), maxStack);
                fromSlot.SetItem(fromSlot.GetItem(), totalAmount - maxStack);
            }
        }
        else
        {
            Debug.Log($"Swapping items!!!");
            ItemSO tempItem = toSlot.GetItem();
            int tempAmount = toSlot.GetItemAmount();

            toSlot.SetItem(fromSlot.GetItem(), fromSlot.GetItemAmount());
            fromSlot.SetItem(tempItem, tempAmount);
        }
    }

    private Slot GetHoveredSlot()
    {
        foreach (Slot slot in allSlots)
        {
            if (slot.hovering)
            {
                return slot;
            }
        }
        return null;
    }

    private void UpdateDragIconPosition()
    {
        if (isDragging)
        {
            dragIcon.transform.position = Mouse.current.position.ReadValue();
        }
    }
}
