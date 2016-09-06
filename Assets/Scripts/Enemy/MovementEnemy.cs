using UnityEngine;
using System.Collections;

public class MovementEnemy : MonoBehaviour
{

	private Ray rayMouse;
	private RaycastHit hitObj;
	private NavMeshAgent _NavMeshAgent;
	private Animator _Animator;
	private GameObject targetObj;

	//[HideInInspector]
	//public bool playerInRange = false;


	private StatusEnemy _StatusEnemy;
	private StatusPlayer _StatusPlayer;

	private Transform colliderSightRange;
	private SightEnemy _SightEnemy;

	//private bool playerDead = false;

	//private int layerPlayer = 1 << 20;
	//private int layerObstacle = 1 << 24;
	//private int layerInterObj = 1 << 25;
	private int maskCombinedEnemySight = 0;
	private bool stateSearchTarg = true;
	private bool stateDelayOnPosition = false;
	private float timeleftSearch = 0.0f;
	private float timeDelayOnPosition = 1.5f;
	private SpawningEnemy _SpawningEnemy;
	//private float angle;
	//public float rotationSpeed = 5.1f;
	//public float deadZone = 0.1f;


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
		_NavMeshAgent = gameObject.GetComponent<NavMeshAgent> ();
		_StatusEnemy = gameObject.GetComponent<StatusEnemy> ();
		colliderSightRange = gameObject.transform.FindChild ("ColliderSightRange");
		_SightEnemy = colliderSightRange.GetComponent<SightEnemy> ();
		_SpawningEnemy = gameObject.transform.parent.GetComponent<SpawningEnemy> ();
		//_NavMeshAgent.updateRotation = false;
		//_NavMeshAgent.updatePosition = false;
		_NavMeshAgent.stoppingDistance = _StatusEnemy.rangeAttack * 0.8f;
		//distanceToKeep = _StatusEnemy.fireCastRange - eAgent.stoppingDistance;
		//outOfMeleeRange = ms.psSkills.meleeRange;

//		stoppingDistance = _StatusEnemy.rangeAttack;
//		maskCombinedEnemySight = layerPlayer | layerObstacles | layerInteractiveObject;


	}

	// Update is called once per frame
	void Update ()
	{
		if (_StatusPlayer != null && _StatusPlayer.IsDead) {
			StopActions ();
		}
		if (_SightEnemy.GetPlayerInRangeBool && _SightEnemy.GetPlayerInRangeObj != null) {
			if (targetObj == null) {
				targetObj = _SightEnemy.GetPlayerInRangeObj;
				_StatusPlayer = targetObj.GetComponent<StatusPlayer> ();
			}
			stateSearchTarg = false;
			_NavMeshAgent.SetDestination (targetObj.transform.position);
			
		} else if (!stateSearchTarg) {
			StopActions ();
		}

		//UpdatePosition ();

        
		//if (_StatusEnemy.AiType == StatusEnemy.AiTypes.Melee)
		//{
//		if (!stateSearchTarg && DestinationReached ()) {  // && CanAttackTarget() - delete
//			_Animator.SetBool ("BasicAttackBool", true);
//		} else {
//			_Animator.SetBool ("BasicAttackBool", false);
//		}

		if (_NavMeshAgent.velocity == Vector3.zero) {
			_Animator.SetFloat ("Move", 0.0f);
			if (stateSearchTarg) {  // && CanAttackTarget() - delete
				if (!stateDelayOnPosition) {
					timeleftSearch = Time.time + timeDelayOnPosition;
					stateDelayOnPosition = true;
				}
			} else if (Vector3.Distance (transform.position, targetObj.transform.position) < _NavMeshAgent.stoppingDistance) {
				LookAtTarget ();	
				_Animator.SetBool ("BasicAttackBool", true);
			} else {
				_NavMeshAgent.SetDestination (targetObj.transform.position);
			}
			
				
		} else {
			if (stateSearchTarg) {
				stateDelayOnPosition = false;
			} else {
				_Animator.SetBool ("BasicAttackBool", false);
			}
			_Animator.SetFloat ("Move", 0.5f);
		}

		if (stateSearchTarg && stateDelayOnPosition && timeleftSearch < Time.time) {
			_NavMeshAgent.SetDestination (_SpawningEnemy.getRandomPosition ());
			stateDelayOnPosition = false;
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


	public StatusPlayer getStatusPlayer ()
	{
		return _StatusPlayer;
	}

	public void StopActions ()
	{
		_SightEnemy.setDefaultSphreCollider ();
		stateSearchTarg = true;
		_NavMeshAgent.ResetPath ();
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
		var targetRotation = Quaternion.LookRotation (targetObj.transform.position - transform.position, Vector3.up);
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
		_StatusPlayer.ReceivDamage (_StatusEnemy.getAttackDamage);
		if (_StatusPlayer.IsDead) {
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
