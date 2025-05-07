using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour, IPickItem
{
    [HideInInspector] public bool PickedItem = false;

    [SerializeField] InstantEffectPickupItem spawnPickupDetail;

    public void OnPickItem(List<ItemAttribute> _, Action __)
    {
        PickedItem = true;
    }

    //IEnumerator delaySpawnPickup(int delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    SpawnPickup();
    //}

    public void SpawnPickup()
    {
        Debug.Log("Spawning Pickup");
        var pickup = Instantiate(spawnPickupDetail.itemModel, transform);
        pickup.transform.localPosition = Vector3.zero;
        var pickupScript = pickup.AddComponent<CollectableItem>();
        pickupScript.SetCollectable(spawnPickupDetail.itemAttributes, this.gameObject);
    }
}
