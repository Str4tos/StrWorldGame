using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utilities;


    public delegate void AttributeChangedEventHandler(GameObject target, StatusPlayer.Attribute attribute);

    /// <summary>
    /// SkillComponent implementing the AttributeModifier
    /// </summary>
    public class AttributeComponent : SkillComponent
    {
        public AttributeModifier modifier;

        public event AttributeChangedEventHandler AttributeChanged;

        public StatusPlayer.Attribute Attribute;
        public float Modifier = 0;
        public float Duration = 0;

        void Start()
        {
            modifier = new AttributeModifier(Attribute, Modifier, Duration);
        }

        public override void Apply(List<GameObject> targets)
        {
            foreach (GameObject obj in targets)
            {
                if (!TextUtility.CheckIfTagsMatch(Tags, obj.tag))
                {
                    Debug.Log("Target with tag " + obj.tag + " cannot be targeted");
                    continue;
                }

                obj.GetComponent<CharacterStats>().AddAttributeModifier(modifier);
                finished = true;
                Debug.Log("Attribute Component finished");

                if (AttributeChanged != null)
                {
                    AttributeChanged(obj, modifier.stat);
                }
            }
        }
    }
