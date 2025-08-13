using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_InteractionEvents : MonoBehaviour, IPointerDownHandler
{
    [Header("Событие")]
    public UnityEvent OnPressEvent;

    private bool _isEnabled = true;

    public void SetInteractionEnabled(bool enabled)
    {
        _isEnabled = enabled;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isEnabled)
        {
            OnPressEvent?.Invoke();
        }
    }
}



