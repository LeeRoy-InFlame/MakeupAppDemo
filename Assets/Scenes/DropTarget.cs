using UnityEngine;
using UnityEngine.EventSystems;

public class DropTarget : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        if (dropped == null) return;

        if (dropped.CompareTag("HandWithCream"))
        {
            Debug.Log("Прыщи исчезают!");
            // Здесь вызови метод, делающий прыщи прозрачными и т.д.
        }
        else
        {
            Debug.Log("Сброшен другой объект: " + dropped.name);
        }
    }
}

