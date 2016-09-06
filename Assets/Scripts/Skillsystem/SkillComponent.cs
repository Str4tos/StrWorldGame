using UnityEngine;
using System.Collections;
using System.Collections.Generic;


    /// <summary>
    /// Base for all logic attached to the skill.
    /// </summary>
	public class SkillComponent : MonoBehaviour
	{
		protected bool finished = false;

        // Tags to check against when executing. If no Tags are giving, all tags are assumed valid. 
        public string[] Tags;

		public bool IsFinished
		{
			get{return finished;}	
		}
		
		public virtual void Apply(List<GameObject> targets)
		{
			
		}
	}


