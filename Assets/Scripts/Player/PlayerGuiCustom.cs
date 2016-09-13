using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerGuiCustom : MonoBehaviour
{

    private InventoryBag invBag;
    private InventoryCharacter invCharacter;
    private InventoryStorage invStorage;
    private InventoryShop invShop;
    private InventoryCrafting invCrafting;
    private TooltipCustom toolTip;

    private PlayerMenu playerMenu;

    private Image healthBar;
    private Image manaBar;
    private Slider expBar;
    private Text healthBarText;
    private Text manaBarText;
    private Text expBarText;


    #region Get/Set
    public TooltipCustom ToolTip
    {
        get { return toolTip; }
        set { toolTip = value; }
    }
    public InventoryCrafting InvCrafting
    {
        get { return invCrafting; }
        set { invCrafting = value; }
    }
    public InventoryStorage InvStorage
    {
        get { return invStorage; }
        set { invStorage = value; }
    }
    public InventoryCharacter InvCharacter
    {
        get { return invCharacter; }
        set { invCharacter = value; }
    }
    public InventoryBag InvBag
    {
        get { return invBag; }
        set { invBag = value; }
    }
    public InventoryShop InvShop
    {
        get { return invShop; }
        set { invShop = value; }
    }

    #endregion Get/Set

    void Awake()
    {
        healthBar = GameObject.Find("MainCanvas/HealthGlobe_@").GetComponent<Image>();
        healthBarText = healthBar.gameObject.transform.GetComponentInChildren<Text>();
        manaBar = GameObject.Find("MainCanvas/ManaGlobe_@").GetComponent<Image>();
        manaBarText = manaBar.gameObject.transform.GetComponentInChildren<Text>();
        expBar = GameObject.Find("MainCanvas/ExpBar_@").GetComponent<Slider>();
        expBarText = expBar.gameObject.transform.GetComponentInChildren<Text>();
        playerMenu = GameObject.Find("MainCanvas/GeneralMenu").GetComponent<PlayerMenu>();

		invBag = GameObject.FindWithTag("InvBag").GetComponent<InventoryBag>();
		invStorage = GameObject.FindWithTag("InvStorage").GetComponent<InventoryStorage>();
		invShop = GameObject.FindWithTag("InvShop").GetComponent<InventoryShop>();
		invCrafting = GameObject.FindWithTag("InvCrafting").GetComponent<InventoryCrafting>();
		invCharacter = GameObject.FindWithTag("InvCharacter").GetComponent<InventoryCharacter>();
		toolTip = GameObject.FindWithTag("Tooltip").GetComponent<TooltipCustom>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            playerMenu.ButnResume();
        }

        if (Input.GetKeyDown(InputManager.CharacterSystemCode))
        {
            if (invCharacter.IsActive())
            {
                if (toolTip.isActive)
					toolTip.HideToolTip();
                invCharacter.CloseInventory();
            }
            else
            {
				invCharacter.OpenInventory();
            }
        }

        if (Input.GetKeyDown(InputManager.InventoryCode))
        {
            if (invBag.IsActive())
            {
                if (toolTip.isActive)
					toolTip.HideToolTip();
                invBag.CloseInventory();
            }
            else
            {
                invBag.OpenInventory();
            }
        }

        

        //if (Input.GetKeyDown(InputManager.CraftSystemCode))
        //{
        //    if (!craftSystem.activeSelf)
        //        craftSystemInventory.openInventory();
        //    else
        //    {
        //        if (cS != null)
        //            cS.backToInventory();
        //        if (toolTip != null)
        //            toolTip.deactivateTooltip();
        //        craftSystemInventory.closeInventory();
        //    }
        //}
    }

    public void setGuiHealth(float health, float maxHealth)
    {

        healthBar.fillAmount = health / maxHealth;
        healthBarText.text = health.ToString("#");

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
        expBarText.text = percent.ToString("P1");
    }
}
