using BookLibrary;
using Config;
using FrameWork;
using GameData.Domains.Item.Display;
using UnityEngine;
using UnityEngine.UI;

namespace Liuguang.mod.Taiwu.BookLibrary;

internal class LifeSkillWindow
{
    private GameObject? rootObject;
    private LifeSkillBookDialog BookDialog = new();
    private readonly List<GameObject> SkillTypeBtnObjects = new();
    private int SkillType = -1;
    /// <summary>
    /// 当前页的筛选结果
    /// </summary>
    private readonly List<LifeSkillItem> CurrentItems = new(18);
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
        rootObject = UiTool.CreateRectObject("LifeSkillWindow");
        rootObject.SetActive(false);
        rootObject.transform.SetParent(parent.transform);
        var rect = rootObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的四条边
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            //与上边距离
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = new(0, -67.2f);
        }
        ShowTipFunc = showTipFunc;
        AddSkillTypeNav(rootObject);
        AddBookListContainer(rootObject);
        AddPageBtns(rootObject);
        BookDialog.InitUI(rootObject, showTipFunc);
        ActiveSkillType(0);
        LoadPageItems();
    }

    public void SetActive(bool active)
    {
        rootObject?.SetActive(active);
    }

    /// <summary>
    /// 技艺类型筛选栏
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
            var navHeight = 48;
            rect.offsetMin = new(0, -navHeight);
            rect.offsetMax = Vector2.zero;
        }
        var layout = skillTypeNav.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8.0f;
        List<string> skillTypeList = new(LifeSkillType.Instance.Count + 1);
        skillTypeList.Add("全部");
        foreach (var lifeSkillTypeItem in LifeSkillType.Instance)
        {
            skillTypeList.Add(lifeSkillTypeItem.Name);
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
            var navHeight = 48;
            rect.offsetMin = new(0, navHeight + 8);
            rect.offsetMax = new(0, -navHeight - 8);
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
        layout.spacing = new(8, 8);
        layout.cellSize = new(192, 192 + 32 + 16 + 48 + 16);
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
            var (areaWidth, areaHeight) = (304, 48);
            rect.SetSize(new(areaWidth, areaHeight));
            rect.anchoredPosition = new(-areaWidth / 2, areaHeight / 2);
        }
        var layout = pageBtnsObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8.0f;
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
    /// <param name="lifeSkillItem"></param>
    /// <returns></returns>
    private bool FilterBook(LifeSkillItem lifeSkillItem)
    {
        if (lifeSkillItem.SkillBookId < 0)
        {
            return false;
        }
        if ((SkillType > 0) && (SkillType - 1 != lifeSkillItem.Type))
        {
            return false;
        }
        return true;
    }

    private void LoadPageItems()
    {
        ItemPagination.ItemTotalCount = LifeSkill.Instance.Where(FilterBook).Count();
        if (ItemPagination.CurrentPage > ItemPagination.TotalPage)
        {
            ItemPagination.CurrentPage = 1;
        }
        //
        var offset = (ItemPagination.CurrentPage - 1) * ItemPagination.MaxItemsLimit;
        CurrentItems.Clear();
        foreach (var item in LifeSkill.Instance.Where(FilterBook).Skip(offset).Take(ItemPagination.MaxItemsLimit))
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
            bookNodeObject.transform.localScale = Vector3.one;
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

    private GameObject CreateBookNode(int index, LifeSkillItem lifeSkillItem)
    {
        var bookObject = UiTool.CreateRectObject("#313331".HexStringToColor(), $"Book{index}");
        //鼠标悬浮区
        var mouseAreaObject = UiTool.CreateRectObject("MouseArea");
        mouseAreaObject.transform.SetParent(bookObject.transform);
        var rect = mouseAreaObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的左上角
            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;
            //
            var (width, height) = (192, 192 + 32);
            rect.anchoredPosition = new(width / 2, -height / 2);
            rect.sizeDelta = new(width, height);
        }
        var mouseArea = mouseAreaObject.AddComponent<MouseArea>();
        var mouseTipManager = SingletonObject.getInstance<MouseTipManager>();
        mouseArea.EnterAction = () =>
        {
            var skillBookConfig = SkillBook.Instance[lifeSkillItem.SkillBookId];
            var itemData = new ItemDisplayData()
            {
                Key = new(skillBookConfig.ItemType, 0, lifeSkillItem.SkillBookId, 0),
                Amount = 1,
                Durability = skillBookConfig.MaxDurability,
                MaxDurability = skillBookConfig.MaxDurability,
                Weight = skillBookConfig.BaseWeight,
                Value = skillBookConfig.BaseValue,
            };
            var argsBox = new ArgumentBox();
            argsBox.Set("ItemData", itemData);
            argsBox.Set("ShowPageInfo", false);
            argsBox.Set("templateDataOnly", false);
            mouseTipManager.ShowTips(TipType.SkillBook, argsBox);
        };
        mouseArea.ExitAction = () =>
        {
            mouseTipManager.HideTips(TipType.SkillBook);
        };
        //图标区
        var iconObject = UiTool.CreateRectObject("Icon");
        iconObject.transform.SetParent(mouseAreaObject.transform);
        var iconRect = iconObject.GetComponent<RectTransform>();
        if (iconRect != null)
        {
            //锚点为parent的左上角
            iconRect.anchorMin = Vector2.up;
            iconRect.anchorMax = Vector2.up;
            //
            iconRect.anchoredPosition = new(96, -96);
            iconRect.sizeDelta = new(160, 160);
        }
        var iconImg = iconObject.AddComponent<CImage>();
        var skillBookItem = SkillBook.Instance[lifeSkillItem.SkillBookId];
        iconImg.SetSprite(skillBookItem.Icon);
        //文本区
        var textObject = UiTool.CreateRectObject("Text");
        textObject.transform.SetParent(mouseAreaObject.transform);
        var textRect = textObject.GetComponent<RectTransform>();
        if (textRect != null)
        {
            //锚点为parent的左上角
            textRect.anchorMin = Vector2.up;
            textRect.anchorMax = Vector2.up;
            //
            textRect.anchoredPosition = new(96, -192 - 16);
            textRect.sizeDelta = new(192, 32);
        }
        //
        var text = textObject.AddComponent<Text>();
        UiTool.InitText(text);
        text.text = lifeSkillItem.Name;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Colors.Instance.GradeColors[lifeSkillItem.Grade];
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
            btnRect.offsetMin = new(16, 16);
            btnRect.offsetMax = new(-16, 64);
        }
        var createBtn = createBtnObject.GetComponent<Button>();
        createBtn.onClick.AddListener(() =>
        {
            var playerId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
            if (playerId > 0)
            {
                BookDialog.SetSkillItem(lifeSkillItem);
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
