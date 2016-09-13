using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PS
{
    /// <summary>
    ///  Makes the generates particles rotate around their origin
    /// </summary>
    class RotatingOrbs : PSScript
    {
        // Time since the script has started. Set to != 0 to prevent first multiplication with zero
        float elapsedTime = 0.0001f;

        List<ParticleSystem> trails = new List<ParticleSystem>();
        ParticleSystem.Particle[] particleList = new ParticleSystem.Particle[0];

        public float RotationSpeedX = 2;
        public float RotationSpeedY = 2;
        public float DistanceX = 1;
        public float DistanceY = 1;
        public ParticleSystem Trail;

        protected override void Start()
        {
            base.Start();
        }

        void Update()
        {
            // If particles have been created/removed, update the particle list
            if (system.particleCount != particleList.Length)
            {
                particleList = new ParticleSystem.Particle[system.particleCount];
                system.GetParticles(particleList);
            }

            if (Trail != null)
            {
                // Make sure all particles have trails attached
                if (particleList.Length != trails.Count)
                {
                    if (particleList.Length < trails.Count)
                    {
                        trails.Clear();
                        for (int i = 0; i < particleList.Length; i++)
                        {
                            trails.Add((ParticleSystem)Instantiate(Trail));
                        }
                    }
                    else
                    {
                        while (particleList.Length > trails.Count)
                        {
                            trails.Add((ParticleSystem)Instantiate(Trail));
                        }
                    }
                }
            }

            elapsedTime += Time.deltaTime;
            for (int i = 0; i < particleList.Length; ++i)
            {
                // Rotate the particles around the attached game object
                particleList[i].position = transform.position + new Vector3(Mathf.Cos((((i + 1) / (float)particleList.Length) * 360) % 360 * Mathf.Deg2Rad + elapsedTime * RotationSpeedX) * DistanceX, Mathf.Sin((((i + 1) / (float)particleList.Length) * 360) % 360 * Mathf.Deg2Rad + elapsedTime * RotationSpeedY) * DistanceY, 0);

                if (Trail != null)
                    trails[i].transform.position = particleList[i].position;
            }

            system.SetParticles(particleList, system.particleCount);
        }
    }
}