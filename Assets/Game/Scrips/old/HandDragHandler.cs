using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float followSpeed = 10f; // скорость следования
    [SerializeField] private float maxDistance = 500f; // ограничение на рывок

    private RectTransform _rectTransform;
    private Canvas _canvas;

    private Vector2 _targetPosition;
    private bool _isDragging;

    private GrabbableItem _currentItem;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _targetPosition = _rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (_isDragging)
        {
            // Плавное движение к цели
            _rectTransform.anchoredPosition = Vector2.Lerp(
                _rectTransform.anchoredPosition,
                _targetPosition,
                Time.deltaTime * followSpeed
            );
        }
    }

    public void HoldItem(GrabbableItem item)
    {
        _currentItem = item;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
        UpdateTarget(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateTarget(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
    }

    private void UpdateTarget(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            // ограничение рывка (если надо)
            if (Vector2.Distance(_rectTransform.anchoredPosition, localPoint) < maxDistance)
            {
                _targetPosition = localPoint;
            }
        }
    }
}


