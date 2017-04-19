using UnityEngine;
using System.Collections;

public class MovementEnemy : MonoBehaviour
{
    public float attackRange = 1.0f;

	private Ray rayMouse;
	private RaycastHit hitObj;
	private UnityEngine.AI.NavMeshAgent _NavMeshAgent;
	private Animator _Animator;

	//[HideInInspector]
	//public bool playerInRange = false;


	private StatusEnemy _StatusEnemy;

	private Transform targetTransform;

	//private bool playerDead = false;

	//private int layerPlayer = 1 << 20;
	//private int layerObstacle = 1 << 24;
	//private int layerInterObj = 1 << 25;
	private int maskCombinedEnemySight = 0;
	private bool stateSearchTarg = true;
	private bool stateDelayOnPosition = false;
	private bool stopUpdating = false;
	private float timeleftSearch = 0.0f;
	private float timeDelayOnPosition = 3.0f;
	private SpawningEnemy _SpawningEnemy;
	//private float angle;
	//public float rotationSpeed = 5.1f;
	//public float deadZone = 0.1f;

	private enum StateEnemy{AttackTarget, SearchTarget, StayOnPosition};
	private StateEnemy _StateEnemy = StateEnemy.SearchTarget;

	public GameObject fireball;
	private GameObject spellCastPoint;
	//private float distanceToKeep;
	//private float outOfMeleeRange;
	private bool aiReady = false;
	private bool getAwayTimeRunning = false;
	private bool canGetAway = true;
	//private bool customDestRangedSet = false;
	//private string currentAiState = "none";

	// Use this for initialization
	void Start ()
	{
		//spellCastPoint = transform.FindChild ("SpellCastPoint").gameObject;
		_Animator = gameObject.GetComponent<Animator> ();
		_NavMeshAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		_StatusEnemy = gameObject.GetComponent<StatusEnemy> ();
		_SpawningEnemy = transform.parent.GetComponent<SpawningEnemy> ();
		//_NavMeshAgent.updateRotation = false;
		//_NavMeshAgent.updatePosition = false;
		_NavMeshAgent.stoppingDistance = attackRange * 0.8f;
		//distanceToKeep = _StatusEnemy.fireCastRange - eAgent.stoppingDistance;
		//outOfMeleeRange = ms.psSkills.meleeRange;

//		stoppingDistance = _StatusEnemy.rangeAttack;
//		maskCombinedEnemySight = layerPlayer | layerObstacles | layerInteractiveObject;


	}


