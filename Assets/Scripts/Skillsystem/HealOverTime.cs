using UnityEngine;
using System.Collections.Generic;
using System.Timers;


	/// <summary>
    /// Buff that restores health in intervals over a given time.
    /// </summary
    public class HealeOverTime : BasicBuff
	{
        float interval;
        int overallheal;

        public float GetInterval
        {
            get { return interval; }
        }

        public int GetOverallHeal
        {
            get { return overallheal; }
        }

        public float elapsedTimeSinceActivation = 0;

        public HealeOverTime(float interval, int overallheal, float duration)
            : base(duration)
        {
            this.interval = interval;
            this.overallheal = overallheal;
        }
	}

