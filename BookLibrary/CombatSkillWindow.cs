using Config;
using UnityEngine;
using UnityEngine.UI;

namespace Liuguang.mod.Taiwu.BookLibrary;

internal class CombatSkillWindow
{
    private GameObject? rootObject;
    private CombatSkillBookDialog BookDialog = new();
    private readonly List<GameObject> SkillTypeBtnObjects = new();
    private readonly List<GameObject> SectTypeBtnObjects = new();
    private int SkillType = -1;
    private int SectType = -1;
    /// <summary>
    /// 当前页的筛选结果
    /// </summary>
    private readonly List<CombatSkillItem> CurrentItems = new(18);
    /// <summary>
    /// 分页
    /// </summary>
    private readonly Pagination ItemPagination = new(18);

    private Action<int, string>? ShowTipFunc;

    #region ui
    private GameObject? bookListContainer;
    private Button? prevButton;
    private Button? nextButton;
    private Text? pageText;
    #endregion

    public void InitUI(GameObject parent, Action<int, string> showTipFunc)
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
        ShowTipFunc = showTipFunc;
        AddSkillTypeNav(rootObject);
        AddSectTypeNav(rootObject);
        AddBookListContainer(rootObject);
        AddPageBtns(rootObject);
        BookDialog.InitUI(rootObject, showTipFunc);
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
            SkillTypeBtnObjects.Add(skillTypeBtnObject);
            var btnComponent = skillTypeBtnObject.GetComponent<Button>();
            var btnIndex = index;
            btnComponent.onClick.AddListener(() =>
            {
                ActiveSkillType(btnIndex);
                ItemPagination.CurrentPage = 1;
                LoadPageItems();
            });
        }

    }

    private void ActiveSkillType(int bookTypeIndex)
    {
        if (bookTypeIndex == SkillType)
        {
            return;
        }
        SkillType = bookTypeIndex;
        for (var index = 0; index < SkillTypeBtnObjects.Count; index++)
        {
            var bookTypeBtnObject = SkillTypeBtnObjects[index];
            var image = bookTypeBtnObject.GetComponent<Image>();
            var imageColor = (SkillType == index) ? "#ed991c" : "#9b886d";
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
            SectTypeBtnObjects.Add(sectTypeBtnObject);
            var btnComponent = sectTypeBtnObject.GetComponent<Button>();
            var btnIndex = index;
            btnComponent.onClick.AddListener(() =>
            {
                ActiveSectType(btnIndex);
                ItemPagination.CurrentPage = 1;
                LoadPageItems();
            });
        }

    }

    private void ActiveSectType(int sectTypeIndex)
    {
        if (sectTypeIndex == SectType)
        {
            return;
        }
        SectType = sectTypeIndex;
        for (var index = 0; index < SectTypeBtnObjects.Count; index++)
        {
            var sectTypeBtnObject = SectTypeBtnObjects[index];
            var image = sectTypeBtnObject.GetComponent<Image>();
            var imageColor = (SectType == index) ? "#ed991c" : "#9b886d";
            image.color = imageColor.HexStringToColor();
        }
    }

    /// <summary>
    /// 书籍列表区
    /// </summary>
    /// <param name="parent"></param>
    private void AddBookListContainer(GameObject parent)
    {
        var bookListArea = UiTool.CreateRectObject("BookListArea");
        bookListArea.transform.SetParent(parent.transform);
        var rect = bookListArea.GetComponent<RectTransform>();
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
        //
        var scrollViewObject = UiTool.CreateVerticalScrollView("BookListScrollView");
        scrollViewObject.transform.SetParent(bookListArea.transform);
        var scrollViewRect = scrollViewObject.GetComponent<RectTransform>();
        if (scrollViewRect != null)
        {
            //锚点为parent
            scrollViewRect.anchorMin = Vector2.zero;
            scrollViewRect.anchorMax = Vector2.one;
            scrollViewRect.offsetMin = Vector2.zero;
            scrollViewRect.offsetMax = Vector2.zero;
        }
        var scrollRect = scrollViewObject.GetComponent<ScrollRect>();
        bookListContainer = scrollRect.content.gameObject;
        var layout = bookListContainer.AddComponent<GridLayoutGroup>();
        layout.spacing = new(5, 5);
        layout.cellSize = new(120, 120 + 20 + 10 + 30 + 10);
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 6;
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
            ItemPagination.GotoPrev();
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
            ItemPagination.GotoNext();
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
        if ((SkillType > 0) && (SkillType - 1 != combatSkillItem.Type))
        {
            return false;
        }
        if ((SectType > 0) && (SectType - 1 != combatSkillItem.SectId))
        {
            return false;
        }
        return true;
    }

    private void LoadPageItems()
    {
        ItemPagination.ItemTotalCount = CombatSkill.Instance.Where(FilterBook).Count();
        if (ItemPagination.CurrentPage > ItemPagination.TotalPage)
        {
            ItemPagination.CurrentPage = 1;
        }
        //
        var offset = (ItemPagination.CurrentPage - 1) * ItemPagination.MaxItemsLimit;
        CurrentItems.Clear();
        foreach (var item in CombatSkill.Instance.Where(FilterBook).Skip(offset).Take(ItemPagination.MaxItemsLimit))
        {
            CurrentItems.Add(item);
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
        for (int i = 0; i < CurrentItems.Count; i++)
        {
            var bookNodeObject = CreateBookNode(i, CurrentItems[i]);
            bookNodeObject.transform.SetParent(bookListContainer.transform);
        }
        //翻页按钮的状态更新
        if (prevButton != null)
        {
            prevButton.enabled = ItemPagination.HasPrev;
        }
        if (nextButton != null)
        {
            nextButton.enabled = ItemPagination.HasNext;
        }
        //页码文本更新
        if (pageText != null)
        {
            pageText.text = $"{ItemPagination.CurrentPage}/{ItemPagination.TotalPage}";
        }
    }

    private GameObject CreateBookNode(int index, CombatSkillItem combatSkillItem)
    {
        var bookObject = UiTool.CreateRectObject("#313331".HexStringToColor(), $"Book{index}");
        //图标区
        var iconObject = UiTool.CreateRectObject("Icon");
        iconObject.transform.SetParent(bookObject.transform);
        var iconRect = iconObject.GetComponent<RectTransform>();
        if (iconRect != null)
        {
            //锚点为parent的左上角
            iconRect.anchorMin = Vector2.up;
            iconRect.anchorMax = Vector2.up;
            //
            iconRect.anchoredPosition = new(60, -60);
            iconRect.sizeDelta = new(100, 100);
        }
        var iconImg = iconObject.AddComponent<CImage>();
        iconImg.SetSprite(combatSkillItem.Icon);
        iconImg.SetColor(Colors.Instance.FiveElementsColors[combatSkillItem.FiveElements]);
        //文本区
        var textObject = UiTool.CreateRectObject("Text");
        textObject.transform.SetParent(bookObject.transform);
        var textRect = textObject.GetComponent<RectTransform>();
        if (textRect != null)
        {
            //锚点为parent的左上角
            textRect.anchorMin = Vector2.up;
            textRect.anchorMax = Vector2.up;
            //
            textRect.anchoredPosition = new(60, -120 - 10);
            textRect.sizeDelta = new(120, 20);
        }
        //
        var text = textObject.AddComponent<Text>();
        UiTool.InitText(text);
        text.text = combatSkillItem.Name;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Colors.Instance.GradeColors[combatSkillItem.Grade];
        //按钮区
        var createBtnObject = UiTool.CreateButtonObject("#9b886d".HexStringToColor(), "获取", "CreateBookBtn");
        createBtnObject.transform.SetParent(bookObject.transform);
        var btnRect = createBtnObject.GetComponent<RectTransform>();
        if (btnRect != null)
        {
            //锚点为parent的下边
            btnRect.anchorMin = Vector2.zero;
            btnRect.anchorMax = Vector2.right;
            //
            btnRect.offsetMin = new(10, 10);
            btnRect.offsetMax = new(-10, 40);
        }
        var createBtn = createBtnObject.GetComponent<Button>();
        createBtn.onClick.AddListener(() =>
        {
            var playerId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
            if (playerId > 0)
            {
                BookDialog.SetSkillItem(combatSkillItem);
                BookDialog.SetActive(true);
            }
            else
            {
                ShowTipFunc?.Invoke(1, "进入游戏之后, 才能获取书籍");
            }
        });
        return bookObject;
    }
}
