using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UI_Windows = new Dictionary<UI_Window, GameObject>();

        foreach (var pair in uiWindowPairs)
        {
            if (!UI_Windows.ContainsKey(pair.windowType))
            {
                UI_Windows.Add(pair.windowType, pair.windowObject);
            }
            else
            {
                Debug.LogWarning($"Duplicate UI_Window type found: {pair.windowType}. Only the first one will be used.");
            }
        }
    }

    public GameObject[] GetUIObjects()
    {
        List<GameObject> uiObjects = new List<GameObject>();
        foreach (var pair in uiWindowPairs)
        {
            uiObjects.Add(pair.windowObject);
        }

        return uiObjects.ToArray();
    }

    [SerializeField] UIWindowPair[] uiWindowPairs;
    public Dictionary<UI_Window, GameObject> UI_Windows { get; private set; }
}
public enum UI_Window {
    None, InventoryUI, EquipmentUI, HUD, 
    HotBar, PauseMenu, EquimentTooltip, InteractToolip, 
    StageMap, BoxInventoryUI, TestStatics, OpenBook
}
