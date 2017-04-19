using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorPlayer : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetArrowAttack(bool isAttack)
    {
        animator.SetBool("BasicAttackBool", isAttack);
    }
    public void SetAttackSpeed(float attackSpeed)
    {
        animator.SetFloat("AttackSpeed", attackSpeed);
    }
    public void SetRunning(bool isRunning)
    {
        animator.SetBool("RunningBool", isRunning);
    }
}
