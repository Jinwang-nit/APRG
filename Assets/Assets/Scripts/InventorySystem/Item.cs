using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public new string name = "New Item";
    public string description = "New description";
    public Sprite icon;
    public int currentQuantity = 1;
    public int maxQuantity = 16;
    public int equippableItemIndex = -1;
}
