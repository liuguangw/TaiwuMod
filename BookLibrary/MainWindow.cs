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
        var mainPanel = CreateMainPanel(rootObject, "#1c1c1c".HexStringToColor());
        AddMainTitle(mainPanel, "̫�������");
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
        rootObject?.SetActive(!rootObject.activeSelf);
    }

    /// <summary>
    /// ��������
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
        var mainPanel = CreateRectObject(bgColor, "MainPanel");
        mainPanel.transform.SetParent(parent.transform);
        var rect = mainPanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            //ê�㷶Χ,�������С����Ӧ(width=60%,height=72%)
            rect.anchorMin = new(0.2f, 0.14f);
            rect.anchorMax = new(0.8f, 0.86f);
            //��ê����������غ�
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        return mainPanel;
    }

    private void AddCloseButton(GameObject parent)
    {
        var btnSize = 30;
        //�����رհ�ť
        var btnObj = CreateRectObject("#ff0000".HexStringToColor(), "CloseBtn");
        btnObj.transform.SetParent(parent.transform);
        var rect = btnObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            //ê��Ϊ���Ͻ�
            rect.anchorMin = Vector2.one;
            rect.anchorMax = Vector2.one;
            //���ùرհ�ť�Ĵ�С
            rect.SetWidth(btnSize);
            rect.SetHeight(btnSize);
            //��ť���ĵ������ê��Ķ�λ
            rect.anchoredPosition = new(-btnSize / 2, -btnSize / 2);
        }
        var closeBtn = btnObj.AddComponent<Button>();
        //��ɫ
        var btnColors = closeBtn.colors;
        btnColors.normalColor = new(1.0f, 0f, 0f, 0.5f);
        btnColors.highlightedColor = new(1.0f, 0f, 0f, 1.0f);
        btnColors.pressedColor = btnColors.normalColor;
        closeBtn.colors = btnColors;
        //
        closeBtn.onClick.AddListener(SwitchActiveStatus);
        //
        var textObject = CreateRectObject("Text");
        textObject.transform.SetParent(btnObj.transform);
        textObject.transform.localPosition = Vector3.zero;
        var textComponent = textObject.AddComponent<Text>();
        InitText(textComponent);
        textComponent.text = "X";
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = "#ffffff".HexStringToColor();
    }

    private void AddMainTitle(GameObject parent, string title)
    {

        var titlePanel = CreateRectObject("#302a28".HexStringToColor(), "TitlePanel");
        titlePanel.transform.SetParent(parent.transform);
        var rect = titlePanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            //ê��Ϊparent���ϱ���
            rect.anchorMin = Vector2.up;//(0,1)
            rect.anchorMax = Vector2.one;//(1,1)
            //���ñ���������ê�����(��ʱΪparent���ϱ���)���½ǡ����Ͻǵľ���
            //�߶ȹ̶�Ϊ30
            rect.offsetMin = new(0, -30);
            rect.offsetMax = new(0, 0);
        }
        var textObject = CreateRectObject("Text");
        textObject.transform.SetParent(titlePanel.transform);
        textObject.transform.localPosition = Vector3.zero;
        var textComponent = textObject.AddComponent<Text>();
        InitText(textComponent);
        textComponent.text = title;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = "#e1d1af".HexStringToColor();
    }

    /// <summary>
    /// ����һ����������
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private GameObject CreateRectObject(string name = "")
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
    /// ��������ɫ�ľ���
    /// </summary>
    /// <param name="backgroundColor"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private GameObject CreateRectObject(Color backgroundColor, string name = "")
    {
        var obj = CreateRectObject(name);
        var image = obj.AddComponent<Image>();
        image.type = Image.Type.Sliced;
        image.color = backgroundColor;
        return obj;
    }

    /// <summary>
    /// ����Ĭ�ϵ���������
    /// </summary>
    /// <param name="textComponent"></param>
    private void InitText(Text textComponent)
    {
        textComponent.fontSize = 16;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
    }
}