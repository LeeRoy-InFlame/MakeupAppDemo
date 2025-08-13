using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class UseCream : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop вызван");

        // Проверяем, руку ли отпустили
        HandController hand = eventData.pointerDrag?.GetComponent<HandController>();
        if (hand == null) return;

        // Проверка, что рука держит именно крем
        GrabbableItem item = hand.HeldItem;
        if (item == null || item.Type != GrabbableItem.ItemType.Cream) return;

        // Получаем все UI-элементы под курсором
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Ищем зону Acne
        foreach (var result in results)
        {
            var zone = result.gameObject.GetComponent<DropTargetZone>();
            if (zone != null && zone.zone == DropTargetZone.ZoneType.Acne)
            {
                AcneOff(result.gameObject); // передаём саму зону
                hand.PerformItemAction();
                return;
            }
        }

        

        Debug.Log("Крем применён не к зоне Acne");
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
            Debug.LogWarning("На зоне Acne нет компонента AcneFade!");
        }
    }
}



