using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;

public class UI_Controller : Singleton_LastIn<UI_Controller>
{
    private void Awake()
    {
        UI_WindowPrefabNames[UI_Window.HUD] = "HUD";
        UI_WindowPrefabNames[UI_Window.HotBar] = "HotBar";
        UI_WindowPrefabNames[UI_Window.InventoryUI] = "PlayerInventory";
        UI_WindowPrefabNames[UI_Window.EquipmentUI] = "Equipment";
        UI_WindowPrefabNames[UI_Window.BoxInventoryUI] = "BoxInventory";
        UI_WindowPrefabNames[UI_Window.PauseMenu] = "PausePanel";
        UI_WindowPrefabNames[UI_Window.InteractToolip] = "InteractTooltip";
        UI_WindowPrefabNames[UI_Window.EquipmentTooltip] = "EquipmentTooltip";
        UI_WindowPrefabNames[UI_Window.LoseUI] = "LoseUI";
        UI_WindowPrefabNames[UI_Window.NotifyTooltip] = "NotifyTooltip";
        UI_WindowPrefabNames[UI_Window.OpenBook] = "OpenBook";
        UI_WindowPrefabNames[UI_Window.BuyItemUI] = "BuyItemUI";

        // ui window scripts setting
        UI_WindowTypes[UI_Window.PauseMenu] = typeof(PauseMenu);
        UI_WindowTypes[UI_Window.EquipmentTooltip] = typeof(TooltipWindow);
        UI_WindowTypes[UI_Window.InteractToolip] = typeof(InteractObject);
        UI_WindowTypes[UI_Window.BoxInventoryUI] = typeof(BoxInventoryController);
        UI_WindowTypes[UI_Window.StageMap] = typeof(StageMapController);
        UI_WindowTypes[UI_Window.OpenBook] = typeof(BookContentControl);
        UI_WindowTypes[UI_Window.BossHUD] = typeof(BossHudControl);
        UI_WindowTypes[UI_Window.BuyItemUI] = typeof(BuyItemUIController);
        UI_WindowTypes[UI_Window.NotifyTooltip] = typeof(NotifyTooltipController);

    }
    private void Start()
    {
        if (SceneManager.GetActiveScene() == gameObject.scene)
        {
            Debug.Log("Initializing ui");
            Initialize(); // load if current scene is dungeon
        }
        //Initialize(); // load if current scene is dungeon
    }

    void CreateAndSetUI(UI_Window window)
    {
        if (UI_Windows.ContainsKey(window) && !UI_Windows[window].IsGameObjectNullOrDestroyed()) return;
        string path = UI_PATH + UI_WindowPrefabNames[window];
        UI_Windows[window] = Instantiate(Resources.Load<GameObject>(path), transform);
        Debug.Log("ui window loaded: " + UI_Windows[window].name);
    }
    public void Initialize()
    {
        for (int i = 0; i < Enum.GetValues(typeof(UI_Window)).Length; i++)
        {
            UI_Window window = (UI_Window)i;
            if (UI_Windows.ContainsKey(window))
            {
                Destroy(UI_Windows[window]);
                UI_Windows.Remove(window);
            }
        }
        CreateAndSetUI(UI_Window.InventoryUI);
        CreateAndSetUI(UI_Window.EquipmentUI);
        CreateAndSetUI(UI_Window.BoxInventoryUI);
        CreateAndSetUI(UI_Window.EquipmentTooltip);
        CreateAndSetUI(UI_Window.InteractToolip);
        CreateAndSetUI(UI_Window.PauseMenu);
        CreateAndSetUI(UI_Window.BuyItemUI);
        CreateAndSetUI(UI_Window.OpenBook);

        CreateAndSetUI(UI_Window.NotifyTooltip);
        CreateAndSetUI(UI_Window.HUD);
        CreateAndSetUI(UI_Window.HotBar);
        CreateAndSetUI(UI_Window.LoseUI);

        // settings in inspector
        foreach (var pair in uiWindowPairs)
        {
            if (pair.windowObject == null) continue;
            UI_Window window = pair.windowType;
            if (UI_Windows.ContainsKey(window) && !UI_Windows[window].IsGameObjectNullOrDestroyed()) {
                Debug.LogWarning($"Duplicate UI_Window type found: {pair.windowType}. Only the first one will be used.");
                return;
            }
            UI_Windows[window] = Instantiate(pair.windowObject, transform);
            Debug.Log("ui window loaded: " + UI_Windows[window].name);
        }

        UI_Windows[UI_Window.WinUI].transform.SetAsLastSibling();
        UI_Windows[UI_Window.LoseUI].transform.SetAsLastSibling();
        CloseAllClosableWindows();
    }

