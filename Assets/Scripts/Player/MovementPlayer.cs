using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class MovementPlayer : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private AnimatorPlayer animatorPlayer;
    private Transform target;

    private bool isMove = false;
    private bool isFollowTargegt = false;
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animatorPlayer = GetComponent<AnimatorPlayer>();
    }

    private void Update()
    {
        
        if (navMeshAgent.velocity == Vector3.zero)
        {
            if (isFollowTargegt)
            {
                if ((target.position - transform.position).sqrMagnitude < 4.0f)
                {
                    MovePosition(target.position);
                    return;
                }
            }
            if (isMove)
            {
                animatorPlayer.SetRunning(false);
                isMove = false;
            }
        }
        else
        {
            if (!isMove)
            {
                animatorPlayer.SetRunning(true);
                isMove = true;
            }
        }


    }

    public void SetFollowTarget(Transform target)
    {
        isFollowTargegt = true;
        this.target = target;
    }
    public void MovePosition(Vector3 newPosition)
    {
        if (!isMove)
        {
            isMove = true;
            animatorPlayer.SetRunning(true);
        }
        navMeshAgent.SetDestination(newPosition);
    }
    public void StopMove()
    {
        if (isMove)
        {
            animatorPlayer.SetRunning(false);
            navMeshAgent.Stop();
        }
    }
    //public void MovePositionOnDistance(Vector3 newPosition, float distance, VoidMethod OnDone)
    //{
    //    MovePosition(newPosition);
    //    StartCoroutine(CheckDistanceToPoint(newPosition, distance, OnDone));
    //}
    //private IEnumerator CheckDistanceToPoint(Vector3 newPosition, float distance, VoidMethod OnDone)
    //{
    //    distance *= distance;
    //    while ((transform.position - newPosition).sqrMagnitude > distance)
    //    {
    //        yield return new WaitForEndOfFrame();
    //    }
    //    if (OnDone != null)
    //        OnDone();
    //}

    public void LookAtPosition(Vector3 position)
    {
        var targetRotation = Quaternion.LookRotation(position - transform.position, Vector3.up);
        targetRotation = Quaternion.Slerp(transform.rotation, targetRotation, 60.0f);
        targetRotation.x = transform.rotation.x;
        targetRotation.z = transform.rotation.z;
        transform.rotation = targetRotation;
    }

    //public void EventBasicAttack()
    //{
    //    _StatusEnemy.ReceivDamage(_StatusPlayer.getAttackDamage, _StatusPlayer);
    //    if (_StatusEnemy.IsDead)
    //    {
    //        _Animator.SetBool("BasicAttackBool", false);
    //    }
    //}

}
