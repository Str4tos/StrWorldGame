using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utilities;

    public delegate void HealAppliedEventHandler(GameObject target, int amount);
   
    /// <summary>
    /// SkillComponent implementing healing
    /// </summary>
    public class HealComponent : SkillComponent
    {
        public int Heal;

        public event HealAppliedEventHandler HealApplied;

        public override void Apply(List<GameObject> targets)
        {
            foreach (GameObject obj in targets)
            {
                if (!TextUtility.CheckIfTagsMatch(Tags, obj.tag))
                {
                    Debug.Log("Target with tag " + obj.tag + " cannot be targeted");
                    continue;
                }

                obj.GetComponent<CharacterStats>().ChangeHealth(Heal);
                finished = true;

                if (HealApplied != null)
                {
                    HealApplied(obj, Heal);
                }
            }
        }
    }

