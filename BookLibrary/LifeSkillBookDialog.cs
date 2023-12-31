﻿using Config;
using UnityEngine;
using UnityEngine.UI;

namespace Liuguang.mod.Taiwu.BookLibrary;

/// <summary>
/// 发放技艺书的询问界面
/// </summary>
internal class LifeSkillBookDialog
{
    /// <summary>
    /// 要发放的书籍列表
    /// </summary>
    private List<LifeSkillItem>? SkillItemList;
    /// <summary>
    /// 数量
    /// </summary>
    private int BookAmount = 1;
    private string BookTitle
    {
        get
        {
            if (SkillItemList == null)
            {
                return "未初始化书籍";
            }
            if (SkillItemList.Count == 0)
            {
                return "未初始化书籍";
            }
            if (SkillItemList.Count == 1)
            {
                return SkillItemList[0].Name;
            }
            return "批量获取书籍";
        }
    }

    private Color BookColor
    {
        get
        {
            if (SkillItemList == null)
            {
                return Color.red;
            }
            if (SkillItemList.Count == 1)
            {
                return Colors.Instance.GradeColors[SkillItemList[0].Grade];
            }
            return Color.white;
        }
    }
    private GameObject? rootObject;
    private Text? TitleText;

    private Action<int, string>? ShowTipFunc;

    public void InitUI(GameObject parent, Action<int, string> showTipFunc)
    {
        //遮罩层
        rootObject = UiTool.CreateRectObject(new(0, 0, 0, 0.7f), "CombatSkillBookDialogMask");
        rootObject.SetActive(false);
        rootObject.transform.SetParent(parent.transform, false);
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
        dialogObject.transform.SetParent(rootObject.transform, false);
        var dialogRect = dialogObject.GetComponent<RectTransform>();
        if (dialogRect != null)
        {
            //锚点为parent(50%width,40%height)
            dialogRect.anchorMin = new(0.25f, 0.3f);
            dialogRect.anchorMax = new(0.75f, 0.7f);
            //
            dialogRect.offsetMin = Vector2.zero;
            dialogRect.offsetMax = Vector2.zero;
        }
        //
        AddMainTitle(dialogObject);
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
        titlePanel.transform.SetParent(parent.transform, false);
        var rect = titlePanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的上边线
            rect.anchorMin = Vector2.up;//(0,1)
            rect.anchorMax = Vector2.one;//(1,1)
            //高度固定为48
            rect.offsetMin = new(0, -48);
            rect.offsetMax = new(0, 0);
        }
        var textObject = UiTool.CreateRectObject("Text");
        textObject.transform.SetParent(titlePanel.transform, false);
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

    public void SetSkillItem(LifeSkillItem item)
    {
        List<LifeSkillItem> itemList = new() { item };
        SetSkillItemList(itemList);
    }

    public void SetSkillItemList(List<LifeSkillItem> itemList)
    {
        SkillItemList = itemList;
        if (TitleText != null)
        {
            TitleText.text = BookTitle;
            TitleText.color = BookColor;
        }
    }

    private void AddAmountInput(GameObject parent)
    {
        var pagePanel = UiTool.CreateRectObject("AmountInputPanel");
        pagePanel.transform.SetParent(parent.transform, false);
        var offsetTop = 48 + 24;//距离上方的距离
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
        textObject.transform.SetParent(pagePanel.transform, false);
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
        pageContainerObject.transform.SetParent(pagePanel.transform, false);
        var containerRect = pageContainerObject.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = new(textWidth + 16, 0);
            containerRect.offsetMax = Vector2.zero;
        }
        var inputFieldObject = UiTool.CreateInputField(Color.white, "输入需要的数量", "InputField");
        inputFieldObject.transform.SetParent(pageContainerObject.transform, false);
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
        btnPanel.transform.SetParent(parent.transform, false);
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
        cancelBtnObject.transform.SetParent(btnPanel.transform, false);
        var confirmBtnObject = UiTool.CreateButtonObject("#ed991c".HexStringToColor(), "确定", "ConfirmBtn");
        confirmBtnObject.transform.SetParent(btnPanel.transform, false);
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
        if (SkillItemList == null)
        {
            ShowTipFunc?.Invoke(1, "选择的书籍书籍出错");
            return;
        }
        var bookIds = new List<short>();
        foreach (var item in SkillItemList)
        {
            bookIds.Add(item.SkillBookId);
        }
        BookApi.AsyncGetBook(GameUi.ShowItemList, playerId, bookIds, BookAmount);
        //关闭当前弹窗
        SetActive(false);
        //ShowTipFunc?.Invoke(0, $"获得了{SkillItem!.Name} * {BookAmount}");
    }


    private void AddCloseButton(GameObject parent)
    {
        var btnSize = 48;
        //创建关闭按钮
        var btnObj = UiTool.CreateButtonObject("#ff0000".HexStringToColor(), "X", "CloseBtn");
        btnObj.transform.SetParent(parent.transform, false);
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
