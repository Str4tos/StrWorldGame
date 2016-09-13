using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Objectscripts;
using System;
using Utilities;

    public delegate void SkillEventHandler(Skill sender);

    /// <summary>
    /// Contains the execution logic, attributes and components of a Skill.
    /// </summary>
	public class Skill : MonoBehaviour
	{
		public enum StartAt : int {Caster, MouseRayCollision, Target}
		public enum ExecuteComponentsOn {Collision, MinimalDistanceToTarget, Instantly, Channeled}
		public enum EndOn {Collision, ComponentsFinished, MinimalDistanceToTarget, DurationExpired}

        public event SkillEventHandler CasttingAborted;
        public event SkillEventHandler CasttimeElapsed;
        public event SkillEventHandler SkillFinished;
        public event SkillEventHandler SkillInitialized;
        public event SkillEventHandler SkillActivated;

		List<SkillComponent> components = new List<SkillComponent>();

        List<VisualizationComponent> visualizations = new List<VisualizationComponent>();

		GameObject caster;

        // The distance use to calculate minimal distances for the MinimalDistanceToTarget property of ExecuteComponentsOn and EndOn
        const float MINIMALDISTANCETOTARGET = 1.0f;

		public Vector3 StartPositionOffset = Vector3.zero;
		public StartAt startAt = StartAt.Caster;
		public ExecuteComponentsOn executeComponentsOn = ExecuteComponentsOn.Collision;
		public EndOn endOn = EndOn.Collision;
		public bool RequiresTarget = false;
        public bool WaitForMouseClick = true;
        public bool CollideWithTerrainOnly = true;
		
        // For basic movement if no PhysicsComponent is used
		public float Velocity = 0;
		
		public Texture Icon;
		
		public float CoolDown = 0;
		public float CastTime = 0;
		float elapsedCasttime = 0;

        public float Range = 50;

        public float Duration = 1;

        // Used for Channeled mode
        public float Interval = 1;
        private int counter = 1;

        float elapsedSkillTime = 0;

		GameObject target;
		bool collisionOccured = false;
		
        // Define the current state of the skill
		bool initialized = false;
        bool isActive = false;
        bool deferedDestroy = false;

        public string SkillName = "Name";
        public string Description = "Description";

        PhysicsComponent pComp = null;

        public string[] Tags = {" "};

        public bool MovementInterruptsCasting = false;

        // Position of the caster during the last frame, use to determine if the caster moved.
        private Vector3 lastCasterPosition = Vector3.zero;

        public GameObject GetCaster
        {
            get { return caster; }
        }

        public GameObject GetTarget
        {
            get { return target; }
        }

        public List<SkillComponent> GetSkillComponents<T>()
        {
            if (typeof(T) == typeof(PhysicsComponent))
            {
                return new List<SkillComponent>{pComp};
            }
            else if (typeof(T) == typeof(VisualizationComponent))
            {
                List<SkillComponent> ret = new List<SkillComponent>();
                foreach (SkillComponent comp in visualizations)
                    ret.Add(comp);
                return ret;
            }
            else if (typeof(T) == typeof(DamageComponent))
            {
                List<SkillComponent> ret = new List<SkillComponent>();
                foreach (SkillComponent comp in components)
                {
                    if (comp is DamageComponent)
                        ret.Add(comp);
                }
                return ret;
            }
            else if (typeof(T) == typeof(DamageOverTimeComponent))
            {
                List<SkillComponent> ret = new List<SkillComponent>();
                foreach (SkillComponent comp in components)
                {
                    if (comp is DamageOverTimeComponent)
                        ret.Add(comp);
                }
                return ret;
            }
            else if (typeof(T) == typeof(HealComponent))
            {
                List<SkillComponent> ret = new List<SkillComponent>();
                foreach (SkillComponent comp in components)
                {
                    if (comp is HealComponent)
                        ret.Add(comp);
                }
                return ret;
            }
            else
            {
                List<SkillComponent> ret = new List<SkillComponent>();
                foreach (SkillComponent comp in components)
                {
                    if(comp is HealOverTimeComponent)
                        ret.Add(comp);
                }
                return ret;
            }
        }
		
		void Update ()
		{
			if(initialized)
			{
                // If there is casttime, no other skill functionality is executed
				if(CastTime > 0 && elapsedCasttime < CastTime)
				{
					elapsedCasttime += Time.deltaTime;
					
					// Once the casttime has finished, activate the rest of the skill
                    if (elapsedCasttime >= CastTime)
                    {
                        if (CasttimeElapsed != null)
                        {
                            CasttimeElapsed(this);
                        }
                        ActivateSkill();
                        Debug.Log("Casttime elapsed");
                    }

                    lastCasterPosition = caster.transform.position;
				}
                if (CastTime > 0 || executeComponentsOn == ExecuteComponentsOn.Channeled)
                {
                    if (Vector3.Distance(caster.transform.position, lastCasterPosition) > 0.01f)
                    {
                        if (CasttingAborted != null)
                        {
                            CasttingAborted(this);
                        }
                        DestroySkill();
                    }
                }
                if(isActive)
                {
                    elapsedSkillTime += Time.deltaTime;

                    // Move objects without physics
                    if (target != null)
                    {
                        foreach (VisualizationComponent vComp in visualizations)
                        {
                            foreach (GameObject obj in vComp.ObjectVisualizations)
                            {
                                obj.transform.position += Vector3.Normalize(target.transform.position - obj.transform.position) * Velocity * Time.deltaTime * 50;
                            }
                            if (vComp.GetParticleSystemInstance != null)
                            {
                                vComp.GetParticleSystemInstance.transform.position += Vector3.Normalize(target.transform.position - vComp.GetParticleSystemInstance.transform.position) * Velocity * Time.deltaTime * 50;
                            }
                            if (vComp.GetGameObjectInstance != null)
                            {
                                vComp.GetGameObjectInstance.transform.position += Vector3.Normalize(target.transform.position - vComp.GetGameObjectInstance.transform.position) * Velocity * Time.deltaTime * 50;
                            }
                        }
                    }

                    // Execute components as soon as they are close enough to the target
                    if (executeComponentsOn == ExecuteComponentsOn.MinimalDistanceToTarget)
                    {
                        foreach (VisualizationComponent vComp in visualizations)
                        {
                            if (vComp.GetParticleSystemInstance != null && Vector3.Distance(vComp.GetParticleSystemInstance.transform.position, target.transform.position) < MINIMALDISTANCETOTARGET)
                            {
                                Debug.Log(vComp.GetParticleSystemInstance + " reached the target. Executing skill components");
                                ExecuteSkillComponents(new List<GameObject>() { target });
                                break;
                            }
                        }

                        foreach (VisualizationComponent vComp in visualizations)
                        {
                            foreach (GameObject obj in vComp.ObjectVisualizations)
                            {
                                if (Vector3.Distance(obj.transform.position, target.transform.position) < MINIMALDISTANCETOTARGET)
                                    ExecuteSkillComponents(new List<GameObject>() { target });
                            }
                        }
                    }
                    if (executeComponentsOn == ExecuteComponentsOn.Instantly)
                    {
                        ExecuteSkillComponents(new List<GameObject>() { target });
                    }
                    // Execute according to the interval
                    if(executeComponentsOn == ExecuteComponentsOn.Channeled && counter <= (int)(Duration / Interval))
                    {
                        if (elapsedSkillTime > counter * Interval)
                        {
                            ExecuteSkillComponents(new List<GameObject>() { target });
                            counter++;
                        }
                    }
                    if (CheckIfFinished())
                    {
                        DestroySkill();
                    }
                }
                // Wait for all components to finish up before destroying the skill all together
                if (deferedDestroy)
                {
                    foreach (VisualizationComponent vComp in visualizations)
                    {
                        // Destroys the visualization component. The removal of its components is handles in its OnDestroy function
                        if (!vComp.SmoothDestroyFinished)
                        {
                            return;
                        }
                    }

                    Debug.Log("Skill Destroyed");
                    Destroy(gameObject);
                }
			}
		}

        /// <summary>
        /// Initializes the destruction of the skill and all its components
        /// </summary>
        void DestroySkill()
        {
            Debug.Log("Skill finished");
            if (SkillFinished != null)
            {
                SkillFinished(this);
            }

            // Destroy physics component
            Destroy(pComp);

            bool wantDeferedDestroy = false;
            foreach (VisualizationComponent vComp in visualizations)
            {
                // Destroys the visualization component. The removal of its components is handles in its OnDestroy function
                if (vComp.SmoothDestroy())
                {
                    wantDeferedDestroy = true;
                }
            }

            // Defere the destruction of the skill, so the visualization component can fade out
            if (wantDeferedDestroy)
            {
                Debug.Log("Visualization Components wants to SmoothDestroy, defering destruction of skill ...");
                isActive = false;
                deferedDestroy = true;
                return;
            }
            // Destroy skill
            Debug.Log("Skill Destroyed");
            Destroy(gameObject);
        }

        void MouseUtility_MouseClick(int button)
        {
            // Waits for a mouse click after initialization if StartAt was set to MouseRayCollision
            if (button == 0)
            {
                initialized = true;
                if (CastTime == 0)
                {
                    Debug.Log("Mouse Clicked, activating skill");
                    ActivateSkill();
                }

                MouseUtility.MouseClick -= new MouseClickEventHandler(MouseUtility_MouseClick);
            }
        }
	
		/// <summary>
		/// Checks if the skill is finished according to the 'ExecuteComponentsOn' mode.
		/// </summary>
		/// <returns>
		/// True if the Skill has finished, else otherwise.
		/// </returns>
		bool CheckIfFinished()
		{
			switch(endOn)
			{
				case EndOn.ComponentsFinished:
				case EndOn.MinimalDistanceToTarget:
                    // Check is ALL components are finished

                    // If there was no target given, skill components might never execute, therefore we can only check for physics- and visualization components
                    if (target != null)
                    {
                        foreach (SkillComponent comp in components)
                        {
                            if (!comp.IsFinished)
                            {
                                return false;
                            }
                        }
                    }
                    foreach(VisualizationComponent comp in visualizations)
					{
						if(!comp.IsFinished)
						{
							return false;	
						}
					}
                    if (pComp != null && !pComp.IsFinished)
                    {
                        return false;
                    }
					break;        
				case EndOn.Collision:
                    // Check if collision occured
					if(!collisionOccured)
						return false;
					break;
                case EndOn.DurationExpired:
                    if (elapsedSkillTime < Duration)
                        return false;
                    break;
			}
			return true;
		}
		
        /// <summary>
        /// Executes the skills visualization and physics
        /// </summary>
        /// <returns></returns>
		private int ActivateSkill()
		{
            BaseAdditionalSkillLogic[] logic = GetComponents<BaseAdditionalSkillLogic>();

            foreach (BaseAdditionalSkillLogic lComp in logic)
            {
                lComp.Initialize();
            }

            // Retrieve all skill components
            Component[] skillComponents = GetComponentsInChildren(typeof(SkillComponent));
            pComp = null;

            foreach (Component comp in skillComponents)
            {
                if (comp is SkillComponent)
                {
                    if (comp is VisualizationComponent)
                        visualizations.Add((VisualizationComponent)comp); // Store visualizations separately
                    else if (comp is PhysicsComponent)
                        pComp = (PhysicsComponent)comp; // Only 1 physics component can be active at a time
                    else
                        components.Add((SkillComponent)comp);
                   Debug.Log(comp);
                }
            }

            foreach (VisualizationComponent vComp in visualizations)
            {
                vComp.ActivateVisualization(caster, VisualizationComponent.ExecutionTime.Start);

                // Initialize particle system
                if (vComp.GetParticleSystemInstance != null)
                {
                    if (startAt == StartAt.Caster)
                        vComp.GetParticleSystemInstance.transform.position = caster.transform.position + caster.transform.right * StartPositionOffset.x + caster.transform.up * StartPositionOffset.y + caster.transform.forward * StartPositionOffset.z;
                    else if (startAt == StartAt.MouseRayCollision)
                    {
                        // Get's the raycasthit of the active terrain. If no terrain is used, any other object is used
                        if(Terrain.activeTerrain != null && CollideWithTerrainOnly)
                            vComp.GetParticleSystemInstance.transform.position = RaycastUtility.GetSpecificMouseRaycasthit(Terrain.activeTerrain.name) + caster.transform.right * StartPositionOffset.x + caster.transform.up * StartPositionOffset.y + caster.transform.forward * StartPositionOffset.z;
                        else
                            vComp.GetParticleSystemInstance.transform.position = RaycastUtility.GetNearestMouseRaycasthit() + caster.transform.right * StartPositionOffset.x + caster.transform.up * StartPositionOffset.y + caster.transform.forward * StartPositionOffset.z;
                    }
                    else if (startAt == StartAt.Target)
                        vComp.GetParticleSystemInstance.transform.position = target.transform.position + caster.transform.right * StartPositionOffset.x + caster.transform.up * StartPositionOffset.y + caster.transform.forward * StartPositionOffset.z;

                    Debug.Log("Particle System Initialized");
                }
                // Initialize Object Visualization
                else if (vComp.GetGameObjectInstance != null)
                {
                   if (startAt == StartAt.Caster)
                       vComp.GetGameObjectInstance.transform.position = caster.transform.position + caster.transform.right * StartPositionOffset.x + caster.transform.up * StartPositionOffset.y + caster.transform.forward * StartPositionOffset.z;
                   else if (startAt == StartAt.MouseRayCollision)
                   {
                       if (Terrain.activeTerrain != null && CollideWithTerrainOnly)
                           vComp.GetGameObjectInstance.transform.position = RaycastUtility.GetSpecificMouseRaycasthit(Terrain.activeTerrain.name) + caster.transform.right * StartPositionOffset.x + caster.transform.up * StartPositionOffset.y + caster.transform.forward * StartPositionOffset.z;
                       else
                           vComp.GetGameObjectInstance.transform.position = RaycastUtility.GetNearestMouseRaycasthit() + caster.transform.right * StartPositionOffset.x + caster.transform.up * StartPositionOffset.y + caster.transform.forward * StartPositionOffset.z;
                   }
                   else if (startAt == StartAt.Target)
                       vComp.GetGameObjectInstance.transform.position = target.transform.position + caster.transform.right * StartPositionOffset.x + caster.transform.up * StartPositionOffset.y + caster.transform.forward * StartPositionOffset.z;
 
                    Debug.Log("Object Visualization Initialized");
                }
            }
            // Initialize physics
            if (pComp != null)
            {
                pComp.SetCaster = caster;

                foreach (VisualizationComponent vComp in visualizations)
                {
                    if (vComp.GetParticleSystemInstance != null)
                    {
                        vComp.GetParticleSystemInstance.GetComponent<OnTrigger>().AfterCollision += new OnTriggerEventHandler(ForwardCollisions);
                        pComp.AddVisualization(vComp.GetParticleSystemInstance.gameObject);
                    }
                    else if (vComp.GetGameObjectInstance != null)
                    {
                        vComp.GetGameObjectInstance.GetComponent<OnTrigger>().AfterCollision += new OnTriggerEventHandler(ForwardCollisions);
                        pComp.AddVisualization(vComp.GetGameObjectInstance);
                    }
                }
                pComp.ActivateComponent();
            }

            if (SkillActivated != null)
            {
                SkillActivated(this);
            }

            isActive = true;	
			return 0;
		}
		
        /// <summary>
        /// Initializes the skill.
        /// </summary>
        /// <param name="caster">GameObject the skill originates from</param>
        /// <param name="target">GameObject the skill targets</param>
        /// <returns>0 if the skill was successfully initialized
        /// -1 if the skill requires a target, but non was given
        /// -2 if the targets Tag didn't meet the requirements
        /// -3 if the target is out of range</returns>
		public int ActivateSkill(GameObject caster, GameObject target)
		{
            if (RequiresTarget)
            {
                // Selftargeted skill should also work while targeting an enemy, or nothing
                // So if on of the tags is the casters tag and none of the tags is the targets tag, make the caster the target
                bool casterTagFound = false;
                for (int i = 0; i < Tags.Length; i++)
                {
                    if (Tags[i] == caster.tag)
                    {
                        casterTagFound = true;
                    }
                    if (target != null && Tags[i] == target.tag)
                    {
                        casterTagFound = false;
                        break;
                    }
                }

                if (casterTagFound)
                {
                    target = caster;
                }

                if (target == null)
                {
                    // Target is required but non is given
                    Destroy(gameObject);
                    return -1;
                }

                if (!TextUtility.CheckIfTagsMatch(Tags, target.tag))
                {
                    // Target doesn't match tags
                    Destroy(gameObject);
                    return -2;
                }

                if (Vector3.Distance(caster.transform.position, target.transform.position) > Range)
                {
                    // Target is too far away
                    Destroy(gameObject);
                    return -3;
                }
            }
			
			this.caster = caster;
			this.target = target;
            lastCasterPosition = this.caster.transform.position;
			
            if (startAt == StartAt.MouseRayCollision && WaitForMouseClick)
            {
                MouseUtility.MouseClick += new MouseClickEventHandler(MouseUtility_MouseClick);
                Debug.Log("Waiting for Mouse input");
            }
            else
            {
                initialized = true;
                // If we have casttime, defere the activation (casttime and activation are handled in Update())
                if (CastTime == 0)
                {
                    ActivateSkill();
                    Debug.Log("No Casttime, activating skill");
                }
            }

            if (SkillInitialized != null)
            {
                SkillInitialized(this);
            }

			return 0;
		}

        /// <summary>
        /// Initializes the skill.
        /// </summary>
        /// <param name="caster">GameObject the skill originates from</param>
        /// <returns>0 if the skill was successfully initialized
        /// -1 if the skill requires a target, but non was given
        /// -2 if the targets Tag didn't meet the requirements</returns>
		public int ActivateSkill(GameObject caster)
		{
			return ActivateSkill(caster, null);	
		}
		
        /// <summary>
        /// Sends all components a collision notice.
        /// </summary>
        /// <param name="sender">Object for which we are tracking collision</param>
        /// <param name="targets">Object collided with</param>
        /// <param name="pos">Position the collision occured</param>
		void ForwardCollisions(GameObject sender, List<GameObject> targets, Vector3 pos)
		{
            Debug.Log("Collision Detected");
			ExecuteSkillComponents(targets);
			collisionOccured = true;

            // Activate visual components
            foreach (VisualizationComponent vComp in visualizations)
            {
                if (vComp.ActivateVisualization(caster, VisualizationComponent.ExecutionTime.Collision))
                {
                    if (vComp.GetParticleSystemInstance != null)
                    {
                        vComp.GetParticleSystemInstance.transform.position = pos;

                        Debug.Log("Particle System Initialized");
                    }
                    else if (vComp.GetGameObjectInstance != null)
                    {
                        vComp.GetGameObjectInstance.transform.position = targets[0].transform.position;

                        Debug.Log("Object Visualization Initialized");
                    }
                }
            }
            // If the skill should end on collision, make sure no more collision affect the skill
			if(endOn == EndOn.Collision)
			{
                sender.GetComponent<OnTrigger>().AfterCollision -= new OnTriggerEventHandler(ForwardCollisions);
			}
		}
		
        /// <summary>
        /// Executes all SkillComponents attached to this skill
        /// </summary>
		void ExecuteSkillComponents(List<GameObject> targets)
		{
            // We have to check again which Tags can be targeted, since the initial check could not check the objects we are now colliding with.
            List<GameObject> check = new List<GameObject>();

            foreach (GameObject obj in targets)
            {
                if (TextUtility.CheckIfTagsMatch(Tags, obj.tag))
                    check.Add(obj);
            }

            foreach (SkillComponent comp in components)
            {
                comp.Apply(check);
            }

            if(pComp != null)
                pComp.Apply(check);
		}
	}