	// Update is called once per frame
	void Update ()
	{
		if (stopUpdating)
			return; //Stop Updating and waiting Destroy
		
		if (_StatusEnemy.IsDead) { //On Dead
			_NavMeshAgent.Stop ();
			_SpawningEnemy.enemyDie (gameObject.name);
			stopUpdating = true;
			return;
		}

		switch (_StateEnemy) {

		case StateEnemy.AttackTarget:
			// If target lost or target is dead then start search new target
			if (_StatusEnemy.TargetStatus == null || _StatusEnemy.TargetStatus.IsDead) {
				_Animator.SetBool ("BasicAttackBool", false);
				targetTransform = null;
				_StateEnemy = StateEnemy.SearchTarget;
				_NavMeshAgent.ResetPath ();
				return;
			}
			//If can attack target
			if (Vector3.Distance (transform.position, targetTransform.position) < _NavMeshAgent.stoppingDistance) {
				LookAtTarget ();	
				_Animator.SetBool ("BasicAttackBool", true);
			} else { // else: target a long way then move
				_Animator.SetBool ("BasicAttackBool", false);
				_NavMeshAgent.SetDestination (targetTransform.position);
			}	
			break;

		case StateEnemy.SearchTarget:
			//If there is target
			if (_StatusEnemy.TargetStatus != null) {
				targetTransform = _StatusEnemy.TargetStatus.transform;
				_StateEnemy = StateEnemy.AttackTarget;
			} else { //else: start Stay on point
				timeleftSearch = Time.time + timeDelayOnPosition;
				_StateEnemy = StateEnemy.StayOnPosition;
			}
			break;
		case StateEnemy.StayOnPosition:
			//If there is target
			if (_StatusEnemy.TargetStatus != null) {
				targetTransform = _StatusEnemy.TargetStatus.transform;
				_StateEnemy = StateEnemy.AttackTarget;
			//else: if it time chenge position 
			}else if (timeleftSearch < Time.time) {
				_NavMeshAgent.SetDestination (_SpawningEnemy.getRandomPosition ());
				_StateEnemy = StateEnemy.SearchTarget;
			}
			break;
		}

		//Chtngt animation: Stop or move
		if (_NavMeshAgent.velocity == Vector3.zero) {
			_Animator.SetFloat ("Move", 0.0f);
		} else {
			_Animator.SetFloat ("Move", 0.5f);
		}




		//// here we check whether enemy has reached its NavMesh destination or not.
		//if (DestinationReachedBool)
		//{
		//    _Animator.SetFloat("Move", 0.0f);
		//}
		//// if destination has not been reached enemy start moving. Here and only here actuall movement is implemented
		//else
		//{
		//    _Animator.SetFloat("Move", 0.5f);
		//    ResetTriggers();
		//}


		//if (Physics.Raycast(colliderSightRange.transform.position, targetObj.transform.position - transform.position, out hitEnemy, 55f, maskCombinedEnemySight))
		//{
		//    if (hitEnemy.transform.tag == "Player")
		//    {
		//if (_StatusEnemy.AiType == StatusEnemy.AiTypes.Melee)
		//{
		//    _NavMeshAgent.SetDestination(targetObj.transform.position);
		//    if (DestinationReachedBool) // && CanAttackTarget() - delete
		//    {
		//        UseBasicAttack(true);
		//        targetAttacking = true;
		//        //_Animator.SetTrigger("Atack");
		//    }

		//}

		//if (_StatusEnemy.AiType == AiTypes.Range)
		//{
		//    FBCheckState();
		//    if (currentAiState == "FB_FollowPlayer")
		//    {
		//        SetDestinationCustomRanged();
		//    }
		//    if (currentAiState == "FB_CastFB")
		//    {
		//        CastFireball();
		//    }
		//    if (currentAiState == "FB_GetAway")
		//    {
		//        SetDestinationCustomRanged();
		//    }
		//}


		//if (animator.GetFloat("Move") == 0.0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("atack2"))
		//{
		//    LookAtTarget();
		//}

	}

