using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Controller : Singleton<UI_Controller>
{
    private void Start()
    {
        UI_WindowTypes[UI_Window.PauseMenu] = typeof(PauseMenu);
        UI_WindowTypes[UI_Window.EquimentTooltip] = typeof(TooltipWindow);
        UI_WindowTypes[UI_Window.InteractToolip] = typeof(InteractObject);
        UI_WindowTypes[UI_Window.BoxInventoryUI] = typeof(BoxInventoryController);
        UI_WindowTypes[UI_Window.StageMap] = typeof(StageMapController);
        UI_WindowTypes[UI_Window.OpenBook] = typeof(BookContentControl);
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Initialize(); // load if current scene is dungeon
        }
        //Initialize(); // load if current scene is dungeon
    }
    public void Initialize()
    {
        _UI_Manager = GameObject.FindFirstObjectByType<UI_Manager>();
        //Debug.Log("UI_Manager found: " + _UI_Manager.name);
        CloseAllClosableWindows();
    }

    public void SetUIActive(UI_Window window, bool active)
    {
        if (UI_Windows.ContainsKey(window))
        {
            UI_Windows[window].SetActive(active);
            if(IsClosableWindow(window) && active)
            {// close tooltip if other window opened
                SetUIActive(UI_Window.InteractToolip, false);
            }
            OnUIClosed(window);
        }
    }

    public bool HasClosableWindowActive()
    {
        foreach (var UI in UI_Windows)
        {
            //Debug.Log("checking UI: " + UI.Key);
            if (!UI.Value.activeSelf) continue;
            if (IsClosableWindow(UI.Key))
            {
                return true;
            }
        }
        return false;
    }

    public void CloseAllClosableWindows()
    {
        foreach (var UI in UI_Windows)
        {
            if (IsClosableWindow(UI.Key))
            {
                if (UI.Key == UI_Window.PauseMenu && UI.Value.activeSelf)
                {
                    GetUIScript<PauseMenu>().OnExitWindow();
                }
                OnUIClosed(UI.Key);
                UI.Value.SetActive(false);
            }
        }
        Inputdata.EnableAllInput(true);
    }

    public bool IsClosableWindow(UI_Window window)
    {
        return IsEquipmentWindow(window) ||
               window == UI_Window.PauseMenu ||
               window == UI_Window.StageMap ||
               window == UI_Window.OpenBook;
    }

    public bool IsEquipmentWindow(UI_Window window)
    {
        return window == UI_Window.InventoryUI ||
               window == UI_Window.EquipmentUI ||
               window == UI_Window.BoxInventoryUI;
    }

    void OnUIClosed(UI_Window window) {
        if (IsEquipmentWindow(window)) {
            SetUIActive(UI_Window.EquimentTooltip, false);
        }
    }

    public void ShowInteractTooltipIfHasInteractable()
    {
        if (GetUIScript<InteractObject>().HasInteractObject())
        {
            SetUIActive(UI_Window.InteractToolip, true);
        }
    }

    public void OnClosableWindowExit(GameObject window)
    {
        ShowInteractTooltipIfHasInteractable();
        window.SetActive(false);
        foreach(var kvp in UI_Windows)
        {
            if(kvp.Value == window)
            {
                OnUIClosed(kvp.Key);
                break;
            }
        }
        if(!HasClosableWindowActive()) 
            Inputdata.EnableAllInput(true);
    }

    public GameObject GetInventoryItemContainer(UI_Window window)
    {
        if (!UI_Windows.ContainsKey(window)) return null;
        if (UI_Windows[window].TryGetComponent<InventoryWindowManager>(out var inventoryUI))
        {
            return inventoryUI.SlotContainer;
        }
        else return null;
    }

    public void OpenInventoryUI(UI_Window window, List<ItemData> itemsShow)
    {
        SetUIActive(window, true);
        IDisplayItem itemDisplayer = 
            GetInventoryItemContainer(window).GetComponent<IDisplayItem>();
        if (window == UI_Window.InventoryUI || 
            window == UI_Window.BoxInventoryUI)
        {
            InventorySlotManager manager = 
                GetInventoryItemContainer(window).GetComponent<InventorySlotManager>();
            Debug.Log("Setting slots num to " + itemsShow.Count);
            manager.ForceSetSlotNum(itemsShow.Count);
        }
        for (int i = 0; i < itemsShow.Count; i++)
        {
            ItemData item = itemsShow[i];
            if(itemsShow[i] == null)
                itemDisplayer.SetItemAtSlot(null, i);
            else
                itemDisplayer.SetItemAtSlot(itemsShow[i], i);
        }
    }

    #region some nasty getComponent methods
    public static T GetUIScript<T>() where T : Component
    {
        Type type = typeof(T);
        if (_UIComponents.ContainsKey(type) && _UIComponents[type] is T uiComponent)
        {
            return uiComponent;
        }
        else
        {
            T component = GetUIComponent<T>();
            if (component != null)
            {
                _UIComponents[type] = component;
            }
            return component;
        }
    }
    static T GetUIComponent<T>() where T : Component
    {
        Type type = typeof(T);

        if (UI_WindowTypes.TryGetValue(GetUIWindowKey<T>(), out _))
        {
            var uiManager = GameObject.FindGameObjectWithTag("UI_Manager");
            if (uiManager == null)
            {
                Debug.LogWarning("UI_Manager not found!");
                return null;
            }

            var uiWindow = uiManager.GetComponent<UI_Manager>().UI_Windows;
            if (uiWindow.TryGetValue(GetUIWindowKey<T>(), out GameObject uiGameObject))
            {
                if (uiGameObject == null)
                {
                    Debug.LogWarning("UI Window not found for type " + type.Name);
                    return null;
                }

                if (uiGameObject.TryGetComponent<T>(out var component))
                {
                    return component;
                }

                component = uiGameObject.GetComponentInChildren<T>();
                if (component != null)
                {
                    return component;
                }
            }
        }

        Debug.LogWarning("Component of type " + type.Name + " not found in UI Manager!");
        return null;
    }

    static UI_Window GetUIWindowKey<T>()
    {
        foreach (var kvp in UI_WindowTypes)
        {
            if (kvp.Value == typeof(T))
            {
                return kvp.Key;
            }
        }
        Debug.LogError("UI_WindowType not found for type " + typeof(T).Name);
        return UI_Window.None;
    }
