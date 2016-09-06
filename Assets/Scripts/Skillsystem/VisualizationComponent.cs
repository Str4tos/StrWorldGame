using System.Collections.Generic;
using UnityEngine;

public delegate void ParticleSystemInitializedEventHandler(GameObject particlesystem);

/// <summary>
/// SkillComponent to visualize the skill by means of particle systems or meshes
/// </summary>
public class VisualizationComponent : SkillComponent
{
    public event ParticleSystemInitializedEventHandler ParticleSystemInitialized;

    // Do not access this for anything else than instantiating!
    public ParticleSystem PSVisualization;
    public GameObject ObjectVisualization;
    public GameObject PositionPointSpell;

    GameObject gameObjectInstance;
    ParticleSystem particleSystemInstance;

    public List<GameObject> ObjectVisualizations = new List<GameObject>();
    public SkillObjectRetriever GetVisualizationObjectsFrom;

    public enum _VisualizationType : int { ParticleSystem, Mesh, GetObjectsByScript }
    public enum ExecutionTime : int { Start, Collision }

    public ExecutionTime ExecuteAt;

    public _VisualizationType VisualizationType;

    // Used to fade out the visualization
    public bool FadeOutOnDestroy = true;
    private bool destroy = false;
    public bool SmoothDestroyFinished = false;
    public bool CustomEffects = false;

    public ParticleSystem GetParticleSystemInstance
    {
        get { return particleSystemInstance; }
    }
    public GameObject GetGameObjectInstance
    {
        get { return gameObjectInstance; }
    }

    /// <summary>
    /// Instantiates the visualization
    /// </summary>
    /// <param name="caster">Object where the visualization originates from</param>
    /// <param name="executeAt">Current state of the skill</param>
    /// <returns>True is visualization was activated, false if not</returns>
    public bool ActivateVisualization(GameObject caster, ExecutionTime executeAt)
    {
        if (executeAt != ExecuteAt)
            return false;

        Debug.Log("Activating Visualization Component");

        if (VisualizationType == _VisualizationType.Mesh)
        {
            gameObjectInstance = (GameObject)Instantiate(ObjectVisualization);
            finished = true;
        }
        else if (VisualizationType == _VisualizationType.GetObjectsByScript)
        {
            ObjectVisualizations.AddRange(GetVisualizationObjectsFrom.GetGameObjects(caster));
            finished = true;
        }
        else if (VisualizationType == _VisualizationType.ParticleSystem)
        {
            particleSystemInstance = (ParticleSystem)Instantiate(PSVisualization);
            if (ParticleSystemInitialized != null)
            {
                ParticleSystemInitialized(particleSystemInstance.gameObject);
            }

            // If the PS runs indefinitely, mark it as finished right away, since there is no way to determine a finished state
            if (particleSystemInstance.duration == Mathf.Infinity || particleSystemInstance.loop)
            {
                finished = true;
                Debug.Log("Visualization Component Finished");
            }
        }

        return true;
    }

    public bool ActivateCustomEffect(Transform CustPoint)
    {
        gameObjectInstance = (GameObject)Instantiate(ObjectVisualization, CustPoint.position, CustPoint.rotation);
        gameObjectInstance.transform.parent = CustPoint;
        finished = true;
        return true;
    }

    void Update()
    {
        // Check if the PS has finished
        if (!finished)
        {
            //If the PS has been removed, or there are no more particles left, the visualization is finished
            if (particleSystemInstance != null && particleSystemInstance.particleCount == 0)
            {
                finished = true;
                Debug.Log("Visualization Component Finished");
            }
        }

        // Wait for the visualization to finish or fade out
        if (destroy && !SmoothDestroyFinished)
        {
            if (particleSystemInstance != null)
            {
                if (particleSystemInstance.particleCount == 0)
                {
                    SmoothDestroyFinished = true;
                    return;
                }
            }
            else if (gameObjectInstance != null)
            {
            //    MeshRenderer _MeshRenderer;
            //    //if (CustomEffects)
            //    //    _MeshRenderer = gameObjectInstance.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
            //    //else
            //        _MeshRenderer = gameObjectInstance.GetComponent<MeshRenderer>();

            //    for (int i = 0; i < _MeshRenderer.materials.Length; i++)
            //    {
            //        if (!_MeshRenderer.materials[i].HasProperty("_TintColor"))
            //        {
            //            Debug.LogError("Cannot find the color property in the shader given. Make sure the shader used for this mesh has a property called \"_TintColor\" (which is what it's usually called in a shader that supports alpha blending, which is required to do this), change the propertyName parameter accordingly, or remove the mark from \"Fade Out\" for this visualization component");
            //            SmoothDestroyFinished = true;
            //            return;
            //        }

            //        Color color = _MeshRenderer.materials[i].GetColor("_TintColor");

            //        color.a -= 0.0015f;
            //        _MeshRenderer.materials[i].SetColor("_TintColor", color);

            //        if (color.a <= 0)
            //        {
            //            SmoothDestroyFinished = true;
            //            return;
            //        }
            //    }
            }
            else
            {
                SmoothDestroyFinished = true;
                return;
            }
        }
    }

    void OnDestroy()
    {
        // Destroy all objects handed down by the get object script
        for (int i = 0; i < ObjectVisualizations.Count; i++)
        {
            Destroy(ObjectVisualizations[i]);
        }
        if (particleSystemInstance != null)
            Destroy(particleSystemInstance.gameObject);
        Destroy(gameObjectInstance);
    }

    /// <summary>
    /// Trys to fade out the current visualization before destroying it
    /// <returns>True if the visualization is going to smooth destroy, false if the visualization is destroyed right away</returns>
    /// </summary>
    public bool SmoothDestroy()
    {
        destroy = true;
        if (FadeOutOnDestroy)
        {
            if (particleSystemInstance != null && particleSystemInstance.emissionRate != 0)
            {
                // Stops the emission of new particles
                if (particleSystemInstance.duration == Mathf.Infinity || particleSystemInstance.loop)
                {
                    particleSystemInstance.enableEmission = false;
                }
                return true;
            }
            else if (gameObjectInstance != null)
            {
                return true;
            }
        }
        SmoothDestroyFinished = true;
        return false;
    }

}

