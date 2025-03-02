using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatBillboard : MonoBehaviour
{
    EnemyStateExecutor _executor;
    GameObject _hpBarObj;
    Slider _hpBar;
    TMP_Text _hpBarText;
    public Camera Cam { get; set; }
    void OnEnable()
    {
        _executor = transform.parent.GetComponent<EnemyStateExecutor>();
        _hpBarObj = transform.GetChild(0).gameObject;
        _hpBar = _hpBarObj.GetComponent<Slider>();
        _hpBarText = _hpBarObj.transform.GetChild(2).GetComponent<TMP_Text>();
        if(Cam == null)
        {
            Cam = Camera.main;
        }
    }

    private void LateUpdate()
    {
        float maxHP = _executor.MaxHP;
        float HP = _executor.HP;
        _hpBar.value = HP / maxHP;
        if (HP % 1 != 0)
        {
            HP = (float)System.Math.Round(HP, 2);
        }
        _hpBarText.text = Mathf.Floor(maxHP) + " / " + HP;

        // make the canvas points at player
        transform.LookAt(transform.position + Cam.transform.forward);
    }

}
