using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utilities;


    /// <summary>
    /// Skill component handling collision, (pseudo-)physics-base movement and rotation
    /// </summary>
	public class PhysicsComponent : SkillComponent
	{
        public enum Direction { Random, CasterOrientation, Custom }

		public float AppliedForce;
        public float ForceDuration = 0;
        public float AppliedVelocity;
        List<GameObject> addForceTo = new List<GameObject>();
		List<float> forceAdded = new List<float>();

        public float DesiredCollisionRadius = 0;
        public float ExpansionTimeInSec = 0;

        List<SphereCollider> sphereCol = new List<SphereCollider>();
        private float elapsedTime = 0;
        private float radiusOriginal = 0;

        List<GameObject> Visualizations = new List<GameObject>();
        public bool IgnoreCasterCollision = true;

        GameObject caster;
        Vector3 direction = new Vector3();
        public Vector3 CustomDirection = Vector3.forward;

        public Direction _direction = Direction.CasterOrientation;

        private List<Vector3> randomDirections = new List<Vector3>();

        public GameObject SetCaster
        {
            set
            {
                caster = value;
                if (_direction == Direction.CasterOrientation)
                {
                    direction = caster.transform.forward;
                }
            }
        }

        /// <summary>
        /// Marks a visualization to be affected by physics
        /// </summary>
        /// <param name="vis">The visualization that shall be affected by physics</param>
        public void AddVisualization(GameObject vis)
        {
            Visualizations.Add(vis);

            Component[] comps = vis.GetComponentsInChildren(typeof(SphereCollider));

            if (comps.Length != 0)
            {
               sphereCol.Add((SphereCollider)comps[0]);
            }
        }

        /// <summary>
        /// Initializes the physics component
        /// </summary>
        public void ActivateComponent()
        {
            if (Visualizations != null)
            {
                if (IgnoreCasterCollision)
                {
                    foreach (GameObject vComp in Visualizations)
                    {
                        Physics.IgnoreCollision(vComp.GetComponent<Collider>(), caster.GetComponent<Collider>());
                    }
                }

                if (_direction == Direction.Random)
                {
                    for (int i = 0; i < Visualizations.Count; i++)
                    {
                        randomDirections.Add(Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f))));
                    }
                }
            }
        }
		
		void Update ()
        {
            // Moves the visual component by applying force to it.
            if (AppliedVelocity != 0 && caster != null)
            {
                for(int i = 0; i < Visualizations.Count; i++)
                {
                    if (Visualizations[i].GetComponent<Rigidbody>() != null)
                    {
                        if (_direction == Direction.Random)
                            direction = randomDirections[i];
                        Visualizations[i].GetComponent<Rigidbody>().AddForce(direction * ValueUtility.SPEEDNORMALIZER * AppliedVelocity);
                        Visualizations[i].transform.rotation = Quaternion.LookRotation(direction);
                    }
                    else
                    {
                        Debug.LogError("Component " + Visualizations[i] + " does not have a rigidbody attached and can therefore not be moved by physics. Add a rigidbody to " + Visualizations[i] + " or remove the physics component");
                    }
                }
            }

            // If the expansion time is elapsed the components is finished
            // There might still be movement being applied, but since we have no target, there is no way of knowing when it's finished
            if (elapsedTime >= ExpansionTimeInSec)
            {
                if (addForceTo.Count != 0)
                {
                    finished = false;
                }
                else
                {
                    finished = true;
                    Debug.Log("Physics Component finished");
                    return;
                }
            }

            // Applies force to all components we collided with for a given duration
			for(int i = 0; i < addForceTo.Count; i++)
			{
				if(forceAdded[i] > ForceDuration || addForceTo[i] == null)
				{
					addForceTo.RemoveAt (i);
					forceAdded.RemoveAt(i);
				}
			}
			for(int i = 0; i < addForceTo.Count; i++)
			{
                Vector3 dir = addForceTo[i].transform.position - caster.transform.position;
                dir.y = 0;
                dir = Vector3.Normalize(dir);
                addForceTo[i].transform.position += (dir * AppliedForce * Time.deltaTime);	
				forceAdded[i] += Time.deltaTime;
			}

            // Expands the collider to the radius given, over the time given
            foreach (SphereCollider sCol in sphereCol)
            {
                if (sCol != null && ExpansionTimeInSec != 0 && DesiredCollisionRadius != 0 && elapsedTime < ExpansionTimeInSec)
                {
                    sCol.radius = Mathf.Lerp(radiusOriginal, DesiredCollisionRadius, elapsedTime);

                    // Changing the radius of the collider doesn't force the rigitbody to wake up. If the target isn't moving either, no collision is detected since both are asleep.
                    // Rigitbody.WakeUp() doesn't do the trick here for some reason, "changing" the position seem to work well though
                    Visualizations[sphereCol.IndexOf(sCol)].transform.position += Vector3.zero;
                    elapsedTime += Time.deltaTime;
                }
            }
		}

        void OnDestroy()
        {
            foreach (GameObject vComp in Visualizations)
            {
                if (vComp != null && vComp.GetComponent<Rigidbody>() != null)
                {
                    vComp.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
            }
        }

        public override void Apply(List<GameObject> targets)
        {
            foreach (GameObject obj in targets)
            {
                Debug.Log(obj);
                addForceTo.Add(obj);
                forceAdded.Add(0);
            }
        }
	}

