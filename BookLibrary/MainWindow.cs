using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Liuguang.mod.Taiwu.BookLibrary;

internal class MainWindow
{

    private GameObject? rootObject;
    private GameObject? TipArea;
    private Text? TipText;
    private readonly List<GameObject> bookTypeBtnObjects = new(2);
    private int bookType = -1;
    private readonly CombatSkillWindow combatSkillWindow = new();
    private readonly LifeSkillWindow lifeSkillWindow = new();

    public void InitUI()
    {
        rootObject = CreateCanvas("taiwu.BookLibrary.root");
        rootObject.SetActive(false);
        CreateMask(rootObject);
        var mainPanel = CreateMainPanel(rootObject);
        AddMainTitle(mainPanel, "太吾出版社");
        AddMainContainer(mainPanel);
        AddCloseButton(mainPanel);
    }

    public void DestroyUI()
    {
        if (rootObject != null)
        {
            UnityEngine.Object.Destroy(rootObject);
        }
    }


    public void SwitchActiveStatus()
    {
        if(rootObject is null) {  return; }
        if (!rootObject.activeSelf)
        {
            var playerId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
            if (playerId <= 0)
            {
                AdaptableLog.Error("必须进入游戏才能使用mod");
                return;
            }
        }
        rootObject.SetActive(!rootObject.activeSelf);
    }

    /// <summary>
    /// 创建画布
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    private GameObject CreateCanvas(string objectName)
    {
        var obj = UiTool.CreateRectObject(objectName);
        obj.layer = LayerMask.NameToLayer("UI");
        //parent
        var parentUI = GameObject.Find("/Camera_UIRoot/Canvas/LayerVeryTop");
        if (parentUI != null)
        {
            obj.transform.SetParent(parentUI.transform, false);
        }
        else
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
        }
        var rect = obj.GetComponent<RectTransform>();
        {
            //锚点为parent
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            //
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        var canvas = obj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //没有找到游戏里的挂载点时,需要使用CanvasScaler来进行缩放
        if (parentUI == null)
        {
            var canvasScaler = obj.AddComponent<CanvasScaler>();
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referencePixelsPerUnit = 100;
            canvasScaler.referenceResolution = new(2560, 1440);
        }
        //
        obj.AddComponent<GraphicRaycaster>();
        return obj;
    }

    /// <summary>
    /// 创建遮罩层
    /// </summary>
    /// <param name="parent"></param>
    private void CreateMask(GameObject parent)
    {
        //遮罩层
        var maskObject = UiTool.CreateRectObject(new(0, 0, 0, 0.7f), "Mask");
        maskObject.transform.SetParent(parent.transform, false);
        var rect = maskObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            //
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }

    private GameObject CreateMainPanel(GameObject parent)
    {
        var bgColor = "#1c1c1c".HexStringToColor();
        var mainPanel = UiTool.CreateRectObject(bgColor, "MainPanel");
        mainPanel.transform.SetParent(parent.transform, false);
        var rect = mainPanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点范围,主窗体大小自适应(width=60%,height=72%)
            rect.anchorMin = new(0.2f, 0.14f);
            rect.anchorMax = new(0.8f, 0.86f);
            //与锚点的四条边重合
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        return mainPanel;
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
        closeBtn.onClick.AddListener(SwitchActiveStatus);
    }

    private void AddMainTitle(GameObject parent, string title)
    {

        var titlePanel = UiTool.CreateRectObject("#302a28".HexStringToColor(), "TitlePanel");
        titlePanel.transform.SetParent(parent.transform, false);
        var rect = titlePanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的上边线
            rect.anchorMin = Vector2.up;//(0,1)
            rect.anchorMax = Vector2.one;//(1,1)
            //设置标题框相对于锚点矩形(此时为parent的上边线)左下角、右上角的距离
            //高度固定为30
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
        textComponent.text = title;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = "#e1d1af".HexStringToColor();
    }

