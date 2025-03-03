using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : MonoBehaviour, IConsumableItem
{
    PlayerStateExecutor _executor;
    Action ConsumeItemFunc;
    public void Awake()
    {
        _executor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateExecutor>();
    }
    public void OnItemConsume(Item item)
    {
        ConsumeItemFunc();
    }

    public void testMethod()
    {
        Debug.Log("Item Consumed");
    }
}

public interface IConsumableItem
{
    public void OnItemConsume(Item item);
}
