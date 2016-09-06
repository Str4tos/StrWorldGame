using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// SkillComponent implementing dealing damage
/// </summary>
public class AOEDamageComponent : SkillComponentCustom
{
    public float useRadius = 1.0f;

    public override void Apply()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(_Skill.Caster.transform.position, useRadius);
        foreach (Collider col in objectsInRange)
        {
            if (_Skill.CheckIfTagsMatch(Tags, col.tag))
            {
                Status targetStatus = (Status)col.gameObject.GetComponent(typeof(Status));
                if (targetStatus != null)
                {
                    targetStatus.ReceivDamage(_Skill.SkillPower);
                }
            }
        }
    }

    
}


