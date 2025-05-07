using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for the pickup items that will effect on player once he touch the item
// usually positive stuff
[CreateAssetMenu(menuName = "Items/Pickup Item")]
public class InstantEffectPickupItem : ScriptableObject
{
    public int itemID;
    public GameObject itemModel;

    public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();

}
