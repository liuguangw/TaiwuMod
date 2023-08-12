using UnityEngine;
using UnityEngine.UI;

namespace Liuguang.mod.Taiwu.BookLibrary;
internal static class UiTool
{
    private static Font? defaultFont;

    /// <summary>
    /// 创建一个矩形物体
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject CreateRectObject(string name = "")
    {
        var obj = new GameObject();
        if (!string.IsNullOrEmpty(name))
        {
            obj.name = name;
        }
        obj.AddComponent<RectTransform>();
        return obj;
    }

    /// <summary>
    /// 带背景颜色的矩形
    /// </summary>
    /// <param name="backgroundColor"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject CreateRectObject(Color backgroundColor, string name = "")
    {
        var obj = CreateRectObject(name);
        var image = obj.AddComponent<Image>();
        image.type = Image.Type.Sliced;
        image.color = backgroundColor;
        return obj;
    }

    /// <summary>
    /// 设置默认的字体属性
    /// </summary>
    /// <param name="textComponent"></param>
    public static void InitText(Text textComponent)
    {
        textComponent.fontSize = 16;
        if (defaultFont == null)
        {
            defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        textComponent.font = defaultFont;
    }

    /// <summary>
    /// 创建按钮对象
    /// </summary>
    /// <param name="backgroundColor"></param>
    /// <param name="btnText"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject CreateButtonObject(Color backgroundColor, string btnText, string name = "")
    {
        var btnObject = CreateRectObject(backgroundColor, name);
        btnObject.AddComponent<Button>();
        var textObject = CreateRectObject("Text");
        textObject.transform.SetParent(btnObject.transform);
        var rect = textObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        var textComponent = textObject.AddComponent<Text>();
        InitText(textComponent);
        textComponent.text = btnText;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = "#ffffff".HexStringToColor();
        return btnObject;
    }

    private static GameObject CreateScrollbar()
    {
        var scrollbarObject = CreateRectObject(Color.black, "Scrollbar");
        scrollbarObject.GetComponent<RectTransform>();
        var scrollbar = scrollbarObject.AddComponent<Scrollbar>();
        //
        var slideObject = CreateRectObject("Sliding Area");
        slideObject.transform.SetParent(scrollbarObject.transform);
        var slideRect = slideObject.GetComponent<RectTransform>();
        slideRect.anchorMin = Vector2.zero;
        slideRect.anchorMax = Vector2.one;
        slideRect.sizeDelta = new Vector2(-10f, -10f);
        //
        var handleObject = CreateRectObject(Color.blue, "Handle");
        handleObject.transform.SetParent(slideObject.transform);
        var handleRect = handleObject.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(10f, 10f);
        //
        scrollbar.handleRect = handleRect;
        scrollbar.targetGraphic = handleObject.GetComponent<Image>();
        return scrollbarObject;
    }

    public static GameObject CreateVerticalScrollView(string name = "")
    {
        var scrollView = CreateRectObject(name);
        var viewPortObj = CreateRectObject("ViewPort");
        viewPortObj.transform.SetParent(scrollView.transform);
        var viewPortRect = viewPortObj.GetComponent<RectTransform>();
        viewPortRect.anchorMin = Vector2.zero;
        viewPortRect.anchorMax = Vector2.one;
        viewPortRect.offsetMin = Vector2.zero;
        viewPortRect.offsetMax = new(-25, 0);
        //mask
        var mask = viewPortObj.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        var maskImage = viewPortObj.AddComponent<Image>();
        maskImage.type = Image.Type.Sliced;
        //
        var contentObj = CreateRectObject("Content");
        contentObj.transform.SetParent(viewPortObj.transform);
        var contentRect = contentObj.GetComponent<RectTransform>();
        var contentSizeFitter = contentObj.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var scrollBarObj = CreateScrollbar();
        scrollBarObj.transform.SetParent(scrollView.transform);
        var scrollBar = scrollBarObj.GetComponent<Scrollbar>();
        scrollBar.SetDirection(Scrollbar.Direction.BottomToTop, true);
        var scrollBarRect = scrollBarObj.GetComponent<RectTransform>();
        scrollBarRect.anchorMin = Vector2.right;
        scrollBarRect.anchorMax = Vector2.one;
        scrollBarRect.offsetMin = new(-20, 0);
        scrollBarRect.offsetMax = Vector2.zero;
        //
        var scrollRect = scrollView.AddComponent<ScrollRect>();
        scrollRect.content = contentRect;
        scrollRect.viewport = viewPortRect;
        scrollRect.verticalScrollbar = scrollBar;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        return scrollView;
    }

    public static GameObject CreateInputField(Color backgroundColor, string placeholderText = "请输入...", string name = "")
    {
        var gameObject = CreateRectObject(backgroundColor, name);
        var placeholderObject = CreateRectObject("Placeholder");
        placeholderObject.transform.SetParent(gameObject.transform);
        var textObject = CreateRectObject("Text");
        textObject.transform.SetParent(placeholderObject.transform);
        //
        var inputField = gameObject.AddComponent<InputField>();
        var text = textObject.AddComponent<Text>();
        text.text = "";
        text.color = "#0f0f0f".HexStringToColor();
        text.alignment = TextAnchor.MiddleLeft;
        text.supportRichText = false;
        InitText(text);
        //
        var text2 = placeholderObject.AddComponent<Text>();
        InitText(text2);
        text2.text = placeholderText;
        text2.fontStyle = FontStyle.Italic;
        text2.alignment = TextAnchor.MiddleLeft;
        Color color = text.color;
        color.a *= 0.5f;
        text2.color = color;
        //
        var component = textObject.GetComponent<RectTransform>();
        component.anchorMin = Vector2.zero;
        component.anchorMax = Vector2.one;
        component.sizeDelta = Vector2.zero;
        component.offsetMin = new(5, 0);
        component.offsetMax = new(-5,0);
        var component2 = placeholderObject.GetComponent<RectTransform>();
        component2.anchorMin = Vector2.zero;
        component2.anchorMax = Vector2.one;
        component2.offsetMin = Vector2.zero;
        component2.offsetMax = Vector2.zero;
        inputField.textComponent = text;
        inputField.placeholder = text2;
        return gameObject;
    }
}
