using UnityEngine;

public class ColorActivatorZone : MonoBehaviour, IColorIndexProvider
{
    [SerializeField] private int _colorIndex;
    public int GetColorIndex() => _colorIndex;
}
