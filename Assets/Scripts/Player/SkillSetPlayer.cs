using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class SkillSetPlayer : MonoBehaviour
{
    //Skill
    public List<SkillCustom> skills = new List<SkillCustom>();
    private SkillCustom skillActive = null;

    //---Caster
    private NavMeshAgent _NavMeshAgent;
    private Animator _Animator;
    //[HideInInspector]
    //public GameObject Target;

    //---Hotbar skills
    private Image[] slotsPanelSkillImage = new Image[4];
    private Image[] slotsPanelSkillCooldown = new Image[4];
    private int[] panelSkillsId;
    //---Main skill
    private int mainSkillId;
    private Image mainSkill;
    private Image mainSkillCooldown;

    //---Choise main skill panel
    private GameObject panelChoiseSkill;
    private GameObject slotsChoiseSkill;
    private GameObject slotChoiseSkillPrefab;

    //Mouse click for select
    private GraphicRaycaster _GraphicRaycaster;
    private List<RaycastResult> _RaycastResult;
    private PointerEventData _PointerEventData = new PointerEventData(null);

    //EventSystem
    private EventSystem _EventSystem;

    void Start()
    {
        _Animator = gameObject.GetComponent<Animator>();
        _NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        //--SlotsSkill
        GameObject slotsPanelSkillObj = GameObject.Find("MainCanvas/HotbarSkills/SlotsSkill");
        Transform tempSlotTran;
        for (int i = 0; i < 4; i++)
        {
            tempSlotTran = slotsPanelSkillObj.transform.GetChild(i);
            slotsPanelSkillImage[i] = tempSlotTran.GetComponent<Image>();
            // Child id 0 - Buttun
            slotsPanelSkillCooldown[i] = tempSlotTran.GetChild(0).GetComponent<Image>();
        }
        panelSkillsId = new int[] { -1, -1, -1, -1 };

        //--HotbarMainSkill
        tempSlotTran = GameObject.Find("MainCanvas/HotbarMainSkill").transform;
        // Child id 0 - MainSkillButton
        mainSkill = tempSlotTran.GetChild(0).GetComponent<Image>();
        // Child id 1 - CooldownImage
        mainSkillCooldown = tempSlotTran.GetChild(1).GetComponent<Image>();
        mainSkillId = -1;

        //--ChoiseSkillsPanel
        panelChoiseSkill = GameObject.FindWithTag("MainCanvas").transform.FindChild("ChoiseSkillsPanel").gameObject;
        // Child id 0 - SlotsSkills
        slotsChoiseSkill = panelChoiseSkill.transform.GetChild(0).gameObject;
        panelChoiseSkill.SetActive(false);

        _GraphicRaycaster = GameObject.FindWithTag("MainCanvas").GetComponent<GraphicRaycaster>();

        _EventSystem = UnityEngine.EventSystems.EventSystem.current;
    }

    void Update()
    {
        //if mouse on UI
        if (_EventSystem.IsPointerOverGameObject())
        {
            // Stop use Skill
            MainSkillButtonPresser(true);

            //Chenge main skill by ChoiseSkillsPanel
            ChoiseMainSkill();
            if (panelChoiseSkill.activeSelf)
            {
                //Bind selected skill to hotbar

                BindSelectedSkill();
            }
        }
        //Main skill activation
        else 
        {
            MainSkillButtonPresser(false);
        }

        //Fast chenge main skill by hotkey (1,2,3,4 - defoult)
        FastChengeMainSkill();

        
        
    }

    private void MainSkillButtonPresser(bool KeyUpOnly)
    {
        if (mainSkillId < 0)
            return;
        if (!KeyUpOnly && Input.GetKeyDown(InputManager.MainSkillCode))
        {
            _NavMeshAgent.ResetPath();
            _Animator.SetBool("BasicAttackBool", false);
            if (skillActive == null)
            {
                skillActive = (SkillCustom)Instantiate(skills[mainSkillId]);
            }
            else if (skillActive.SkillName != skills[mainSkillId].SkillName)
            {
                Destroy(skillActive.gameObject);
                skillActive = (SkillCustom)Instantiate(skills[mainSkillId]);
            }
            skillActive.CallSkill(gameObject, true);
        }
        
        if (Input.GetKeyUp(InputManager.MainSkillCode))
        {
            skillActive.CallSkill(gameObject, false);
        }
    }

    private void FastChengeMainSkill()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown(InputManager.HotBarSkillsCode[i]) && panelSkillsId[i] > -1)
            {
                mainSkillId = panelSkillsId[i];
                mainSkill.sprite = slotsPanelSkillImage[i].sprite;
            }
        }
    }

    private void ChoiseMainSkill()
    {

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            int skillSelectedId = RaycastUI();
            if (skillSelectedId > -1)
            {
                mainSkillId = skillSelectedId;
                mainSkill.sprite = skills[skillSelectedId].Icon;
                panelChoiseSkill.SetActive(false);
            }
            else if (skillSelectedId == -1)
            {
                if (panelChoiseSkill.activeSelf)
                {
                    panelChoiseSkill.SetActive(false);
                }
                else
                {
                    RefreshChoiseSkillsPanel();
                    panelChoiseSkill.SetActive(true);
                }
            }

        }
    }

    private void BindSelectedSkill()
    {
        int skillSelectedId = RaycastUI();
        if (skillSelectedId > -1)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Input.GetKeyUp(InputManager.HotBarSkillsCode[i]))
                {
                    if (panelSkillsId[i] > -1)
                    {
                        slotsChoiseSkill.transform.GetChild(panelSkillsId[i]).GetChild(0).GetComponent<Text>().text = "";
                    }
                    panelSkillsId[i] = skillSelectedId;
                    slotsPanelSkillImage[i].sprite = skills[skillSelectedId].Icon;
                    // Child id 0 - Text
                    _RaycastResult[0].gameObject.transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
                    break;
                }
            }
        }
    }

    private int RaycastUI()
    {
        _RaycastResult = new List<RaycastResult>();
        _PointerEventData.position = Input.mousePosition;
        //Debug.Log (_PointerEventData.lastPress.name);
        _GraphicRaycaster.Raycast(_PointerEventData, _RaycastResult);
        if (_RaycastResult != null)
        {
            if (_RaycastResult[0].gameObject.CompareTag("SkillToSelect"))
            {
                return Int32.Parse(_RaycastResult[0].gameObject.name);
            }
            else if (_RaycastResult[0].gameObject.name == "MainSkillButton")
            {

                return -1;

            }
        }
        return -5;
    }

    private void RefreshChoiseSkillsPanel()
    {
        if (slotChoiseSkillPrefab == null)
        {
            slotChoiseSkillPrefab = (GameObject)Resources.Load("Prefabs/GUI/SlotSkillChoise");
        }

        Transform slotsChoiseSkillTran = slotsChoiseSkill.transform;
        if (skills.Count > slotsChoiseSkillTran.childCount)
        {
            for (int i = slotsChoiseSkillTran.childCount; i < skills.Count; i++)
            {
                Instantiate(slotChoiseSkillPrefab, slotsChoiseSkillTran);
                //GameObject tempSlot = 
                //tempSlot.transform.SetParent (slotsChoiseSkillTran);
            }
        }
        else
        {
            for (int i = slotsChoiseSkillTran.childCount - 1; i >= skills.Count; i--)
            {
                Destroy(slotsChoiseSkillTran.GetChild(i).gameObject);
            }
        }


        for (int i = 0; i < skills.Count; i++)
        {
            slotsChoiseSkillTran.GetChild(i).GetComponent<Image>().sprite = skills[i].Icon;
            slotsChoiseSkillTran.GetChild(i).name = i.ToString();
        }

    }




}

