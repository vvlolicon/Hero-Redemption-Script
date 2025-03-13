using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for the pickup items that will effect on player once he touch the item
// usually positive stuff
[CreateAssetMenu(menuName = "Items/Pickup Item")]
public class InstantEffectPickupItem : ScriptableObject
{
    public int itemID;
    [SerializeField] public GameObject itemModel;

    [SerializeField]
    public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();

}
