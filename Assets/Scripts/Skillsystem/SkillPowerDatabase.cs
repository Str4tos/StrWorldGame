using UnityEngine;
using System.Collections;
using System;

public class SkillPowerDatabase : MonoBehaviour
{
    public delegate float SkillPowerValue(GameObject caster);
    public int idSkillPower = 0;

    private SkillPowerValue[] _SkillPowerValue;

    public float getSkillPower(GameObject caster)
    {
        _SkillPowerValue = new SkillPowerValue[] { BladeWave };
        return _SkillPowerValue[idSkillPower](caster);
    }


    private float BladeWave(GameObject caster) //id 0
    {
        Status _StatusCaster = (Status)caster.GetComponent(typeof(Status));
		return _StatusCaster.getAttackDamage * (1.8f + _StatusCaster.energy / 1000.0f);
    }
}
