using System.Collections.Generic;
using UnityEngine;


    /// <summary>
    /// Base class for additional logic attached to skills. Use Initialize() instead of Start() which is called by the Skill, before other components execute Start()
    /// </summary>
	public abstract class BaseAdditionalSkillLogic : MonoBehaviour
	{
        public virtual void Initialize()
        {

        }
	}