    private void AddMainContainer(GameObject parent)
    {
        var mainContainer = UiTool.CreateRectObject("MainContainer");
        mainContainer.transform.SetParent(parent.transform, false);
        var rect = mainContainer.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的四条边线
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            //设置间距
            var padding = 13;
            rect.offsetMin = new(padding, padding);
            rect.offsetMax = new(-padding, -padding - 48);
        }
        string[] bookTypeList = { "功法书", "技艺书" };
        AddBookTypeNav(mainContainer, bookTypeList);
        AddTipArea(mainContainer);
        combatSkillWindow.InitUI(mainContainer, ShowTip);
        lifeSkillWindow.InitUI(mainContainer, ShowTip);
        ActiveBookType(0);
    }

    private void AddBookTypeNav(GameObject parent, string[] bookTypeList)
    {
        var bookTypeNav = UiTool.CreateRectObject("BookTypeNav");
        bookTypeNav.transform.SetParent(parent.transform, false);
        var rect = bookTypeNav.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的左上角
            rect.anchorMin = Vector2.up;//(0,1)
            rect.anchorMax = Vector2.up;//(0,1)
            //固定bookTypeNav大小
            var navWidth = 250;
            var navHeight = 54;
            rect.SetSize(new(navWidth, navHeight));
            //距离
            rect.anchoredPosition = new(navWidth / 2, -navHeight / 2);
        }
        var layout = bookTypeNav.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8.0f;
        for (var index = 0; index < bookTypeList.Length; index++)
        {
            var bookTypeBtnObject = UiTool.CreateButtonObject("#9b886d".HexStringToColor(), bookTypeList[index], $"BookType-{index}");
            bookTypeBtnObject.transform.SetParent(bookTypeNav.transform, false);
            bookTypeBtnObjects.Add(bookTypeBtnObject);
            var btnComponent = bookTypeBtnObject.GetComponent<Button>();
            var btnIndex = index;
            btnComponent.onClick.AddListener(() => ActiveBookType(btnIndex));
        }

    }

    private void AddTipArea(GameObject parent)
    {
        var tipAreaObject = UiTool.CreateRectObject("#198754".HexStringToColor(), "TipArea");
        tipAreaObject.transform.SetParent(parent.transform, false);
        tipAreaObject.SetActive(false);
        var rect = tipAreaObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            //锚点为parent的上边
            rect.anchorMin = Vector2.up;//(0,1)
            rect.anchorMax = Vector2.one;//(1,1)
            //
            rect.offsetMin = new(265.6f, -54.4f);
            rect.offsetMax = Vector2.zero;
        }
        var textObject = UiTool.CreateRectObject("Text");
        textObject.transform.SetParent(tipAreaObject.transform, false);
        var textRect = textObject.GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new(12.8f, 0);
            textRect.offsetMax = new(-12.8f, 0);
        }
        var text = textObject.AddComponent<Text>();
        UiTool.InitText(text);
        text.text = "this is a tip text";
        text.alignment = TextAnchor.MiddleLeft;
        text.fontStyle = FontStyle.Bold;
        //
        TipArea = tipAreaObject;
        TipText = text;
    }

    private void ActiveBookType(int bookTypeIndex)
    {
        if (bookTypeIndex == bookType)
        {
            return;
        }
        bookType = bookTypeIndex;
        for (var index = 0; index < bookTypeBtnObjects.Count; index++)
        {
            var bookTypeBtnObject = bookTypeBtnObjects[index];
            var image = bookTypeBtnObject.GetComponent<Image>();
            var imageColor = (bookType == index) ? "#ed991c" : "#9b886d";
            image.color = imageColor.HexStringToColor();
        }
        combatSkillWindow.SetActive(bookType == 0);
        lifeSkillWindow.SetActive(bookType == 1);
    }

    private async void ShowTip(int tipType, string tipContent)
    {
        if (TipArea != null)
        {
            var image = TipArea.GetComponent<Image>();
            var tipBackground = (tipType == 0) ? "#198754" : "#dc3545";
            image.color = tipBackground.HexStringToColor();
            if (TipText != null)
            {
                TipText.text = tipContent;
            }
            TipArea.SetActive(true);
            await Task.Delay(1600);
            TipArea.SetActive(false);
        }
    }

}
