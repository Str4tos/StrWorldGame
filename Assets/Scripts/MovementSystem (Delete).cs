using UnityEngine;
using System.Collections;

public class MovementSystem : MonoBehaviour
{
    protected Ray rayMouse;
    protected RaycastHit hitObj;
    protected UnityEngine.AI.NavMeshAgent _NavMeshAgent;
    protected Animator _Animator;
    protected GameObject targetObj;

    
     // this layer is for registering movement. Targeting plane prefab should be placed just beneath the ground mesh used to bake nav mesh.   
    protected int layerObstacles = 1 << 24;
    protected int layerInteractiveObject = 1 << 25; // this is doors mostly.    
    protected int layerBullets = 1 << 26;
    protected int layerTargetingPlaneToShoot = 1 << 27;
    //protected int layerGround = 1 << 28;
    //protected int layerEnemiesProjectiles = 1 << 29;


    //protected bool DestinationReachedBool = false;
    protected float angle;
    //protected float stoppingDistance;
    // deadZone and rotationSpeed controls player character rotations
	//public float movementSpeed = 6;
	//public float deadZone = 8.0f;
    //public float rotationSpeed = 22.0f;
    
    

    //public CharacterController pCharacterController;
    

//    protected void UpdatePosition(){
//
//         // here we make sure that navmeshAgent follows player
//        Vector3 worldDeltaPosition = _NavMeshAgent.nextPosition - transform.position;
//        if (worldDeltaPosition.magnitude > _NavMeshAgent.radius)
//        {
//            _NavMeshAgent.nextPosition = transform.position + 0.9f * worldDeltaPosition;
//        }
//
//        // here we calculate angle difference between current player direction a a direction to go, without it player will "LookAt" so often that it will look unnatural.
//        angle = FindAngle(transform.forward, _NavMeshAgent.desiredVelocity, transform.up);
//        if (Mathf.Abs(angle) > deadZone)
//        {
//            LookAtCustom();
//        }
//
//    }
    
//	protected bool DestinationReached(){
//		if (Vector3.Distance(transform.position, _NavMeshAgent.destination) < _NavMeshAgent.stoppingDistance)
//			return true;
//		else
//			return false;
//	}

    

    
    protected float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        if (toVector == Vector3.zero)
            return 0f;
        angle = Vector3.Angle(fromVector, toVector);
        Vector3 normal = Vector3.Cross(fromVector, toVector);
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));
        angle *= Mathf.Deg2Rad;
        return angle;
    }

    

    // # this is custom version of "LookAt" it should be used in update or while button is being held, cause it rotates player over time, it also allows to control the speed of rotation with "rotationSpeed". It is designed to look at point rather then an object.
    protected virtual void LookAtCustom()
    {
        
    }

//    protected void UseBasicAttack(bool activeAttack)
//    {
//        //if (mAnimator == null)
//        //    mAnimator = gameObject.GetComponent<Animator>();
//        _Animator.SetBool("BasicAttackBool", activeAttack);
//        if (activeAttack)
//        {
//            timeAnimationAttack = _Animator.GetCurrentAnimatorClipInfo(0).Length;
//            InvokeRepeating("DealDamageAttack", timeAnimationAttack * 0.3f, timeAnimationAttack);
//        }
//        else
//        {
//            CancelInvoke("DealDamageAttack");
//        }
//    }

//    protected virtual void DealDamageAttack()
//    {
//      
//    }

}
