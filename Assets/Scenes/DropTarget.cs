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
            Debug.Log("����� ��������!");
            // ����� ������ �����, �������� ����� ����������� � �.�.
        }
        else
        {
            Debug.Log("������� ������ ������: " + dropped.name);
        }
    }
}

