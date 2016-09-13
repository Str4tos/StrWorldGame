using UnityEngine;
using System.Collections;

    /// <summary>
    /// Creates a particle stream affected by perlin noise
    /// </summary>
    class NoiseRay : PSScript
    {
        // Whether the particles are shown constantly, or only in short intervals
        public enum NoiseRenderMode { Continuous, Flickering }

        public Transform Source;
        public Transform target;
        public int Waves = 100;

        // Determines how fast the "waves" move
        public float speed = 1f;

        // Determines how large the "waves" get
        public float scale = 1f;

        float flickerTime = 0;

        public NoiseRenderMode RenderMode = NoiseRenderMode.Continuous;

        //Perlin perlinNoise = new Perlin();
        float oneOverParticles;

        private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[0];

        protected override void Start()
        {
            base.Start();

            // Emits particles used to create the noise effect. Particle count is dependend on the distance of the target and the source
            system.Emit((int)((Vector3.Distance(Source.position, target.position) + 0.10f) *4));
            particles = new ParticleSystem.Particle[system.particleCount];

            oneOverParticles = 1f / (float)system.particleCount;
            system.GetParticles(particles);
        }

    //    void Update()
    //    {
    //        if (RenderMode == NoiseRenderMode.Flickering)
    //        {
    //            flickerTime += Time.deltaTime;

    //            // Switch the renderer on and off to create the flickering effect
    //            if (flickerTime > 0.3f && system.renderer.enabled)
    //            {
    //                system.renderer.enabled = false;
    //            }
    //            else if (flickerTime > 0.5f)
    //            {
    //                system.renderer.enabled = true; 
                    
    //                CalculateNewParticlePosition();

    //                flickerTime = 0;
    //            }
    //        }
    //        else if (RenderMode == NoiseRenderMode.Continuous)
    //        {
    //            CalculateNewParticlePosition();
    //        }
    //   }

    //    /// <summary>
    //    /// Calculates noise affected particle positions between the source and the target
    //    /// </summary>
    //    void CalculateNewParticlePosition()
    //    {
    //        float timex = Time.time * speed * 0.1365143f;
    //        float timey = Time.time * speed * 0.21688f;
    //        float timez = Time.time * speed * 1.1564f;

    //        for (int i = 0; i < particles.Length; i++)
    //        {
    //            Vector3 position = Vector3.Lerp(Source.transform.position + Source.transform.forward, target.position, oneOverParticles * (float)i);

    //            Vector3 offset = new Vector3(perlinNoise.Noise(timex + position.x, timex + position.y, timex + position.z),
    //                                        perlinNoise.Noise(timey + position.x, timey + position.y, timey + position.z),
    //                                        perlinNoise.Noise(timez + position.x, timez + position.y, timez + position.z));
    //            position += (offset * scale * ((float)(particles.Length - 1 - i) * oneOverParticles));

    //            particles[i].position = position;
    //            particles[i].color = Color.white;
    //        }

    //        system.SetParticles(particles, particles.Length);
    //    }
 
}