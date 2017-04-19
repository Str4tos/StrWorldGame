using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class SkillSetPlayer : MonoBehaviour
{
    //Skill
    public List<GameObject> skillPrefabs = new List<GameObject>();
    private List<int> unlockSkillId = new List<int>();
    private Skill selectedSkill = null;
    private bool isActiveSkill = false;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void MainSkillButtonPresser(bool KeyUpOnly)
    {
        
        //if (!KeyUpOnly && Input.GetKeyDown(InputManager.MainSkillCode))
        //{
        //    _NavMeshAgent.ResetPath();
        //    _Animator.SetBool("BasicAttackBool", false);
        //    if (skillActive == null)
        //    {
        //        skillActive = (Skill)Instantiate(skills[mainSkillId]);
        //    }
        //    else if (skillActive.SkillName != skills[mainSkillId].SkillName)
        //    {
        //        Destroy(skillActive.gameObject);
        //        skillActive = (Skill)Instantiate(skills[mainSkillId]);
        //    }
        //    skillActive.CallSkill(gameObject, true);
        //}
        
        //if (Input.GetKeyUp(InputManager.MainSkillCode))
        //{
        //    skillActive.CallSkill(gameObject, false);
        //}
    }

    private void FillPanelChoiseSkill()
    {
        if (allPlayerSkills.Count > 0)
        {
            foreach (var skill in allPlayerSkills)
            {
                AddSkillToChoisePanel(skill);
            }
        }
    }

    private GameObject GetSkillPrefabById(int idSkill)
    {
        foreach(var skillPrefab in skillPrefabs)
        {

        }
    }

    public class SkillInfo
    {
        public GameObject prefab;
        public int id;
        public string name;
        public string description;
        public Sprite icon;

    }
}

