using UnityEngine;

public class ArrayOfColors : MonoBehaviour, IColorZone
{
    [SerializeField] private GameObject[] _colorVariants;

    private int _currentIndex;
    public int CurrentIndex => _currentIndex;

    public void ApplyColor(int colorIndex)
    {
        _currentIndex = colorIndex;

        for (int i = 0; i < _colorVariants.Length; i++)
            _colorVariants[i].SetActive(false);

        if (colorIndex >= 0 && colorIndex < _colorVariants.Length)
            _colorVariants[colorIndex].SetActive(true);
    }
}

