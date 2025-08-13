using UnityEngine;
using UnityEngine.UI;

public class AcneFade : MonoBehaviour
{
    [SerializeField] private Image acneImage; // ссылка на Image прыща
    [SerializeField] private float fadeDuration = 2f; // за сколько секунд исчезает

    private float currentTime = 0f;
    private bool fading = false;

    public void StartFade()
    {
        currentTime = 0f;
        fading = true;
    }

    private void Update()
    {
        if (!fading) return;

        currentTime += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);

        var color = acneImage.color;
        color.a = alpha;
        acneImage.color = color;

        if (currentTime >= fadeDuration)
        {
            fading = false;
            acneImage.gameObject.SetActive(false); // полностью скрываем объект
        }
    }
}

