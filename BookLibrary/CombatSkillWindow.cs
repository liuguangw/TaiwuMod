using Config;
using UnityEngine;
using UnityEngine.UI;

namespace Liuguang.mod.Taiwu.BookLibrary;

internal class CombatSkillWindow
{
    private GameObject? rootObject;
    private readonly List<GameObject> skillTypeBtnObjects = new();
    private readonly List<GameObject> sectTypeBtnObjects = new();
    private int skillType = -1;
    private int sectType = -1;
    public void InitUI(GameObject parent)
    {
        rootObject = UiTool.CreateRectObject("#333333".HexStringToColor(), "CombatSkillWindow");
        rootObject.SetActive(false);
        rootObject.transform.SetParent(parent.transform);
        var rect = rootObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的四条边
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            //与上边距离42
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = new(0, -42.0f);
        }
        AddSkillTypeNav(rootObject);
        AddSectTypeNav(rootObject);
        ActiveSkillType(0);
        ActiveSectType(0);
    }

    public void SetActive(bool active)
    {
        rootObject?.SetActive(active);
    }

    /// <summary>
    /// 功法类型筛选栏
    /// </summary>
    /// <param name="parent"></param>
    private void AddSkillTypeNav(GameObject parent)
    {
        var skillTypeNav = UiTool.CreateRectObject("SkillTypeNav");
        skillTypeNav.transform.SetParent(parent.transform);
        var rect = skillTypeNav.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的上边线
            rect.anchorMin = Vector2.up;//(0,1)
            rect.anchorMax = Vector2.one;//(1,1)
            //固定高度
            var navHeight = 30;
            rect.offsetMin = new(0, -navHeight);
            rect.offsetMax = Vector2.zero;
        }
        var layout = skillTypeNav.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 5.0f;
        List<string> skillTypeList = new(CombatSkillType.Instance.Count + 1);
        skillTypeList.Add("全部");
        foreach (var combatSkillTypeItem in CombatSkillType.Instance)
        {
            skillTypeList.Add(combatSkillTypeItem.Name);
        }
        for (var index = 0; index < skillTypeList.Count; index++)
        {
            var skillTypeBtnObject = UiTool.CreateButtonObject("#9b886d".HexStringToColor(), skillTypeList[index], $"SkillType-{index}");
            skillTypeBtnObject.transform.SetParent(skillTypeNav.transform);
            skillTypeBtnObjects.Add(skillTypeBtnObject);
            var btnComponent = skillTypeBtnObject.GetComponent<Button>();
            var btnIndex = index;
            btnComponent.onClick.AddListener(() => ActiveSkillType(btnIndex));
        }

    }

    private void ActiveSkillType(int bookTypeIndex)
    {
        if (bookTypeIndex == skillType)
        {
            return;
        }
        skillType = bookTypeIndex;
        for (var index = 0; index < skillTypeBtnObjects.Count; index++)
        {
            var bookTypeBtnObject = skillTypeBtnObjects[index];
            var image = bookTypeBtnObject.GetComponent<Image>();
            var imageColor = (skillType == index) ? "#ed991c" : "#9b886d";
            image.color = imageColor.HexStringToColor();
        }
    }

    /// <summary>
    /// 门派类型筛选栏
    /// </summary>
    /// <param name="parent"></param>
    private void AddSectTypeNav(GameObject parent)
    {
        var sectTypeNav = UiTool.CreateRectObject("SectTypeNav");
        sectTypeNav.transform.SetParent(parent.transform);
        var rect = sectTypeNav.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的上边线
            rect.anchorMin = Vector2.up;//(0,1)
            rect.anchorMax = new(0.75f, 1.0f);
            //固定高度
            var navHeight = 30;
            rect.offsetMin = new(0, -2 * navHeight - 5);
            rect.offsetMax = new(0, -navHeight - 5);
        }
        var layout = sectTypeNav.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 5.0f;
        List<string> sectTypeList = new() { "全", "无" };
        foreach (var organizationItem in Organization.Instance.Where(item => item.IsSect))
        {
            sectTypeList.Add(organizationItem.Name.Substring(0, 1));
        }
        for (var index = 0; index < sectTypeList.Count; index++)
        {
            var sectTypeBtnObject = UiTool.CreateButtonObject("#9b886d".HexStringToColor(), sectTypeList[index], $"SectType-{index}");
            sectTypeBtnObject.transform.SetParent(sectTypeNav.transform);
            sectTypeBtnObjects.Add(sectTypeBtnObject);
            var btnComponent = sectTypeBtnObject.GetComponent<Button>();
            var btnIndex = index;
            btnComponent.onClick.AddListener(() => ActiveSectType(btnIndex));
        }

    }

    private void ActiveSectType(int sectTypeIndex)
    {
        if (sectTypeIndex == sectType)
        {
            return;
        }
        sectType = sectTypeIndex;
        for (var index = 0; index < sectTypeBtnObjects.Count; index++)
        {
            var sectTypeBtnObject = sectTypeBtnObjects[index];
            var image = sectTypeBtnObject.GetComponent<Image>();
            var imageColor = (sectType == index) ? "#ed991c" : "#9b886d";
            image.color = imageColor.HexStringToColor();
        }
    }

}
