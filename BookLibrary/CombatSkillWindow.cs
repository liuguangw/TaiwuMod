﻿using Config;
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

    /// <summary>
    /// 每页最多显示多少条
    /// </summary>
    private readonly int maxItemsLimit = 20;
    /// <summary>
    /// 当前页的筛选结果
    /// </summary>
    private readonly List<CombatSkillItem> currentItems = new(20);
    /// <summary>
    /// 当前是第几页
    /// </summary>
    private int currentPage = 1;
    /// <summary>
    /// 总共有多少条记录
    /// </summary>
    private int itemTotalCount = 0;
    /// <summary>
    /// 总共有多少页
    /// </summary>
    private int itemTotalPage
    {
        get
        {
            var totalPage = itemTotalCount / maxItemsLimit;
            if ((itemTotalCount % maxItemsLimit) > 0)
            {
                totalPage++;
            }
            return totalPage;
        }
    }

    #region ui
    private GameObject? bookListContainer;
    private Button? prevButton;
    private Button? nextButton;
    private Text? pageText;
    #endregion

    public void InitUI(GameObject parent)
    {
        rootObject = UiTool.CreateRectObject("CombatSkillWindow");
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
        AddBookListContainer(rootObject);
        AddPageBtns(rootObject);
        ActiveSkillType(0);
        ActiveSectType(0);
        LoadPageItems();
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
            btnComponent.onClick.AddListener(() =>
            {
                ActiveSkillType(btnIndex);
                currentPage = 1;
                LoadPageItems();
            });
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
            btnComponent.onClick.AddListener(() =>
            {
                ActiveSectType(btnIndex);
                currentPage = 1;
                LoadPageItems();
            });
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

    /// <summary>
    /// 书籍列表区
    /// </summary>
    /// <param name="parent"></param>
    private void AddBookListContainer(GameObject parent)
    {
        bookListContainer = UiTool.CreateRectObject("#333333".HexStringToColor(), "BookListContainer");
        bookListContainer.transform.SetParent(parent.transform);
        var rect = bookListContainer.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            //底部和顶部留下固定距离
            var navHeight = 30;
            rect.offsetMin = new(0, navHeight + 5);
            rect.offsetMax = new(0, -2 * navHeight - 10);
        }
        var layout = bookListContainer.AddComponent<GridLayoutGroup>();
        layout.spacing = new(5.0f, 5.0f);
    }

    /// <summary>
    /// 添加翻页按钮
    /// </summary>
    /// <param name="parent"></param>
    private void AddPageBtns(GameObject parent)
    {
        var pageBtnsObject = UiTool.CreateRectObject("PageBtns");
        pageBtnsObject.transform.SetParent(parent.transform);
        var rect = pageBtnsObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的右下角
            rect.anchorMin = Vector2.right;
            rect.anchorMax = Vector2.right;
            //固定大小
            var (areaWidth, areaHeight) = (190, 30);
            rect.SetSize(new(areaWidth, areaHeight));
            rect.anchoredPosition = new(-areaWidth / 2, areaHeight / 2);
        }
        var layout = pageBtnsObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 5.0f;
        //添加按钮
        var prevBtnObject = UiTool.CreateButtonObject("#9b886d".HexStringToColor(), "上一页", "PrevPageBtn");
        prevBtnObject.transform.SetParent(pageBtnsObject.transform);
        prevButton = prevBtnObject.GetComponent<Button>();
        prevButton.onClick.AddListener(() =>
        {
            currentPage -= 1;
            LoadPageItems();
        });
        var pageTextObject = UiTool.CreateRectObject("PageText");
        var text = pageTextObject.AddComponent<Text>();
        UiTool.InitText(text);
        text.text = "1/10";
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        pageText = text;
        pageTextObject.transform.SetParent(pageBtnsObject.transform);
        var nextBtnObject = UiTool.CreateButtonObject("#9b886d".HexStringToColor(), "下一页", "NextPageBtn");
        nextBtnObject.transform.SetParent(pageBtnsObject.transform);
        nextButton = nextBtnObject.GetComponent<Button>();
        nextButton.onClick.AddListener(() =>
        {
            currentPage += 1;
            LoadPageItems();
        });
    }

    /// <summary>
    /// 筛选规则
    /// </summary>
    /// <param name="combatSkillItem"></param>
    /// <returns></returns>
    private bool FilterBook(CombatSkillItem combatSkillItem)
    {
        if (combatSkillItem.BookId < 0)
        {
            return false;
        }
        if ((skillType > 0) && (skillType - 1 != combatSkillItem.Type))
        {
            return false;
        }
        if ((sectType > 0) && (sectType - 1 != combatSkillItem.SectId))
        {
            return false;
        }
        return true;
    }

    private void LoadPageItems()
    {
        itemTotalCount = CombatSkill.Instance.Where(FilterBook).Count();
        if (currentPage > itemTotalPage)
        {
            currentPage = 1;
        }
        //
        var offset = (currentPage - 1) * maxItemsLimit;
        currentItems.Clear();
        foreach (var item in CombatSkill.Instance.Where(FilterBook).Skip(offset).Take(maxItemsLimit))
        {
            currentItems.Add(item);
        }
        UpdatePageUi();
    }

    /// <summary>
    /// 帅选完成后,更新界面ui
    /// </summary>
    private void UpdatePageUi()
    {
        //清理已经存在的书籍列表
        var existsItemCount = bookListContainer!.transform.childCount;
        for (var i = existsItemCount - 1; i >= 0; i--)
        {
            var tmpItem = bookListContainer.transform.GetChild(i).gameObject;
            UnityEngine.Object.DestroyImmediate(tmpItem);
        }
        //添加新的书籍
        for (int i = 0; i < currentItems.Count; i++)
        {
            var bookNodeObject = CreateBookNode(i, currentItems[i]);
            bookNodeObject.transform.SetParent(bookListContainer.transform);
        }
        //翻页按钮的状态更新
        if (prevButton != null)
        {
            prevButton.enabled = currentPage > 1;
        }
        if (nextButton != null)
        {
            nextButton.enabled = currentPage < itemTotalPage;
        }
        //页码文本更新
        if (pageText != null)
        {
            pageText.text = $"{currentPage}/{itemTotalPage}";
        }
    }

    private GameObject CreateBookNode(int index, CombatSkillItem combatSkillItem)
    {
        var bookObject = UiTool.CreateRectObject("#b3ae8c".HexStringToColor(), $"Book{index}");
        var textObject = UiTool.CreateRectObject("Text");
        textObject.transform.SetParent(bookObject.transform);
        var rect = textObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            //底部留出20的间距
            rect.offsetMin = new(0, 20);
            rect.offsetMax = Vector2.zero;
        }
        //
        var text = textObject.AddComponent<Text>();
        UiTool.InitText(text);
        text.text = combatSkillItem.Name;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.red;
        return bookObject;
    }
}
