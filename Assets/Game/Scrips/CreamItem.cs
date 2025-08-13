using UnityEngine;

public class CreamItem : MonoBehaviour, IUsableItem
{
    public bool ShouldReleaseAfterUse => true;
    public void UseOnZone(GameObject targetZone, HandController hand)
    {
        var fade = targetZone.GetComponent<AcneFade>();
        if (fade != null)
        {
            fade.StartFade();
            if (ShouldReleaseAfterUse == true)
            {
                hand.PerformItemAction();
            }
        }
    }
}

