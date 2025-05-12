using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    List<ItemAttribute> itemAttributes = new();
    GameObject spawner;
    List<Collider> _triggeredColliders = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_triggeredColliders.Contains(other)) return;
            _triggeredColliders.Add(other);
            other.GetComponent<IPickItem>()?.OnPickItem(itemAttributes, () =>
            {
                if(spawner!= null)
                {
                    spawner.GetComponent<IPickItem>()?.OnPickItem(null, null);
                }
                Destroy(gameObject);
            });
        }
    }

    public void SetCollectable(List<ItemAttribute> itemAttributes, GameObject spawner)
    {
        foreach(ItemAttribute attribute in itemAttributes)
        {
            //Debug.Log($"Adding attribute: {attribute.AtrbName}, {attribute.AtrbValue}");
            this.itemAttributes.Add(attribute);
        }
        this.spawner = spawner;
    }
}

public interface IPickItem
{
    void OnPickItem(List<ItemAttribute> itemAttributes, Action callback);
}
