using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Movement Settings

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 1f; // скорость движения руки к цели
    [SerializeField] private float _followSpeed = 10f; // скорость следования за курсором
    [SerializeField] private float _maxDragDistance = 500f; // максимальная дистанция отрыва
    [SerializeField] private float _returnSpeed = 1f; // скорость возвращения руки
    [SerializeField] private Transform _initialHandPosition; // исходная позиция руки

    #endregion

    #region Components

    [Header("Components")]
    [SerializeField] private Animator _animator; // аниматор руки

    private CanvasGroup _canvasGroup; // группа канвы для блокировки raycast
    private RectTransform _rectTransform; // трансформ руки
    private Canvas _canvas; // канва

    #endregion

    #region Drag State

    private Vector2 _dragTargetPos; // позиция, куда следует двигаться
    private bool _isDragging; // идёт ли drag
    private Vector2 _dragOffset;

    #endregion

    #region Action State

    private bool _isPerformingAction; // выполняется ли действие (встряска)
    private float _actionTime;
    private float _actionDuration = 1f; // длительность встряски
    private float _actionAmplitude = 0.1f; // амплитуда встряски
    private Vector3 _actionStartPosition; // стартовая позиция для встряски
    private bool _shouldReturnAfterAction = true; // нужно ли возвращаться после действия

    #endregion

    #region Held Item

    private GrabbableItem _heldItem; // текущий предмет
    public GrabbableItem HeldItem => _heldItem;
    public bool IsHoldingItem => _heldItem != null;

    #endregion

    #region Move To Item

    private bool _isMovingToItem = false; // двигается ли к предмету
    private float _grabMoveT;
    private Vector3 _grabStartPos, _grabTargetPos;

    #endregion

    #region Move To Use

    private bool _isMovingToUsePos = false; // двигается ли к позиции использования
    private float _useMoveT;
    private Vector3 _useStartPos, _useTargetPos;

    #endregion

    #region Return Sequence

    private bool _isReturningSequence; // выполняется ли возвращение
    private Vector3 _returnTarget;
    private float _returnT;

    private enum ReturnPhase { None, ToReturnPoint, DropItem, ToStart }
    private ReturnPhase _returnPhase = ReturnPhase.None;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _dragTargetPos = _rectTransform.anchoredPosition;
    }

    private void Update()
    {
        HandleDragFollow();
        HandleMoveToGrabPoint();
        HandleMoveToUsePosition();
        HandlePerformAction();
        HandleReturnSequence();
    }

    #endregion

    #region Grab Logic

    public void StartGrab(Transform itemGrabPoint, GrabbableItem item)
    {
        _isMovingToItem = true;
        _grabMoveT = 0;
        _grabStartPos = transform.position;
        _grabTargetPos = itemGrabPoint.position;

        _heldItem = item;
        SetAllUIInteractionsEnabled(false); // блокировка UI пока в руке предмет
    }

    private void HandleMoveToGrabPoint()
    {
        if (!_isMovingToItem) return;

        _grabMoveT += Time.deltaTime * _moveSpeed;
        float t = Mathf.SmoothStep(0, 1, _grabMoveT);
        transform.position = Vector3.Lerp(_grabStartPos, _grabTargetPos, t);

        if (_grabMoveT >= 1f)
        {
            _isMovingToItem = false;
            _animator.SetTrigger("Take");
            Invoke(nameof(FinishGrab), 0.3f);
        }
    }

    private void FinishGrab()
    {
        if (_heldItem == null) return;

        _heldItem.OnGrabComplete(this);

        Transform afterGrabTarget = _heldItem.GetAfterGrabHandPosition();
        if (afterGrabTarget != null)
        {
            _isMovingToUsePos = true;
            _useMoveT = 0;
            _useStartPos = transform.position;
            _useTargetPos = afterGrabTarget.position;
        }
    }

    private void HandleMoveToUsePosition()
    {
        if (!_isMovingToUsePos) return;

        _useMoveT += Time.deltaTime * _moveSpeed;
        float t = Mathf.SmoothStep(0, 1, _useMoveT);
        transform.position = Vector3.Lerp(_useStartPos, _useTargetPos, t);

        if (_useMoveT >= 1f)
        {
            _isMovingToUsePos = false;
        }
    }

    #endregion

    #region Drag Logic

    private void HandleDragFollow()
    {
        if (!_isDragging) return;

        _rectTransform.anchoredPosition = Vector2.Lerp(
            _rectTransform.anchoredPosition,
            _dragTargetPos,
            Time.deltaTime * _followSpeed
        );
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
        _isDragging = true;
        eventData.pointerDrag = gameObject;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            // Вычисляем смещение между пальцем и текущей позицией руки
            _dragOffset = _rectTransform.anchoredPosition - localPoint;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateDragTarget(eventData);
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(
        _canvas.transform as RectTransform,
        eventData.position,
        eventData.pressEventCamera,
        out Vector2 localPoint))
    {
            Vector2 target = localPoint + _dragOffset;

            // Дополнительная проверка на слишком сильное отдаление
            if (Vector2.Distance(_rectTransform.anchoredPosition, target) < _maxDragDistance)
            {
                _dragTargetPos = target;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _isDragging = false;
        TryUseHeldItem();
    }

    private void UpdateDragTarget(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            if (Vector2.Distance(_rectTransform.anchoredPosition, localPoint) < _maxDragDistance)
            {
                _dragTargetPos = localPoint;
            }
        }
    }

    #endregion

    #region Action & Return

    public void PerformItemAction(bool returnAfter = true)
    {
        _isPerformingAction = true;
        _actionTime = 0f;
        _actionStartPosition = transform.position;
        _shouldReturnAfterAction = returnAfter;
    }

    private void HandlePerformAction()
    {
        if (!_isPerformingAction) return;

        _actionTime += Time.deltaTime;
        float offsetX = Mathf.Sin(_actionTime * 10f) * _actionAmplitude;

        transform.position = _actionStartPosition + new Vector3(offsetX, 0f, 0f);

        if (_actionTime >= _actionDuration)
        {
            _isPerformingAction = false;
            transform.position = _actionStartPosition;

            if (_shouldReturnAfterAction)
            {
                StartReturnSequence();
            }
        }
    }

    public void StartReturnSequence()
    {
        if (_heldItem == null) return;

        _isReturningSequence = true;
        _returnT = 0f;
        _returnPhase = ReturnPhase.ToReturnPoint;
        _returnTarget = _heldItem.ReturnPoint.position;
        _animator.SetTrigger("TakeOff");

        SetAllUIInteractionsEnabled(true); // включаем UI обратно
    }

    private void HandleReturnSequence()
    {
        if (!_isReturningSequence) return;

        _returnT += Time.deltaTime * _returnSpeed;
        float t = Mathf.SmoothStep(0, 1, _returnT);

        switch (_returnPhase)
        {
            case ReturnPhase.ToReturnPoint:
                transform.position = Vector3.Lerp(transform.position, _returnTarget, t);
                if (t >= 1f)
                {
                    _returnPhase = ReturnPhase.DropItem;
                    _returnT = 0f;
                }
                break;

            case ReturnPhase.DropItem:
                if (_heldItem != null)
                {
                    _heldItem.transform.SetParent(_heldItem.ReturnPoint, worldPositionStays: false);
                    _heldItem.transform.localPosition = Vector3.zero;
                    _heldItem.OnReturn();
                    _heldItem = null;
                }

                _returnPhase = ReturnPhase.ToStart;
                _returnTarget = _initialHandPosition.position;
                _returnT = 0f;
                break;

            case ReturnPhase.ToStart:
                transform.position = Vector3.Lerp(transform.position, _returnTarget, t);
                if (t >= 1f)
                {
                    _isReturningSequence = false;
                    _returnPhase = ReturnPhase.None;
                }
                break;
        }
    }

    #endregion

    #region Use Logic

    private void TryUseHeldItem()
    {
        if (_heldItem == null) return;

        RectTransform rayOrigin = _heldItem.RaycastOrigin;
        if (rayOrigin == null) return;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, rayOrigin.position);
        PointerEventData pointerEvent = new PointerEventData(EventSystem.current) { position = screenPos };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEvent, results);

        foreach (var result in results)
        {
            if (result.gameObject == _heldItem.ReturnPoint.gameObject)
            {
                Debug.Log("Возврат предмета на место");
                StartReturnSequence();
                return;
            }

            if (!result.gameObject.TryGetComponent(out DropTargetZone dropTarget))
                continue;

            switch (_heldItem.Type)
            {
                case GrabbableItem.ItemType.Lipstick:
                    if (dropTarget.zone == DropTargetZone.ZoneType.Lips)
                    {
                        TryUseItem(result.gameObject);
                        return;
                    }
                    break;

                case GrabbableItem.ItemType.EyesBrush:
                    if (dropTarget.zone == DropTargetZone.ZoneType.Color ||
                        (dropTarget.zone == DropTargetZone.ZoneType.Eyes &&
                         _heldItem.GetComponent<IColorZone>() is ArrayOfColors colorEyesBrashArray &&
                         colorEyesBrashArray.CurrentIndex >= 0))
                    {
                        TryUseItem(result.gameObject);
                        return;
                    }
                    break;

                case GrabbableItem.ItemType.BlushBrush:
                    if (dropTarget.zone == DropTargetZone.ZoneType.Color ||
                        (dropTarget.zone == DropTargetZone.ZoneType.Cheekbones &&
                         _heldItem.GetComponent<IColorZone>() is ArrayOfColors colorBlushBrushArray &&
                         colorBlushBrushArray.CurrentIndex >= 0))
                    {
                        TryUseItem(result.gameObject);
                        return;
                    }
                    break;

                case GrabbableItem.ItemType.Cream:
                    TryUseItem(result.gameObject);
                    return;
            }
        }
    }

    private void TryUseItem(GameObject target)
    {
        IUsableItem usable = _heldItem.GetComponent<IUsableItem>();
        if (usable != null)
        {
            usable.UseOnZone(target, this);
        }
    }

    #endregion

    #region UI Lock

    private void SetAllUIInteractionsEnabled(bool enabled)
    {
        var uiEvents = Object.FindObjectsByType<UI_InteractionEvents>(FindObjectsSortMode.None);

        foreach (var uiEvent in uiEvents)
        {
            uiEvent.SetInteractionEnabled(enabled);
        }
    }

    #endregion
}






