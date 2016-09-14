using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour
{

    protected Animator _Animator;


    

    // characteristics
    public string personalName = "";
    protected string locationName = ""; 
	protected bool dead = false;

    [Range(1, 400)]
    public int level = 1;
    public int strenght = 10;
    public int agility = 10;
    public int vitality = 10;
    public int energy = 10;

    //calculate Stats
	protected int attackDmg = 0;
    protected int defense = 0;
    protected int wizardyDmg = 0;
    protected float maxHealth = 1.0f;
    protected float maxMana = 1.0f;
    protected float health = 1.0f;
    protected float mana = 1.0f;
    protected float attackSpeed = 1.0f;

    // Attack Stats
    public float rangeAttack = 1.0f;

    [HideInInspector]
    public int weaponAtk = 0, equipDef = 0;

    // in percent
    [HideInInspector]
    public float weaponPOWpercent = 1, equipHPpercent = 1, equipMPpercent = 1;
    [HideInInspector]
    public float buffDmgPercent = 1, buffBarrPercent = 1, buffHealthPercent = 1;

	//Popup
	protected Color popupColorBasic;
	protected Color popupColorCritical;
	protected Color popupColorMiss = new Color (0.8f, 0.8f, 0.8f);
	protected int popupDmgMotion;

    //Positive Buffs
    [HideInInspector]
    public bool buffDmg = false, buffBarrier = false, buffHealth = false;

    //Negative Buffs
    [HideInInspector]
    public bool poison = false, flame = false, freeze = false;

    //Effect
    public GameObject poisonEffect;
    public GameObject flameEffect;
    public GameObject freezEffect;

    protected virtual void CalculateStats()
    {
        attackDmg = strenght /5 + weaponAtk;
        attackDmg = (int)((float)attackDmg * buffDmgPercent);
        wizardyDmg = energy * 2;
        wizardyDmg = (int)((float)wizardyDmg * weaponPOWpercent * buffDmgPercent);
        defense = agility / 7 + equipDef;
        //defense = (int)((float)defense);
        attackSpeed = 1.0f + (float)agility / 1500.0f;
		maxHealth = 90.0f + (float)vitality * 2.0f;
        maxHealth = maxHealth * equipHPpercent * buffHealthPercent;
		maxMana = 20.0f + (float)energy * 1.5f;
        maxMana = maxMana * equipMPpercent;
        checkMaxHp();
        checkMaxMp();
    }

	public virtual void ReceivDamage(float amount, Status _Status){
		if (dead)
			return;
		bool Critical = false;
		if (buffBarrier)
			amount = (amount * buffBarrPercent);
		amount -= defense;

		//Chance 0-10% more dmg
		amount *= 1 + Random.value / 20f;

		//Chance crit attack 40%
		if (Random.value > 0.6f) {
			amount *= 1.8f;
			Critical = true;
		} 
		if(Critical)
			ReceivDamage (amount, popupColorCritical);
		else
			ReceivDamage (amount, popupColorBasic);

	}

	public virtual void ReceivDamage (float amount, Color popupColor)
	{
		if (dead)
			return;

		if (amount < 1.0f) {
			PopupInfo.setText ("Miss", transform, popupDmgMotion, popupColorMiss);
			return;
		}

		health -= amount;

		if (health < 1)
			Death ();
		PopupInfo.setText (amount.ToString ("N0"), transform, popupDmgMotion, popupColor);
			
	}

    public bool IsDead
    {
        get { return dead; }
        set { dead = value; }
    }

    protected virtual void Death() { }

    public float getMaxHealth { get { return maxHealth; } }
    public float getHealth { get { return health; } }
    public float getAttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
	public float getAttackDamage{
		get{ return attackDmg;}
	}

	public void restoreHPandMP()
    {
        health = maxHealth;
        mana = maxMana;
    }

    protected void checkMaxHp()
    {
        if (health > maxHealth)
            health = maxHealth;
    }

    protected void checkMaxMp()
    {
        if (mana > maxMana)
            mana = maxMana;
    }




}