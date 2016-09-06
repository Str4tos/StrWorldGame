using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Objectscripts;


    /// <summary>
    /// Helperclass retrieving all Orbs currently attached to the character
    /// </summary>
	public class GetOrbs : SkillObjectRetriever
	{	
		public override List<GameObject> GetGameObjects(GameObject target)
		{
			List<GameObject> orbs = target.GetComponent<MageStats>().Orbs;
	
			foreach(GameObject obj in orbs)
			{
				obj.GetComponentInChildren<OrbitMovement>().enabled = false;
			}
			return orbs;
		}
	}

