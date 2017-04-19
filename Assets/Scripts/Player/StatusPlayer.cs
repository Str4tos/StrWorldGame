using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class StatusPlayer : Status
{
    #region Fields
    // Expirience
    private const float expMultiplier = 1.1f;
    public int statPoints = 0;
    public float currExp = 0;
    [HideInInspector]
    public float requiredExpForLvl = 37;

    [SerializeField]
    private GameObject lvlUpPrefab;

    private AnimatorPlayer animatorPlayer;

    // List of all attributes
    public enum Attribute { Attackspeed, Movementspeed }

    protected int weaponAtk = 0, equipDef = 0;
    protected float weaponPOWpercent = 1, equipHPpercent = 1, equipMPpercent = 1;

    public VoidMethod OnChangeExp;
    #endregion

    private void Awake()
    {
        animatorPlayer = GetComponent<AnimatorPlayer>();
    }
    private void Start()
    {
        CalculateOptions();
        RestoreHPandMP();
        CalculateCurrExp();
        popupColorBasic = Color.red;
        popupColorCritical = Color.red;
        popupDmgMotion = 1;
        StartManaRegeneration();

    }
    
    public void GainEXP(float gain, int enemyLvl)
    {
        if (enemyLvl - level > 50)
        {
            currExp += gain / 5.0f;
        }
        else
        {
            currExp += gain;
        }
        if (currExp > requiredExpForLvl)
        {
            LevelUp();
        }
        UpdateExpOnINterface();
    }
    public void LevelUp()
    {
        level++;
        currExp -= requiredExpForLvl;
        statPoints += 5;

        CalculateCurrExp();
        CalculateOptions();
        RestoreHPandMP();

        ShowLevelUpEffect();
    }
    public void ShowLevelUpEffect()
    {
        GameObject lvlUpEffectInstanc = Instantiate(lvlUpPrefab);
        lvlUpEffectInstanc.transform.position = transform.position + Vector3.up;
        Destroy(lvlUpEffectInstanc, 1.1f);
    }
    /// <summary>
    /// This method call CalculateStats()
    /// </summary>
    public void CalculateOptions()
    {
        CalculateStats();
        AttackDmg += weaponAtk;
        Defense = agility / 7 + equipDef;
        WizardyDmg *= weaponPOWpercent;
        MaxHealth *= equipHPpercent;
        MaxMana *= equipMPpercent;
    }
    
    public void UpdateHealthOnInterface()
    {
        PlayerInterface.Instance.UpdateHealthAmount(Health, MaxHealth);
    }
    public void UpdateManaOnInterface()
    {
        PlayerInterface.Instance.UpdateManaAmount(Mana, MaxMana);
    }
    public void UpdateExpOnINterface()
    {
        PlayerInterface.Instance.UpdateExpAmount(currExp, requiredExpForLvl);
        if (OnChangeExp != null)
            OnChangeExp();
    }

    protected void CalculateCurrExp()
    {
        requiredExpForLvl = requiredExpForLvl * expMultiplier;
        UpdateExpOnINterface();
    }
    protected override void Death()
    {

    }
    protected override void CalculateStats()
    {
        base.CalculateStats();
        animatorPlayer.SetAttackSpeed(AttackSpeed);
    }
    protected override void SetHealth(float health)
    {
        base.SetHealth(health);
        UpdateHealthOnInterface();
    }
    protected override void SetMana(float mana)
    {
        base.SetMana(mana);
        UpdateManaOnInterface();
    }
    
    ////----------States--------
    //public IEnumerator OnPoison(int hurtTime)
    //{
    //    int amount = 0;
    //    GameObject eff = new GameObject();
    //    Destroy(eff.gameObject);
    //    if (!poison)
    //    {
    //        //int chance = 100;
    //        //chance -= statusResist.poisonResist;
    //        //if (chance > 0)
    //        //{
    //        //    int per = Random.Range(0, 100);
    //        //    if (per <= chance)
    //        //    {
    //        poison = true;
    //        amount = (int)(maxHealth * 0.02f); // Hurt 2% of Max HP
    //        //}

    //        //}
    //        //--------------------
    //        while (poison && hurtTime > 0)
    //        {
    //            if (poisonEffect)
    //            { //Show Poison Effect
    //                eff = Instantiate(poisonEffect, transform.position, poisonEffect.transform.rotation) as GameObject;
    //                eff.transform.parent = transform;
    //            }
    //            yield return new WaitForSeconds(0.7f); // Reduce HP  Every 0.7f Seconds
    //ReceivDamage(amount, popupColorBasic);
    //            if (eff)
    //            { //Destroy Effect if it still on a map
    //                Destroy(eff.gameObject);
    //            }
    //            hurtTime--;
    //            if (hurtTime <= 0)
    //            {
    //                poison = false;
    //            }
    //        }
    //    }
    //}


    // be Delete
    //public IEnumerator OnSilence(float dur)
    //{
    //    GameObject eff = new GameObject();
    //    Destroy(eff.gameObject);
    //    if (!silence)
    //    {
    //        int chance = 100;
    //        chance -= statusResist.silenceResist;
    //        if (chance > 0)
    //        {
    //            int per = Random.Range(0, 100);
    //            if (per <= chance)
    //            {
    //                silence = true;
    //                if (silenceEffect)
    //                {
    //                    eff = Instantiate(silenceEffect, transform.position, transform.rotation) as GameObject;
    //                    eff.transform.parent = transform;
    //                }
    //                yield return new WaitForSeconds(dur);
    //                if (eff)
    //                { //Destroy Effect if it still on a map
    //                    Destroy(eff.gameObject);
    //                }
    //                silence = false;
    //            }

    //        }

    //    }
    //}

    //public IEnumerator OnWebbedUp(float dur)
    //{
    //    GameObject eff = new GameObject();
    //    Destroy(eff.gameObject);
    //    if (!web)
    //    {
    //        int chance = 100;
    //        chance -= statusResist.webResist;
    //        if (chance > 0)
    //        {
    //            int per = Random.Range(0, 100);
    //            if (per <= chance)
    //            {
    //                web = true;
    //                freeze = true; // Freeze Character On (Character cannot do anything)
    //                if (webbedUpEffect)
    //                {
    //                    eff = Instantiate(webbedUpEffect, transform.position, transform.rotation) as GameObject;
    //                    eff.transform.parent = transform;
    //                }
    //                if (webbedUpAnimation)
    //                {// If you Assign the Animation then play it
    //                    if (useMecanim)
    //                    {
    //                        GetComponent<PlayerMecanimAnimationC>().PlayAnim(webbedUpAnimation.name);
    //                    }
    //                    else
    //                    {
    //                        mainModel.GetComponent<Animation>()[webbedUpAnimation.name].layer = 25;
    //                        mainModel.GetComponent<Animation>().Play(webbedUpAnimation.name);
    //                    }
    //                }
    //                yield return new WaitForSeconds(dur);
    //                if (eff)
    //                { //Destroy Effect if it still on a map
    //                    Destroy(eff.gameObject);
    //                }
    //                if (webbedUpAnimation && !useMecanim)
    //                {// If you Assign the Animation then stop playing
    //                    mainModel.GetComponent<Animation>().Stop(webbedUpAnimation.name);
    //                }
    //                freeze = false; // Freeze Character Off
    //                web = false;
    //            }

    //        }

    //    }
    //}

    //public IEnumerator OnStun(float dur)
    //{
    //    GameObject eff = new GameObject();
    //    Destroy(eff.gameObject);
    //    if (!stun)
    //    {
    //        int chance = 100;
    //        chance -= statusResist.stunResist;
    //        if (chance > 0)
    //        {
    //            int per = Random.Range(0, 100);
    //            if (per <= chance)
    //            {
    //                stun = true;
    //                freeze = true; // Freeze Character On (Character cannot do anything)
    //                if (stunEffect)
    //                {
    //                    eff = Instantiate(stunEffect, transform.position, stunEffect.transform.rotation) as GameObject;
    //                    eff.transform.parent = transform;
    //                }
    //                if (stunAnimation)
    //                {// If you Assign the Animation then play it
    //                    if (useMecanim)
    //                    {
    //                        GetComponent<PlayerMecanimAnimationC>().PlayAnim(stunAnimation.name);
    //                    }
    //                    else
    //                    {
    //                        mainModel.GetComponent<Animation>()[stunAnimation.name].layer = 25;
    //                        mainModel.GetComponent<Animation>().Play(stunAnimation.name);
    //                    }
    //                }
    //                yield return new WaitForSeconds(dur);
    //                if (eff)
    //                { //Destroy Effect if it still on a map
    //                    Destroy(eff.gameObject);
    //                }
    //                if (stunAnimation && !useMecanim)
    //                {// If you Assign the Animation then stop playing
    //                    mainModel.GetComponent<Animation>().Stop(stunAnimation.name);
    //                }
    //                freeze = false; // Freeze Character Off
    //                stun = false;
    //            }

    //        }

    //    }

    //}

    //public IEnumerator OnBarrier(float dur)
    //{
    //    //Increase Defense
    //    if (!buffBarrier)
    //    {
    //        buffBarrier = true;
    //        buffBarrPercent = 1.0F - energy / 37000.0F;
    //        if (buffBarrPercent > 0.2F)
    //            buffBarrPercent = 0.2F;
    //        //CalculateStatus();
    //        yield return new WaitForSeconds(dur);
    //        buffBarrPercent = 1.0F;
    //        buffBarrier = false;
    //        //CalculateStatus();
    //    }

    //}
    //public IEnumerator OnHealthBuff(float dur)
    //{
    //    //Increase Magic Defense
    //    if (!buffHealth)
    //    {
    //        buffHealth = true;
    //        buffHealthPercent = 1.25F;
    //        CalculateStats();
    //        yield return new WaitForSeconds(dur);
    //        buffHealthPercent = 1.0F;
    //        buffHealth = false;
    //        CalculateStats();
    //    }

    //}
    //public IEnumerator OnDmgBuff(float dur)
    //{
    //    //Increase Attack
    //    if (!buffDmg)
    //    {
    //        buffDmg = true;
    //        buffDmgPercent = 1.0F + energy / 37000.0F;
    //        CalculateStats();
    //        yield return new WaitForSeconds(dur);
    //        buffDmgPercent = 1.0F;
    //        buffDmg = false;
    //        CalculateStats();
    //    }

    //}

    //public IEnumerator OnFaith(int amount, float dur)
    //{
    //    //Increase Magic Attack
    //    if (!faith)
    //    {
    //        faith = true;
    //        buffMatk = 0;
    //        buffMatk += amount;
    //        CalculateStatus();
    //        yield return new WaitForSeconds(dur);
    //        buffMatk = 0;
    //        faith = false;
    //        CalculateStatus();
    //    }

    //}

    //public void ApplyBuff(int statId, float dur, int amount)
    //{
    //    if (statId == 1)
    //    {
    //        //Increase Defense
    //        StartCoroutine(OnBarrier(dur));
    //    }
    //    if (statId == 2)
    //    {
    //        //Increase Max Health
    //        StartCoroutine(OnHealthBuff(dur));
    //    }
    //    if (statId == 3)
    //    {
    //        //Increase Attack
    //        StartCoroutine(OnDmgBuff(dur));
    //    }

    //}


    public void LoadDataPlayer(StatusPlayerData statusData)
    {
        personalName = statusData.personalName;
        locationName = statusData.locationName;

        level = statusData.level;
        strenght = statusData.strenght;
        agility = statusData.agility;
        vitality = statusData.vitality;
        energy = statusData.energy;


        statPoints = statusData.statPoints;
        currExp = statusData.currExp;
        requiredExpForLvl = statusData.currExpToLevelUp;
    }
    public StatusPlayerData GetDataPlayer()
    {
        StatusPlayerData result = new StatusPlayerData()
        {
            personalName = personalName,
            locationName = locationName,

            level = level,
            strenght = strenght,
            agility = agility,
            vitality = vitality,
            energy = energy,


            statPoints = statPoints,
            currExp = currExp,
            currExpToLevelUp = requiredExpForLvl
        };
        return result;
    }

}
