using UnityEngine;
using UnityEngine.UI;

namespace Liuguang.mod.Taiwu.BookLibrary;

internal class MainWindow
{

    private GameObject? rootObject;

    public void InitUI()
    {
        rootObject = CreateCanvas("taiwu.BookLibrary.root", new(1024f, 768f));
        rootObject.SetActive(false);
        var bgColor = "#1c1c1c".HexStringToColor();
        var mainPanel = CreateMainPanel(rootObject, bgColor);
        AddMainTitle(mainPanel, "太吾出版社");
        AddCloseButton(mainPanel);
    }

    /// <summary>
    /// 创建画布
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private GameObject CreateCanvas(string objectName, Vector2 size)
    {
        var obj = new GameObject(objectName);
        UnityEngine.Object.DontDestroyOnLoad(obj);
        //add canvas
        var canvas = obj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //
        var canvasScaler = obj.AddComponent<CanvasScaler>();
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasScaler.referencePixelsPerUnit = 100f;
        canvasScaler.referenceResolution = size;
        //
        obj.AddComponent<GraphicRaycaster>();
        return obj;
    }

    private GameObject CreateMainPanel(GameObject parent, Color bgColor)
    {
        var (width, height) = CalcWindowSize();
        var mainPanel = CreateRectObject(new(width, height), "MainPanel");
        mainPanel.transform.SetParent(parent.transform);
        //窗体居中
        mainPanel.transform.localPosition = Vector3.zero;
        //
        var image = mainPanel.AddComponent<Image>();
        image.type = Image.Type.Sliced;
        image.color = bgColor;
        return mainPanel;
    }

    private void AddCloseButton(GameObject parent)
    {
        var btnSize = 30;
        //创建关闭按钮
        var btnObj = CreateRectObject(new(btnSize, btnSize), "CloseBtn");
        btnObj.transform.SetParent(parent.transform);
        var (width, height) = CalcWindowSize();
        btnObj.transform.localPosition = new((width - btnSize) / 2, (height - btnSize) / 2);
        //
        var btnBackground = btnObj.AddComponent<Image>();
        btnBackground.type = Image.Type.Sliced;
        btnBackground.color = "#ff0000".HexStringToColor();
        //
        var closeBtn = btnObj.AddComponent<Button>();
        //颜色
        var btnColors = closeBtn.colors;
        btnColors.normalColor = new(1.0f, 0f, 0f, 0.5f);
        btnColors.highlightedColor = new(1.0f, 0f, 0f, 1.0f);
        btnColors.pressedColor = btnColors.normalColor;
        closeBtn.colors = btnColors;
        //
        closeBtn.onClick.AddListener(SwitchActiveStatus);
        //
        var textObj = CreateTextObject(new(btnSize, btnSize), text =>
        {
            text.text = "X";
            text.alignment = TextAnchor.MiddleCenter;
            text.color = "#ffffff".HexStringToColor();
        }, "Text");
        textObj.transform.SetParent(btnObj.transform);
        textObj.transform.localPosition = Vector3.zero;
    }

    private void AddMainTitle(GameObject parent, string title)
    {
        var (width, height) = CalcWindowSize();
        var titleHeight = 30;
        var titlePanelSize = new Vector2(width, titleHeight);
        var titlePanel = CreateRectObject(titlePanelSize, "TitlePanel");
        titlePanel.transform.SetParent(parent.transform);
        titlePanel.transform.localPosition = new(0, (height - titleHeight) / 2);
        //
        var image = titlePanel.AddComponent<Image>();
        image.type = Image.Type.Sliced;
        image.color = "#302a28".HexStringToColor();
        //
        var textPanel = CreateTextObject(titlePanelSize, text =>
        {
            text.text = title;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = "#e1d1af".HexStringToColor();
        }, "Text");
        textPanel.transform.SetParent(titlePanel.transform);
        textPanel.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 创建一个矩形物体
    /// </summary>
    /// <param name="size"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private GameObject CreateRectObject(Vector2 size, string name = "")
    {
        var obj = new GameObject();
        if (!string.IsNullOrEmpty(name))
        {
            obj.name = name;
        }
        var rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        return obj;
    }

    private GameObject CreateTextObject(Vector2 size, Action<Text> createAction, string name = "")
    {
        var obj = new GameObject();
        if (!string.IsNullOrEmpty(name))
        {
            obj.name = name;
        }
        var rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        //
        var textComponent = obj.AddComponent<Text>();
        textComponent.fontSize = 16;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        createAction(textComponent);
        return obj;
    }

    private (int, int) CalcWindowSize()
    {
        var width = Mathf.Min(Screen.width, 740);
        var height = (Screen.height < 400) ? Screen.height : 450;
        return (width, height);
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
        rootObject?.SetActive(!rootObject.activeSelf);
    }
}