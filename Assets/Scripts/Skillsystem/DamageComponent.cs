using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utilities;


    public delegate void DamageDealtEventHandler(GameObject target, int amount);

    /// <summary>
    /// SkillComponent implementing dealing damage
    /// </summary>
    public class DamageComponent : SkillComponent
    {
        public int Damage;

        public event DamageDealtEventHandler DamageDealt;

        public override void Apply(List<GameObject> targets)
        {
            foreach (GameObject obj in targets)
            {
                if (!TextUtility.CheckIfTagsMatch(Tags, obj.tag))
                {
                    Debug.Log("Target with tag " + obj.tag + " cannot be targeted");
                    continue;
                }

                Debug.Log(obj);
			obj.GetComponent<StatusEnemy>().ReceivDamage(Damage, Color.red);
                finished = true;
                Debug.Log("Damange Component finished");

                if (DamageDealt != null)
                {
                    DamageDealt(obj, Damage);
                }
            }
        }
    }

