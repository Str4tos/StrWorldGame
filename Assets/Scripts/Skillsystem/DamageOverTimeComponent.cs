using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utilities;


    /// <summary>
    /// SkillComponent implementing DamageOverTime
    /// </summary>
    public class DamageOverTimeComponent : SkillComponent
    {
        public int Interval;
        public int Overalldamage;
        public int Duration;

        public override void Apply(List<GameObject> targets)
        {
           foreach (GameObject obj in targets)
            {
                if (!TextUtility.CheckIfTagsMatch(Tags, obj.tag))
                {
                    Debug.Log("Enemy with tag " + obj.tag + " cannot be targeted");
                    continue;
                }

                Debug.Log("DoT Added");
                obj.GetComponent<CharacterStats>().AddDoT(new DamageOverTime(Interval, Overalldamage, Duration));
                finished = true;
            }
        }
    }

