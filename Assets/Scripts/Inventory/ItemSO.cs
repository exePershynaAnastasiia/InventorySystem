using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "NewItem")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int maxStack;
    public GameObject itemPrefab;
    public GameObject handItemPrefab;
    public string description;
}
