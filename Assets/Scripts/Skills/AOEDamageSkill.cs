using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AOEDamageSkill : ISkillComponent
{
    public float useRadius = 1.0f;
    public string[] targetTags;
    public SkillPower skillPowerComponent;

    public void Initialize(Status casterStatus)
    {
    }
    
    public string Apply(Status casterStatus)
    {
        Collider[] objectsInRange = Physics.OverlapSphere(casterStatus.transform.position, useRadius);
        float damage = skillPowerComponent.GetPower(casterStatus);
        foreach (Collider col in objectsInRange)
        {
            if (IsCanAttackThis(col))
            {
                Status targetStatus = col.gameObject.GetComponent<Status>();
                if (targetStatus)
                {
                    targetStatus.ReceivDamage(damage);
                }
            }
        }
        return string.Empty;
    }

    private bool IsCanAttackThis(Collider col)
    {
        foreach (string tag in targetTags)
            if (col.CompareTag(tag))
                return true;
        return false;
    }
}


