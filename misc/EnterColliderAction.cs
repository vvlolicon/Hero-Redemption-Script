using UnityEngine;

public class EnterColliderAction : MonoBehaviour
{
    public event System.Action<Collider> OnEnterAction;

    private void OnTriggerEnter(Collider other)
    {
        OnEnterAction.Invoke(other);
    }

    private void OnDestroy()
    {
        OnEnterAction = null;
    }
}