    public void SetUIActive(UI_Window window, bool active)
    {
        if (UI_Windows.ContainsKey(window))
        {
            UI_Windows[window].SetActive(active);
            if (IsClosableWindow(window) && active)
            {// close tooltip if other window opened
                SetUIActive(UI_Window.InteractToolip, false);
                if (window == UI_Window.LoseUI || window ==  UI_Window.WinUI)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            else
            {
                OnUIClosed(window);
            }
        }
    }

    public bool IsUIActive(UI_Window window) => 
        UI_Windows.ContainsKey(window) && 
        UI_Windows[window].activeSelf;

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
            if(UI.Value == null || UI.Value.IsDestroyed()) 
                continue;
            if(!UI_Windows.ContainsKey(UI.Key))
            {
                Debug.Log("UI_Windows does not contain key: " + UI.Key);
                continue;
            }
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
               window == UI_Window.OpenBook ||
               window == UI_Window.TutorialUI ||
               window == UI_Window.BuyItemUI ||
               window == UI_Window.MiniGameRules;
    }

    public bool IsEquipmentWindow(UI_Window window)
    {
        return window == UI_Window.InventoryUI ||
               window == UI_Window.EquipmentUI ||
               window == UI_Window.BoxInventoryUI;
    }

    void OnUIClosed(UI_Window window) {
        if (IsEquipmentWindow(window)) {
            SetUIActive(UI_Window.EquipmentTooltip, false);
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

    public void UpdateInventoryItem(UI_Window window, int index)
    {
        if (IsUIActive(window))
        {
            IDisplayItem itemDisplayer =
                GetInventoryItemContainer(window)
                .GetComponent<IDisplayItem>();
            itemDisplayer.UpdateItemAtSlot(index);
        }
    }

    public void SetInventoryItemAtSlot(UI_Window window, ItemData item, int index)
    {
        if (IsUIActive(window))
        {
            IDisplayItem itemDisplayer =
                GetInventoryItemContainer(window)
                .GetComponent<IDisplayItem>();
            itemDisplayer.SetItemAtSlot(item, index);
        }
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

    public void PopMessage(string message)
    {
        GetUIScript<NotifyTooltipController>().PopMessage(message);
    }

    #region some nasty getComponent methods
    public static T GetUIScript<T>() where T : Component
    {
        Type type = typeof(T);
        if (_UIComponents.ContainsKey(type) && _UIComponents[type] is T uiComponent)
        {
            if(!uiComponent.IsCompNullOrDestroyed())
                return uiComponent;
        }
        T component = GetUIComponent<T>();
        if (component != null)
        {
            _UIComponents[type] = component;
        }
        return component;
    }
    static T GetUIComponent<T>() where T : Component
    {
        Type type = typeof(T);

        if (UI_WindowTypes.TryGetValue(GetUIWindowKey<T>(), out _))
        {

            if (UI_Windows.TryGetValue(GetUIWindowKey<T>(), out GameObject uiGameObject))
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

    static Dictionary<UI_Window, GameObject> UI_Windows = new();
    [SerializeField] UIWindowPair[] uiWindowPairs;

    Dictionary<UI_Window, string> UI_WindowPrefabNames = new();
    public const string UI_PATH = "UserInterface/";

    static Dictionary<UI_Window, Type> UI_WindowTypes = new Dictionary<UI_Window, Type>();
    static Dictionary<Type, UnityEngine.Object> _UIComponents = new();

    PlayerInputData Inputdata { get { return PlayerInputData.Instance; } }

    public GameObject InventoryUI { get { return UI_Windows[UI_Window.InventoryUI]; } }
    public GameObject EquipmentUI { get { return UI_Windows[UI_Window.EquipmentUI]; } }
    public GameObject Hotbar { get { return UI_Windows[UI_Window.HotBar]; } }
    public GameObject PauseMenu { get { return UI_Windows[UI_Window.PauseMenu]; } }
    public GameObject EquimentTooltip { get { return UI_Windows[UI_Window.EquipmentTooltip]; } }
    public GameObject InteractToolip { get { return UI_Windows[UI_Window.InteractToolip]; } }
    public GameObject BoxInventoryUI { get { return UI_Windows[UI_Window.BoxInventoryUI]; } }
    public GameObject StageMap { get { return UI_Windows[UI_Window.StageMap]; } }
}
