using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour{

    public string itemName;
    public enum Type { Weapon, Armor, Consumable, Misc };
    public Type itemType;
    public string itemRarity;
    public int itemCost;

    public int itemArmorRating;
    public int itemDamage;

    public int itemSpriteIndex;

    public Sprite itemSprite;

    public Item(string name, Type type, string rarity, int cost, int armorBonus, int damage, int spriteIndex)
    {
        itemName = name;
        itemType = type;
        itemRarity = rarity;
        itemCost = cost;
        itemArmorRating = armorBonus;
        itemDamage = damage;
        itemSpriteIndex = spriteIndex;
    }
}
