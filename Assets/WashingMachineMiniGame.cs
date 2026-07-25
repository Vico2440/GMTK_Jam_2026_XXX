using UnityEngine;
using UnityEngine.UI; // Ajouté pour gérer la couleur de l'image
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class WashingMachineMiniGame : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string minigameID = "MachineLaver";
    
    [Tooltip("Combien de mousses au premier lancement ?")]
    [SerializeField] private int initialFoamCount = 10;
    
    [Tooltip("Combien de mousses EN PLUS à chaque nouvelle itération ?")]
    [SerializeField] private int difficultyIncrease = 5;

    [Header("Visuels de la Machine")]
    [Tooltip("Glisse ici l'image de fond de ta machine à laver")]
    [SerializeField] private Image machineImage; 
    [SerializeField] private Color brokenColor = new Color(1f, 0.4f, 0.4f); // Rouge clair
    
    [Header("Génération Procédurale")]
    [SerializeField] private GameObject foamPrefab;
    [SerializeField] private RectTransform spawnArea; 

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI statusText;

    private int currentDifficulty;
    private int foamsLeftToWipe = 0;
    private bool isCompleted = false;
    private List<GameObject> spawnedFoams = new List<GameObject>();
    
    // Pour mémoriser l'animation de tremblement et pouvoir l'arrêter
    private Tween shakeTween; 

    private void Awake()
    {
        currentDifficulty = initialFoamCount;
    }

    private void OnEnable()
    {
        isCompleted = false;

        // 1. ANIMATION & COULEUR DE LA MACHINE EN PANNE
        if (machineImage != null)
        {
            machineImage.color = brokenColor;
            
            // On s'assure d'arrêter une ancienne animation s'il y en avait une
            shakeTween?.Kill(); 
            machineImage.rectTransform.localRotation = Quaternion.identity;
            
            // Fait trembler la machine (Rotation de 3 degrés, en boucle à l'infini)
            shakeTween = machineImage.rectTransform
                .DOShakeRotation(0.5f, new Vector3(0, 0, 3f), vibrato: 10, randomness: 90f)
                .SetLoops(-1, LoopType.Restart);
        }

        // 2. Nettoyer les vieilles mousses s'il en reste
        foreach (var foam in spawnedFoams)
        {
            if (foam != null) Destroy(foam);
        }
        spawnedFoams.Clear();

        // 3. Préparer la nouvelle difficulté
        foamsLeftToWipe = currentDifficulty;
        Rect spawnRect = spawnArea.rect;

        // 4. GÉNÉRATION ALÉATOIRE
        for (int i = 0; i < currentDifficulty; i++)
        {
            GameObject newFoam = Instantiate(foamPrefab, spawnArea);
            spawnedFoams.Add(newFoam);

            float randomX = Random.Range(spawnRect.xMin, spawnRect.xMax);
            float randomY = Random.Range(spawnRect.yMin, spawnRect.yMax);

            RectTransform rt = newFoam.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(randomX, randomY);

            rt.localEulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
            rt.localScale = Vector3.one * Random.Range(0.7f, 1.3f);

            newFoam.GetComponent<FoamItem>().manager = this;
        }

        if (statusText != null)
        {
            statusText.text = "FROTTE LA MOUSSE !";
            statusText.color = Color.cyan;
        }
    }

    public void OnFoamWiped()
    {
        if (isCompleted) return;

        foamsLeftToWipe--;

        if (foamsLeftToWipe <= 0)
        {
            CompleteMiniGame();
        }
    }

    private void CompleteMiniGame()
    {
        isCompleted = true;
        
        // --- ON RÉPARE LA MACHINE VISUELLEMENT ---
        if (machineImage != null)
        {
            // Arrête le tremblement et remet la machine droite
            shakeTween?.Kill();
            machineImage.rectTransform.localRotation = Quaternion.identity;
            
            // Remet la couleur blanche normale avec une petite transition fluide
            machineImage.DOColor(Color.white, 0.3f); 
            
            // Petit rebond de victoire
            machineImage.rectTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
        }

        currentDifficulty += difficultyIncrease;

        if (statusText != null)
        {
            statusText.text = "MACHINE PROPRE !";
            statusText.color = Color.green;
        }

        CrisisManager.Instance?.ResolveCrisis(minigameID);

        DOVirtual.DelayedCall(1.2f, () =>
        {
            MinigameManager.Instance?.CloseMinigame(minigameID);
        });
    }

    private void OnDisable()
    {
        // Sécurité : On coupe l'animation si on ferme le menu violemment
        shakeTween?.Kill();
        if (machineImage != null)
        {
            machineImage.rectTransform.localRotation = Quaternion.identity;
        }
    }
}