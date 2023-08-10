using System.Reflection;
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
    public static GameObject CreateButtonObject(Color backgroundColor,string btnText, string name = "")
    {
        var btnObject = CreateRectObject(backgroundColor, name);
        btnObject.AddComponent<Button>();
        var textObject = CreateRectObject("Text");
        textObject.transform.SetParent(btnObject.transform);
        var rect = textObject.GetComponent<RectTransform>();
        if(rect != null)
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
}
