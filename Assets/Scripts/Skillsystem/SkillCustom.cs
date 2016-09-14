using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Objectscripts;

public class SkillCustom : MonoBehaviour
{

    //---Skill
    public string SkillName = "Name"; //Unique
    public string Description = "Description";
    public Sprite Icon;
    public float SpeedMultiplier = 1.0f;
    public float SkillAnimationID = 1.0f;
    private bool isActive = false;
    private List<SkillComponentCustom> SkillComponents = new List<SkillComponentCustom>();
    private string componentError;

    //---Caster
    private GameObject caster = null;
    private Status statusCaster;
    private Animator _Animator;
    private float skillPower;
    private SkillPowerDatabase _SkillPowerComponent;

    //---Visualization
    public GameObject ObjectVisualization;
    public float CountdownVisualization = 1.5f; //to destroy visualization
    private Transform PositionCastSpell;
    private GameObject ObjectVisualizationInstance;
    private bool isFinishedVisualization = false;

	private MaterialGradientSkill gradientSkill;

    #region get/set

    public GameObject Caster
    {
        get { return caster; }
        set { caster = value; }
    }
	public Status StatusCaster
	{
		get { return statusCaster; }
		set { statusCaster = value; }
	}
    public float SkillPower
    {
        get { return skillPower; }
        set { skillPower = value; }
    }

    public string ComponentError
    {
        get { return componentError; }
        set { componentError = value; }
    }

    #endregion

    // Use this for initialization
    void Awake()
    {
        _SkillPowerComponent = gameObject.GetComponent<SkillPowerDatabase>();
		foreach (SkillComponentCustom component in gameObject.GetComponents<SkillComponentCustom>())
        {
            SkillComponents.Add(component);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }



    private void Activate()
    {
        //--Component apply
        componentError = null;
        foreach (SkillComponentCustom component in SkillComponents)
        {
            component.Apply();
            if (componentError != null)
            {
                StopSkill();
                PopupInfo.setText(componentError, caster.transform, 1, Color.cyan);
                return;
            }
        }

        //--ActivateVisualization()
        //---Mesh
//        PositionCastSpell = caster.transform.Find("SpellCastPoint");
//        ObjectVisualizationInstance = (GameObject)Instantiate(ObjectVisualization, PositionCastSpell.position, PositionCastSpell.rotation);
        //ObjectVisualizationInstance.GetComponent<OnTrigger>().AfterCollision += new OnTriggerEventHandler(ForwardCollisions);
        //----Destroy after CountdownVisualization time
        //Destroy(ObjectVisualizationInstance, CountdownVisualization);

		ObjectVisualizationInstance.transform.rotation = PositionCastSpell.rotation;
		ObjectVisualizationInstance.transform.position = PositionCastSpell.position;
    
    }

    public void CallSkill(GameObject caster, bool ActiveState)
    {
        if (ActiveState)
        {
            if (this.caster == null)
            {
                this.caster = caster;
                statusCaster = (Status)caster.GetComponent(typeof(Status));
                _Animator = caster.GetComponent<Animator>();
            }
            _Animator.SetFloat("UseSkill",SkillAnimationID);
			InvokeRepeating("Activate", 0, SpeedMultiplier / statusCaster.getAttackSpeed);
			skillPower = _SkillPowerComponent.getSkillPower(statusCaster);
            foreach (SkillComponentCustom component in SkillComponents)
            {
                component.Initialize();
            }
			PositionCastSpell = caster.transform.Find("SpellCastPoint");
			ObjectVisualizationInstance = (GameObject)Instantiate(ObjectVisualization);
			gradientSkill = ObjectVisualizationInstance.transform.GetChild(0).GetChild(0).GetComponent<MaterialGradientSkill> ();
			gradientSkill.TimeMultiplier = SpeedMultiplier * statusCaster.getAttackSpeed;
        }
        else
        {
            if (ObjectVisualizationInstance != null)
            {
                StopSkill();
            }
        }
    }

    private void StopSkill()
    {
        _Animator.SetFloat("UseSkill", 0.0f);
        CancelInvoke("Activate");
		gradientSkill.loop = false;
		Destroy(ObjectVisualizationInstance, CountdownVisualization);
    }

    //private void ForwardCollisions(GameObject sender, List<GameObject> targets, Vector3 pos)
    //{
    //    ApplyDamage(targets);
    //}

    //private void ApplyDamage(List<GameObject> targets)
    //{
    //    foreach (GameObject targetObj in targets)
    //    {
    //        if (targetObj.tag == TagUsege)
    //        {
    //            Status targetStatus = (Status)targetObj.GetComponent(typeof(Status));
    //            targetStatus.ReceivDamage(skillPower);
    //            ObjectVisualizationInstance.GetComponent<OnTrigger>().AfterCollision -= new OnTriggerEventHandler(ForwardCollisions);
    //        }
    //    }
    //}

    public bool CheckIfTagsMatch(string[] Tags, string curTag)
    {
        if (Tags.Length > 0)
        {
            for (int i = 0; i < Tags.Length; i++)
            {
                if (Tags[i] == curTag)
                {
                    return true;
                }
            }
        }
        return false;
    }

}
