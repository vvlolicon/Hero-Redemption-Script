using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractTooltipControl : MonoBehaviour
{
    [SerializeField] TMP_Text tooltipText;

    public void SetTooltipTitle(string title)
    {
        tooltipText.text = title;
    }
}
