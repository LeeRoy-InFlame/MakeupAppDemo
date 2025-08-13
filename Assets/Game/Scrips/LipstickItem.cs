using UnityEngine;

public class LipstickItem : MonoBehaviour, IUsableItem
{
    [SerializeField] private int _colorIndex; // ������ ����� ��� ����������
    public bool ShouldReleaseAfterUse => true; // ���������� � ���������

    public void UseOnZone(GameObject targetZone, HandController hand)
    {
        var zone = targetZone.GetComponent<IColorZone>();
        if (zone != null)
        {
            zone.ApplyColor(_colorIndex);
            if(ShouldReleaseAfterUse == true)
            {
                hand.PerformItemAction();
            }
        }
        else
        {
            Debug.LogWarning("������� ���� �� ��������� IColorZone");
        }
    }
}
