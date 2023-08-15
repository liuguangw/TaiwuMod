using Config;
using GameData.Domains.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Liuguang.mod.Taiwu.BookLibrary;

/// <summary>
/// 发放功法书的询问界面
/// </summary>
internal class CombatSkillBookDialog
{
    /// <summary>
    /// 要发放的书籍
    /// </summary>
    private CombatSkillItem? SkillItem;
    /// <summary>
    /// 总纲类型 [0 - 4]
    /// </summary>
    private sbyte OutlinePageType = 2;
    /// <summary>
    /// 普通页的正逆(0正 1逆)
    /// </summary>
    private sbyte[] NormalPageTypes = { 0, 0, 0, 0, 0 };
    /// <summary>
    /// 数量
    /// </summary>
    private int BookAmount = 1;
    private string BookTitle
    {
        get
        {
            return SkillItem?.Name ?? "未初始化书籍";
        }
    }

    private Color BookColor
    {
        get
        {
            if (SkillItem == null)
            {
                return Color.white;
            }
            return Colors.Instance.GradeColors[SkillItem.Grade];
        }
    }
    private GameObject? rootObject;
    private Text? TitleText;
    private readonly List<GameObject> OutlinePageObjects = new(5);
    private readonly List<GameObject> DirectPageObjects = new(5);
    private readonly List<GameObject> ReversePageObjects = new(5);

    private Color NotActiveColor = "#515550".HexStringToColor();
    private Color ActiveColor = "#8c4882".HexStringToColor();

    private Action<int, string>? ShowTipFunc;

    public void InitUI(GameObject parent, Action<int, string> showTipFunc)
    {
        //遮罩层
        rootObject = UiTool.CreateRectObject(new(0, 0, 0, 0.7f), "CombatSkillBookDialogMask");
        rootObject.SetActive(false);
        rootObject.transform.SetParent(parent.transform);
        var rect = rootObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            //
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        ShowTipFunc = showTipFunc;
        //
        var dialogObject = UiTool.CreateRectObject("#4c4c4c".HexStringToColor(), "CombatSkillBookDialog");
        dialogObject.transform.SetParent(rootObject.transform);
        var dialogRect = dialogObject.GetComponent<RectTransform>();
        if (dialogRect != null)
        {
            //锚点为parent(50%width,60%height)
            dialogRect.anchorMin = new(0.25f, 0.2f);
            dialogRect.anchorMax = new(0.75f, 0.8f);
            //
            dialogRect.offsetMin = Vector2.zero;
            dialogRect.offsetMax = Vector2.zero;
        }
        //
        AddMainTitle(dialogObject);
        AddOutlinePages(dialogObject);
        ActiveOutlinePageType(OutlinePageType);
        AddDirectPages(dialogObject);
        AddReversePages(dialogObject);
        for (int i = 0; i < NormalPageTypes.Length; i++)
        {
            ActiveNormalPageType(i, NormalPageTypes[i]);
        }
        AddAmountInput(dialogObject);
        AddConfirmBtns(dialogObject);
        AddCloseButton(dialogObject);
    }

    public void SetActive(bool active)
    {
        rootObject?.SetActive(active);
    }

