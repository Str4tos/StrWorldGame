using UnityEngine;

    /// <summary>
    /// Base script for all particle system scripts
    /// </summary>
	class PSScript : MonoBehaviour
	{
        protected ParticleSystem system;

        protected virtual void Start()
        {
            system = GetComponent<ParticleSystem>();
        }
	
}
