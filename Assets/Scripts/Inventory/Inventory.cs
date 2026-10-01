using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public ItemSO testItem1;
    public ItemSO testItem2;

    public Image dragIcon;
    public float pickupRange = 3f;
    private ItemOnGround lookedAtItem = null;
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer lookedAtRenderer = null;


    public GameObject hotBarObj;
    public GameObject inventorySlotParent;
    public GameObject containerObj;

    public GameObject itemDescriptionParent;
    public Image itemDescriptionIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;


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
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            containerObj.SetActive(!containerObj.activeSelf);
            Cursor.lockState = containerObj.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = containerObj.activeSelf ? true : false;

            PlayerCamera.Instance.updatingRotation = !PlayerCamera.Instance.updatingRotation;
        }

        DetectLookedAtItem();
        PickUp();

        StartDrag();
        UpdateDragIconPosition();
        EndDrag();

        UpdateItemDescription();
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

    private void PickUp()
    {
        if (lookedAtRenderer != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ItemOnGround itemOnGround = lookedAtRenderer.GetComponent<ItemOnGround>();
            if (itemOnGround != null)
            {
                AddItem(itemOnGround.item, itemOnGround.amount);
                Destroy(itemOnGround.gameObject);
                lookedAtRenderer = null;
                lookedAtItem = null;
            }
        }
    }

    private void DetectLookedAtItem()
    {
        if (lookedAtRenderer != null)
        {
            lookedAtRenderer.material = originalMaterial;
            lookedAtRenderer = null;
            originalMaterial = null;
        }
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            ItemOnGround itemOnGround = hit.collider.GetComponent<ItemOnGround>();
            Debug.Log("Hit: " + hit.collider.name);
            if (itemOnGround != null)
            {
                Renderer renderer = itemOnGround.GetComponent<Renderer>();
                if (renderer != null)
                {
                    originalMaterial = renderer.material;
                    renderer.material = highlightMaterial;
                    lookedAtRenderer = renderer;
                    Debug.Log("Highlighting");
                }
            }
            else
            {
                lookedAtItem = null;
            }
        }
        else
        {
            lookedAtItem = null;
        }
    }

    private void UpdateItemDescription()
    {
        Slot hoveredSlot = GetHoveredSlot();
        if (hoveredSlot != null && hoveredSlot.GetItem() != null)
        {
            itemDescriptionParent.SetActive(true);
            itemDescriptionIcon.sprite = hoveredSlot.GetItem().itemIcon;
            itemNameText.text = hoveredSlot.GetItem().itemName;
            itemDescriptionText.text = hoveredSlot.GetItem().description;
        }
        else
        {
            itemDescriptionParent.SetActive(false);
        }
    }
}
