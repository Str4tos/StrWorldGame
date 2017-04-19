using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour
{
    #region Fields
    // characteristics
    public string personalName = "";
    protected string locationName = "";
    public bool IsDead { get; protected set; }

    [Header("Stats")]
    [Range(1, 400)]
    public int level;
    public int strenght;
    public int agility;
    public int vitality;
    public int energy;

    //calculate Stats
    public float AttackDmg { get; protected set; }
    public float Defense { get; protected set; }
    public float WizardyDmg { get; protected set; }
    public float AttackSpeed { get; protected set; }
    public float MaxHealth { get; protected set; }
    public float MaxMana { get; protected set; }
    public float Health { get; private set; }
    public float Mana { get; private set; }

    [SerializeField]
    private float manaRegen = 0.005f;
    
    

    protected float buffDmgPercent = 1, buffBarrPercent = 1, buffHealthPercent = 1;

    //Popup
    protected Color popupColorBasic;
    protected Color popupColorCritical;
    protected Color popupColorMiss = new Color(0.8f, 0.8f, 0.8f);
    protected int popupDmgMotion;

    //Positive Buffss
    [HideInInspector]
    public bool buffDmg = false, buffBarrier = false, buffHealth = false;
    
    #endregion
    
    public void ReceivDamage(float amount)
    {
        if (IsDead)
            return;
        bool critical = false;
        if (buffBarrier)
            amount = (amount * buffBarrPercent);
        amount -= Defense;

        //Chance 0-10% more dmg
        amount *= 1 + Random.value / 20f;

        //Chance crit attack 40%
        if (Random.value > 0.6f)
        {
            amount *= 1.8f;
            critical = true;
        }
        if (critical)
            ReceivDamage(amount, popupColorCritical);
        else
            ReceivDamage(amount, popupColorBasic);

    }
    public void ReceivDamage(float amount, Color popupColor)
    {
        if (IsDead)
            return;

        if (amount < 1.0f)
        {
            PopupInfo.setText("Miss", transform, popupDmgMotion, popupColorMiss);
            return;
        }
        SetHealth(Health - amount);
        if (Health == 0.0f)
            Death();
        PopupInfo.setText(amount.ToString("N0"), transform, popupDmgMotion, popupColor);
    }
    
    public void RestoreHPandMP()
    {
        SetHealth(MaxHealth);
        SetMana(MaxMana);
    }
    public void RestoreHP(float hp)
    {
        if (Health == MaxHealth)
            return;
        SetHealth(Health + hp);
    }
    public void RestoreMP(float mp)
    {
        if (Mana == MaxMana)
            return;
        SetMana(Mana + mp);
    }
    public bool TrySpendMP(float mp)
    {
        if (Mana < mp)
            return false;
        SetMana(Mana - mp);
        return true;
    }
    
    protected virtual void CalculateStats()
    {
        AttackDmg = strenght / 5;
        WizardyDmg = energy * 2;
        Defense = agility / 7;
        AttackSpeed = 1.0f + agility / 1500.0f;
        MaxHealth = 90.0f + vitality * 2.0f;
        MaxMana = 20.0f + energy * 1.5f;
    }
    protected virtual void Death()
    {
        StopManaRegeneration();
    }
    protected virtual void SetHealth(float health)
    {
        Health = Mathf.Clamp(health, 0.0f, MaxHealth);
    }
    protected virtual void SetMana(float mana)
    {
        Mana = Mathf.Clamp(mana, 0.0f, MaxMana);
    }

    protected void StartManaRegeneration()
    {
        InvokeRepeating("ManaRegeneration", 1.0f, 1.0f);
    }
    protected void StopManaRegeneration()
    {
        CancelInvoke("ManaRegeneration");
    }

    /// <summary>
    /// InvokeRepeating mana regeneration
    /// </summary>
    private void ManaRegeneration()
    {
        if (Mana < MaxMana)
        {
            SetMana(Mana + MaxMana * manaRegen);
        }
    }

}