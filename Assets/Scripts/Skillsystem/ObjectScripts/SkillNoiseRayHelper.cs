using UnityEngine;
using System.Collections.Generic;
using PS;

namespace Objectscripts
{
    /// <summary>
    /// Initializes the script attached to the noiseray particle system.
    /// </summary>
    class SkillNoiseRayHelper : BaseAdditionalSkillLogic
	{
        /// <summary>
        /// Initialize is called by the Skill before the Start() method of other scripts is called, as well as before any other skill components are initialized.
        /// </summary>
        public override void Initialize()
        {
            GetComponent<VisualizationComponent>().ParticleSystemInitialized += new ParticleSystemInitializedEventHandler(VisualizationComponent_ParticleSystemInitialized);
        }

        void VisualizationComponent_ParticleSystemInitialized(GameObject particlesystem)
        {
            particlesystem.GetComponent<NoiseRay>().target = GetComponent<Skill>().GetTarget.transform;
            particlesystem.GetComponent<NoiseRay>().Source = GetComponent<Skill>().GetCaster.transform;
            GetComponent<VisualizationComponent>().ParticleSystemInitialized -= new ParticleSystemInitializedEventHandler(VisualizationComponent_ParticleSystemInitialized);
        }
	}
}
