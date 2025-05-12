using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickupSpawner : MonoBehaviour, IPickItem
{
    [HideInInspector] public bool PickedItem = false;
    GameObject spawnedPickup;
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
        if (PickedItem || spawnedPickup!= null) return;
        Debug.Log($"Spawning Pickup for {transform.parent.gameObject.name} at {gameObject.name}");
        spawnedPickup = Instantiate(spawnPickupDetail.itemModel, transform);
        spawnedPickup.transform.localPosition = Vector3.zero;
        var pickupScript = spawnedPickup.GetOrAddComponent<CollectableItem>();
        pickupScript.SetCollectable(spawnPickupDetail.itemAttributes, this.gameObject);
    }
}
