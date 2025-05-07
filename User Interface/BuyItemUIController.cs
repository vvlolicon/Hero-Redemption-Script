using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuyItemUIController : MonoBehaviour
{
    List<BuyItemOption> _itemOptions = new();
    List<Item> _itemsSold;
    [SerializeField] GameObject _optionPrefab;
    [SerializeField] RectTransform _container;
    [SerializeField] TMP_Text _title;
    [SerializeField] TMP_Text _playerDeposit;
 
    string _ownerNPC;
    bool initialzed = false;
    PlayerBackpack PlayerBackpack { get { return PlayerCompManager.TryGetPlayerComp<PlayerBackpack>(); } }

    public void SetUI(List<Item> itemsSold, string ownerNPC)
    {
        // skip if already initialized and owner is the same
        if (initialzed && _ownerNPC.Equals(ownerNPC)) return; 
        // initialize interface with sold items
        initialzed = true;
        _itemsSold = itemsSold;
        _ownerNPC = ownerNPC;
        _title.text = ownerNPC + "'s Shop";
        ClearOptions();
        for (int i = 0; i<itemsSold.Count; i++)
        {
            var item = itemsSold[i];
            var option = Instantiate(_optionPrefab, _container);
            var optionScript = option.GetComponent<BuyItemOption>();
            optionScript.SetOption(item);
            optionScript.BuyItemButton.interactable = CanAffordItem(item);
            optionScript.BuyItemButton.onClick.AddListener(() => BuyItem(item));
        }
        _container.sizeDelta = new Vector2(_container.sizeDelta.x, itemsSold.Count * 60f + 10f);
        _playerDeposit.text = PlayerBackpack.PlayerOwnedMoney + "$";
    }

    void ClearOptions()
    {
        for(int i = _itemOptions.Count-1; i>=0; i--)
        {
            var option = _itemOptions[i];
            option.BuyItemButton.onClick.RemoveAllListeners();
            Destroy(option.gameObject);
            _itemOptions.RemoveAt(i);
        }
    }

    void BuyItem(Item item)
    {
        if (CanAffordItem(item) && PlayerBackpack.HasEmptySlot())
        {
            PlayerBackpack.PlayerOwnedMoney -= item.itemValue;
            PlayerBackpack.AddItemToPlayerBackpack(item.GetItemDataClone());
            bool HasEmptySlot = PlayerBackpack.HasEmptySlot();
            int playerDeposit = PlayerBackpack.PlayerOwnedMoney;
            foreach (BuyItemOption option in _itemOptions)
            {
                option.BuyItemButton.interactable =
                    option.CanAffordItem(playerDeposit) && HasEmptySlot;
            }
            _playerDeposit.text = playerDeposit + "$";
        }
    }

    bool CanAffordItem(Item item) =>
        PlayerBackpack.PlayerOwnedMoney >= item.itemValue;

}