using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MinigameIntroSplash : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("L'objet Image UI qui contient ton sprite (ex: TAKE IT!)")]
    [SerializeField] private Image splashImage;
    
    [Tooltip("Taille de départ (très grand = plus d'impact)")]
    [SerializeField] private float startScale = 6f; 
    
    [Tooltip("Vitesse à laquelle le texte s'écrase à l'écran")]
    [SerializeField] private float slamDuration = 0.25f; 
    
    [Tooltip("Combien de temps le texte reste affiché avant de disparaître ?")]
    [SerializeField] private float stayDuration = 1.0f;

    private Tween activeTween;

    private void OnEnable()
    {
        if (splashImage == null) return;

        activeTween?.Kill();

        splashImage.gameObject.SetActive(true);
        splashImage.color = Color.white;
        splashImage.transform.localScale = Vector3.one * startScale;

        activeTween = splashImage.transform
            .DOScale(new Vector3(2f,2f,2f), slamDuration)
            .SetEase(Ease.OutBack) 
            .OnComplete(() =>
            {
                splashImage.DOFade(0f, 0.2f)
                    .SetDelay(stayDuration)
                    .OnComplete(() => 
                    {
                        splashImage.gameObject.SetActive(false);
                    });
            });
    }

    private void OnDisable()
    {
        activeTween?.Kill();
    }
}