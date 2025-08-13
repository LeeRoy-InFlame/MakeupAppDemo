using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class UseCream : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop ������");

        // ���������, ���� �� ���������
        HandController hand = eventData.pointerDrag?.GetComponent<HandController>();
        if (hand == null) return;

        // ��������, ��� ���� ������ ������ ����
        GrabbableItem item = hand.HeldItem;
        if (item == null || item.Type != GrabbableItem.ItemType.Cream) return;

        // �������� ��� UI-�������� ��� ��������
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // ���� ���� Acne
        foreach (var result in results)
        {
            var zone = result.gameObject.GetComponent<DropTargetZone>();
            if (zone != null && zone.zone == DropTargetZone.ZoneType.Acne)
            {
                AcneOff(result.gameObject); // ������� ���� ����
                hand.PerformItemAction();
                return;
            }
        }

        

        Debug.Log("���� ������� �� � ���� Acne");
    }

    private void AcneOff(GameObject zoneObject)
    {
        var fade = zoneObject.GetComponent<AcneFade>();
        if (fade != null)
        {
            fade.StartFade();
        }
        else
        {
            Debug.LogWarning("�� ���� Acne ��� ���������� AcneFade!");
        }
    }
}



