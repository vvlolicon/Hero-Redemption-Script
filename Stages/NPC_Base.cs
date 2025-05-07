using UnityEngine;

public abstract class NPC_Base : MonoBehaviour, IInteractableObject
{
    [SerializeField] protected string NPC_Name;

    public string GetInterableTitle() => NPC_Name;

    public void Interact()
    {
    }
}