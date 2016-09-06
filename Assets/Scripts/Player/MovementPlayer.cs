using UnityEngine;
using System.Collections;

public class MovementPlayer : MonoBehaviour {

    
	private Ray rayMouse;
	private RaycastHit hitObj;
	private NavMeshAgent _NavMeshAgent;
	private Animator _Animator;
	private GameObject targetObj;

    private int maskMoveEnemiesDoors = 0;
	private int layerTargetingPlaneToMove = 1 << 23;
    //private int maskMoveEnemies = 0;
    //private int maskShootEnemies = 0;

    private KeyCode moveCode;
    private KeyCode useSkillCode;
    
    private StatusPlayer _StatusPlayer;
    private StatusEnemy _StatusEnemy;

    private Collider colliderHit;

    private bool pendingMeleeAtack = false;

//    private bool pendingOpenDoor = false;
//    private bool isNearDoor = false;
//    private GameObject door;
//    private GameObject nearDoorId;
//    private Transform doorPos; 
    
    private string pressedObject;

	// Use this for initialization
	void Start () {

		int layerEnemyMouseCollider = 1 << 22; // this layer is for enemies - more precisly it is for ColliderMouse child game object of the enemy prefab. By changing the size of the collider you can change mouse precision for targeting enemies   

		maskMoveEnemiesDoors = layerTargetingPlaneToMove | layerEnemyMouseCollider;

        _StatusPlayer = gameObject.GetComponent<StatusPlayer>();
        _Animator = gameObject.GetComponent<Animator>();
        _NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        //maskMoveEnemies = layerTargetingPlaneToMove | layerEnemyMouseCollider;
        //maskShootEnemies = layerTargetingPlaneToShoot | layerEnemyMouseCollider;

//        _NavMeshAgent.updatePosition = false;
//        _NavMeshAgent.updateRotation = false;
//		_NavMeshAgent.speed = movementSpeed;

//        deadZone *= Mathf.Deg2Rad;
        //stoppingDistance = mNavAgent.stoppingDistance;

        moveCode = InputManager.MoveCode;
        useSkillCode = InputManager.MainSkillCode;
	}
	
	// Update is called once per frame
	void Update () {
		//Block raycast on UI
		if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()) { 
			
			if (Input.GetKey (moveCode)) {
				
				pressedObject = CheckHitOnMouse ();

				if (pressedObject == "Decor") {
					return;
				}

				if (pressedObject == "targetingPlane") {
					pendingMeleeAtack = false;
					_Animator.SetBool ("BasicAttackBool", false);
					_NavMeshAgent.SetDestination (MoveDestination ());
				} else if (pressedObject == "enemyCollider") {
					targetObj = hitObj.transform.parent.gameObject;
					pendingMeleeAtack = true;
				}

			}

			if (Input.GetKeyUp (moveCode)) {

				//NPC Click ^
				if (pressedObject == "NPC") {
					//ms.psSkills.PlayerOpensDoor();
				}

				if (pressedObject == "door") {
					//ms.psSkills.PlayerOpensDoor();
				}
			}

			if (Input.GetKey (useSkillCode)) {
				pressedObject = CheckHitOnMouse ();
            
				LookAtHere (hitObj.point);
            
            
			}
		}
			

        //var rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        //var targetRotationShotEnemy = Quaternion.LookRotation(Input.mousePosition, Vector3.up); // - transform.position
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationShotEnemy, Time.deltaTime * rotationSpeed);
        //transform.rotation = new Quaternion(0f, transform.rotation.y, 0f, transform.rotation.w);
        //UpdatePosition();


        // this enables tracking. Tracking makes character follow a monster, without it player would go to the old monster position.
