using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttackPlayer : MonoBehaviour
{
    public float rangeAttack = 1.0f;

    private float attackAnimLengh = 1.167f;

    private Status target;
    private bool isAttack = false;
    private bool isNearTarget = false;

    private float timeNextAttack = 0.0f;

    private StatusPlayer statusPlayer;
    private MovementPlayer movementPlayer;
    private AnimatorPlayer animatorPlayer;

    private void Awake()
    {
        statusPlayer = GetComponent<StatusPlayer>();
        movementPlayer = GetComponent<MovementPlayer>();
        animatorPlayer = GetComponent<AnimatorPlayer>();
    }

    public void AttackTarget(Transform target)
    {
        isAttack = true;
        this.target = target.GetComponent<Status>();
        
    }
    public void StopAttackTarget()
    {
        if (isAttack)
        {
            isAttack = false;
            animatorPlayer.SetArrowAttack(false);
            target = null;
            isNearTarget = false;
        }
    }

    private void Update()
    {
        if (!isAttack)
            return;
        if (target.IsDead)
            StopAttackTarget();
        if (timeNextAttack <= Time.time)
        {
            if (CheckDistanceForAttack())
            {
                if (isNearTarget)
                {
                    animatorPlayer.SetArrowAttack(false);
                    isNearTarget = false;
                }
                movementPlayer.MovePosition(target.transform.position);
            }
            else
            {
                if (!isNearTarget)
                {
                    movementPlayer.StopMove();
                    animatorPlayer.SetArrowAttack(true);
                    isNearTarget = true;
                    SetStartTimeAttack();
                    return;
                }
                target.ReceivDamage(statusPlayer.AttackDmg);
                timeNextAttack = Time.time + attackAnimLengh / statusPlayer.AttackSpeed;
                
            }
        }
    }

    private bool CheckDistanceForAttack()
    {
        return (target.transform.position - transform.position).sqrMagnitude > rangeAttack * rangeAttack;
    }
    private void SetStartTimeAttack()
    {
        timeNextAttack = Time.time + (attackAnimLengh / statusPlayer.AttackSpeed) * 0.2f;
    }

}
