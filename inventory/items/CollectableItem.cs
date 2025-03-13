using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public InstantEffectPickupItem itemAttribute;
    public GameObject spawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<IPickItem>()?.OnPickItem(itemAttribute);
            if(spawner!= null)
                spawner.GetComponent<IPickItem>()?.OnPickItem(itemAttribute);
            Destroy(gameObject);
        }
    }
}

public interface IPickItem
{
    void OnPickItem(InstantEffectPickupItem pickedItem);
}
