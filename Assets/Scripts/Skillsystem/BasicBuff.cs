using UnityEngine;
using System.Collections.Generic;



    /// <summary>
    /// Base class for all buffs, which are all components that have an effect over time.
    /// </summary>
	public abstract class BasicBuff
	{
        protected string name;
        protected string description;
        protected float duration;
        public float elapsedTime = 0;

        Texture2D icon;

        public BasicBuff(float duration)
        {
            this.duration = duration;
        }

        public string GetName
        {
            get { return name; }
        }

        public string GetDescription
        {
            get { return description; }
        }

        public float Duration
        {
            get { return duration; }
            set { duration = value; }
        }
	}

