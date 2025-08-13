using UnityEngine;

public interface IUsableItem
{
    void UseOnZone(GameObject targetZone, HandController hand);
    bool ShouldReleaseAfterUse { get; } 
}