//        if (pendingMeleeAtack)
//        {
//            if (targetObj != null)
//            {
//                SetDestinationCustom(targetObj.transform.position);
//            }
//            //stoppingDistance = ms.psSkills.meleeRange * 0.5f;
//        }
//        else
//        { //stoppingDistance = mNavAgent.stoppingDistance; 
//        }

        // this is related to tracking. This enables character to make an attack after he reaches destination(while no button is being held. It is not required to use it to make an attack when you hold button) 
        if (pendingMeleeAtack)
        {
			
			if (Vector3.Distance(targetObj.transform.position, transform.position) < _StatusPlayer.rangeAttack)
            {
                pendingMeleeAtack = false;
				_NavMeshAgent.ResetPath();
				_StatusEnemy = targetObj.GetComponent<StatusEnemy>();
				LookAtHere(targetObj.transform.position);
				_Animator.SetBool("BasicAttackBool", true);

            }
            else
            {
				pendingMeleeAtack = true;
				_Animator.SetBool("BasicAttackBool", false);
				_NavMeshAgent.SetDestination(targetObj.transform.position);
            }
            
        }

//        // this allows player to open a door when a button is NOT being held.
//        if (pendingOpenDoor == true && isNearDoor == true && door != null && door.GetInstanceID() == nearDoorId.GetInstanceID())
//        {
//            if (door.GetComponent<aRPG_OpenDoor>().onceOpened == false)
//            {
//                door.GetComponent<aRPG_OpenDoor>().OpenDoor();
//                return;
//            }
//        }
//
		if (_NavMeshAgent.velocity == Vector3.zero) {
			_Animator.SetFloat ("Move", 0.0f);
		} else {
			_Animator.SetFloat("Move", 0.5f);
		}

        // here we stop movement when a destination is reached

//		if ((isRunning && !DestinationReached ()) || pendingMeleeAtack) {
//			//trackingToEnemy = false;
//			//_NavMeshAgent.Stop();
//
//		} else {
//			
//		}
        // and finally most important part. Here we initiate movement.
//        if (newPathPending)
//        {
////			transform.position += transform.forward* movementSpeed * Time.deltaTime;
////			transform.position = new Vector3(transform.position.x, _NavMeshAgent.nextPosition.y, transform.position.z);
//        }
//        else
//        {
//            StopMoveNavAgent();
//        }

	}

//
//    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
//    {
//        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
//    }

	private Vector3 MoveDestination()
	{
		rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(rayMouse, out hitObj, 60.0f, layerTargetingPlaneToMove))
		{
			return hitObj.point;
		}
		else { return transform.position; }
	}

    private string CheckHitOnMouse()
    {
        rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
//        if (Physics.Raycast(rayMouse, out hitObj, 60.0f, layerUI))
//        {
//            return null;
//        }
		if (Physics.Raycast(rayMouse, out hitObj, 20.0f, maskMoveEnemiesDoors))
		//if(Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.TransformDirection(Vector3.down),out hitObj, 60.0f, maskMoveEnemiesDoors))
            return hitObj.transform.tag;
        else { return ""; }
    }

//    protected override void LookAtCustom()
//    {
//        var targetRotation = Quaternion.LookRotation(_NavMeshAgent.desiredVelocity, Vector3.up);
//        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
//        angle = 0f;
//    }

//    private void SetDestinationCustom(Vector3 destination)
//    {
//        newPathPending = true;
//        _Animator.SetFloat("Move", 0.5f);
//        _NavMeshAgent.SetDestination(destination);
//
//        //ResetTriggers();
//    }

    private void LookAtHere(Vector3 point)
    {
        var targetRotationShotEnemy = Quaternion.LookRotation(point - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationShotEnemy, 60.0f);
        transform.rotation = new Quaternion(0f, transform.rotation.y, 0f, transform.rotation.w);
    }

//	public void StopMoveNavAgent()
//    {
//        newPathPending = false;
//        _Animator.SetFloat("Move", 0.0f);
//        _NavMeshAgent.ResetPath();
//        _NavMeshAgent.Stop();
//        _NavMeshAgent.enabled = false;
//        _NavMeshAgent.enabled = true;
//    }

//    protected override void DealDamageAttack()
//    {
//        _StatusEnemy.ReceivDamage(_StatusPlayer.attackDmg);
//        if (_StatusEnemy.IsDead)
//        {
//			_Animator.SetBool("BasicAttackBool", false);
//        }
//        Debug.Log("Attack to enemy!" + Time.time);
//    }

	public void EventBasicAttack(){
		_StatusEnemy.ReceivDamage(_StatusPlayer.getAttackDamage);
		if (_StatusEnemy.IsDead)
		{
			_Animator.SetBool("BasicAttackBool", false);
		}
	}

}
