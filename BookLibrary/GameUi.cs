﻿using Config;
using FrameWork;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Liuguang.mod.Taiwu.BookLibrary;

/// <summary>
/// 调用游戏内置的UI
/// </summary>
internal static class GameUi
{
    /// <summary>
    /// 展示获取到的物品列表
    /// </summary>
    /// <param name="itemDisplayDataList"></param>
    public static void ShowItemList(List<ItemDisplayData> itemDisplayDataList)
    {
        var argsBox = EasyPool.Get<ArgumentBox>();
        argsBox.SetObject("DisplayData", itemDisplayDataList);
        argsBox.Set("Title", LocalStringManager.Get("LK_Get_Item_Get"));
        UIElement.GetItem.SetOnInitArgs(argsBox);
        UIManager.Instance.ShowUI(UIElement.GetItem);
    }

    /// <summary>
    /// 显示技能信息提示
    /// </summary>
    /// <param name="combatSkillId">技能id</param>
    public static void ShowCombatSkillTips(short combatSkillId)
    {
        var mouseTipManager = SingletonObject.getInstance<MouseTipManager>();
        var argsBox = EasyPool.Get<ArgumentBox>();
        argsBox.Set("CombatSkillId", combatSkillId);
        var playerId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
        argsBox.Set("CharId", playerId);
        argsBox.Set("PracticeLevel", 0);
        argsBox.Set("CheckEquipRequirePracticeLevel", false);
        argsBox.Set("UsePracticeLevelInDisplayData", false);
        argsBox.Set("ShowOnlyTemplateInfo", true);
        mouseTipManager.ShowTips(TipType.CombatSkill, argsBox);
    }

    /// <summary>
    /// 隐藏技能信息提示
    /// </summary>
    public static void HideCombatSkillTips()
    {
        var mouseTipManager = SingletonObject.getInstance<MouseTipManager>();
        mouseTipManager.HideTips(TipType.CombatSkill);
    }

    /// <summary>
    /// 显示书籍信息提示
    /// </summary>
    /// <param name="bookId">书籍id</param>
    public static void ShowSkillBookTips(short bookId)
    {
        var skillBookConfig = SkillBook.Instance[bookId];
        var itemData = new ItemDisplayData()
        {
            Key = new(skillBookConfig.ItemType, 0, bookId, 0),
            Amount = 1,
            Durability = skillBookConfig.MaxDurability,
            MaxDurability = skillBookConfig.MaxDurability,
            Weight = skillBookConfig.BaseWeight,
            Value = skillBookConfig.BaseValue,
        };

        var mouseTipManager = SingletonObject.getInstance<MouseTipManager>();
        var argsBox = EasyPool.Get<ArgumentBox>();
        argsBox.Set("ItemData", itemData);
        argsBox.Set("ShowPageInfo", false);
        argsBox.Set("templateDataOnly", false);
        mouseTipManager.ShowTips(TipType.SkillBook, argsBox);
    }

    /// <summary>
    /// 隐藏书籍信息提示
    /// </summary>
    public static void HideSkillBookTips()
    {
        var mouseTipManager = SingletonObject.getInstance<MouseTipManager>();
        mouseTipManager.HideTips(TipType.SkillBook);
    }

    /// <summary>
    /// 创建一个可勾选框(38*38)
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public static GameObject CreateCheckBox(GameObject parent, string objectName = "CheckBox")
    {
        var checkBoxObj = UiTool.CreateRectObject(objectName);
        checkBoxObj.transform.SetParent(parent.transform, false);
        var checkBox = checkBoxObj.AddComponent<CToggle>();
        var background = UiTool.CreateRectObject("Background");
        background.transform.SetParent(checkBoxObj.transform, false);
        var rect = background.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        var backgroundImage = background.AddComponent<CImage>();
        backgroundImage.SetSprite("sp_gouxuankuang_0");
        checkBox.targetGraphic = backgroundImage;
        //
        var checkmark = UiTool.CreateRectObject("Checkmark");
        checkmark.transform.SetParent(background.transform, false);
        rect = checkmark.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new(-3.5f, 2.0f);
            rect.offsetMax = new(3.5f, 2.0f);
        }
        var checkmarkImage = checkmark.AddComponent<CImage>();
        checkmarkImage.SetSprite("sp_icon_gou");
        checkBox.graphic = checkmarkImage;
        //
        return checkBoxObj;
    }
}