	// works like LookAt function already in unity, but it is slightly smoother, refers only to XZ coordinates. It makes transform look at NavMesh point so this function should be called only when you know that there is a destination in NavMesh.
	//	protected override void LookAtCustom ()
	//	{
	//		var targetRotationShotEnemy = Quaternion.LookRotation (_NavMeshAgent.steeringTarget - transform.position, Vector3.up);
	//		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotationShotEnemy, Time.deltaTime * rotationSpeed);
	//		transform.rotation = new Quaternion (0f, transform.rotation.y, 0f, transform.rotation.w);
	//		angle = 0f;
	//
	//	}
	// works like LookAt function already in unity, but it is slightly smoother. Is used to look at the player.
	void LookAtTarget ()
	{
		var targetRotation = Quaternion.LookRotation (targetTransform.position - transform.position, Vector3.up);
		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * _NavMeshAgent.angularSpeed);
		transform.rotation = new Quaternion (0f, transform.rotation.y, 0f, transform.rotation.w);
	}

	//public void SetDestinationCustomRanged()
	//{
	//    customDestRangedSet = true;
	//    Vector3 playerVector = (transform.position - ms.player.transform.position).normalized;
	//    Vector3 positionToReach = ms.player.transform.position + playerVector * distanceToKeep;
	//    Vector3 positionCorrected = new Vector3(positionToReach.x, transform.position.y, positionToReach.z);
	//    _NavMeshAgent.SetDestination(positionCorrected);
	//}

	// Event functions are called by animations, check animation properities to set desired timing.
	void EventBasicAtack ()
	{
		_StatusEnemy.TargetStatus.ReceivDamage (_StatusEnemy.AttackDmg);
		if (_StatusEnemy.TargetStatus.IsDead) {
			_Animator.SetBool ("BasicAttackBool", false);
		}
	}

	// here we actually deal damage to the player and stun.
	//void DoDamage()
	//{
	//    var targetScript = ms.player.GetComponent<aRPG_Health>();
	//    var targetScript2 = ms.player.GetComponent<aRPG_PlayerMovement>();
	//    if (DestinationReachedBool)
	//    {
	//        targetScript.health -= esStats.DealDamage();
	//        if (esStats.stunsTarget && Random.value < esStats.stunChance)
	//        {
	//            targetScript2.Stunned();
	//        }
	//    }
	//    if (targetScript.health <= 0) { playerDead = true; }
	//}

	//public void CastFireball()
	//{
	//    StopEnemyMoveNavAgent();
	//    LookAtTarget();
	//    eAnimator.SetBool("EnFireballBool", true);
	//}

	//public void EventEnemyCastFireball()
	//{
	//    // below condition is required to avoid instantiation during transition, below statement make animator state Fireball case/name sensitive, keep that in mind while working with animator.
	//    if (eAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fireball"))
	//    {
	//        GameObject instantiatedFireball = Instantiate(fireball, new Vector3(spellCastPoint.transform.position.x, spellCastPoint.transform.position.y, spellCastPoint.transform.position.z), spellCastPoint.transform.rotation) as GameObject;
	//        instantiatedFireball.GetComponent<aRPG_EnemyFireball>().GetCasterObject(gameObject);
	//    }
	//}

	// animations runing are stopped and TakeDamage animation is ran. Here is where Stun is implemented.
	//public void DamageTaken()
	//{
	//    eAnimator.SetTrigger("TakeDamage");

	//    if (eAnimator.GetCurrentAnimatorStateInfo(0).IsName("atack2"))
	//    {
	//        eAnimator.ResetTrigger("Atack");
	//        eAnimator.Play("Idle_Glance", 0);
	//    }
	//    if (eAnimator.GetAnimatorTransitionInfo(0).IsName("zombieAtakTrans"))
	//    {
	//        eAnimator.ResetTrigger("Atack");
	//        eAnimator.Play("Idle_Glance", 0);
	//    }
	//}

	//public void ResetTriggers()
	//{
	//    eAnimator.ResetTrigger("Atack");
	//    eAnimator.SetBool("EnFireballBool", false);

	//}

//	IEnumerator AIDelay (float time)
//	{
//		aiReady = true;
//		yield return new WaitForSeconds (time);
//		aiReady = false;
//	}
//
//	IEnumerator GetAwayTime (float time)
//	{
//		getAwayTimeRunning = true;
//		yield return new WaitForSeconds (time);
//		if (canGetAway == true) {
//			canGetAway = false;
//		} else {
//			canGetAway = true;
//		}
//		getAwayTimeRunning = false;
//	}

	//    protected override void DealDamageAttack()
	//    {
	//        _StatusPlayer.ReceivDamage(_StatusEnemy.attackDmg);
	//        if (_StatusPlayer.IsDead)
	//        {
	//            UseBasicAttack(false);
	//        }
	//        Debug.Log("Attack to enemy!" + Time.time);
	//    }

//	bool CanAttackTarget ()
//	{
//		if (Vector3.Distance (transform.position, targetObj.transform.position) < _StatusEnemy.rangeAttack) {
//			return true;
//		} else {
//			return false;
//		}
//	}



}