#endregion
    //Dictionary<>
    UI_Manager _UI_Manager;
    public GameObject TestStatics { get; private set; }
    Dictionary<UI_Window, GameObject> UI_Windows { get { return _UI_Manager.UI_Windows; } }

    static Dictionary<UI_Window, Type> UI_WindowTypes = new Dictionary<UI_Window, Type>();
    static Dictionary<Type, UnityEngine.Object> _UIComponents = new();

    PlayerInputData Inputdata { get { return PlayerInputData.Instance; } }

    public GameObject InventoryUI { get { return UI_Windows[UI_Window.InventoryUI]; } }
    public GameObject EquipmentUI { get { return UI_Windows[UI_Window.EquipmentUI]; } }
    public GameObject Hotbar { get { return UI_Windows[UI_Window.HotBar]; } }
    public GameObject PauseMenu { get { return UI_Windows[UI_Window.PauseMenu]; } }
    public GameObject EquimentTooltip { get { return UI_Windows[UI_Window.EquimentTooltip]; } }
    public GameObject InteractToolip { get { return UI_Windows[UI_Window.InteractToolip]; } }
    public GameObject BoxInventoryUI { get { return UI_Windows[UI_Window.BoxInventoryUI]; } }
    public GameObject StageMap { get { return UI_Windows[UI_Window.StageMap]; } }
}
