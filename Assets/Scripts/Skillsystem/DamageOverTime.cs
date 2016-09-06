using UnityEngine;
using System.Collections.Generic;
using System.Timers;


    /// <summary>
    /// Buff that deal damage in intervals over a given time.
    /// </summary>
	public class DamageOverTime : BasicBuff
	{
        float interval;
        int overalldamage;

        public float GetInterval
        {
            get { return interval; }
        }

        public int GetOveralldamage
        {
            get { return overalldamage; }
        }

        public float elapsedTimeSinceActivation = 0;

        public DamageOverTime(float interval, int overalldamage, float duration)
            : base(duration)
        {
            this.interval = interval;
            this.overalldamage = overalldamage;
        }
	}

