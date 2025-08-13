using UnityEngine;

public class ColorItem : MonoBehaviour, IUsableItem
{
    [SerializeField] private int _colorIndex; // индекс цвета для применения
    public bool ShouldReleaseAfterUse => true; // используем и отпускаем
    public void UseOnZone(GameObject targetZone, HandController hand)
    {
        var zone = targetZone.GetComponent<IColorZone>();
        if (zone != null)
        {
            zone.ApplyColor(_colorIndex);
            hand.PerformItemAction();
        }
        else
        {
            Debug.LogWarning("Целевая зона не реализует IColorZone");
        }
    }
}

