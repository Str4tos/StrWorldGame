using System.Collections.Generic;
using UnityEngine;

    public class Multi : BaseAdditionalSkillLogic
	{
        public int Amount = 5;

        public override void Initialize()
        {
            GetComponent<Skill>().SkillInitialized += new SkillEventHandler(SkillInitialized);
        }

        void SkillInitialized(Skill sender)
        {
            // Search the skill if the characters skill list, since be need to instantiate new ones
            Skill s = null;

            foreach (Skill skill in sender.GetCaster.GetComponent<CharacterStats>().GetSkills)
            {
                if (skill.SkillName == sender.SkillName)
                {
                    s = skill;
                    break;
                }
            }

            for (int i = 0; i < Amount; i++)
            {
                Skill skill = (Skill)Instantiate(s);
                skill.GetComponent<Multi>().Amount = 0;
                skill.CastTime = 0;
                skill.ActivateSkill(sender.GetCaster, sender.GetTarget);
            }
        }
	}

