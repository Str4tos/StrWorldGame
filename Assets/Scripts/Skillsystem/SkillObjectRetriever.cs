using UnityEngine;
using System.Collections;
using System.Collections.Generic;


    /// <summary>
    /// Base class for scripts that obtains gameobjects to be used as skill visualization.
    /// </summary>
	public class SkillObjectRetriever : MonoBehaviour
	{		
        /// <summary>
        /// Override this function with logic that creates or obtains gameobjects to use as visualization.
        /// </summary>
		public virtual List<GameObject> GetGameObjects(GameObject target)
		{
			return new List<GameObject>();
		}
	}


