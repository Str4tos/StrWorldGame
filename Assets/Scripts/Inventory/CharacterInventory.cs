using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharacterInventory : Inventory
{
    #region Fields
    [Header("Character Options")]
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text pointsText;
    [SerializeField]
    private Text expText;

    [SerializeField]
    private Text damageStatText;
    [SerializeField]
    private Text defenceStatText;
    [SerializeField]
    private Text attackSpeedStatText;
    [SerializeField]
    private Text healthStatText;
    [SerializeField]
    private Text manaStatText;
    [SerializeField]
    private Text wizardyDmgStatText;

    [Header("General Stats")]
    [SerializeField]
    private Text strenghtPointsText;
    [SerializeField]
    private Text agilityPointsText;
    [SerializeField]
    private Text vitalityPointsText;
    [SerializeField]
    private Text energyPointsText;


    [Header("General Stats buttons")]
    [SerializeField]
    private GameObject strenghtPlus;
    [SerializeField]
    private GameObject agilityPlus;
    [SerializeField]
    private GameObject vitalityPlus;
    [SerializeField]
    private GameObject energyPlus;


    private List<ItemEquipedData> itemsEquipedData;
    private ItemEquipType[] itemEquipTypes;
    private StatusPlayer _StatusPlayer;

    #endregion

    protected override void Start()
    {
        typeParentInv = TypeParentInv.equip;
        _StatusPlayer = PlayerController.currPlayer._StatusPlayer;
        _StatusPlayer.OnChangeExp = RefreshExpLvl;

        itemEquipTypes = new ItemEquipType[]{
            ItemEquipType.Agation, ItemEquipType.Head, ItemEquipType.Wings,
            ItemEquipType.WeaponRightHend, ItemEquipType.Chest, ItemEquipType.WeaponLeftHend,
            ItemEquipType.Hands, ItemEquipType.Pants, ItemEquipType.Shoes,
            ItemEquipType.RingRight, ItemEquipType.Necklace, ItemEquipType.RingLeft
        };
        itemsEquipedData = new List<ItemEquipedData>(12);
        for (int i = 0; i < itemEquipTypes.Length; i++)
        {
            itemsEquipedData.Add(new ItemEquipedData(itemEquipTypes[i], new ItemEquip()));
        }

        base.Start();
    }

    public override void OpenInventory()
    {
        base.OpenInventory();
        RefreshCharacterStats();
    }


    #region Equip
    public ItemEquip Equip(ItemEquip equipItem)
    {
        ItemEquipType tempType = equipItem.itemEquipType;
        ItemEquip itemForReturn = null;
        if (tempType == ItemEquipType.Ring)
        {
            if (!GetItemEquipData(ItemEquipType.RingLeft).IsEquiped)
                tempType = ItemEquipType.RingLeft;
            else tempType = ItemEquipType.RingRight;
        }

        ItemEquipedData _ItemEquipedData = GetItemEquipData(tempType);
        if (_ItemEquipedData.IsEquiped)
        {
            Destroy(slotContainer.GetChild(_ItemEquipedData.Item.indexItemInList).GetChild(0).gameObject);
            itemForReturn = _ItemEquipedData.Item;
            itemForReturn.indexItemInList = equipItem.indexItemInList;
        }
        ItemEquip newItem = equipItem.getCopy();
        newItem.indexItemInList = GetIndexItemInList(tempType);
        _ItemEquipedData.IsEquiped = true;
        _ItemEquipedData.Item = newItem;

        AddItemInObj(newItem, newItem.indexItemInList);
        return itemForReturn;
    }
    public void UnEquip(Item item)
    {
        ItemEquipedData _ItemEquipedData = itemsEquipedData[item.indexItemInList];
        Destroy(slotContainer.GetChild(item.indexItemInList).GetChild(0).gameObject);
        _ItemEquipedData.UnEquiped();
    }
    public override ItemEquip GetEquipItem(Item item)
    {
        return itemsEquipedData[item.indexItemInList].Item;
    }
    public ItemEquip GetEquipItem(ItemEquipType equipType)
    {
        if (equipType == ItemEquipType.Ring)
        {
            if (GetItemEquipData(ItemEquipType.RingLeft).IsEquiped)
                equipType = ItemEquipType.RingLeft;
            else equipType = ItemEquipType.RingRight;
        }
        return GetItemEquipData(equipType).Item;
    }

    public InventoryCharacterData GetInventoryData()
    {
        InventoryCharacterData result = new InventoryCharacterData();
        result.itemsEquipedData = itemsEquipedData;
        return result;
    }

    public void LoadSerialization(InventoryCharacterData loadedInventory)
    {
        itemsEquipedData = loadedInventory.itemsEquipedData;
        foreach (ItemEquipedData itemEquipedData in itemsEquipedData)
        {
            if (!itemEquipedData.Item.IsEmpty())
                AddItemInObj(itemEquipedData.Item, itemEquipedData.Item.indexItemInList);
        }
    }

    private int GetIndexItemInList(ItemEquipType tempType)
    {
        for (int i = 0; i < itemEquipTypes.Length; i++)
        {
            if (itemEquipTypes[i] == tempType)
                return i;
        }
        return -1;
    }

    private ItemEquipedData GetItemEquipData(ItemEquipType equipType)
    {
        for (int i = 0; i < itemsEquipedData.Count; i++)
            if (itemsEquipedData[i].Type == equipType)
                return itemsEquipedData[i];
        return null;
    }
    #endregion

    #region Character Stats
    public void BtnIncreaseStr()
    {
        _StatusPlayer.strenght++;
        _StatusPlayer.statPoints--;
        _StatusPlayer.CalculateOptions();
        RefreshStrenght();
        CheckForPoints();
    }
    public void BtnIncreaseAgi()
    {
        _StatusPlayer.agility++;
        _StatusPlayer.statPoints--;
        _StatusPlayer.CalculateOptions();
        RefreshAgility();
        CheckForPoints();
    }
    public void BtnIncreaseVit()
    {
        _StatusPlayer.vitality++;
        _StatusPlayer.statPoints--;
        _StatusPlayer.CalculateOptions();
        RefreshVitality();
        CheckForPoints();
        _StatusPlayer.UpdateHealthOnInterface();
    }
    public void BtnIncreaseEne()
    {
        _StatusPlayer.energy++;
        _StatusPlayer.statPoints--;
        _StatusPlayer.CalculateOptions();
        RefreshEnergy();
        CheckForPoints();
        _StatusPlayer.UpdateManaOnInterface();
    }

    /// <summary>
    /// Only for tests
    /// </summary>
    public void AddStatsForTest()
    {
        _StatusPlayer.strenght += 99;
        _StatusPlayer.agility += 99;
        _StatusPlayer.vitality += 99;
        _StatusPlayer.energy += 99;
        BtnIncreaseStr();
        BtnIncreaseAgi();
        BtnIncreaseVit();
        BtnIncreaseEne();
    }

    private void RefreshExpLvl()
    {
        levelText.text = "Level: " + _StatusPlayer.level;
        expText.text = "Exp: " + _StatusPlayer.currExp + "/" + _StatusPlayer.requiredExpForLvl;
        CheckForPoints();
    }
    private void RefreshStrenght()
    {
        strenghtPointsText.text = _StatusPlayer.strenght.ToString();
        damageStatText.text = "Damage: " + _StatusPlayer.AttackDmg;
    }
    private void RefreshAgility()
    {
        agilityPointsText.text = _StatusPlayer.agility.ToString();
        defenceStatText.text = "Defense: " + _StatusPlayer.Defense;
        attackSpeedStatText.text = "Attack speed: " + (_StatusPlayer.AttackSpeed * 10f).ToString("F");
    }
    private void RefreshVitality()
    {
        vitalityPointsText.text = _StatusPlayer.vitality.ToString();
        healthStatText.text = "Health: " + _StatusPlayer.Health.ToString("#") + "/" + _StatusPlayer.MaxHealth;
    }
    private void RefreshEnergy()
    {
        energyPointsText.text = _StatusPlayer.energy.ToString();
        manaStatText.text = "Mana: " + _StatusPlayer.Mana.ToString("#") + "/" + _StatusPlayer.MaxMana;
        wizardyDmgStatText.text = "Wizardy Dmg: " + _StatusPlayer.WizardyDmg;
    }
    private void CheckForPoints()
    {
        pointsText.text = "Point " + _StatusPlayer.statPoints;
        if (_StatusPlayer.statPoints > 0)
        {
            SetActivePointButtons(true);
        }
        else
        {
            SetActivePointButtons(false);
        }
    }
    private void SetActivePointButtons(bool isActive)
    {
        if (strenghtPlus.activeSelf != isActive)
        {
            pointsText.gameObject.SetActive(isActive);
            strenghtPlus.SetActive(isActive);
            agilityPlus.SetActive(isActive);
            vitalityPlus.SetActive(isActive);
            energyPlus.SetActive(isActive);
        }
    }

    public void RefreshCharacterStats()
    {
        RefreshExpLvl();
        RefreshStrenght();
        RefreshAgility();
        RefreshVitality();
        RefreshEnergy();
        CheckForPoints();
    }

    #endregion

}
