using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvents : MonoBehaviour
{
    private Animator anim;
    //private PlayerStateExecutor _executor;

    public Sword1 weapon;

    void Awake()
    {
        anim = GetComponent<Animator>();
        //_executor = GetComponentInParent<PlayerStateExecutor>();
    }

    public void EnableMove()
    {
        //_executor.EnableMove(true);
    }
    public void DisableMove()
    {
        //_executor.EnableMove(false);
    }

    public void EnableWeaponColl()
    {
        weapon.EnableColliders();
    }
    public void DisableWeaponColl()
    {
        weapon.DisableColliders();
    }
}
