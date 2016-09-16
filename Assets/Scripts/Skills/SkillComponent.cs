using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Base for all logic attached to the skill.
/// </summary>
public class SkillComponent : MonoBehaviour
{
    //protected bool finished = false;

    // Tags to check against when executing. If no Tags are giving, all tags are assumed valid. 
    public string[] Tags;

    protected Skill _Skill;

    //public bool IsFinished
    //{
    //    get{return finished;}	
    //}

    public virtual void Initialize()
    {
        _Skill = gameObject.GetComponent<Skill>();
    }

    public virtual void Apply()
    {

    }
}


