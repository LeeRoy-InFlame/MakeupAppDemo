using UnityEngine;

public class GrabbableItem : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] private Transform _grabPoint;
    [SerializeField] private Transform _takePosition;
    [SerializeField] private Transform _afterGrabHandPosition;
    [SerializeField] private Transform _returnPoint;
    [SerializeField] private RectTransform _raycastOrigin;

    [Header("Meta")]
    [SerializeField] private HandController _hand;
    [SerializeField] private ItemType _itemType;

    public RectTransform RaycastOrigin => _raycastOrigin;

    private Vector3 _originalScale;

    public enum ItemType { None, Cream, EyesBrush, Lipstick, BlushBrush }

    public ItemType Type => _itemType;
    public Transform ReturnPoint => _returnPoint;
    public Transform GetAfterGrabHandPosition() => _afterGrabHandPosition;

    public void OnClick()
    {
        _hand.StartGrab(_grabPoint, this);
    }

    public void OnGrabComplete(HandController hand)
    {
        _originalScale = transform.localScale;

        transform.SetParent(hand.transform);
        transform.position = _takePosition.position;
        transform.localRotation = _takePosition.localRotation;

        foreach (var comp in GetComponents<UI_InteractionEvents>())
        {
            if (comp != this)
                comp.enabled = false;
        }
    }

    public void OnReturn()
    {
        transform.localScale = _originalScale;

        foreach (var comp in GetComponents<UI_InteractionEvents>())
        {
            comp.enabled = true;
        }
    }
}
