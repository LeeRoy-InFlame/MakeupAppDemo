using UnityEngine;

public class BrushItem : MonoBehaviour, IUsableItem, IColorZone
{
    [SerializeField] private ArrayOfColors _colorDisplay;

    private int _currentColorIndex = -1;
    public bool ShouldReleaseAfterUse => false;
    public void UseOnZone(GameObject targetZone, HandController hand)
    {
        // ���� ��� ������� � ���� ����
        if (targetZone.TryGetComponent<IColorIndexProvider>(out var provider))
        {
            int newColorIndex = provider.GetColorIndex();
            _currentColorIndex = newColorIndex;
            _colorDisplay.ApplyColor(newColorIndex);
            Debug.Log($"����� ����� ����: {newColorIndex}");
            hand.PerformItemAction(false);
        }
        // ���� ��� ���� ��� ��������� �����
        else if (targetZone.TryGetComponent<IColorZone>(out var zone))
        {
            if (_currentColorIndex >= 0)
            {
                zone.ApplyColor(_currentColorIndex);
                Debug.Log($"����� ������� ����: {_currentColorIndex}");
                hand.PerformItemAction(true);
            }
            else
            {
                Debug.LogWarning("����� ���� �� �������� ����.");
            }
        }
        else
        {
            Debug.LogWarning("������� ���� �� ������������ ��������������.");
        }
    }

    public void ApplyColor(int colorIndex)
    {
        _currentColorIndex = colorIndex;
        _colorDisplay?.ApplyColor(colorIndex);
    }
}


