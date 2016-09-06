using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Objectscripts
{
	public class CreateOrb : MonoBehaviour
	{
		public GameObject Orb;
		
		void Start()
		{		
			GetComponent<Skill>().SkillFinished += new SkillEventHandler(SkillFinished);
		}
	
        void SkillFinished(Skill sender)
		{
            List<SkillComponent> comps = sender.GetSkillComponents<DamageComponent>();
            Debug.Log(comps.Count);

            foreach (DamageComponent dComp in comps)
            {
                if (dComp.IsFinished)
                {
                    MageStats stats = sender.GetCaster.GetComponent<MageStats>();
                    if (stats == null)
                    {
                        Debug.LogError("Caster needs MageStats attached to add Orbs");
                        return;
                    }
                    GameObject orb = Instantiate(Orb) as GameObject;

                    stats.Orbs.Add(orb);
                }
            }
		}
	}	
}

