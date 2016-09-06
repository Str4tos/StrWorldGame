using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utilities;


    /// <summary>
    /// SkillComponent implementing HealOverTime
    /// </summary>
    public class HealOverTimeComponent : SkillComponent
    {
        public int Interval;
        public int OverallHeal;
        public int Duration;

        public override void Apply(List<GameObject> targets)
        {
           foreach (GameObject obj in targets)
            {
                if (!TextUtility.CheckIfTagsMatch(Tags, obj.tag))
                {
                    Debug.Log("Target with tag " + obj.tag + " cannot be targeted");
                    continue;
                }

                obj.GetComponent<CharacterStats>().AddHoT(new HealeOverTime(Interval, OverallHeal, Duration));
                finished = true;
            }
        }
    }

