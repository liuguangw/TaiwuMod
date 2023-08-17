using Config;
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
    /// 是否已经开启批量操作
    /// </summary>
    private bool IsMutiActive = false;
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
    private GameObject? mutiSelectAllObject;
    #endregion

    public void InitUI(GameObject parent, Action<int, string> showTipFunc)
    {
        rootObject = UiTool.CreateRectObject("LifeSkillWindow");
        rootObject.SetActive(false);
        rootObject.transform.SetParent(parent.transform, false);
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
        AddMutiNav(rootObject);
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
        skillTypeNav.transform.SetParent(parent.transform, false);
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
            skillTypeBtnObject.transform.SetParent(skillTypeNav.transform, false);
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
        bookListArea.transform.SetParent(parent.transform, false);
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
        scrollViewObject.transform.SetParent(bookListArea.transform, false);
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
    /// 批量操作区
    /// </summary>
    /// <param name="parent"></param>
    private void AddMutiNav(GameObject parent)
    {
        var mutiBtnObject = UiTool.CreateToggleButton("批量操作", Color.white, "#ed991c".HexStringToColor(), "#9b886d".HexStringToColor(), "MutiActionButton");
        mutiBtnObject.transform.SetParent(parent.transform, false);
        var navHeight = 48;
        //固定大小
        var mutiBtnWidth = 150;
        {
            var rect = mutiBtnObject.GetComponent<RectTransform>();
            if (rect != null)
            {
                //锚点为parent的左下角
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.anchoredPosition = new(mutiBtnWidth / 2, navHeight / 2);
                rect.sizeDelta = new(mutiBtnWidth, navHeight);
            }
        }
        //
        var offsetX = mutiBtnWidth + 10;
        //全选
        var selectAllObj = GameUi.CreateCheckBox(parent, "SelectAllCheckBox");
        selectAllObj.SetActive(false);
        var checkBoxSize = 38;
        {
            var rect = selectAllObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                //锚点为parent的左下角
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.anchoredPosition = new(checkBoxSize / 2 + offsetX, navHeight / 2);
                rect.sizeDelta = new(checkBoxSize, checkBoxSize);
            }
        }
        mutiSelectAllObject = selectAllObj;
        //全选文本
        offsetX += (checkBoxSize + 10);
        var selectAllTextObj = UiTool.CreateRectObject("SelectAllText");
        selectAllTextObj.SetActive(false);
        selectAllTextObj.transform.SetParent(parent.transform, false);
        var textWidth = 70;
        {
            var rect = selectAllTextObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                //锚点为parent的左下角
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.anchoredPosition = new(textWidth / 2 + offsetX, navHeight / 2);
                rect.sizeDelta = new(textWidth, navHeight);
            }
        }
        var text = selectAllTextObj.AddComponent<Text>();
        UiTool.InitText(text);
        text.alignment = TextAnchor.MiddleLeft;
        text.text = "全选";
        //获取按钮
        offsetX += (textWidth + 20);
        var fetchBookBtnObject = UiTool.CreateButtonObject("#9b886d".HexStringToColor(), "获取", "FetchBookBtn");
        fetchBookBtnObject.SetActive(false);
        fetchBookBtnObject.transform.SetParent(parent.transform, false);
        var fetchBookBtnWidth = 90;
        {
            var rect = fetchBookBtnObject.GetComponent<RectTransform>();
            if (rect != null)
            {
                //锚点为parent的左下角
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.anchoredPosition = new(fetchBookBtnWidth / 2 + offsetX, navHeight / 2);
                rect.sizeDelta = new(fetchBookBtnWidth, navHeight);
            }
        }
        var buttonObject = fetchBookBtnObject.GetComponent<Button>();
        if (buttonObject != null)
        {
            buttonObject.onClick.AddListener(ProcessMutiFetch);
        }
        //开/关批量模式的事件
        var mutiBtnToggle = mutiBtnObject.GetComponent<Toggle>();
        mutiBtnToggle.onValueChanged.AddListener(isOn =>
        {
            selectAllObj.SetActive(isOn);
            selectAllTextObj.SetActive(isOn);
            fetchBookBtnObject.SetActive(isOn);
            if (!isOn)
            {
                //退出批量操作时,取消全选框的勾选状态
                var selectAllToggle = selectAllObj.GetComponent<CToggle>();
                selectAllToggle.isOn = false;
            }
            //显示/隐藏全部书籍的勾选框
            var existsItemCount = bookListContainer!.transform.childCount;
            for (var i = 0; i < existsItemCount; i++)
            {
                var tmpItem = bookListContainer.transform.GetChild(i).gameObject;
                var checkBoxObject = tmpItem.transform.Find("MouseArea/CheckBox").gameObject;
                checkBoxObject.SetActive(isOn);
                if (!isOn)
                {
                    //退出批量操作时,取消所有勾选状态
                    var toggle = checkBoxObject.GetComponent<CToggle>();
                    toggle.isOn = false;
                }
            }
            IsMutiActive = isOn;
        });
        //全选/取消全选的处理
        var selectAllToggle = selectAllObj.GetComponent<CToggle>();
        selectAllToggle.onValueChanged.AddListener(isOn =>
        {
            //显示/隐藏全部书籍的勾选框
            var existsItemCount = bookListContainer!.transform.childCount;
            for (var i = 0; i < existsItemCount; i++)
            {
                var tmpItem = bookListContainer.transform.GetChild(i).gameObject;
                var checkBoxObject = tmpItem.transform.Find("MouseArea/CheckBox").gameObject;
                var toggle = checkBoxObject.GetComponent<CToggle>();
                toggle.isOn = isOn;
            }
        });
    }

    /// <summary>
    /// 添加翻页按钮
    /// </summary>
    /// <param name="parent"></param>
    private void AddPageBtns(GameObject parent)
    {
        var pageBtnsObject = UiTool.CreateRectObject("PageBtns");
        pageBtnsObject.transform.SetParent(parent.transform, false);
        var rect = pageBtnsObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的右下角
            rect.anchorMin = Vector2.right;
            rect.anchorMax = Vector2.right;
            //固定大小
            var (areaWidth, areaHeight) = (354, 48);
            rect.SetSize(new(areaWidth, areaHeight));
            rect.anchoredPosition = new(-areaWidth / 2, areaHeight / 2);
        }
        var layout = pageBtnsObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8.0f;
        //添加按钮
        var prevBtnObject = UiTool.CreateButtonObject("#9b886d".HexStringToColor(), "上一页", "PrevPageBtn");
        prevBtnObject.transform.SetParent(pageBtnsObject.transform, false);
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
        pageTextObject.transform.SetParent(pageBtnsObject.transform, false);
        var nextBtnObject = UiTool.CreateButtonObject("#9b886d".HexStringToColor(), "下一页", "NextPageBtn");
        nextBtnObject.transform.SetParent(pageBtnsObject.transform, false);
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
        //取消全选框的勾选
        if (mutiSelectAllObject != null)
        {
            var selectAllToggle = mutiSelectAllObject.GetComponent<CToggle>();
            selectAllToggle.isOn = false;
        }
        //添加新的书籍
        for (int i = 0; i < CurrentItems.Count; i++)
        {
            var bookNodeObject = CreateBookNode(i, CurrentItems[i]);
            bookNodeObject.transform.SetParent(bookListContainer.transform, false);
            //显示勾选框
            if (IsMutiActive)
            {
                var checkBoxObject = bookNodeObject.transform.Find("MouseArea/CheckBox").gameObject;
                checkBoxObject.SetActive(true);
            }
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
        mouseAreaObject.transform.SetParent(bookObject.transform, false);
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
            GameUi.ShowSkillBookTips(lifeSkillItem.SkillBookId);
        };
        mouseArea.ExitAction = GameUi.HideSkillBookTips;
        mouseArea.ClickAction = () =>
        {
            if (IsMutiActive)
            {
                //点击区域时,切换勾选状态
                var checkBoxObject = mouseArea.transform.Find("CheckBox");
                if (checkBoxObject != null)
                {
                    var toggle = checkBoxObject.GetComponent<CToggle>();
                    if (toggle != null)
                    {
                        toggle.isOn = !toggle.isOn;
                    }
                }
            }
        };
        //图标区
        var iconObject = UiTool.CreateRectObject("Icon");
        iconObject.transform.SetParent(mouseAreaObject.transform, false);
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
        //选择框
        var checkBoxObj = GameUi.CreateCheckBox(mouseAreaObject);
        checkBoxObj.SetActive(false);
        var checkBoxSize = 38;
        var checkBoxRect = checkBoxObj.GetComponent<RectTransform>();
        if (checkBoxRect != null)
        {
            //左上角
            checkBoxRect.anchorMin = Vector2.up;
            checkBoxRect.anchorMax = Vector2.up;
            checkBoxRect.anchoredPosition = new(checkBoxSize / 2 + 5, -checkBoxSize / 2 - 5);
            checkBoxRect.sizeDelta = new(checkBoxSize, checkBoxSize);
        }
        //文本区
        var textObject = UiTool.CreateRectObject("Text");
        textObject.transform.SetParent(mouseAreaObject.transform, false);
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
        createBtnObject.transform.SetParent(bookObject.transform, false);
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

    /// <summary>
    /// 处理批量模式获取书籍
    /// </summary>
    private void ProcessMutiFetch()
    {
        var playerId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
        if (playerId <= 0)
        {
            ShowTipFunc?.Invoke(1, "进入游戏之后, 才能获取书籍");
            return;
        }
        //本页书籍数量
        var existsItemCount = bookListContainer!.transform.childCount;
        if (existsItemCount <= 0)
        {
            ShowTipFunc?.Invoke(1, "本页没有可选项");
            return;
        }
        //list
        var selectedItemList = new List<LifeSkillItem>();
        for (var i = 0; i < existsItemCount; i++)
        {
            var tmpItem = bookListContainer.transform.GetChild(i).gameObject;
            var checkBoxObject = tmpItem.transform.Find("MouseArea/CheckBox").gameObject;
            var toggle = checkBoxObject.GetComponent<CToggle>();
            if (toggle.isOn)
            {
                selectedItemList.Add(CurrentItems[i]);
            }
        }
        if (selectedItemList.Count <= 0)
        {
            ShowTipFunc?.Invoke(1, "没有勾选所需的书籍");
            return;
        }
        if (selectedItemList.Count == 1)
        {
            BookDialog.SetSkillItem(selectedItemList[0]);
        }
        else
        {
            BookDialog.SetSkillItemList(selectedItemList);
        }
        BookDialog.SetActive(true);
    }
}
