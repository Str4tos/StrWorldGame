using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerGui : MonoBehaviour
{
    public GameObject inventory;
    public GameObject equipmentSystem;
    public GameObject craftSystem;
    private Inventory craftSystemInventory;
    private CraftSystem cS;
    private Inventory mainInventory;
    private Inventory characterSystemInventory;
    private Tooltip toolTip;

	private Image healthBar;
	private Image manaBar;
	private Slider expBar;
	private Text healthBarText;
	private Text manaBarText;
	private Text expBarText;




    //int normalSize = 3;

    public void OnEnable()
    {
        //Inventory.ItemEquip += OnBackpack;
        //Inventory.UnEquipItem += UnEquipBackpack;

        Inventory.ItemEquip += OnGearItem;
        Inventory.ItemConsumed += OnConsumeItem;
        Inventory.UnEquipItem += OnUnEquipItem;

        Inventory.ItemEquip += EquipWeapon;
        Inventory.UnEquipItem += UnEquipWeapon;
    }

    public void OnDisable()
    {
        //Inventory.ItemEquip -= OnBackpack;
        //Inventory.UnEquipItem -= UnEquipBackpack;

        Inventory.ItemEquip -= OnGearItem;
        Inventory.ItemConsumed -= OnConsumeItem;
        Inventory.UnEquipItem -= OnUnEquipItem;

        Inventory.UnEquipItem -= UnEquipWeapon;
        Inventory.ItemEquip -= EquipWeapon;
    }

    void EquipWeapon(Item item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            //add the weapon if you unequip the weapon
        }
    }

    void UnEquipWeapon(Item item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            //delete the weapon if you unequip the weapon
        }
    }

    

    //void changeInventorySize(int size)
    //{
    //    dropTheRestItems(size);

    //    if (mainInventory == null)
    //        mainInventory = inventory.GetComponent<Inventory>();
    //    if (size == 3)
    //    {
    //        mainInventory.width = 3;
    //        mainInventory.height = 1;
    //        mainInventory.updateSlotAmount();
    //        mainInventory.adjustInventorySize();
    //    }
    //    if (size == 6)
    //    {
    //        mainInventory.width = 3;
    //        mainInventory.height = 2;
    //        mainInventory.updateSlotAmount();
    //        mainInventory.adjustInventorySize();
    //    }
    //    else if (size == 12)
    //    {
    //        mainInventory.width = 4;
    //        mainInventory.height = 3;
    //        mainInventory.updateSlotAmount();
    //        mainInventory.adjustInventorySize();
    //    }
    //    else if (size == 16)
    //    {
    //        mainInventory.width = 4;
    //        mainInventory.height = 4;
    //        mainInventory.updateSlotAmount();
    //        mainInventory.adjustInventorySize();
    //    }
    //    else if (size == 24)
    //    {
    //        mainInventory.width = 6;
    //        mainInventory.height = 4;
    //        mainInventory.updateSlotAmount();
    //        mainInventory.adjustInventorySize();
    //    }
    //}

    void dropTheRestItems(int size)
    {
        if (size < mainInventory.ItemsInInventory.Count)
        {
            for (int i = size; i < mainInventory.ItemsInInventory.Count; i++)
            {
                GameObject dropItem = new GameObject(); //(GameObject)Instantiate(mainInventory.ItemsInInventory[i].itemDropModel)
                dropItem.AddComponent<PickUpItem>();
                dropItem.GetComponent<PickUpItem>().item = mainInventory.ItemsInInventory[i];
                dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition;
            }
        }
    }

	void Awake(){
		healthBar = GameObject.Find ("MainCanvas/HealthGlobe_@").GetComponent<Image>();
		healthBarText = healthBar.gameObject.transform.GetComponentInChildren<Text>();
		manaBar = GameObject.Find ("MainCanvas/ManaGlobe_@").GetComponent<Image>();
		manaBarText = manaBar.gameObject.transform.GetComponentInChildren<Text>();
		expBar = GameObject.Find("MainCanvas/ExpBar_@").GetComponent<Slider>();
		expBarText = expBar.gameObject.transform.GetComponentInChildren<Text>();
//		inventory = GameObject.Find("MainCanvas/
//		equipmentSystem;
//		craftSystem;


	}

    void Start()
    {
        //if (inputManagerDatabase == null)
        //    inputManagerDatabase = (InputManager)Resources.Load("Database/InputManager");

        if (craftSystem != null)
            cS = craftSystem.GetComponent<CraftSystem>();

        if (GameObject.FindGameObjectWithTag("Tooltip") != null)
            toolTip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>();
        if (inventory != null)
            mainInventory = inventory.GetComponent<Inventory>();
        if (equipmentSystem != null)
            characterSystemInventory = equipmentSystem.GetComponent<Inventory>();
        if (craftSystem != null)
            craftSystemInventory = craftSystem.GetComponent<Inventory>();



    }


    public void OnConsumeItem(Item item)
    {
        for (int i = 0; i < item.itemAttributes.Count; i++)
        {
            if (item.itemAttributes[i].attribute == ItemAtributeType.Health)
            {
                //if ((currentHealth + item.itemAttributes[i].attributeValue) > maxHealth)
                //    currentHealth = maxHealth;
                //else
                //    currentHealth += item.itemAttributes[i].attributeValue;
            }
            //if (item.itemAttributes[i].attribute == "Mana")
            //{
            //    //if ((currentMana + item.itemAttributes[i].attributeValue) > maxMana)
            //    //    currentMana = maxMana;
            //    //else
            //    //    currentMana += item.itemAttributes[i].attributeValue;
            //}
            //if (item.itemAttributes[i].attributeName == "Armor")
            //{
            //    //if ((currentArmor + item.itemAttributes[i].attributeValue) > maxArmor)
            //    //    currentArmor = maxArmor;
            //    //else
            //    //    currentArmor += item.itemAttributes[i].attributeValue;
            //}
            //if (item.itemAttributes[i].attributeName == "Damage")
            //{
            //    //if ((currentDamage + item.itemAttributes[i].attributeValue) > maxDamage)
            //    //    currentDamage = maxDamage;
            //    //else
            //    //    currentDamage += item.itemAttributes[i].attributeValue;
            //}
        }

    }

    public void OnGearItem(Item item)
    {
        for (int i = 0; i < item.itemAttributes.Count; i++)
        {
            //if (item.itemAttributes[i].attributeName == "Health")
            //    maxHealth += item.itemAttributes[i].attributeValue;
            //if (item.itemAttributes[i].attributeName == "Mana")
            //    maxMana += item.itemAttributes[i].attributeValue;
            //if (item.itemAttributes[i].attributeName == "Armor")
            //    maxArmor += item.itemAttributes[i].attributeValue;
            //if (item.itemAttributes[i].attributeName == "Damage")
            //    maxDamage += item.itemAttributes[i].attributeValue;
        }
    }

    public void OnUnEquipItem(Item item)
    {
        for (int i = 0; i < item.itemAttributes.Count; i++)
        {
            //if (item.itemAttributes[i].attributeName == "Health")
            //    maxHealth -= item.itemAttributes[i].attributeValue;
            //if (item.itemAttributes[i].attributeName == "Mana")
            //    maxMana -= item.itemAttributes[i].attributeValue;
            //if (item.itemAttributes[i].attributeName == "Armor")
            //    maxArmor -= item.itemAttributes[i].attributeValue;
            //if (item.itemAttributes[i].attributeName == "Damage")
            //    maxDamage -= item.itemAttributes[i].attributeValue;
        }

    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(InputManager.CharacterSystemCode))
        {
            if (!equipmentSystem.activeSelf)
            {
				//characterSystem.SetActive (true);
                characterSystemInventory.openInventory();
            }
            else
            {
                if (toolTip != null)
                    toolTip.deactivateTooltip();
                characterSystemInventory.closeInventory();
            }
        }

        if (Input.GetKeyDown(InputManager.InventoryCode))
        {
            if (!inventory.activeSelf)
            {
				//inventory.SetActive (true);
				mainInventory.openInventory();
            }
            else
            {
                if (toolTip != null)
                    toolTip.deactivateTooltip();
                mainInventory.closeInventory();
            }
        }

        if (Input.GetKeyDown(InputManager.CraftSystemCode))
        {
            if (!craftSystem.activeSelf)
                craftSystemInventory.openInventory();
            else
            {
                if (cS != null)
                    cS.backToInventory();
                if (toolTip != null)
                    toolTip.deactivateTooltip();
                craftSystemInventory.closeInventory();
            }
        }

    }

    public void setGuiHealth(float health, float maxHealth)
    {
		
		healthBar.fillAmount = health / maxHealth;
		healthBarText.text =  health.ToString("#");

	}

    public void setGuiMana(float mana, float maxMana)
    {
		manaBar.fillAmount = mana / maxMana;
		manaBarText.text = mana.ToString("#");
	}

    public void setGuiExp(float currExp, float currExpToLevelUp)
    {
		float percent = currExp / currExpToLevelUp;
		expBar.value = percent * 100;
		expBarText.text = percent.ToString ("P1");
	}

}
