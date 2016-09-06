using System.Collections.Generic;
using UnityEngine;
using Utilities;


    /// <summary>
    /// Provides the ability of jumping to additional targets to a skill, from outside the core skill system
    /// </summary>
    public class Jump : BaseAdditionalSkillLogic
    {
        public int NumberOfJumps = 1;
        public int Range = 50;

        Skill skill;

        public override void  Initialize()
        {
            skill = GetComponent<Skill>();
            skill.SkillFinished += new SkillEventHandler(skill_SkillFinished);
        }

        void skill_SkillFinished(Skill sender)
        {
            // End skill if all jumps have been used
            if (NumberOfJumps == 0)
            {
                return;
            }

            // Find Objects in range
            Collider[] cols = Physics.OverlapSphere(transform.position, Range);

            // Executes the skill onto the neasest target. If no fitting target is found, the execution stops and additional jumps are lost.
            for (int i = 0; i < cols.Length; i++)
            {
                if (TextUtility.CheckIfTagsMatch(sender.Tags, cols[i].gameObject.tag))
                {
                    // Make sure it's not "jumping" to the character where it already is
                    if (sender.GetTarget == cols[i].gameObject)
                        continue;

                    // Search the skill if the characters skill list, since be need to instantiate a new one
                    Skill s = null;

                    foreach (Skill skill in sender.GetCaster.GetComponent<CharacterStats>().GetSkills)
                    {
                        if (skill.SkillName == sender.SkillName)
                        {
                            s = skill;
                            break;
                        }
                    }

                    Debug.Log("Jumping");
                    // Create another skill, reducing the number of jumps and setting the current target as source and the new found target as target.
                    Skill curSkill = (Skill)Instantiate(s);
                    curSkill.gameObject.GetComponent<Jump>().NumberOfJumps--;
                    curSkill.ActivateSkill(sender.GetTarget, cols[i].gameObject);
                    return;
                }
            }
        }
    }

