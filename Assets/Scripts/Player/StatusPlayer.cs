using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class StatusPlayer : Status
{

    private PlayerGuiCustom _PlayerGui;

    // Expirience
    public int statPoints = 0;
    public float currExp = 0;
    [HideInInspector]
    public float currExpToLevelUp = 37;
    private float additionExp = 37;
    public float movementSpeed = 6;
    public float manaRegen = 0.005f; //percent MaxMana

    private GameObject lvlUpPrefab;

    // List of all attributes
    public enum Attribute { Attackspeed, Movementspeed }

    private Transform StatsPanel;

    //----------- Skills
    List<Skill> skills = new List<Skill>();
    List<GameObject> icons = new List<GameObject>();
    [HideInInspector]
    public GameObject Target;

    // Used for skill cooldowns

    List<float> timeSinceLastExecution = new List<float>();

    // Used to store the last skill if it had casttime, to make sure no other skill can be cast during the casttime
    Skill lastSkill = null;

    List<DamageOverTime> DoTs = new List<DamageOverTime>();
    List<HealeOverTime> HoTs = new List<HealeOverTime>();
    List<AttributeModifier> modifiers = new List<AttributeModifier>();

    // Basic values of the attributes, accessible in the inspector
    //public float BaseAttackspeed = 1;
    //public float BaseMovementspeed = 8;

    public event CharacterStatsChangedEvent HealthChanged;
    public event CharacterStatsChangedEvent Died;
    public event BuffEndedEventHandler BuffAdded;
    public event BuffEndedEventHandler BuffEnded;
    public event SkillEventHandler SkillExecuted;

    void Start()
    {
        _PlayerGui = gameObject.GetComponent<PlayerGuiCustom>();
        _Animator = gameObject.GetComponent<Animator>();
        //        CalculateStats();
        //		currExpToLevelUp = level * additionExp;
        CalculateStats();
        restoreHPandMP();
		popupColorBasic = Color.red;
		popupColorCritical = Color.red;
        popupDmgMotion = 1;
        currExpToLevelUp = level * additionExp;
        _Animator.SetFloat("AttackSpeed", attackSpeed);
        _PlayerGui.setGuiHealth(health, maxHealth);
        _PlayerGui.setGuiMana(mana, maxMana);
        _PlayerGui.setGuiExp(currExp, currExpToLevelUp);
        InvokeRepeating("ManaRegen", 0.5f, 0.5f);

        //--Visualization Lvl Up
        lvlUpPrefab = (GameObject)Resources.Load("Prefabs/Effect/energyBlast");

    }


    void Update()
    {
        //Optimal Exemple
        //         float timFlag;
        //public float pause = 3;
        //    void Update () {
        //    if( timFlag + pause  <= Time.time)
        //    {
        //        timFlag = Time.time;
        //    }
        //    }


        #region Buffs

        // Solving this with timers and events from within DamageOverTime would have been nicer, but since Unity doesn't like threads other than the main thread using
        // its properties and methods, we are just going to check for the elasped time here.
        List<BasicBuff> delete = new List<BasicBuff>();
        foreach (DamageOverTime dot in DoTs)
        {

            dot.elapsedTime += Time.deltaTime;
            dot.elapsedTimeSinceActivation += Time.deltaTime;

            if (dot.elapsedTimeSinceActivation > dot.GetInterval)
            {
				ReceivDamage(dot.GetOveralldamage / (int)(dot.Duration / dot.GetInterval), popupColorBasic);
                dot.elapsedTimeSinceActivation -= dot.GetInterval;
            }
            if (dot.elapsedTime > dot.Duration)
            {
                delete.Add(dot);
            }
        }

        foreach (DamageOverTime dot in delete)
        {
            if (BuffEnded != null)
            {
                BuffEnded(dot);
            }
            DoTs.Remove(dot);
        }

        delete.Clear();
        foreach (HealeOverTime hot in HoTs)
        {

            hot.elapsedTime += Time.deltaTime;
            hot.elapsedTimeSinceActivation += Time.deltaTime;

            if (hot.elapsedTimeSinceActivation > hot.GetInterval)
            {
				ReceivDamage(hot.GetOverallHeal / (int)(hot.Duration / hot.GetInterval), popupColorBasic);
                hot.elapsedTimeSinceActivation -= hot.GetInterval;
            }
            if (hot.elapsedTime > hot.Duration)
            {
                delete.Add(hot);
            }
        }

        foreach (HealeOverTime dot in delete)
        {
            if (BuffEnded != null)
            {
                BuffEnded(dot);
            }
            HoTs.Remove(dot);
        }

        delete.Clear();
        foreach (AttributeModifier mod in modifiers)
        {
            mod.elapsedTime += Time.deltaTime;
            if (mod.elapsedTime > mod.Duration)
            {
                delete.Add(mod);
            }
        }

        foreach (AttributeModifier mod in delete)
        {
            if (BuffEnded != null)
            {
                BuffEnded(mod);
            }
            modifiers.Remove(mod);
        }

        #endregion


        for (int i = 0; i < timeSinceLastExecution.Count; i++)
        {
            timeSinceLastExecution[i] += Time.deltaTime;
        }
    }

    protected override void Death()
    {
        //health = 0;
        //enabled = false;
        //dead = true;

        //if (gameObject.tag == "Player")
        //{
        //    //SaveData();
        //}
        //Destroy(gameObject);
        //if (deathBody)
        //{
        //    Instantiate(deathBody, transform.position, transform.rotation);
        //}
        //else
        //{
        //    print("This Object didn't assign the Death Body");
        //}

        //Stratos | перебрать
        //		playerIsDead = true;
        //		Destroy(ms.pCharacterController, 0.5f);
        //		ms.pAnimator.SetBool("DeadBool", true);
        //		ms.psMovement.StopMoveNavAgent();
        //		ms.psMovement.ResetTriggers();
        //
        //		ms.psItemPick.enabled = false;
        //		ms.psMovement.enabled = false;
        //		ms.psInputKeyboard.enabled = false;
        //		ms.psInputMouse.enabled = false;
        //		ms.psSkills.enabled = false;
        //		ms.psEvents.enabled = false;
        //		ms.camMove.enabled = false;
        //
        //		ms.deathMenu.SetActive(true);
    }


    #region Adds, Gets
    public void AddSkill(Skill skill)
    {
        skills.Add(skill);
        timeSinceLastExecution.Add(999);
    }

    public List<Skill> GetSkills
    {
        get { return skills; }
    }

    public List<DamageOverTime> GetDoTs
    {
        get { return DoTs; }
    }

    public List<HealeOverTime> GetHoTs
    {
        get { return HoTs; }
    }

    /// <summary>
    /// Adds an AttributeModifier. Bufflogic and its removal is handled by the CharacterStats.
    /// </summary>
    /// <param name="mod">AttibuteModifier to add</param>
    public void AddAttributeModifier(AttributeModifier mod)
    {
        modifiers.Add(mod);
        if (BuffAdded != null)
        {
            BuffAdded(mod);
        }
    }
    /// <summary>
    /// Adds an DamageOverTime. Bufflogic and its removal is handled by the CharacterStats.
    /// </summary>
    /// <param name="mod">DamageOverTime to add</param>
    public void AddDoT(DamageOverTime dot)
    {
        DoTs.Add(dot);
        if (BuffAdded != null)
        {
            BuffAdded(dot);
        }
    }

    /// <summary>
    /// Adds an HealeOverTime. Bufflogic and its removal is handled by the CharacterStats.
    /// </summary>
    /// <param name="mod">HealeOverTime to add</param>
    public void AddHoT(HealeOverTime hot)
    {
        HoTs.Add(hot);
        if (BuffAdded != null)
        {
            BuffAdded(hot);
        }
    }
    #endregion

    #region AttributeGetters

    /// <summary>
    /// Returns the attackspeed with all currently active buffs applied
    /// </summary>
    public float GetAttackspeed
    {
        get
        {
            float curAttackspeed = attackSpeed;
            foreach (AttributeModifier mod in modifiers)
            {
                if (mod.stat == Attribute.Attackspeed)
                {
                    curAttackspeed += mod.modifier;
                }
            }

            return curAttackspeed;
        }
    }

    /// <summary>
    /// Returns the movementspeed with all currently active buffs applied
    /// </summary>
    public float GetMovementspeed
    {
        get
        {
            float curMovementspeed = movementSpeed;
            foreach (AttributeModifier mod in modifiers)
            {
                if (mod.stat == Attribute.Movementspeed)
                {
                    curMovementspeed += mod.modifier;
                }
            }
            if (curMovementspeed < 0)
                return 0;
            return curMovementspeed;
        }
    }

    #endregion

    #region SkillSystem

    /// <summary>
    /// Executes a skill if the cooldown has expired
    /// </summary>
    /// <param name="skill">Skill to execute</param>
    public void UseAttack(int skill)
    {
        if (lastSkill != null)
        {
            return;
        }
        // Check whether the cooldown is finished
        if (timeSinceLastExecution[skill - 1] > skills[skill - 1].CoolDown)
        {
            Skill curSkill = (Skill)Instantiate(skills[skill - 1]);
            curSkill.SkillActivated += new SkillEventHandler(SkillActivated);

            // Activate the skill and check for the result
            _Animator.SetFloat("UseSkill", 1.0f);
            int ret = curSkill.ActivateSkill(gameObject, Target);
            if (ret == 0)
            {
                // Skill is active, start casttime and cooldown
                if (curSkill.executeComponentsOn == Skill.ExecuteComponentsOn.Channeled)
                {
                    Casttimebar.ActivateChannelTime(curSkill.Duration);
                    lastSkill = curSkill;
                }
                else
                {
                    Casttimebar.ActivateCasttime(curSkill.CastTime);
                }

                if (curSkill.CastTime == 0)
                {
                    timeSinceLastExecution[skill - 1] = 0;
                }
                else
                {
                    lastSkill = curSkill;
                }

                curSkill.CasttimeElapsed += new SkillEventHandler(curSkill_CasttimeElapsed);
                curSkill.CasttingAborted += new SkillEventHandler(curSkill_CasttingAborted);
                curSkill.SkillFinished += new SkillEventHandler(curSkill_SkillFinished);
            }
            else
            {
                Debug.Log(ret);
            }
        }
    }

    /// <summary>
    /// Forwards all active skills
    /// </summary>
    /// <param name="sender">Skill that was activated</param>
    void SkillActivated(Skill sender)
    {
        if (SkillExecuted != null)
        {
            SkillExecuted(sender);
        }
    }

    void curSkill_SkillFinished(Skill sender)
    {
        _Animator.SetFloat("UseSkill", 0f);
        Casttimebar.Abort();
        sender.CasttimeElapsed -= new SkillEventHandler(curSkill_CasttimeElapsed);
        sender.CasttingAborted -= new SkillEventHandler(curSkill_CasttingAborted);
        sender.SkillFinished -= new SkillEventHandler(curSkill_SkillFinished);
    }

    void curSkill_CasttimeElapsed(Skill sender)
    {
        lastSkill = null;
        // Search the skill that fired the event in the local list to identify the correct cooldown
        Skill s = null;

        foreach (Skill skill in skills)
        {
            if (skill.SkillName == sender.SkillName)
            {
                s = skill;
                break;
            }
        }

        timeSinceLastExecution[skills.IndexOf(s)] = 0;

        sender.SkillFinished -= new SkillEventHandler(curSkill_SkillFinished);
    }

    void curSkill_CasttingAborted(Skill sender)
    {
        Casttimebar.Abort();
        lastSkill = null;
        sender.SkillFinished -= new SkillEventHandler(curSkill_SkillFinished);
    }
    #endregion

	public override void ReceivDamage(float amount, Color colorPopup)
    {
		base.ReceivDamage(amount, colorPopup);
        _PlayerGui.setGuiHealth(health, maxHealth);
    }


    protected override void CalculateStats()
    {
        base.CalculateStats();
        currExpToLevelUp = level * additionExp;
        _Animator.SetFloat("AttackSpeed", attackSpeed);
        _PlayerGui.setGuiHealth(health, maxHealth);
        _PlayerGui.setGuiMana(mana, maxMana);
    }

    public void gainEXP(float gain, int enemyLvl)
    {
        if (enemyLvl - level > 50)
        {
            currExp += gain / 5.0f;
        }
        else
        {
            currExp += gain;
        }
        if (currExp > currExpToLevelUp)
        {
            levelUp();
        }
        _PlayerGui.setGuiExp(currExp, currExpToLevelUp);
    }

    public void levelUp()
    {
        level++;
        currExp -= currExpToLevelUp;
        statPoints += 5;
        GameObject lvlUpEffectInstanc = (GameObject)Instantiate(lvlUpPrefab);
        lvlUpEffectInstanc.transform.position = transform.position + Vector3.up;
        Destroy(lvlUpEffectInstanc, 1.1f);
        //Stratos | Learn Skills per lvl
        //        gainEXP(0);
        //        if (GetComponent<SkillWindowC>())
        //        {
        //            GetComponent<SkillWindowC>().LearnSkillByLevel(level);
        //        }

        CalculateStats();
        restoreHPandMP();
		_PlayerGui.setGuiHealth(health, maxHealth);

        if (StatsPanel != null && StatsPanel.gameObject.activeSelf)
        {
            RefreshStatsInInventory(StatsPanel);
        }
    }

    public void heal(float hp)
    {
        if (health == maxHealth)
            return;
        health += hp;
        checkMaxHp();
        _PlayerGui.setGuiHealth(health, maxHealth);
    }

    /// <summary>
    /// InvokeRepeating mana regeneration
    /// </summary>
    private void ManaRegen()
    {
        if (mana < maxMana)
        {
            mana += maxMana * manaRegen;
            checkMaxMp();
            _PlayerGui.setGuiMana(mana, maxMana);
        }
    }

    /// <summary>
    /// Chenge mana points value.\
    /// Returns: True = Action complete | False = Deficiency MP.
    /// </summary>
    /// <param name="mp">Quantity mana points</param>
    /// <param name="KindOfAction">True = Increase | False = Dcrease</param>
    /// <returns>True = Action complete. False = Deficiency MP</returns>
    public bool ManaPoints(float mp, bool KindOfAction)
    {
        if (KindOfAction)
        {
            mana += mp;
            checkMaxMp();
        }
        else
        {
            if (mana < mp)
            {
                return false;
            }
            mana -= mp;
        }
        _PlayerGui.setGuiMana(mana, maxMana);
        return true;
    }

    //----------States--------
    public IEnumerator OnPoison(int hurtTime)
    {
        int amount = 0;
        GameObject eff = new GameObject();
        Destroy(eff.gameObject);
        if (!poison)
        {
            //int chance = 100;
            //chance -= statusResist.poisonResist;
            //if (chance > 0)
            //{
            //    int per = Random.Range(0, 100);
            //    if (per <= chance)
            //    {
            poison = true;
            amount = (int)(maxHealth * 0.02f); // Hurt 2% of Max HP
            //}

            //}
            //--------------------
            while (poison && hurtTime > 0)
            {
                if (poisonEffect)
                { //Show Poison Effect
                    eff = Instantiate(poisonEffect, transform.position, poisonEffect.transform.rotation) as GameObject;
                    eff.transform.parent = transform;
                }
                yield return new WaitForSeconds(0.7f); // Reduce HP  Every 0.7f Seconds
				ReceivDamage(amount, popupColorBasic);
                if (eff)
                { //Destroy Effect if it still on a map
                    Destroy(eff.gameObject);
                }
                hurtTime--;
                if (hurtTime <= 0)
                {
                    poison = false;
                }
            }
        }
    }


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

    public void ApplyAbnormalStat(int statId, float dur)
    {
        if (statId == 0)
        {
            OnPoison(Mathf.FloorToInt(dur));
            StartCoroutine(OnPoison(Mathf.FloorToInt(dur)));
        }
        //if (statId == 1)
        //{
        //    //OnSilence(dur);
        //    StartCoroutine(OnSilence(dur));
        //}
        //if (statId == 2)
        //{
        //    //OnStun(dur);
        //    StartCoroutine(OnStun(dur));
        //}
        //if (statId == 3)
        //{
        //    //OnWebbedUp(dur);
        //    StartCoroutine(OnWebbedUp(dur));
        //}


    }

    public IEnumerator OnBarrier(float dur)
    {
        //Increase Defense
        if (!buffBarrier)
        {
            buffBarrier = true;
            buffBarrPercent = 1.0F - energy / 37000.0F;
            if (buffBarrPercent > 0.2F)
                buffBarrPercent = 0.2F;
            //CalculateStatus();
            yield return new WaitForSeconds(dur);
            buffBarrPercent = 1.0F;
            buffBarrier = false;
            //CalculateStatus();
        }

    }

    public IEnumerator OnHealthBuff(float dur)
    {
        //Increase Magic Defense
        if (!buffHealth)
        {
            buffHealth = true;
            buffHealthPercent = 1.25F;
            CalculateStats();
            yield return new WaitForSeconds(dur);
            buffHealthPercent = 1.0F;
            buffHealth = false;
            CalculateStats();
        }

    }

    public IEnumerator OnDmgBuff(float dur)
    {
        //Increase Attack
        if (!buffDmg)
        {
            buffDmg = true;
            buffDmgPercent = 1.0F + energy / 37000.0F;
            CalculateStats();
            yield return new WaitForSeconds(dur);
            buffDmgPercent = 1.0F;
            buffDmg = false;
            CalculateStats();
        }

    }

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

    public void ApplyBuff(int statId, float dur, int amount)
    {
        if (statId == 1)
        {
            //Increase Defense
            StartCoroutine(OnBarrier(dur));
        }
        if (statId == 2)
        {
            //Increase Max Health
            StartCoroutine(OnHealthBuff(dur));
        }
        if (statId == 3)
        {
            //Increase Attack
            StartCoroutine(OnDmgBuff(dur));
        }

    }


    public void LoadDataPLayer(StatusPlayerData statusData)
    {
        personalName = statusData.personalName;
        locationName = statusData.locationName;

        level = statusData.level;
        strenght = statusData.strenght;
        agility = statusData.agility;
        vitality = statusData.vitality;
        energy = statusData.energy;

        rangeAttack = statusData.rangeAttack;

        statPoints = statusData.statPoints;
        currExp = statusData.currExp;
        currExpToLevelUp = statusData.currExpToLevelUp;
    }

    public StatusPlayerData GgetDataPLayer()
    {
        StatusPlayerData result = new StatusPlayerData();
        result.personalName = personalName;
        result.locationName = locationName;

        result.level = level;
        result.strenght = strenght;
        result.agility = agility;
        result.vitality = vitality;
        result.energy = energy;

        result.rangeAttack = rangeAttack;

        result.statPoints = statPoints;
        result.currExp = currExp;
        result.currExpToLevelUp = currExpToLevelUp;
        return result;
    }

    #region Stats Panel
    /*Specific order of elements
	 * 0 - Level
	 * 1 - Point
	 * 2 - Exp
	 * 3 - Strength
	 * 4 - Damage
	 * 5 - Agility
	 * 6 - Defence
	 * 7 - AttackSpeed
	 * 8 - Vitality
	 * 9 - Health
	 * 10 - Energy
	 * 11 - Mana
	 * 12 - WizardyDmg
	 * Complex stats: 0 - StatText, 1 - StatPoint, 2 - button increase stat
	 * 
	*/
    public void RefreshStatsInInventory(Transform statsTran)
    {
        if (statsTran == null)
            return;
        if (StatsPanel == null)
            StatsPanel = statsTran;
        RefreshExpLvl();
        RefreshStrenght();
        RefreshAgility();
        RefreshVitality();
        RefreshEnergy();
        checkForPoints();
    }

    private void RefreshExpLvl()
    {
        if (StatsPanel == null)
            return;
        StatsPanel.GetChild(0).GetComponent<Text>().text = "Level: " + level;
        StatsPanel.GetChild(2).GetComponent<Text>().text = "Exp: " + currExp + "/" + currExpToLevelUp;
    }
    private void RefreshStrenght()
    {
        if (StatsPanel == null)
            return;
        StatsPanel.GetChild(3).GetChild(1).GetComponent<Text>().text = strenght.ToString();
        StatsPanel.GetChild(4).GetComponent<Text>().text = "Damage: " + attackDmg;
    }
    private void RefreshAgility()
    {
        if (StatsPanel == null)
            return;
        StatsPanel.GetChild(5).GetChild(1).GetComponent<Text>().text = agility.ToString();
        StatsPanel.GetChild(6).GetComponent<Text>().text = "Defense: " + defense;
        StatsPanel.GetChild(7).GetComponent<Text>().text = "Attack speed: " + (attackSpeed * 10f).ToString("F");
    }
    private void RefreshVitality()
    {
        if (StatsPanel == null)
            return;
        StatsPanel.GetChild(8).GetChild(1).GetComponent<Text>().text = vitality.ToString();
        StatsPanel.GetChild(9).GetComponent<Text>().text = "Health: " + health.ToString("#") + "/" + maxHealth;
    }
    private void RefreshEnergy()
    {
        if (StatsPanel == null)
            return;
        StatsPanel.GetChild(10).GetChild(1).GetComponent<Text>().text = energy.ToString();
        StatsPanel.GetChild(11).GetComponent<Text>().text = "Mana: " + mana.ToString("#") + "/" + maxMana;
        StatsPanel.GetChild(12).GetComponent<Text>().text = "Wizardy Dmg: " + wizardyDmg;
    }

    private void checkForPoints()
    {
        if (StatsPanel == null)
            return;
        Transform PointTran = StatsPanel.GetChild(1);
        PointTran.GetComponent<Text>().text = "Point " + statPoints;
        if (statPoints > 0)
        {
            if (!PointTran.gameObject.activeSelf)
            {
                PointTran.gameObject.SetActive(true);
                //Strenght
                StatsPanel.GetChild(3).GetChild(2).gameObject.SetActive(true);
                //Agility
                StatsPanel.GetChild(5).GetChild(2).gameObject.SetActive(true);
                //Vitality
                StatsPanel.GetChild(8).GetChild(2).gameObject.SetActive(true);
                //Energy
                StatsPanel.GetChild(10).GetChild(2).gameObject.SetActive(true);
            }
        }
        else
        {
            if (PointTran.gameObject.activeSelf)
            {
                PointTran.gameObject.SetActive(false);
                //Strenght
                StatsPanel.GetChild(3).GetChild(2).gameObject.SetActive(false);
                //Agility
                StatsPanel.GetChild(5).GetChild(2).gameObject.SetActive(false);
                //Vitality
                StatsPanel.GetChild(8).GetChild(2).gameObject.SetActive(false);
                //Energy
                StatsPanel.GetChild(10).GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    public void IncreaseStr()
    {
        strenght++;
        statPoints--;
        CalculateStats();
        RefreshStrenght();
        checkForPoints();
    }

    public void IncreaseAgi()
    {
        agility++;
        statPoints--;
        CalculateStats();
        RefreshAgility();
        checkForPoints();
    }

    public void IncreaseVit()
    {
        vitality++;
        statPoints--;
        CalculateStats();
        RefreshVitality();
        checkForPoints();
        _PlayerGui.setGuiHealth(health, maxHealth);
    }

    public void IncreaseEne()
    {
        energy++;
        statPoints--;
        CalculateStats();
        RefreshEnergy();
        checkForPoints();
        _PlayerGui.setGuiMana(mana, maxMana);
    }

    #endregion
}
