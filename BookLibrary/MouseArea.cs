using UnityEngine;
using UnityEngine.EventSystems;

namespace BookLibrary;

/// <summary>
/// 处理鼠标进出事件
/// </summary>
internal class MouseArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action? EnterAction { get; set; }
    public Action? ExitAction { get; set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnterAction?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ExitAction?.Invoke();
    }
}
