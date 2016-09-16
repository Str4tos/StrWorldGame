using UnityEngine;
using System.Collections;
using System;

public class SkillPowerDatabase : MonoBehaviour
{
	public delegate float SkillPowerValue(Status statusCaster);
    public int idSkillPower = 0;

    private SkillPowerValue[] _SkillPowerValue;

    public float getSkillPower(Status statusCaster)
    {
        _SkillPowerValue = new SkillPowerValue[] { BladeWave };
		return _SkillPowerValue[idSkillPower](statusCaster);
    }


	private float BladeWave(Status statusCaster) //id 0
    {
		return statusCaster.getAttackDamage * (1.8f + statusCaster.energy / 1000.0f);
    }
}
