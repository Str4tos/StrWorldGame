using UnityEngine;
using System.Collections;

public class ManaComponent : SkillComponentCustom
{
    public bool itSelf = false; // Self cast or target
    public bool IncrOrDecr = false; // true - Increase | false - Decrease
    public float QuantityMP;
    private StatusPlayer _StatusPlayer;

    public override void Apply()
    {
        if (itSelf)
        {
            _StatusPlayer = _Skill.Caster.GetComponent<StatusPlayer>();
            if (_StatusPlayer == null)
            {
                return;
            }

            if (!_StatusPlayer.ManaPoints(QuantityMP, IncrOrDecr))
            {
                _Skill.ComponentError = "Missing mana";
            }
        }
        else
        {
            // Add Target use <-

        }


    }
}
