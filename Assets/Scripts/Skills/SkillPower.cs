using UnityEngine;
using System.Collections;
using System;

public class SkillPower : MonoBehaviour
{
    private bool attackDmg = false;
    private bool wizardyDmg = false;
    private float defaultMultiplier = 1.0f;
    private float strDivisor = 0.0f;
    private float agiDivisor = 0.0f;
    private float vitDivisor = 0.0f;
    private float eneDivisor = 0.0f;
    
    public float GetPower(Status casterStatus)
    {
        float power = 0.0f;

        if (attackDmg)
            power += casterStatus.AttackDmg;
        if (wizardyDmg)
            power += casterStatus.WizardyDmg;
        float multiplier = defaultMultiplier;
        if (strDivisor != 0.0f)
            multiplier += casterStatus.strenght / strDivisor;
        if (agiDivisor != 0.0f)
            multiplier += casterStatus.agility / agiDivisor;
        if (vitDivisor != 0.0f)
            multiplier += casterStatus.vitality / vitDivisor;
        if (eneDivisor != 0.0f)
            multiplier += casterStatus.energy / eneDivisor;
        return power * multiplier;
    }
}
