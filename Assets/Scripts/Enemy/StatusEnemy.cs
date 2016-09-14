using UnityEngine;
using System.Collections;


public class StatusEnemy : Status {

	private StatusPlayer targetStatus;

	private CharacterController _CharacterController;
	private MouseOverEnemy _MouseOverEnemy;
	private GameObject collaiderMouse;
	public float expReward = 20.0f;
    public enum AiTypes { Melee, Range };
    public AiTypes AiType = AiTypes.Melee;

	public enum modsDefinition { Rare, RareMinion, Champion, Normal, Custom };
	public modsDefinition monsterModsDefinition;

	void Start () {
		_Animator = gameObject.GetComponent<Animator>();
		_CharacterController = gameObject.GetComponent<CharacterController>();
        collaiderMouse = gameObject.transform.FindChild("ColliderMouse").gameObject;
        _MouseOverEnemy = collaiderMouse.GetComponent<MouseOverEnemy>();
		CalculateStats ();
		restoreHPandMP ();
		popupColorBasic = new Color (0.93f, 0.6f, 0.0f); 
		popupColorCritical = new Color(0.75f, 0.40f, 1.0f);
        popupDmgMotion = -2;


//		if (isRandomlySpawned == false)
//		{
//			if (monsterModsDefinition == modsDefinition.Rare)
//			{
//				MakeItRare();
//			}
//
//			if (monsterModsDefinition == modsDefinition.Champion)
//			{
//				if (previousChamp == null)
//				{
//					MakeItChamp();
//				}
//				else
//				{
//					ClonePreviousChamp();
//				}
//			}
//
//			if (monsterModsDefinition == modsDefinition.RareMinion && rareObjForMinions != null)
//			{
//				MakeItMinion();
//			}
//
//		}
//
//
//		ImplementPresets();
	}

	public StatusPlayer TargetStatus{
		get{ return targetStatus; }
		set{ targetStatus = value; }
	}

	public override void ReceivDamage (float amount, Status _Status)
	{
		if (targetStatus == null)
			targetStatus = _Status as StatusPlayer;
		base.ReceivDamage (amount, _Status);
	}

	protected override void Death(){

		_Animator.SetFloat("Move", 0.0f);
		_Animator.SetBool ("BasicAttackBool", false);
		_Animator.SetBool("DeadBool", true);

        _MouseOverEnemy.SetMaterialOutline(false);

		dead = true;

		targetStatus.gainEXP (expReward, level);

		Destroy(_CharacterController);
		Destroy(collaiderMouse);

		Destroy (gameObject, 2.0f);
	}


