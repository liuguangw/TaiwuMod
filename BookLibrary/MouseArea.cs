using UnityEngine;
using UnityEngine.EventSystems;

namespace Liuguang.mod.Taiwu.BookLibrary;

/// <summary>
/// 处理鼠标事件
/// </summary>
internal class MouseArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Action? EnterAction { get; set; }

    public Action? ExitAction { get; set; }

    public Action? ClickAction { get; set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnterAction?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ExitAction?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickAction?.Invoke();
    }
}
