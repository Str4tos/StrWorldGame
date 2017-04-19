using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public delegate void VoidMethod();
[RequireComponent(typeof(SkillPanelPlayer))]
public class PlayerInterface : MonoBehaviour
{
    #region Fields
    public static PlayerInterface Instance { get; private set; }

    [SerializeField]
    private GameObject menuObj;

    [Header("Player info panels")]
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Text healthValue;
    [SerializeField]
    private Image manaBar;
    [SerializeField]
    private Text manaValue;
    [SerializeField]
    private Image expFill;
    [SerializeField]
    private Text expValue;

    [Header("Player inventories")]
    public BagInventory bagInv;
    public CharacterInventory characterInv;
    public StorageInventory storageInv;
    public ShopInventory shopInv;
    public CraftingInventory craftInv;
    public Tooltip tooltipItem;

    public SkillPanelPlayer SkillPanel { get; private set; }

    #endregion

    private void Awake()
    {
        Instance = this;
        SkillPanel = GetComponent<SkillPanelPlayer>();
    }

    #region Info

    public void UpdateHealthAmount(float health, float maxHealth)
    {
        healthBar.fillAmount = health / maxHealth;
        healthValue.text = health.ToString("#");
    }
    public void UpdateManaAmount(float mana, float maxMana)
    {
        manaBar.fillAmount = mana / maxMana;
        manaValue.text = mana.ToString("#");
    }
    public void UpdateExpAmount(float currExp, float currExpToLevelUp)
    {
        float percent = currExp / currExpToLevelUp;
        expFill.fillAmount = percent;
        expValue.text = percent.ToString("P1");
    }
    #endregion


    #region Inventories
    public void ToggleChracterInventory()
    {
        if (characterInv.IsActive())
        {
            HideTooltip();
            characterInv.CloseInventory();
        }
        else
        {
            characterInv.OpenInventory();
        }
    }
    public void ToggleBagInventory()
    {
        if (bagInv.IsActive())
        {
            HideTooltip();
            bagInv.CloseInventory();
        }
        else
        {
            bagInv.OpenInventory();
        }
    }
    public void HideTooltip()
    {
        if (tooltipItem.isActive)
            tooltipItem.HideToolTip();
    }
    #endregion

    #region Main menu
    public void TogglePause()
    {
        menuObj.SetActive(!menuObj.activeSelf);
        Time.timeScale = 1.0f - Time.timeScale;
    }
    public void ButnOptions()
    {

    }
    public void ButnExit()
    {
        SaveLoadPlayer _PlayerSaveLoad = GameObject.FindWithTag("Player").GetComponent<SaveLoadPlayer>();
        _PlayerSaveLoad.SavePlayerData();
        Application.Quit();
    }

    #endregion
}
