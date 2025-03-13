using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_SpawnPickup : MonoBehaviour, IPickItem
{
    public int spawnSeconds = 3;
    public InstantEffectPickupItem spawnPickupDetail;
    GameObject curPickup;

    private void Start()
    {
        SpawnPickup();
    }

    public void OnPickItem(InstantEffectPickupItem pickedItem)
    {
        StartCoroutine(delaySpawnPickup(spawnSeconds));
    }

    IEnumerator delaySpawnPickup(int delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnPickup();
    }

    void SpawnPickup()
    {
        curPickup = Instantiate(spawnPickupDetail.itemModel);
        curPickup.AddComponent<CollectableItem>();
        curPickup.GetComponent<CollectableItem>().itemAttribute = spawnPickupDetail;
        curPickup.GetComponent<CollectableItem>().spawner = this.gameObject;
    }
}
