using UnityEngine;
using System.Collections;

public class SkillManaUsage : MonoBehaviour, ISkillComponent
{
    public float amountMP;

    public string Apply(Status casterStatus)
    {
        if (!casterStatus.TrySpendMP(amountMP))
            return "Missing mana";
        return string.Empty;
    }

    public void Initialize(Status casterStatus)
    {

    }
}
