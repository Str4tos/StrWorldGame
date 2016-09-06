using UnityEngine;
using System.Collections.Generic;


    /// <summary>
    /// Buff that modifies attributes from the CharacterStats
    /// </summary>
	public class AttributeModifier : BasicBuff
	{
        public StatusPlayer.Attribute stat;
        public float modifier;

        public AttributeModifier(StatusPlayer.Attribute attribute, float modifier, float duration)
            : base(duration)
        {
            this.stat = attribute;
            this.modifier = modifier;
        }
	}