    private void AddMainTitle(GameObject parent)
    {

        var titlePanel = UiTool.CreateRectObject("#302a28".HexStringToColor(), "TitlePanel");
        titlePanel.transform.SetParent(parent.transform);
        var rect = titlePanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的上边线
            rect.anchorMin = Vector2.up;//(0,1)
            rect.anchorMax = Vector2.one;//(1,1)
            //高度固定为30
            rect.offsetMin = new(0, -48);
            rect.offsetMax = new(0, 0);
        }
        var textObject = UiTool.CreateRectObject("Text");
        textObject.transform.SetParent(titlePanel.transform);
        var textRect = textObject.GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
        textObject.transform.localPosition = Vector3.zero;
        var textComponent = textObject.AddComponent<Text>();
        UiTool.InitText(textComponent);
        textComponent.text = BookTitle;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = BookColor;
        TitleText = textComponent;
    }

    public void SetSkillItem(CombatSkillItem item)
    {
        SkillItem = item;
        if (TitleText != null)
        {
            TitleText.text = BookTitle;
            TitleText.color = BookColor;
        }
    }

    /// <summary>
    /// 添加一行篇章选择
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="rowIndex">第几行,从0开始</param>
    /// <param name="rowObjectName">对象名称</param>
    /// <param name="pageBtnObjects">篇章按钮对象集合</param>
    /// <param name="labelText">label文本</param>
    /// <param name="langKey">翻译KEY前缀</param>
    /// <param name="pageBtnTextColor">按钮的文字颜色</param>
    /// <param name="clickAction">点击对应按钮的处理</param>
    private void AddPagesRow(GameObject parent, int rowIndex, string rowObjectName, List<GameObject> pageBtnObjects,
        string labelText, string langKey, Color pageBtnTextColor, Action<sbyte> clickAction)
    {
        var pagePanel = UiTool.CreateRectObject(rowObjectName);
        pagePanel.transform.SetParent(parent.transform);
        var offsetTop = (48 + 24) * (rowIndex + 1);//距离上方的距离
        var offsetLength = 16;
        var panelHeight = 48;
        var rect = pagePanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的上边线
            rect.anchorMin = Vector2.up;//(0,1)
            rect.anchorMax = Vector2.one;//(1,1)
            rect.offsetMin = new(offsetLength, -offsetTop - panelHeight);
            rect.offsetMax = new(-offsetLength, -offsetTop);
        }
        var textObject = UiTool.CreateRectObject("Text");
        textObject.transform.SetParent(pagePanel.transform);
        var (textWidth, textHeight) = (panelHeight * 3, panelHeight);
        var textRect = textObject.GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.anchorMin = Vector2.up;
            textRect.anchorMax = Vector2.up;
            textRect.SetSize(new(textWidth, textHeight));
            textRect.anchoredPosition = new(textWidth / 2, -textHeight / 2);
        }
        var textComponent = textObject.AddComponent<Text>();
        UiTool.InitText(textComponent);
        textComponent.text = labelText;
        textComponent.alignment = TextAnchor.MiddleRight;
        //
        var pageContainerObject = UiTool.CreateRectObject("PageContainer");
        pageContainerObject.transform.SetParent(pagePanel.transform);
        var containerRect = pageContainerObject.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = new(textWidth + 16, 0);
            containerRect.offsetMax = Vector2.zero;
        }
        var layout = pageContainerObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8.0f;
        for (sbyte index = 0; index < 5; index++)
        {
            var pageBtnObject = UiTool.CreateButtonObject(NotActiveColor, LocalStringManager.Get($"{langKey}_{index}"), $"PageBtn-{index}");
            pageBtnObject.transform.SetParent(pageContainerObject.transform);
            pageBtnObjects.Add(pageBtnObject);
            var text = pageBtnObject.GetComponentInChildren<Text>();
            text.color = pageBtnTextColor;
            var btnComponent = pageBtnObject.GetComponent<Button>();
            var btnIndex = index;
            btnComponent.onClick.AddListener(() => clickAction.Invoke(btnIndex));
        }
    }

    /// <summary>
    /// 总纲选择行
    /// </summary>
    /// <param name="parent"></param>
    private void AddOutlinePages(GameObject parent)
    {
        var langKey = "LK_CombatSkill_First_Page_Type";
        var pageBtnTextColor = Color.white;
        AddPagesRow(parent, 0, "OutlinePagesPanel", OutlinePageObjects, "总纲", langKey, pageBtnTextColor, ActiveOutlinePageType);
    }

    private void ActiveOutlinePageType(sbyte btnIndex)
    {
        OutlinePageType = btnIndex;
        for (var index = 0; index < OutlinePageObjects.Count; index++)
        {
            var pageBtnObject = OutlinePageObjects[index];
            var image = pageBtnObject.GetComponent<Image>();
            var imageColor = (OutlinePageType == index) ? ActiveColor : NotActiveColor;
            image.color = imageColor;
        }
    }
    /// <summary>
    /// 正练选择行
    /// </summary>
    /// <param name="parent"></param>
    private void AddDirectPages(GameObject parent)
    {
        var langKey = "LK_CombatSkill_Direct_Page";
        var pageBtnTextColor = "#00dcdc".HexStringToColor();
        AddPagesRow(parent, 1, "DirectPagesPanel", DirectPageObjects, "正练篇", langKey, pageBtnTextColor, btnIndex => ActiveNormalPageType(btnIndex, 0));
    }

    private void AddReversePages(GameObject parent)
    {
        var langKey = "LK_CombatSkill_Reverse_Page";
        var pageBtnTextColor = "#d68a00".HexStringToColor();
        AddPagesRow(parent, 2, "ReversePagesPanel", ReversePageObjects, "逆练篇", langKey, pageBtnTextColor, btnIndex => ActiveNormalPageType(btnIndex, 1));
    }

    private void ActiveNormalPageType(int btnIndex, sbyte pageType)
    {
        NormalPageTypes[btnIndex] = pageType;
        var (activeBtnObject, notActiveBtnObject) = (DirectPageObjects[btnIndex], ReversePageObjects[btnIndex]);
        if (pageType != 0)
        {
            (activeBtnObject, notActiveBtnObject) = (notActiveBtnObject, activeBtnObject);
        }
        {
            var image = activeBtnObject.GetComponent<Image>();
            image.color = ActiveColor;
        }
        {
            var image = notActiveBtnObject.GetComponent<Image>();
            image.color = NotActiveColor;
        }
    }

    private void AddAmountInput(GameObject parent)
    {
        var pagePanel = UiTool.CreateRectObject("AmountInputPanel");
        pagePanel.transform.SetParent(parent.transform);
        var offsetTop = (48 + 24) * 4;//距离上方的距离
        var offsetLength = 16;
        var panelHeight = 48;
        var rect = pagePanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的上边线
            rect.anchorMin = Vector2.up;//(0,1)
            rect.anchorMax = Vector2.one;//(1,1)
            rect.offsetMin = new(offsetLength, -offsetTop - panelHeight);
            rect.offsetMax = new(-offsetLength, -offsetTop);
        }
        var textObject = UiTool.CreateRectObject("Text");
        textObject.transform.SetParent(pagePanel.transform);
        var (textWidth, textHeight) = (panelHeight * 3, panelHeight);
        var textRect = textObject.GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.anchorMin = Vector2.up;
            textRect.anchorMax = Vector2.up;
            textRect.SetSize(new(textWidth, textHeight));
            textRect.anchoredPosition = new(textWidth / 2, -textHeight / 2);
        }
        var textComponent = textObject.AddComponent<Text>();
        UiTool.InitText(textComponent);
        textComponent.text = "数量";
        textComponent.alignment = TextAnchor.MiddleRight;
        //
        var pageContainerObject = UiTool.CreateRectObject("PageContainer");
        pageContainerObject.transform.SetParent(pagePanel.transform);
        var containerRect = pageContainerObject.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = new(textWidth + 16, 0);
            containerRect.offsetMax = Vector2.zero;
        }
        var inputFieldObject = UiTool.CreateInputField(Color.white, "输入需要的数量", "InputField");
        inputFieldObject.transform.SetParent(pageContainerObject.transform);
        var inputFieldRect = inputFieldObject.GetComponent<RectTransform>();
        if (inputFieldRect != null)
        {
            inputFieldRect.anchorMin = Vector2.zero;
            inputFieldRect.anchorMax = Vector2.one;
            inputFieldRect.offsetMin = Vector2.zero;
            inputFieldRect.offsetMax = Vector2.zero;
        }
        var inputField = inputFieldObject.GetComponent<InputField>();
        if (inputField != null)
        {
            inputField.text = BookAmount.ToString();
            inputField.contentType = InputField.ContentType.IntegerNumber;
            inputField.onValueChanged.AddListener(s =>
            {
                //Debug.Log($"s = {s}");
                try
                {

                    var amount = Convert.ToInt32(s, 10);
                    BookAmount = amount;
                }
                catch (Exception)
                {

                }
            });
        }
    }

    private void AddConfirmBtns(GameObject parent)
    {
        var btnPanel = UiTool.CreateRectObject("BtnPanel");
        btnPanel.transform.SetParent(parent.transform);
        var rect = btnPanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为右下角
            rect.anchorMin = Vector2.right;//(1,0)
            rect.anchorMax = Vector2.right;//(1,0)
            var (width, height) = (272, 64);
            rect.SetSize(new(width, height));
            rect.anchoredPosition = new(-width / 2 - 16, height / 2 + 16);
        }
        var layout = btnPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 16;
        var cancelBtnObject = UiTool.CreateButtonObject("#9b886d".HexStringToColor(), "取消", "CancelBtn");
        cancelBtnObject.transform.SetParent(btnPanel.transform);
        var confirmBtnObject = UiTool.CreateButtonObject("#ed991c".HexStringToColor(), "确定", "ConfirmBtn");
        confirmBtnObject.transform.SetParent(btnPanel.transform);
        var cancelBtn = cancelBtnObject.GetComponent<Button>();
        if (cancelBtn != null)
        {
            cancelBtn.onClick.AddListener(() => SetActive(false));
        }
        var confirmBtn = confirmBtnObject.GetComponent<Button>();
        if (confirmBtn != null)
        {
            confirmBtn.onClick.AddListener(SendBook);
        }

    }

    private void SendBook()
    {
        var playerId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
        if (playerId <= 0)
        {
            ShowTipFunc?.Invoke(1, "进入游戏之后, 才能获取书籍");
            return;
        }
        if (BookAmount <= 0)
        {
            ShowTipFunc?.Invoke(1, "数量不正确");
            return;
        }
        //GMFunc.GetItem(playerId, 1, ItemType.SkillBook, combatSkillItem.BookId, null);
        byte pageTypes = 0;
        //总纲
        //[0 - 4]
        pageTypes = SkillBookStateHelper.SetOutlinePageType(pageTypes, OutlinePageType);
        //剩余页面
        for (byte pageId = 1; pageId <= 5; pageId++)
        {
            //direction: 0正 or 1逆
            pageTypes = SkillBookStateHelper.SetNormalPageType(pageTypes, pageId, NormalPageTypes[pageId - 1]);
        }
        BookApi.GetBook(playerId, SkillItem!.BookId, BookAmount, pageTypes);
        //关闭当前弹窗
        SetActive(false);
        ShowTipFunc?.Invoke(0, $"获得了{SkillItem!.Name} * {BookAmount}");
    }


    private void AddCloseButton(GameObject parent)
    {
        var btnSize = 48;
        //创建关闭按钮
        var btnObj = UiTool.CreateButtonObject("#ff0000".HexStringToColor(), "X", "CloseBtn");
        btnObj.transform.SetParent(parent.transform);
        var rect = btnObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为右上角
            rect.anchorMin = Vector2.one;
            rect.anchorMax = Vector2.one;
            //设置关闭按钮的大小
            rect.SetSize(new(btnSize, btnSize));
            //按钮中心点相对于锚点的定位
            rect.anchoredPosition = new(-btnSize / 2, -btnSize / 2);
        }
        var closeBtn = btnObj.GetComponent<Button>();
        //颜色
        var btnColors = closeBtn.colors;
        btnColors.normalColor = new(1.0f, 0f, 0f, 0.5f);
        btnColors.highlightedColor = new(1.0f, 0f, 0f, 1.0f);
        btnColors.pressedColor = btnColors.normalColor;
        closeBtn.colors = btnColors;
        //
        closeBtn.onClick.AddListener(() => SetActive(false));
    }
}
