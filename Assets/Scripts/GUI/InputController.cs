using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SkillPanelPlayer))]
public class InputController : MonoBehaviour
{

    [SerializeField]
    private LayerMask planeToMove = -1;
    [SerializeField]
    private LayerMask rayLayers = -1;

    private PlayerController currPlayer;
    private SkillPanelPlayer skillPanelPlayer;

    #region Input settings
    public KeyCode MoveCode = KeyCode.Mouse0;
    public KeyCode MainSkillCode = KeyCode.Mouse1;
    public KeyCode SplitItem = KeyCode.L;
    public KeyCode InventoryCode = KeyCode.V;
    public KeyCode StorageCode = KeyCode.E;
    public KeyCode CharacterSystemCode = KeyCode.C;
    public KeyCode CraftSystemCode = KeyCode.K;
    public KeyCode[] HotBarItemsCode = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };
    public KeyCode[] HotBarSkillsCode = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
    #endregion

    private void Awake()
    {
        skillPanelPlayer = GetComponent<SkillPanelPlayer>();
    }
    private void Start()
    {
        currPlayer = PlayerController.currPlayer;
    }
    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetKeyDown(MainSkillCode))
            {
                currPlayer._MovementPlayer.StopMove();
                currPlayer._ArrowAttackPlayer.StopAttackTarget();
                skillPanelPlayer.ActivateMainSkill();
            }

            if (Input.GetKey(MainSkillCode))
            {
                currPlayer._MovementPlayer.LookAtPosition(GetObjectOnMouse(planeToMove).position);
            }

            if (Input.GetKeyUp(MainSkillCode))
            {
                currPlayer._MovementPlayer.LookAtPosition(GetObjectOnMouse(planeToMove).position);
                skillPanelPlayer.DeactivateMainSkill();
            }

            for (int i = 0; i < HotBarSkillsCode.Length; i++)
            {
                if (Input.GetKeyDown(HotBarSkillsCode[i]))
                {
                    skillPanelPlayer.SelectMainSkillByHotKey(i);
                    break;
                }
            }

            if (Input.GetKey(MoveCode))
            {
                Transform targetTran = GetObjectOnMouse(rayLayers);
                switch (targetTran.tag)
                {
                    case "targetingPlane":
                        currPlayer._MovementPlayer.MovePosition(GetObjectOnMouse(planeToMove).position);
                        currPlayer._ArrowAttackPlayer.StopAttackTarget();
                        break;
                    case "enemyCollider":
                        currPlayer._ArrowAttackPlayer.AttackTarget(targetTran);
                        break;
                }

                //if (trargetTran.tag == "targetingPlane")
                //{
                //    pendingMeleeAtack = false;
                //    _Animator.SetBool("BasicAttackBool", false);
                //    _NavMeshAgent.SetDestination(MoveDestination());
                //}
                //else if (pressedObject == "enemyCollider")
                //{
                //    targetObj = hitObj.transform.parent.gameObject;
                //    pendingMeleeAtack = true;
                //}

            }

        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                PlayerInterface.Instance.TogglePause();
            }

            if (Input.GetKeyUp(CharacterSystemCode))
            {
                PlayerInterface.Instance.ToggleChracterInventory();
            }

            if (Input.GetKeyUp(InventoryCode))
            {
                PlayerInterface.Instance.ToggleBagInventory();
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                for (int i = 0; i < HotBarSkillsCode.Length; i++)
                {
                    if (Input.GetKeyDown(HotBarSkillsCode[i]))
                    {
                        skillPanelPlayer.TryBindSkillToHotKey(i);
                        break;
                    }
                }
            }
        }
    }


    private Transform GetObjectOnMouse(LayerMask layerMask)
    {
        Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitObj;
        if (Physics.Raycast(rayMouse, out hitObj, 20.0f, layerMask))
            return hitObj.transform;
        else
        {
            return null;
        }
    }
}
