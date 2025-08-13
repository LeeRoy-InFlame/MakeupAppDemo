using UnityEngine;

public class BrushItem : MonoBehaviour, IUsableItem, IColorZone
{
    [SerializeField] private ArrayOfColors _colorDisplay;

    private int _currentColorIndex = -1;
    public bool ShouldReleaseAfterUse => false;
    public void UseOnZone(GameObject targetZone, HandController hand)
    {
        // Если это палитра — берём цвет
        if (targetZone.TryGetComponent<IColorIndexProvider>(out var provider))
        {
            int newColorIndex = provider.GetColorIndex();
            _currentColorIndex = newColorIndex;
            _colorDisplay.ApplyColor(newColorIndex);
            Debug.Log($"Кисть взяла цвет: {newColorIndex}");
            hand.PerformItemAction(false);
        }
        // Если это зона для нанесения цвета
        else if (targetZone.TryGetComponent<IColorZone>(out var zone))
        {
            if (_currentColorIndex >= 0)
            {
                zone.ApplyColor(_currentColorIndex);
                Debug.Log($"Кисть нанесла цвет: {_currentColorIndex}");
                hand.PerformItemAction(true);
            }
            else
            {
                Debug.LogWarning("Кисть пока не содержит цвет.");
            }
        }
        else
        {
            Debug.LogWarning("Целевая зона не поддерживает взаимодействие.");
        }
    }

    public void ApplyColor(int colorIndex)
    {
        _currentColorIndex = colorIndex;
        _colorDisplay?.ApplyColor(colorIndex);
    }
}


