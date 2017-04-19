using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillPanelPlayer : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private SkillOnPanel mainPanelSkill;
    [SerializeField]
    private List<SkillOnPanel> hotPanelSkills;
    [SerializeField]
    private GameObject panelChoiseSkillObj;
    [SerializeField]
    private GameObject skillOnChoisePanelPrefab;
    private List<SkillOnPanel> choisePanelSkills = new List<SkillOnPanel>();

    private GraphicRaycaster graphicRaycaster;
    #endregion

    private void Awake()
    {
        graphicRaycaster = GetComponent<GraphicRaycaster>();
    }
    private void Start()
    {
        FillPanelChoiseSkill();
        panelChoiseSkillObj.SetActive(false);
    }

    public void ActivateMainSkill()
    {

    }
    public void DeactivateMainSkill()
    {

    }
    public void TryBindSkillToHotKey(int hotkeyId)
    {
        SkillOnPanel skillUnderMouse = RaycastUI()[0].gameObject.GetComponent<SkillOnPanel>();
        if (skillUnderMouse)
        {
            skillUnderMouse.SetSkillText(hotkeyId.ToString());
            hotPanelSkills[hotkeyId].BindSkill(skillUnderMouse);
        }
    }
    public void SelectMainSkillByHotKey(int hotkeyId)
    {
        mainPanelSkill.SetSkill(hotPanelSkills[hotkeyId]);
    }
    public void SelectMainSkill(int skillID, Sprite skillSprite)
    {
        mainPanelSkill.SetSkill(skillID, skillSprite);
        if (panelChoiseSkillObj.activeSelf)
            TogglePanelChoiseSkill();
    }

    public void TogglePanelChoiseSkill()
    {
        panelChoiseSkillObj.SetActive(!panelChoiseSkillObj.activeSelf);
    }

    public void AddSkillToChoisePanel(Skill skill)
    {
        GameObject instance = Instantiate(skillOnChoisePanelPrefab, panelChoiseSkillObj.transform);
        SkillOnPanel instanceSkill = instance.GetComponent<SkillOnPanel>();
        instanceSkill.SetSkill(skill.idSkill, skill.iconSkill);
        choisePanelSkills.Add(instanceSkill);
    }
    
    private List<RaycastResult> RaycastUI()
    {
        List<RaycastResult> raycastResult = new List<RaycastResult>();
        PointerEventData pointerEventData = new PointerEventData(null)
        {
            position = Input.mousePosition
        };
        graphicRaycaster.Raycast(pointerEventData, raycastResult);
        return raycastResult;
    }
}
