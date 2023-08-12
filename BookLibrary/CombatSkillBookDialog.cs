﻿using Config;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.GameDataBridge;
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
    private CombatSkillItem? combatSkillItem;
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
            return combatSkillItem?.Name ?? "未初始化书籍";
        }
    }

    private Color BookColor
    {
        get
        {
            if (combatSkillItem == null)
            {
                return Color.white;
            }
            return Colors.Instance.GradeColors[combatSkillItem.Grade];
        }
    }
    private GameObject? rootObject;
    private Text? TitleText;
    private readonly List<GameObject> OutlinePageObjects = new(5);
    private readonly List<GameObject> DirectPageObjects = new(5);
    private readonly List<GameObject> ReversePageObjects = new(5);

    private Color NotActiveColor = "#515550".HexStringToColor();
    private Color ActiveColor = "#8c4882".HexStringToColor();

    public void InitUI(GameObject parent)
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
            rect.offsetMin = new(0, -30);
            rect.offsetMax = new(0, 0);
        }
        var textObject = UiTool.CreateRectObject("Text");
        textObject.transform.SetParent(titlePanel.transform);
        textObject.transform.localPosition = Vector3.zero;
        var textComponent = textObject.AddComponent<Text>();
        UiTool.InitText(textComponent);
        textComponent.text = BookTitle;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = BookColor;
        TitleText = textComponent;
    }

    public void SetCombatSkillItem(CombatSkillItem item)
    {
        combatSkillItem = item;
        if (TitleText != null)
        {
            TitleText.text = BookTitle;
            TitleText.color = BookColor;
        }
    }

    private void AddOutlinePages(GameObject parent)
    {
        var pagePanel = UiTool.CreateRectObject("OutlinePagesPanel");
        pagePanel.transform.SetParent(parent.transform);
        var offsetTop = 30 + 15;//标题区
        var offsetLength = 10;
        var panelHeight = 30;
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
        textComponent.text = "总纲";
        textComponent.alignment = TextAnchor.MiddleRight;
        //
        var pageContainerObject = UiTool.CreateRectObject("PageContainer");
        pageContainerObject.transform.SetParent(pagePanel.transform);
        var containerRect = pageContainerObject.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = new(textWidth + 10, 0);
            containerRect.offsetMax = Vector2.zero;
        }
        var layout = pageContainerObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 5.0f;
        for (sbyte index = 0; index < 5; index++)
        {
            var pageBtnObject = UiTool.CreateButtonObject(NotActiveColor, LocalStringManager.Get($"LK_CombatSkill_First_Page_Type_{index}"), $"PageBtn-{index}");
            pageBtnObject.transform.SetParent(pageContainerObject.transform);
            OutlinePageObjects.Add(pageBtnObject);
            var btnComponent = pageBtnObject.GetComponent<Button>();
            var btnIndex = index;
            btnComponent.onClick.AddListener(() => ActiveOutlinePageType(btnIndex));
        }
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

    private void AddDirectPages(GameObject parent)
    {
        var pagePanel = UiTool.CreateRectObject("DirectPagesPanel");
        pagePanel.transform.SetParent(parent.transform);
        var offsetTop = (30 + 15) * 2;//距离上方的距离
        var offsetLength = 10;
        var panelHeight = 30;
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
        textComponent.text = "正练篇";
        textComponent.alignment = TextAnchor.MiddleRight;
        //
        var pageContainerObject = UiTool.CreateRectObject("PageContainer");
        pageContainerObject.transform.SetParent(pagePanel.transform);
        var containerRect = pageContainerObject.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = new(textWidth + 10, 0);
            containerRect.offsetMax = Vector2.zero;
        }
        var layout = pageContainerObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 5.0f;
        for (var index = 0; index < 5; index++)
        {
            var pageBtnObject = UiTool.CreateButtonObject(NotActiveColor, LocalStringManager.Get($"LK_CombatSkill_Direct_Page_{index}"), $"PageBtn-{index}");
            pageBtnObject.transform.SetParent(pageContainerObject.transform);
            DirectPageObjects.Add(pageBtnObject);
            var text = pageBtnObject.GetComponentInChildren<Text>();
            text.color = "#00dcdc".HexStringToColor();
            var btnComponent = pageBtnObject.GetComponent<Button>();
            var btnIndex = index;
            btnComponent.onClick.AddListener(() => ActiveNormalPageType(btnIndex, 0));
        }
    }

    private void AddReversePages(GameObject parent)
    {
        var pagePanel = UiTool.CreateRectObject("ReversePagesPanel");
        pagePanel.transform.SetParent(parent.transform);
        var offsetTop = (30 + 15) * 3;//距离上方的距离
        var offsetLength = 10;
        var panelHeight = 30;
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
        textComponent.text = "逆练篇";
        textComponent.alignment = TextAnchor.MiddleRight;
        //
        var pageContainerObject = UiTool.CreateRectObject("PageContainer");
        pageContainerObject.transform.SetParent(pagePanel.transform);
        var containerRect = pageContainerObject.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = new(textWidth + 10, 0);
            containerRect.offsetMax = Vector2.zero;
        }
        var layout = pageContainerObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 5.0f;
        for (var index = 0; index < 5; index++)
        {
            var pageBtnObject = UiTool.CreateButtonObject(NotActiveColor, LocalStringManager.Get($"LK_CombatSkill_Reverse_Page_{index}"), $"PageBtn-{index}");
            pageBtnObject.transform.SetParent(pageContainerObject.transform);
            ReversePageObjects.Add(pageBtnObject);
            var text = pageBtnObject.GetComponentInChildren<Text>();
            text.color = "#d68a00".HexStringToColor();
            var btnComponent = pageBtnObject.GetComponent<Button>();
            var btnIndex = index;
            btnComponent.onClick.AddListener(() => ActiveNormalPageType(btnIndex, 1));
        }
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
        var offsetTop = (30 + 15) * 4;//距离上方的距离
        var offsetLength = 10;
        var panelHeight = 30;
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
            containerRect.offsetMin = new(textWidth + 10, 0);
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
            rect.anchorMin = Vector2.right;//(0,1)
            rect.anchorMax = Vector2.right;//(1,1)
            var (width, height) = (170, 40);
            rect.SetSize(new(width, height));
            rect.anchoredPosition = new(-width / 2 - 10, height / 2 + 10);
        }
        var layout = btnPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
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
        GameDataBridge.AddMethodCall(-1, DomainHelper.DomainIds.Character, CharacterDomainHelper.MethodIds.CreateInventoryItem,
        playerId, ItemType.SkillBook, combatSkillItem!.BookId, BookAmount, pageTypes);
        //关闭当前弹窗
        SetActive(false);
    }


    private void AddCloseButton(GameObject parent)
    {
        var btnSize = 30;
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