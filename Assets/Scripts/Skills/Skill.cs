using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill : MonoBehaviour
{
    public int idSkill = 0;
    public string nameSkill = "Name";
    public string description = "Description";
    public Sprite iconSkill;
    [SerializeField]
    private float speedMultiplier = 1.0f;
    [SerializeField]
    private List<ISkillComponent> skillComponents = new List<ISkillComponent>();
    
    private Status casterStatus;
    
    private void Activate()
    {
        //--Component apply
        foreach (var skillComponent in skillComponents)
        {
            string error = skillComponent.Apply(casterStatus);
            if (!string.IsNullOrEmpty(error))
            {
                CallSkill(false);
                PopupInfo.setText(error, casterStatus.transform, 1, Color.cyan);
                return;
            }
        }
        
    }

    public void Init(Status casterStatus)
    {
        this.casterStatus = casterStatus;
        foreach (var skillComponent in skillComponents)
            skillComponent.Initialize(casterStatus);
    }
    public void CallSkill(bool isActiveState)
    {
        if (isActiveState)
        {
			InvokeRepeating("Activate", 0, speedMultiplier / casterStatus.AttackSpeed);
        }
        else
        {
            CancelInvoke("Activate");
        }
    }

    

}
