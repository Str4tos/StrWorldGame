using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillOnPanel : MonoBehaviour
{
    private int skillId = -1;
    [SerializeField]
    private Image skillImage;
    [SerializeField]
    private Text skillText;
    [SerializeField]
    private bool mainSkill = false;

    private SkillOnPanel bindedSkill;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickSkill);
    }
    public void BindSkill(SkillOnPanel skillOnPanel)
    {
        if (bindedSkill)
            bindedSkill.SetSkillText("");
        bindedSkill = skillOnPanel;
        SetSkill(skillOnPanel);
    }
    public void SetSkill(SkillOnPanel skillOnPanel)
    {
        int newSkillId = skillOnPanel.GetSkillID();
        if (newSkillId < 0)
            return;
        skillId = newSkillId;
        skillImage.sprite = skillOnPanel.GetSkillSprite();
    }
    public void SetSkill(int skillId, Sprite skillSprite)
    {
        if (skillId < 0)
            return;
        this.skillId = skillId;
        skillImage.sprite = skillSprite;
    }
    public void SetSkillText(string skillText)
    {
        this.skillText.text = skillText;
    }
    public int GetSkillID()
    {
        return skillId;
    }
    public Sprite GetSkillSprite()
    {
        return skillImage.sprite;
    }

    public void OnClickSkill()
    {
        if (mainSkill)
        {
            PlayerInterface.Instance.SkillPanel.TogglePanelChoiseSkill();
        }
        else
        {
            PlayerInterface.Instance.SkillPanel.SelectMainSkill(skillId, skillImage.sprite);
        }
    }

}
