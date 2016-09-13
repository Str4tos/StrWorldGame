using UnityEngine;
using System.Collections.Generic;


    public delegate void CharacterStatsChangedEvent(string name, CharacterStats stats);
    public delegate void BuffEndedEventHandler(BasicBuff buff);

    /// <summary>
    /// Contains attributes and effects that are temporarly or permanently applied to the character
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        // List of all attributes
        public enum Attribute { Attackspeed, Movementspeed }

        List<Skill> skills = new List<Skill>();
        List<GameObject> icons = new List<GameObject>();
        [HideInInspector]
        public GameObject Target;

        // Used for skill cooldowns

        List<float> timeSinceLastExecution = new List<float>();

        // Used to store the last skill if it had casttime, to make sure no other skill can be cast during the casttime
        Skill lastSkill = null;

        public int MaxHealth;
        int health;
        List<DamageOverTime> DoTs = new List<DamageOverTime>();
        List<HealeOverTime> HoTs = new List<HealeOverTime>();
        List<AttributeModifier> modifiers = new List<AttributeModifier>();

        // Basic values of the attributes, accessible in the inspector
        public float BaseAttackspeed = 1;
        public float BaseMovementspeed = 8;

        public event CharacterStatsChangedEvent HealthChanged;
        public event CharacterStatsChangedEvent Died;
        public event BuffEndedEventHandler BuffAdded;
        public event BuffEndedEventHandler BuffEnded;
        public event SkillEventHandler SkillExecuted;

        public void AddSkill(Skill skill)
        {
            skills.Add(skill);
            timeSinceLastExecution.Add(9999);
        }

        public List<Skill> GetSkills
        {
            get { return skills; }
        }

        public int Health
        {
            get { return health; }
            set
            {
                health = value;

                if (health <= 0)
                {
                    health = 0;
                    if (Died != null)
                    {
                        Died(name, this);
                    }
                }
                else if (health > MaxHealth)
                    health = MaxHealth;
            }
        }

        public List<DamageOverTime> GetDoTs
        {
            get { return DoTs; }
        }

        public List<HealeOverTime> GetHoTs
        {
            get { return HoTs; }
        }

        // Getters for all attributes, calculating the current values by the base value and the modification by currently active buffs
        #region AttributeGetters

        /// <summary>
        /// Returns the attackspeed with all currently active buffs applied
        /// </summary>
        public float GetAttackspeed
        {
            get
            {
                float curAttackspeed = BaseAttackspeed;
                foreach (AttributeModifier mod in modifiers)
                {
                    //if (mod.stat == Attribute.Attackspeed)
                    //{
                        curAttackspeed += mod.modifier;
                    //}
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
                float curMovementspeed = BaseMovementspeed;
                foreach (AttributeModifier mod in modifiers)
                {
                    //if (mod.stat == Attribute.Movementspeed)
                    //{
                        curMovementspeed += mod.modifier;
                   // }
                }
                if (curMovementspeed < 0)
                    return 0;
                return curMovementspeed;
            }
        }

        #endregion

        void Start()
        {
            health = MaxHealth;
        }

        /// <summary>
        /// Changes the current health. Cannot drop below 0.
        /// </summary>
        /// <param name="amount">Amount to change the health by</param>
        public void ChangeHealth(int amount)
        {
            Health += amount;

            if (HealthChanged != null)
            {
                HealthChanged(name, this);
            }
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
                    ChangeHealth(-(dot.GetOveralldamage / (int)(dot.Duration / dot.GetInterval)));
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
                    ChangeHealth((hot.GetOverallHeal / (int)(hot.Duration / hot.GetInterval)));
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
    }
