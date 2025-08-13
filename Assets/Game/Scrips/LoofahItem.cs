using UnityEngine;

public class LoofahItem : MonoBehaviour
{
    [SerializeField] private Transform[] _makeupObjects;

    public void MakeupClear()
    {
        foreach (Transform makeup in _makeupObjects)
        {
            foreach (Transform child in makeup)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