	#region aRPG_EnemyStats resources
//	public void MakeItRare()
//	{
//		GenerateRandomAttributes(3);
//		max_health = max_health * rareHPmultiplier;
//
//
//		mouseCollider = gameObject.transform.Find("ColliderMouse").gameObject;
//		esMouseOver = mouseCollider.GetComponent<aRPG_EnemyMouseOver>();
//		esMouseOver.SetMaterials();
//		esMouseOver.SetMaterialsColors("rare");
//	}
//
//	public void MakeItChamp()
//	{
//		GenerateRandomAttributes(1);
//		max_health = max_health * champHPmultiplier;
//
//		mouseCollider = gameObject.transform.Find("ColliderMouse").gameObject;
//		esMouseOver = mouseCollider.GetComponent<aRPG_EnemyMouseOver>();
//		esMouseOver.SetMaterials();
//		esMouseOver.SetMaterialsColors("blue");
//
//	}
//
//	public void ClonePreviousChamp()
//	{
//		aRPG_EnemyStats prevChampScript = previousChamp.GetComponent<aRPG_EnemyStats>();
//		fastAttribute = prevChampScript.fastAttribute;
//		extra_dmgAttribute = prevChampScript.extra_dmgAttribute;
//		extra_HPAttribute = prevChampScript.extra_HPAttribute;
//		fireEnchanted = prevChampScript.fireEnchanted;
//		physicalEnchanted = prevChampScript.physicalEnchanted;
//		magicEnchanted = prevChampScript.magicEnchanted;
//
//		max_health = max_health * champHPmultiplier;
//
//		thisName = prevChampScript.thisName;
//
//		mouseCollider = gameObject.transform.Find("ColliderMouse").gameObject;
//		esMouseOver = mouseCollider.GetComponent<aRPG_EnemyMouseOver>();
//		esMouseOver.SetMaterials();
//		esMouseOver.SetMaterialsColors("blue");
//	}
//
//	public void MakeItMinion()
//	{
//		aRPG_EnemyStats rareObjForMinionsScript = rareObjForMinions.GetComponent<aRPG_EnemyStats>();
//		fastAttribute = rareObjForMinionsScript.fastAttribute;
//		extra_dmgAttribute = rareObjForMinionsScript.extra_dmgAttribute;
//		extra_HPAttribute = rareObjForMinionsScript.extra_HPAttribute;
//		fireEnchanted = rareObjForMinionsScript.fireEnchanted;
//		physicalEnchanted = rareObjForMinionsScript.physicalEnchanted;
//		magicEnchanted = rareObjForMinionsScript.magicEnchanted;
//
//		thisName = "Minion";
//	}
//
//	// this function randomly generates mods for rare/champions. Here you can add you own mods following the pattern, no need to change any other function to add a new mod.
//	void GenerateRandomAttributes(int numberOfModsToGenerate)
//	{
//		int numberOfModsGenerated = 0;
//		int firstGeneratedModNo = 0;
//		int secondGeneratedModNo = 0;
//		while (numberOfModsGenerated < numberOfModsToGenerate)
//		{
//			float random_att_no1;
//			do{
//				random_att_no1 = Random.Range(1, 7);
//
//			} while (random_att_no1 == firstGeneratedModNo || random_att_no1 == secondGeneratedModNo);
//
//
//			if (random_att_no1 == 1)
//			{
//				fastAttribute = true;
//				GenerateName("Fast");
//				if (firstGeneratedModNo == 0)
//				{
//					firstGeneratedModNo = 1;
//				}
//				if (secondGeneratedModNo == 0 && firstGeneratedModNo != 1)
//				{
//					secondGeneratedModNo = 1;
//				}
//				numberOfModsGenerated++;
//				//att1
//			}
//			if (random_att_no1 == 2)
//			{
//				extra_dmgAttribute = true;
//				GenerateName("Extra Damage");
//				if (firstGeneratedModNo == 0)
//				{
//					firstGeneratedModNo = 2;
//				}
//				if (secondGeneratedModNo == 0 && firstGeneratedModNo != 2)
//				{
//					secondGeneratedModNo = 2;
//				}
//				numberOfModsGenerated++;
//				//att2
//			}
//			if (random_att_no1 == 3)
//			{
//				extra_HPAttribute = true;
//				GenerateName("Extra Life");
//				if (firstGeneratedModNo == 0)
//				{
//					firstGeneratedModNo = 3;
//				}
//				if (secondGeneratedModNo == 0 && firstGeneratedModNo != 3)
//				{
//					secondGeneratedModNo = 3;
//				}
//				numberOfModsGenerated++;
//				//att3
//			}
//			if (random_att_no1 == 4)
//			{
//				fireEnchanted = true;
//				GenerateName("Fire");
//				if (firstGeneratedModNo == 0)
//				{
//					firstGeneratedModNo = 4;
//				}
//				if (secondGeneratedModNo == 0 && firstGeneratedModNo != 4)
//				{
//					secondGeneratedModNo = 4;
//				}
//				numberOfModsGenerated++;
//				//att4
//			}
//			if (random_att_no1 == 5)
//			{
//				physicalEnchanted = true;
//				GenerateName("Physical");
//				if (firstGeneratedModNo == 0)
//				{
//					firstGeneratedModNo = 5;
//				}
//				if (secondGeneratedModNo == 0 && firstGeneratedModNo != 5)
//				{
//					secondGeneratedModNo = 5;
//				}
//				numberOfModsGenerated++;
//				//att5
//			}
//			if (random_att_no1 == 6)
//			{
//				magicEnchanted = true;
//				GenerateName("Magic");
//				if (firstGeneratedModNo == 0)
//				{
//					firstGeneratedModNo = 6;
//				}
//				if (secondGeneratedModNo == 0 && firstGeneratedModNo != 6)
//				{
//					secondGeneratedModNo = 6;
//				}
//				numberOfModsGenerated++;
//				//att6
//			}
//
//		}
//	}

//	// it implements mods to the enemy(simple check boolean in inspector is not enought, this function has to be ran).
//	public void ImplementPresets()
//	{
//		if (fastAttribute == false){ speed_bonus = 0f;}
//		else 
//		{
//			esMovement.rotationSpeed = esMovement.rotationSpeed + (2 * esMovement.rotationSpeed * speed_bonus);
//			eAnimator.speed = eAnimator.speed + speed_bonus;
//		}
//
//		if (extra_dmgAttribute == false){ extra_dmg_bonus = 0f; }
//		else 
//		{ 
//			meleeAttackDMG = meleeAttackDMG + meleeAttackDMG * extra_dmg_bonus;
//			stunChance = stunChance * stunChance_multiplier;
//		}
//
//		if (extra_HPAttribute == false) { extra_HP_bonus = 0f;}
//		else { max_health = max_health + max_health * extra_HP_bonus;
//
//		}
//
//		if (fireEnchanted == false)
//		{
//			fire_res = 0f;
//			fire_dmg_bonus = 0f;
//		}
//		if (physicalEnchanted == false)
//		{
//			physical_res = 0f;
//			physical_dmg_bonus = 0f;
//		}
//		if (magicEnchanted == false)
//		{
//			magic_res = 0f;
//			magic_dmg_bonus = 0f;
//		}
//	}

	#endregion
}
