using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatusPlayer))]
[RequireComponent(typeof(SkillSetPlayer))]
[RequireComponent(typeof(MovementPlayer))]
[RequireComponent(typeof(ArrowAttackPlayer))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController currPlayer;

    public SkillSetPlayer _SkillSetPlayer { get; private set; }
    public MovementPlayer _MovementPlayer { get; private set; }
    public StatusPlayer _StatusPlayer { get; private set; }
    public ArrowAttackPlayer _ArrowAttackPlayer { get; private set; }

    private void Awake()
    {
        currPlayer = this;
        _SkillSetPlayer = GetComponent<SkillSetPlayer>();
        _MovementPlayer = GetComponent<MovementPlayer>();
        _StatusPlayer = GetComponent<StatusPlayer>();
        _ArrowAttackPlayer = GetComponent<ArrowAttackPlayer>();

    }
}
